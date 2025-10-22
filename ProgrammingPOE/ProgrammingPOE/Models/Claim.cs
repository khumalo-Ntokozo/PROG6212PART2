using System;
using System.Collections.Generic;

namespace ProgrammingPOE.Models
{
    public enum ClaimStatus
    {
        Pending,
        CoordinatorApproved,
        CoordinatorRejected,
        ManagerApproved,
        ManagerRejected
    }

    public class Claim
    {
        public int ClaimId { get; set; }
        public string LecturerUserId { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public List<SupportingDocument> Documents { get; set; } = new List<SupportingDocument>();
    }
}