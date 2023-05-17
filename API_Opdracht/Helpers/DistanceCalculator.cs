namespace API_Opdracht.Helpers
{
    public static class DistanceCalculator
    {
        public static double CalculateHaversineDistance(string location1, string location2)
        {
            const double earthRadius = 6371;

            var lat1 = double.Parse(location1.Split(',')[0]);
            var lon1 = double.Parse(location1.Split(',')[1]);
            var lat2 = double.Parse(location2.Split(',')[0]);
            var lon2 = double.Parse(location2.Split(',')[1]);

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = earthRadius * c;
            return distance;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}