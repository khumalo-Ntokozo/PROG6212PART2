using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;
using System.IO;

namespace ProgrammingPOE.Controllers
{
    public class LecturerController : Controller
    {
        private readonly DataService _dataService;
        private readonly AuthService _authService;
        private readonly IWebHostEnvironment _environment;

        public LecturerController(DataService dataService, AuthService authService, IWebHostEnvironment environment)
        {
            _dataService = dataService;
            _authService = authService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            if (!_authService.IsInRole("Lecturer"))
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(decimal hoursWorked, decimal hourlyRate, string notes, IFormFile upload)
        {
            if (!_authService.IsInRole("Lecturer"))
                return RedirectToAction("Login", "Account");

            var currentUser = _authService.GetCurrentUser();

            var claim = new Claim
            {
                LecturerUserId = currentUser.Id,
                HoursWorked = hoursWorked,
                HourlyRate = hourlyRate,
                TotalAmount = hoursWorked * hourlyRate,
                Notes = notes,
                Status = ClaimStatus.Pending
            };

            _dataService.AddClaim(claim);

            // Handle file upload
            if (upload != null && upload.Length > 0)
            {
                // Validate file size (5MB limit)
                if (upload.Length > 5 * 1024 * 1024)
                {
                    TempData["Message"] = "File is too large. Maximum size is 5MB.";
                    return View();
                }

                // Get file extension
                var fileExtension = Path.GetExtension(upload.FileName).ToLower();

                // Optional: Validate file types if you want restrictions
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx", ".xls", ".txt", ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["Message"] = "File type not allowed. Please upload PDF, Word, Excel, Image, or Text files.";
                    return View();
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(fileStream);
                }

                // Create document record
                var document = new SupportingDocument
                {
                    ClaimId = claim.ClaimId,
                    FileName = upload.FileName,
                    FilePath = $"/uploads/{uniqueFileName}",
                    FileSize = upload.Length
                };

                _dataService.AddDocument(document);
            }

            TempData["Message"] = "Claim submitted successfully!";
            return RedirectToAction(nameof(TrackClaims));
        }

        public IActionResult TrackClaims()
        {
            if (!_authService.IsInRole("Lecturer"))
                return RedirectToAction("Login", "Account");

            var currentUser = _authService.GetCurrentUser();
            var claims = _dataService.GetClaimsByUser(currentUser.Id);

            // Attach documents to each claim
            foreach (var claim in claims)
            {
                claim.Documents = _dataService.GetDocumentsByClaimId(claim.ClaimId);
            }

            return View(claims);
        }
    }
}