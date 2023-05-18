# Introductie
Dit is een C# .NET API_Opdracht voor het bedrijf Social Brothers.

Het project biedt endpoints voor:
 - CRUD voor adresgegevens
 - Afstand in kilometers tussen 2 adressen op basis van een externe api

## Setup
Open de solution van het project in je IDE naar keuze.
Om te beginnen is er gekozen voor toevoegne van de appsettings.json aan de .gitignore, dit is in verband met de veiligheid van mijn persoonlijke API key van de externe API service.
De gebruiker dient dus zelf in de root van de API_Opdracht (dus in dezelfde laag als waar de appsettings.Development.json staat) een appsettings.json aan te maken:
die moet er als het volgt uitzien:

```bash
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=address.db"
  },
  "AppSettings": {
    "GeocodioApiKey": "your-api-key-here"
  }
}
```
De api key dient door de gebruiker zelf nog gegenereerd te worden, je kunt deze vinden op https://www.geocod.io/. Volg hier de volgende stappen:
1. Maak een account aan
2. Klik op je profiel
3. Navigeer naar "API Keys"
4. En vervolgens "Create API key"
5. deze key patse je in je appsettings.json op de plek waar your-api-key-here staat

## Gebruik

Nu kun je de applicatie opstarten (API_Opdracht: ISS Express) en zie je in de browser de swagger documentatie doormiddel van een UI waarbij je de implementatie ziet van elke route.

Opmerkingen: 
- Bij de POST en PUT endpoints dient het "id" weggehaald te worden in de payload,
 swagger heeft dit als placeholder automatisch erin gezet maar dit veroorzaakt foutieve input, het "id" is namelijk al auto incremented.

- Voor de api/Address/distance endpoint dienen de twee 'id's" van twee bestaande addressen te worden ingevuld, gelievend in de United States (Daar is de api op gebaseerd).
  er staan al twee van deze addressen in de meegegeven sqlite database:

```bash
[
  {
    "id": 1,
    "street": "1600 Amphitheatre Parkway",
    "houseNumber": "1",
    "postcode": "94043",
    "place": "Mountain View",
    "country": "United States"
  },
  {
    "id": 2,
    "street": "1 Infinite Loop",
    "houseNumber": "1",
    "postcode": "95014",
    "place": "Cupertino",
    "country": "United States"
  }
]
```

# Reflectie

Ik ben over het algemeen wel tevreden over de structuur van mijn project.

Ik heb generieke / helper functionaliteiten opgesplits van mijn controller zodat die grotendeels mooi clean blijft, 
ookal denk zeker ik dat het nog wel wat cleaner had gekund.

Ook vind ik dat het project nog eventueele design patterns had kunnen gebruiken voor de overall scalability.

Verder denk ik dat de implementatie van de gehele opdracht goed is gelukt.

