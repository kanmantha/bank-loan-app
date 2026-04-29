using System.ComponentModel.DataAnnotations;

namespace BankLoanAPI.DTOs
{
    /// <summary>
    /// DTO for creating a new loan application
    /// </summary>
    public class CreateLoanApplicationDto
    {
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
        /// Employment status of the applicant
        /// </summary>
        [StringLength(50)]
        public string EmploymentStatus { get; set; }

        /// <summary>
        /// Additional notes or comments
        /// </summary>
        [StringLength(500)]
        public string Notes { get; set; }
    }

    /// <summary>
    /// DTO for updating loan application status
    /// </summary>
    public class UpdateLoanStatusDto
    {
        /// <summary>
        /// New status of the loan application (Pending, Approved, Rejected, UnderReview)
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        /// <summary>
        /// Additional notes or comments for the status update
        /// </summary>
        [StringLength(500)]
        public string Notes { get; set; }
    }

    /// <summary>
    /// DTO for returning loan application data
    /// </summary>
    public class LoanApplicationResponseDto
    {
        public int Id { get; set; }
        public string ApplicantName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal AnnualIncome { get; set; }
        public decimal LoanAmount { get; set; }
        public string LoanPurpose { get; set; }
        public int LoanTermMonths { get; set; }
        public string Status { get; set; }
        public int? CreditScore { get; set; }
        public string EmploymentStatus { get; set; }
        public string Notes { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
