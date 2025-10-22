using System;

namespace ProgrammingPOE.Models
{
    public class SupportingDocument
    {
        public int SupportingDocumentId { get; set; }
        public int ClaimId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}