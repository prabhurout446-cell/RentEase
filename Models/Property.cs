using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentEase.Models
{
    public enum PropertyType
    {
        Apartment,
        House,
        Room,
        CoLiving,
        PG,
        Studio,
        Villa
    }

    public enum ListingType
    {
        EntirePlace,
        Sharing,
        CoLiving
    }

    public enum FurnishingStatus
    {
        Unfurnished,
        SemiFurnished,
        FullyFurnished
    }

    public class Property
    {
        public int Id { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public PropertyType PropertyType { get; set; }

        [Required]
        public ListingType ListingType { get; set; }

        // Location
        [Required, MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PinCode { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Rent & Deposit
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal RentAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositAmount { get; set; }

        public bool IsNegotiable { get; set; }

        // Property Details
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int? TotalFloors { get; set; }
        public int? FloorNumber { get; set; }
        public double? AreaSqFt { get; set; }

        // Furnishing
        public FurnishingStatus FurnishingStatus { get; set; }
        public bool HasAC { get; set; }
        public bool HasWashingMachine { get; set; }
        public bool HasFridge { get; set; }
        public bool HasTV { get; set; }
        public bool HasSofa { get; set; }
        public bool HasDiningTable { get; set; }
        public bool HasWardrobe { get; set; }
        public bool HasMicrowave { get; set; }
        public bool HasGeyser { get; set; }
        public bool HasCurtains { get; set; }
        public bool HasLift { get; set; }
        public bool HasBalcony { get; set; }
        public bool HasParking { get; set; }
        public bool HasPowerBackup { get; set; }
        public bool HasSecurity { get; set; }
        public bool HasGym { get; set; }
        public bool HasSwimmingPool { get; set; }
        public bool HasPlayground { get; set; }
        public bool HasCCTV { get; set; }
        public bool IsGatedCommunity { get; set; }
        public bool HasWifi { get; set; }
        public bool WaterSupply { get; set; }
        public bool PipedGas { get; set; }
        public int? TotalSharingBeds { get; set; }
        public int? AvailableBeds { get; set; }
        public string? GenderPreference { get; set; }
        public string? OccupationPreference { get; set; }
        public bool MealsIncluded { get; set; }
        public bool LaundryIncluded { get; set; }
        public bool HousekeepingIncluded { get; set; }
        public string? NearbyMarkets { get; set; }
        public string? NearbySchools { get; set; }
        public string? NearbyHospitals { get; set; }
        public string? NearbyBusStands { get; set; }
        public string? NearbyMetro { get; set; }
        public string? NearbyMalls { get; set; }
        public string? OtherNearbyPlaces { get; set; }
        public bool PetsAllowed { get; set; }
        public bool BachelorsAllowed { get; set; }
        public bool FamiliesAllowed { get; set; }
        public bool SmokingAllowed { get; set; }

        public DateTime AvailableFrom { get; set; }
        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int ViewCount { get; set; }

        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }

    public class PropertyImage
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Property? Property { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
