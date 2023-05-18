using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using API_Opdracht.Data;
using API_Opdracht.Models;
using API_Opdracht.Extensions;
using API_Opdracht.Helpers;

namespace API_Opdracht.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AddressController(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        // GET /api/address
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses([FromQuery] string? search,
            [FromQuery] string? sortField, [FromQuery] string? sortOrder)
        {
            IQueryable<Address> query = _context.Addresses!;

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.ApplySearch(search);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortField) && (sortOrder == "asc" || sortOrder == "desc"))
            {
                var propertyToSort = sortField.ToLower();
                var isAscending = sortOrder.ToLower() == "asc";

                try
                {
                    query = query.ApplySorting(propertyToSort, isAscending);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest($"Invalid sorting property: {sortField}. {ex.Message}");
                }
            }

            var addresses = await query.ToListAsync();
            return addresses;
        }


        // GET /api/address/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Addresses!.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // POST /api/address
        [HttpPost]
        public async Task<ActionResult<Address>> CreateAddress(Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Addresses!.Add(address);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the address: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
        }


        // PUT /api/address/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAddress(int id, Address updatedAddress)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }

            var existingAddress = await _context.Addresses!.FindAsync(id);

            if (existingAddress == null)
            {
                return NotFound();
            }

            // Update the properties of the existing address with the new values
            existingAddress.Street = updatedAddress.Street;
            existingAddress.HouseNumber = updatedAddress.HouseNumber;
            existingAddress.Postcode = updatedAddress.Postcode;
            existingAddress.Place = updatedAddress.Place;
            existingAddress.Country = updatedAddress.Country;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("updated address with id " + id);
        }

        // DELETE /api/address/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses!.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Ok("Deleted address with id " + id);
        }
        
        // GET /api/address/distance
        [HttpGet("distance")]
        public async Task<ActionResult<double>> CalculateDistance([FromQuery] int addressId1, [FromQuery] int addressId2)
        {
            var address1 = await _context.Addresses!.FindAsync(addressId1);
            var address2 = await _context.Addresses!.FindAsync(addressId2);

            if (address1 == null || address2 == null)
            {
                return NotFound("One or both addresses not found.");
            }

            var address1Query = $"{Uri.EscapeDataString(address1.Street)} {Uri.EscapeDataString(address1.HouseNumber)}, " +
                                $"{Uri.EscapeDataString(address1.Place)}, {address1.Postcode}, {address1.Country}";
            var address2Query = $"{Uri.EscapeDataString(address2.Street)} {Uri.EscapeDataString(address2.HouseNumber)}, " +
                                $"{Uri.EscapeDataString(address2.Place)}, {address2.Postcode}, {address2.Country}";

            var apiKey = _configuration["AppSettings:GeocodioApiKey"];
            
            var baseUrl = "https://api.geocod.io/v1.7";

            var url1 = $"{baseUrl}/geocode?q={address1Query}&api_key={apiKey}&country={Uri.EscapeDataString(address1.Country)}";
            var url2 = $"{baseUrl}/geocode?q={address2Query}&api_key={apiKey}&country={Uri.EscapeDataString(address2.Country)}";

            try
            {
                var response1 = await _httpClient.GetAsync(url1);
                var response2 = await _httpClient.GetAsync(url2);

                if (!response1.IsSuccessStatusCode || !response2.IsSuccessStatusCode)
                {
                    return BadRequest("Failed to geocode one or both addresses.");
                }

                var content1 = await response1.Content.ReadAsStringAsync();
                var content2 = await response2.Content.ReadAsStringAsync();

                var result1 = JsonConvert.DeserializeObject<dynamic>(content1);
                var result2 = JsonConvert.DeserializeObject<dynamic>(content2);

                var address1Location = $"{result1!.results[0].location.lat},{result1.results[0].location.lng}";
                var address2Location = $"{result2!.results[0].location.lat},{result2.results[0].location.lng}";

                var distance = DistanceCalculator.CalculateHaversineDistance(address1Location, address2Location);
                return Ok("distance in km: " + distance);
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to geocode one or both addresses: " + ex );
            }
        }
        
        private bool AddressExists(int id)
        {
            return _context.Addresses!.Any(a => a.Id == id);
        }
    }
}
