using ProgrammingPOE.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProgrammingPOE.Services
{
    public class DataService
    {
        // In-memory storage
        private static List<Claim> _claims = new List<Claim>();
        private static List<SupportingDocument> _documents = new List<SupportingDocument>();
        private static List<ApplicationUser> _users = new List<ApplicationUser>();

        private static int _claimIdCounter = 1;
        private static int _documentIdCounter = 1;
        private static int _userIdCounter = 4; // Start after seeded users

        public DataService()
        {
            SeedData();
        }

        private void SeedData()
        {
            // Seed users if empty
            if (!_users.Any())
            {
                _users.Add(new ApplicationUser
                {
                    Id = "1",
                    UserName = "lecturer@test.com",
                    Email = "lecturer@test.com",
                    FullName = "John Lecturer",
                    Role = "Lecturer",
                    Password = "lecturer123"
                });
                _users.Add(new ApplicationUser
                {
                    Id = "2",
                    UserName = "coordinator@test.com",
                    Email = "coordinator@test.com",
                    FullName = "Jane Coordinator",
                    Role = "Coordinator",
                    Password = "coordinator123"
                });
                _users.Add(new ApplicationUser
                {
                    Id = "3",
                    UserName = "manager@test.com",
                    Email = "manager@test.com",
                    FullName = "Bob Manager",
                    Role = "Manager",
                    Password = "manager123"
                });
            }

            // Seed some sample claims if empty
            if (!_claims.Any())
            {
                _claims.Add(new Claim
                {
                    ClaimId = _claimIdCounter++,
                    LecturerUserId = "1",
                    HoursWorked = 40,
                    HourlyRate = 500,
                    TotalAmount = 20000,
                    Notes = "Monthly teaching hours",
                    Status = ClaimStatus.Pending,
                    SubmissionDate = DateTime.Now.AddDays(-2)
                });
                _claims.Add(new Claim
                {
                    ClaimId = _claimIdCounter++,
                    LecturerUserId = "1",
                    HoursWorked = 35,
                    HourlyRate = 450,
                    TotalAmount = 15750,
                    Notes = "Workshop facilitation",
                    Status = ClaimStatus.CoordinatorApproved,
                    SubmissionDate = DateTime.Now.AddDays(-5)
                });
            }
        }

        // User methods
        public ApplicationUser AuthenticateUser(string email, string password)
        {
            return _users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public ApplicationUser GetUserById(string id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public List<ApplicationUser> GetUsersByRole(string role)
        {
            return _users.Where(u => u.Role == role).ToList();
        }

        public void AddUser(ApplicationUser user)
        {
            user.Id = (_userIdCounter++).ToString();
            _users.Add(user);
        }

        // Claim methods
        public void AddClaim(Claim claim)
        {
            claim.ClaimId = _claimIdCounter++;
            claim.SubmissionDate = DateTime.Now;
            _claims.Add(claim);
        }

        public List<Claim> GetAllClaims() => _claims.OrderByDescending(c => c.SubmissionDate).ToList();

        public List<Claim> GetClaimsByUser(string userId) =>
            _claims.Where(c => c.LecturerUserId == userId).OrderByDescending(c => c.SubmissionDate).ToList();

        public List<Claim> GetPendingClaims()
        {
            return _claims.Where(c => c.Status == ClaimStatus.Pending).ToList();
        }

        public List<Claim> GetCoordinatorApprovedClaims()
        {
            return _claims.Where(c => c.Status == ClaimStatus.CoordinatorApproved).ToList();
        }

        public Claim GetClaimById(int id) =>
            _claims.FirstOrDefault(c => c.ClaimId == id);

        public void UpdateClaimStatus(int claimId, ClaimStatus newStatus)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim != null)
            {
                claim.Status = newStatus;
            }
        }

        // Document methods
        public void AddDocument(SupportingDocument document)
        {
            document.SupportingDocumentId = _documentIdCounter++;
            document.UploadDate = DateTime.Now;
            _documents.Add(document);
        }

        public List<SupportingDocument> GetDocumentsByClaimId(int claimId) =>
            _documents.Where(d => d.ClaimId == claimId).ToList();
    }
}