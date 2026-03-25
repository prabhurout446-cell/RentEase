using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentEase.Data;
using RentEase.Models;
using RentEase.ViewModels;

namespace RentEase.Controllers
{
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PropertyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        public async Task<IActionResult> Index(PropertySearchViewModel search)
        {
            var query = _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .Where(p => p.IsAvailable)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.City))
                query = query.Where(p => p.City.ToLower().Contains(search.City.ToLower()));

            if (!string.IsNullOrWhiteSpace(search.PinCode))
                query = query.Where(p => p.PinCode == search.PinCode);

            if (search.PropertyType.HasValue)
                query = query.Where(p => p.PropertyType == search.PropertyType.Value);

            if (search.ListingType.HasValue)
                query = query.Where(p => p.ListingType == search.ListingType.Value);

            if (search.MinRent.HasValue)
                query = query.Where(p => p.RentAmount >= search.MinRent.Value);

            if (search.MaxRent.HasValue)
                query = query.Where(p => p.RentAmount <= search.MaxRent.Value);

            if (search.FurnishingStatus.HasValue)
                query = query.Where(p => p.FurnishingStatus == search.FurnishingStatus.Value);

            if (search.HasLift.HasValue && search.HasLift.Value)
                query = query.Where(p => p.HasLift);

            if (search.HasBalcony.HasValue && search.HasBalcony.Value)
                query = query.Where(p => p.HasBalcony);

            if (search.HasParking.HasValue && search.HasParking.Value)
                query = query.Where(p => p.HasParking);

            if (search.HasWifi.HasValue && search.HasWifi.Value)
                query = query.Where(p => p.HasWifi);

            // Sort
            query = search.SortBy switch
            {
                "rent_asc" => query.OrderBy(p => p.RentAmount),
                "rent_desc" => query.OrderByDescending(p => p.RentAmount),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                "popular" => query.OrderByDescending(p => p.ViewCount),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            search.TotalCount = await query.CountAsync();
            search.Results = await query
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            if (search.UserLat.HasValue && search.UserLng.HasValue && search.RadiusKm.HasValue)
            {
                var radius = search.RadiusKm.Value;
                search.Results = search.Results
                    .Where(p => p.Latitude.HasValue && p.Longitude.HasValue &&
                           GetDistanceKm(search.UserLat.Value, search.UserLng.Value, p.Latitude.Value, p.Longitude.Value) <= radius)
                    .ToList();
            }

            return View(search);
        }
        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null) return NotFound();

            property.ViewCount++;
            await _context.SaveChangesAsync();

            return View(property);
        }

        [Authorize]
        public IActionResult Create() => View(new PropertyCreateViewModel { AvailableFrom = DateTime.Today });

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var property = MapToProperty(model, user.Id);
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            if (model.Images != null && model.Images.Count > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", property.Id.ToString());
                Directory.CreateDirectory(uploadsDir);
                bool first = true;
                foreach (var file in model.Images.Take(10))
                {
                    if (file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        var fileName = $"{Guid.NewGuid()}{ext}";
                        var filePath = Path.Combine(uploadsDir, fileName);
                        using var stream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        _context.PropertyImages.Add(new PropertyImage
                        {
                            PropertyId = property.Id,
                            ImagePath = $"/uploads/{property.Id}/{fileName}",
                            IsPrimary = first
                        });
                        first = false;
                    }
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Property listed successfully!";
            return RedirectToAction("Details", new { id = property.Id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (property.OwnerId != userId) return Forbid();

            var model = MapToViewModel(property);
            return View(model);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (property.OwnerId != userId) return Forbid();

            UpdatePropertyFromModel(property, model);
            property.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Property updated successfully!";
            return RedirectToAction("Details", new { id });
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (property.OwnerId != userId) return Forbid();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Property removed.";
            return RedirectToAction("MyListings");
        }
        [Authorize]
        public async Task<IActionResult> MyListings()
        {
            var userId = _userManager.GetUserId(User);
            var properties = await _context.Properties
                .Include(p => p.Images)
                .Where(p => p.OwnerId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(properties);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (property.OwnerId != userId) return Forbid();

            property.IsAvailable = !property.IsAvailable;
            await _context.SaveChangesAsync();

            return RedirectToAction("MyListings");
        }

        private static double GetDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRad(double deg) => deg * Math.PI / 180;

        private static Property MapToProperty(PropertyCreateViewModel m, string ownerId)
        {
            var p = new Property { OwnerId = ownerId };
            UpdatePropertyFromModel(p, m);
            return p;
        }

        private static void UpdatePropertyFromModel(Property p, PropertyCreateViewModel m)
        {
            p.Title = m.Title; p.Description = m.Description;
            p.PropertyType = m.PropertyType; p.ListingType = m.ListingType;
            p.Address = m.Address; p.City = m.City; p.State = m.State; p.PinCode = m.PinCode;
            p.Latitude = m.Latitude; p.Longitude = m.Longitude;
            p.RentAmount = m.RentAmount; p.DepositAmount = m.DepositAmount; p.IsNegotiable = m.IsNegotiable;
            p.Bedrooms = m.Bedrooms; p.Bathrooms = m.Bathrooms;
            p.TotalFloors = m.TotalFloors; p.FloorNumber = m.FloorNumber; p.AreaSqFt = m.AreaSqFt;
            p.FurnishingStatus = m.FurnishingStatus;
            p.HasAC = m.HasAC; p.HasWashingMachine = m.HasWashingMachine; p.HasFridge = m.HasFridge;
            p.HasTV = m.HasTV; p.HasSofa = m.HasSofa; p.HasDiningTable = m.HasDiningTable;
            p.HasWardrobe = m.HasWardrobe; p.HasMicrowave = m.HasMicrowave; p.HasGeyser = m.HasGeyser;
            p.HasCurtains = m.HasCurtains;
            p.HasLift = m.HasLift; p.HasBalcony = m.HasBalcony; p.HasParking = m.HasParking;
            p.HasPowerBackup = m.HasPowerBackup; p.HasSecurity = m.HasSecurity; p.HasGym = m.HasGym;
            p.HasSwimmingPool = m.HasSwimmingPool; p.HasPlayground = m.HasPlayground;
            p.HasCCTV = m.HasCCTV; p.IsGatedCommunity = m.IsGatedCommunity; p.HasWifi = m.HasWifi;
            p.WaterSupply = m.WaterSupply; p.PipedGas = m.PipedGas;
            p.TotalSharingBeds = m.TotalSharingBeds; p.AvailableBeds = m.AvailableBeds;
            p.GenderPreference = m.GenderPreference; p.OccupationPreference = m.OccupationPreference;
            p.MealsIncluded = m.MealsIncluded; p.LaundryIncluded = m.LaundryIncluded; p.HousekeepingIncluded = m.HousekeepingIncluded;
            p.NearbyMarkets = m.NearbyMarkets; p.NearbySchools = m.NearbySchools;
            p.NearbyHospitals = m.NearbyHospitals; p.NearbyBusStands = m.NearbyBusStands;
            p.NearbyMetro = m.NearbyMetro; p.NearbyMalls = m.NearbyMalls; p.OtherNearbyPlaces = m.OtherNearbyPlaces;
            p.PetsAllowed = m.PetsAllowed; p.BachelorsAllowed = m.BachelorsAllowed;
            p.FamiliesAllowed = m.FamiliesAllowed; p.SmokingAllowed = m.SmokingAllowed;
            p.AvailableFrom = m.AvailableFrom;
        }

        private static PropertyCreateViewModel MapToViewModel(Property p) => new()
        {
            Title = p.Title, Description = p.Description,
            PropertyType = p.PropertyType, ListingType = p.ListingType,
            Address = p.Address, City = p.City, State = p.State, PinCode = p.PinCode,
            Latitude = p.Latitude, Longitude = p.Longitude,
            RentAmount = p.RentAmount, DepositAmount = p.DepositAmount, IsNegotiable = p.IsNegotiable,
            Bedrooms = p.Bedrooms, Bathrooms = p.Bathrooms,
            TotalFloors = p.TotalFloors, FloorNumber = p.FloorNumber, AreaSqFt = p.AreaSqFt,
            FurnishingStatus = p.FurnishingStatus,
            HasAC = p.HasAC, HasWashingMachine = p.HasWashingMachine, HasFridge = p.HasFridge,
            HasTV = p.HasTV, HasSofa = p.HasSofa, HasDiningTable = p.HasDiningTable,
            HasWardrobe = p.HasWardrobe, HasMicrowave = p.HasMicrowave, HasGeyser = p.HasGeyser,
            HasCurtains = p.HasCurtains,
            HasLift = p.HasLift, HasBalcony = p.HasBalcony, HasParking = p.HasParking,
            HasPowerBackup = p.HasPowerBackup, HasSecurity = p.HasSecurity, HasGym = p.HasGym,
            HasSwimmingPool = p.HasSwimmingPool, HasPlayground = p.HasPlayground,
            HasCCTV = p.HasCCTV, IsGatedCommunity = p.IsGatedCommunity, HasWifi = p.HasWifi,
            WaterSupply = p.WaterSupply, PipedGas = p.PipedGas,
            TotalSharingBeds = p.TotalSharingBeds, AvailableBeds = p.AvailableBeds,
            GenderPreference = p.GenderPreference, OccupationPreference = p.OccupationPreference,
            MealsIncluded = p.MealsIncluded, LaundryIncluded = p.LaundryIncluded, HousekeepingIncluded = p.HousekeepingIncluded,
            NearbyMarkets = p.NearbyMarkets, NearbySchools = p.NearbySchools,
            NearbyHospitals = p.NearbyHospitals, NearbyBusStands = p.NearbyBusStands,
            NearbyMetro = p.NearbyMetro, NearbyMalls = p.NearbyMalls, OtherNearbyPlaces = p.OtherNearbyPlaces,
            PetsAllowed = p.PetsAllowed, BachelorsAllowed = p.BachelorsAllowed,
            FamiliesAllowed = p.FamiliesAllowed, SmokingAllowed = p.SmokingAllowed,
            AvailableFrom = p.AvailableFrom
        };
    }
}
