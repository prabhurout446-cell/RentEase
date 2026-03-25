using System.ComponentModel.DataAnnotations;
using RentEase.Models;

namespace RentEase.ViewModels
{
    public class RegisterViewModel
    {
        [Required, MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    public class PropertyCreateViewModel
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Property Type")]
        public PropertyType PropertyType { get; set; }

        [Required]
        [Display(Name = "Listing Type")]
        public ListingType ListingType { get; set; }

        // Location
        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Pin Code")]
        public string PinCode { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Financial
        [Required]
        [Display(Name = "Monthly Rent (₹)")]
        [Range(0, double.MaxValue)]
        public decimal RentAmount { get; set; }

        [Display(Name = "Security Deposit (₹)")]
        [Range(0, double.MaxValue)]
        public decimal DepositAmount { get; set; }

        [Display(Name = "Rent is Negotiable")]
        public bool IsNegotiable { get; set; }

        // Size
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        [Display(Name = "Total Floors in Building")]
        public int? TotalFloors { get; set; }

        [Display(Name = "Property Floor Number")]
        public int? FloorNumber { get; set; }

        [Display(Name = "Area (sq.ft)")]
        public double? AreaSqFt { get; set; }

        // Furnishing
        [Display(Name = "Furnishing Status")]
        public FurnishingStatus FurnishingStatus { get; set; }

        // Furnished Amenities
        [Display(Name = "Air Conditioner")]
        public bool HasAC { get; set; }
        [Display(Name = "Washing Machine")]
        public bool HasWashingMachine { get; set; }
        [Display(Name = "Refrigerator")]
        public bool HasFridge { get; set; }
        public bool HasTV { get; set; }
        public bool HasSofa { get; set; }
        [Display(Name = "Dining Table")]
        public bool HasDiningTable { get; set; }
        public bool HasWardrobe { get; set; }
        public bool HasMicrowave { get; set; }
        [Display(Name = "Water Heater / Geyser")]
        public bool HasGeyser { get; set; }
        public bool HasCurtains { get; set; }

        // Building Amenities
        [Display(Name = "Lift / Elevator")]
        public bool HasLift { get; set; }
        public bool HasBalcony { get; set; }
        [Display(Name = "Parking Available")]
        public bool HasParking { get; set; }
        [Display(Name = "Power Backup")]
        public bool HasPowerBackup { get; set; }
        [Display(Name = "Security Guard")]
        public bool HasSecurity { get; set; }
        public bool HasGym { get; set; }
        [Display(Name = "Swimming Pool")]
        public bool HasSwimmingPool { get; set; }
        public bool HasPlayground { get; set; }
        [Display(Name = "CCTV Surveillance")]
        public bool HasCCTV { get; set; }
        [Display(Name = "Gated Community")]
        public bool IsGatedCommunity { get; set; }
        [Display(Name = "WiFi Available")]
        public bool HasWifi { get; set; }

        // Utilities
        [Display(Name = "24/7 Water Supply")]
        public bool WaterSupply { get; set; }
        [Display(Name = "Piped Gas Connection")]
        public bool PipedGas { get; set; }

        // Sharing Specifics
        [Display(Name = "Total Sharing Beds")]
        public int? TotalSharingBeds { get; set; }

        [Display(Name = "Available Beds")]
        public int? AvailableBeds { get; set; }

        [Display(Name = "Gender Preference")]
        public string? GenderPreference { get; set; }

        [Display(Name = "Occupation Preference")]
        public string? OccupationPreference { get; set; }

        [Display(Name = "Meals Included")]
        public bool MealsIncluded { get; set; }
        [Display(Name = "Laundry Included")]
        public bool LaundryIncluded { get; set; }
        [Display(Name = "Housekeeping Included")]
        public bool HousekeepingIncluded { get; set; }

        // Nearby Places
        [Display(Name = "Nearby Markets (comma-separated)")]
        public string? NearbyMarkets { get; set; }

        [Display(Name = "Nearby Schools/Colleges")]
        public string? NearbySchools { get; set; }

        [Display(Name = "Nearby Hospitals")]
        public string? NearbyHospitals { get; set; }

        [Display(Name = "Nearby Bus Stands")]
        public string? NearbyBusStands { get; set; }

        [Display(Name = "Nearby Metro Stations")]
        public string? NearbyMetro { get; set; }

        [Display(Name = "Nearby Malls/Shopping")]
        public string? NearbyMalls { get; set; }

        [Display(Name = "Other Nearby Landmarks")]
        public string? OtherNearbyPlaces { get; set; }

        // Tenant Preferences
        [Display(Name = "Pets Allowed")]
        public bool PetsAllowed { get; set; }
        [Display(Name = "Bachelors Allowed")]
        public bool BachelorsAllowed { get; set; }
        [Display(Name = "Families Allowed")]
        public bool FamiliesAllowed { get; set; }
        [Display(Name = "Smoking Allowed")]
        public bool SmokingAllowed { get; set; }

        [Required]
        [Display(Name = "Available From")]
        [DataType(DataType.Date)]
        public DateTime AvailableFrom { get; set; } = DateTime.Today;

        // Images
        public List<IFormFile>? Images { get; set; }
    }

    public class PropertySearchViewModel
    {
        public string? City { get; set; }
        public string? PinCode { get; set; }
        public PropertyType? PropertyType { get; set; }
        public ListingType? ListingType { get; set; }
        public decimal? MinRent { get; set; }
        public decimal? MaxRent { get; set; }
        public FurnishingStatus? FurnishingStatus { get; set; }
        public bool? HasLift { get; set; }
        public bool? HasBalcony { get; set; }
        public bool? HasParking { get; set; }
        public bool? HasWifi { get; set; }
        public string? SortBy { get; set; }
        public double? UserLat { get; set; }
        public double? UserLng { get; set; }
        public double? RadiusKm { get; set; }

        public List<Property> Results { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class ChatViewModel
    {
        public int PropertyId { get; set; }
        public Property? Property { get; set; }
        public string OtherUserId { get; set; } = string.Empty;
        public string OtherUserName { get; set; } = string.Empty;
        public List<ChatMessage> Messages { get; set; } = new();
        public string NewMessage { get; set; } = string.Empty;
    }

    public class InboxViewModel
    {
        public List<ConversationSummary> Conversations { get; set; } = new();
    }

    public class ConversationSummary
    {
        public string OtherUserId { get; set; } = string.Empty;
        public string OtherUserName { get; set; } = string.Empty;
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
