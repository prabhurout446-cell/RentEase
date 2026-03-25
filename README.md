# 🏠 RentEase — Rental Application

A full-featured ASP.NET Core 9 MVC rental platform with SQLite, built for India.

## Features

- **Post Properties** — Apartments, Houses, Rooms, Co-Living, PG, Studio, Villa
- **Detailed Listings** — Furnishing status, amenities, furnished items, building features
- **Nearby Places** — Markets, schools, hospitals, bus stands, metro, malls
- **Sharing / Co-Living** — Total beds, available beds, gender preference, meals, laundry
- **Location-Based Search** — Search by city, pin code, or current GPS location with radius
- **Filters** — Property type, listing type, rent range, furnishing, amenities
- **In-App Chat** — Direct messaging between tenants and landlords
- **Phone Sharing** — Request/share phone numbers securely within chat
- **User Accounts** — Register, login, profile, my listings
- **Image Gallery** — Upload up to 10 photos with carousel display

---

## Quick Start

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Steps

1. **Unzip** the project folder

2. **Open terminal** in the project root (where `RentEase.csproj` is)

3. **Restore packages**
   ```bash
   dotnet restore
   ```

4. **Run the app**
   ```bash
   dotnet run
   ```

5. **Open browser** at: `https://localhost:5001` or `http://localhost:5000`

> The SQLite database (`rentease.db`) is created automatically on first run. No migrations needed!

---

## Project Structure

```
RentEase/
├── Controllers/
│   ├── HomeController.cs          # Landing page, stats
│   ├── PropertyController.cs      # CRUD, search, browse
│   ├── ChatController.cs          # Messaging, inbox, phone sharing
│   └── AccountController.cs       # Register, login, profile
│
├── Models/
│   ├── ApplicationUser.cs         # Identity user + full name
│   ├── Property.cs                # Full property model (50+ fields)
│   └── ChatMessage.cs             # Messages with phone request support
│
├── ViewModels/
│   └── ViewModels.cs              # All form + display ViewModels
│
├── Data/
│   └── ApplicationDbContext.cs    # EF Core + Identity DbContext
│
├── Views/
│   ├── Home/                      # Landing page, about
│   ├── Property/                  # Browse, details, create, edit, my listings
│   ├── Chat/                      # Conversation, inbox
│   ├── Account/                   # Login, register, profile
│   └── Shared/                    # Layout, property card partial
│
└── wwwroot/
    ├── css/site.css               # Full custom stylesheet
    ├── js/site.js                 # JS utilities
    └── images/                    # Placeholder images
```

---

## Key Models

### Property Fields
| Category | Fields |
|----------|--------|
| Basic | Title, Description, PropertyType, ListingType |
| Location | Address, City, State, PinCode, Latitude, Longitude |
| Financial | RentAmount, DepositAmount, IsNegotiable |
| Size | Bedrooms, Bathrooms, AreaSqFt, FloorNumber, TotalFloors |
| Furnishing | FurnishingStatus, HasAC, HasFridge, HasWashingMachine, HasTV, HasSofa, HasWardrobe, HasDiningTable, HasMicrowave, HasGeyser, HasCurtains |
| Building | HasLift, HasBalcony, HasParking, HasPowerBackup, HasSecurity, HasGym, HasSwimmingPool, HasPlayground, HasCCTV, IsGatedCommunity, HasWifi, WaterSupply, PipedGas |
| Sharing | TotalSharingBeds, AvailableBeds, GenderPreference, OccupationPreference, MealsIncluded, LaundryIncluded, HousekeepingIncluded |
| Nearby | NearbyMarkets, NearbySchools, NearbyHospitals, NearbyBusStands, NearbyMetro, NearbyMalls, OtherNearbyPlaces |
| Preferences | PetsAllowed, BachelorsAllowed, FamiliesAllowed, SmokingAllowed |

---

## Tech Stack

| Technology | Version |
|-----------|---------|
| ASP.NET Core MVC | 9.0 |
| Entity Framework Core | 9.0 |
| SQLite | via EF Core |
| ASP.NET Identity | 9.0 |
| Bootstrap | 5.3 |
| Bootstrap Icons | 1.11 |
| Google Fonts (Sora + DM Serif) | CDN |

---

## Configuration

Edit `appsettings.json` to change the database path:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=rentease.db"
  }
}
```

---

## Notes

- The SQLite database file `rentease.db` is created in the project root
- Uploaded property images are stored in `wwwroot/uploads/`
- Location-based search uses browser Geolocation API + Haversine formula
- Phone numbers are only shared via explicit in-chat consent
