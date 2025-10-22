using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;
using ProgrammingPOE.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace ProgrammingPOE.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly DataService _dataService;
        private readonly AuthService _authService;

        public CoordinatorController(DataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;
        }

        public IActionResult ReviewClaims()
        {
            if (!_authService.IsInRole("Coordinator"))
                return RedirectToAction("Login", "Account");

            var claims = _dataService.GetPendingClaims();

            // Create view models with lecturer names
            var claimViewModels = new List<ClaimViewModel>();
            foreach (var claim in claims)
            {
                var user = _dataService.GetUserById(claim.LecturerUserId);
                claim.Documents = _dataService.GetDocumentsByClaimId(claim.ClaimId);

                claimViewModels.Add(new ClaimViewModel
                {
                    Claim = claim,
                    LecturerName = user?.FullName ?? "Unknown Lecturer"
                });
            }

            return View(claimViewModels);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            if (!_authService.IsInRole("Coordinator"))
                return RedirectToAction("Login", "Account");

            _dataService.UpdateClaimStatus(id, ClaimStatus.CoordinatorApproved);
            TempData["Message"] = $"Claim {id} approved by Coordinator.";
            return RedirectToAction(nameof(ReviewClaims));
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            if (!_authService.IsInRole("Coordinator"))
                return RedirectToAction("Login", "Account");

            _dataService.UpdateClaimStatus(id, ClaimStatus.CoordinatorRejected);
            TempData["Message"] = $"Claim {id} rejected by Coordinator.";
            return RedirectToAction(nameof(ReviewClaims));
        }
    }
}