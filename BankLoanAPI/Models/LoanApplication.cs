using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLoanAPI.Models
{
    /// <summary>
    /// Represents a loan application in the banking system
    /// </summary>
    public class LoanApplication
    {
        /// <summary>
        /// Unique identifier for the loan application
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Full name of the applicant
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ApplicantName { get; set; }

        /// <summary>
        /// Email address of the applicant
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Phone number of the applicant
        /// </summary>
        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Annual income of the applicant
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal AnnualIncome { get; set; }

        /// <summary>
        /// Amount of loan requested
        /// </summary>
        [Required]
        [Range(1000, 10000000)]
        public decimal LoanAmount { get; set; }

        /// <summary>
        /// Purpose of the loan (e.g., Home, Car, Education, Personal)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LoanPurpose { get; set; }

        /// <summary>
        /// Loan term in months
        /// </summary>
        [Required]
        [Range(6, 360)]
        public int LoanTermMonths { get; set; }

        /// <summary>
        /// Current status of the loan application (Pending, Approved, Rejected, UnderReview)
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Credit score of the applicant
        /// </summary>
        [Range(300, 850)]
        public int? CreditScore { get; set; }

        /// <summary>
        /// Employment status of the applicant
        /// </summary>
        [StringLength(50)]
        public string EmploymentStatus { get; set; }

        /// <summary>
        /// Additional notes or comments
        /// </summary>
        [StringLength(500)]
        public string Notes { get; set; }

        /// <summary>
        /// Date when the application was submitted
        /// </summary>
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the application was last updated
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
    }
}
