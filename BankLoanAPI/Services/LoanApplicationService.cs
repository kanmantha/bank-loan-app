using BankLoanAPI.Data;
using BankLoanAPI.DTOs;
using BankLoanAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankLoanAPI.Services
{
    /// <summary>
    /// Service class implementing business logic for loan applications
    /// </summary>
    public class LoanApplicationService : ILoanApplicationService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of LoanApplicationService
        /// </summary>
        /// <param name="context">Database context</param>
        public LoanApplicationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all loan applications from the database
        /// </summary>
        public async Task<IEnumerable<LoanApplicationResponseDto>> GetAllApplicationsAsync()
        {
            var applications = await _context.LoanApplications
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            return applications.Select(MapToResponseDto);
        }

        /// <summary>
        /// Retrieves loan applications filtered by status
        /// </summary>
        public async Task<IEnumerable<LoanApplicationResponseDto>> GetApplicationsByStatusAsync(string status)
        {
            var applications = await _context.LoanApplications
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            return applications.Select(MapToResponseDto);
        }

        /// <summary>
        /// Retrieves a specific loan application by ID
        /// </summary>
        public async Task<LoanApplicationResponseDto?> GetApplicationByIdAsync(int id)
        {
            var application = await _context.LoanApplications.FindAsync(id);
            
            if (application == null)
                return null;

            return MapToResponseDto(application);
        }

        /// <summary>
        /// Creates a new loan application with eligibility check
        /// </summary>
        public async Task<LoanApplicationResponseDto> CreateApplicationAsync(CreateLoanApplicationDto createDto)
        {
            // Check loan eligibility
            var (isEligible, reason) = CheckLoanEligibility(createDto.AnnualIncome, createDto.LoanAmount, null);

            var application = new LoanApplication
            {
                ApplicantName = createDto.ApplicantName,
                Email = createDto.Email,
                PhoneNumber = createDto.PhoneNumber,
                AnnualIncome = createDto.AnnualIncome,
                LoanAmount = createDto.LoanAmount,
                LoanPurpose = createDto.LoanPurpose,
                LoanTermMonths = createDto.LoanTermMonths,
                EmploymentStatus = createDto.EmploymentStatus,
                Notes = createDto.Notes,
                Status = isEligible ? "UnderReview" : "Pending",
                ApplicationDate = DateTime.UtcNow
            };

            _context.LoanApplications.Add(application);
            await _context.SaveChangesAsync();

            return MapToResponseDto(application);
        }

        /// <summary>
        /// Updates the status of a loan application
        /// </summary>
        public async Task<LoanApplicationResponseDto?> UpdateApplicationStatusAsync(int id, UpdateLoanStatusDto updateDto)
        {
            var application = await _context.LoanApplications.FindAsync(id);
            
            if (application == null)
                return null;

            application.Status = updateDto.Status;
            application.Notes = updateDto.Notes ?? application.Notes;
            application.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponseDto(application);
        }

        /// <summary>
        /// Deletes a loan application from the database
        /// </summary>
        public async Task<bool> DeleteApplicationAsync(int id)
        {
            var application = await _context.LoanApplications.FindAsync(id);
            
            if (application == null)
                return false;

            _context.LoanApplications.Remove(application);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Checks loan eligibility based on income, loan amount, and credit score
        /// </summary>
        public (bool IsEligible, string Reason) CheckLoanEligibility(decimal annualIncome, decimal loanAmount, int? creditScore)
        {
            // Rule 1: Loan amount should not exceed 5 times annual income
            if (loanAmount > annualIncome * 5)
                return (false, "Loan amount exceeds 5 times annual income");

            // Rule 2: Minimum annual income requirement
            if (annualIncome < 20000)
                return (false, "Annual income must be at least $20,000");

            // Rule 3: Credit score check (if available)
            if (creditScore.HasValue && creditScore.Value < 600)
                return (false, "Credit score is below minimum requirement of 600");

            // Rule 4: Maximum loan amount check
            if (loanAmount > 1000000)
                return (false, "Loan amount exceeds maximum limit of $1,000,000");

            return (true, "Eligible for loan application");
        }

        /// <summary>
        /// Maps LoanApplication entity to LoanApplicationResponseDto
        /// </summary>
        private static LoanApplicationResponseDto MapToResponseDto(LoanApplication application)
        {
            return new LoanApplicationResponseDto
            {
                Id = application.Id,
                ApplicantName = application.ApplicantName,
                Email = application.Email,
                PhoneNumber = application.PhoneNumber,
                AnnualIncome = application.AnnualIncome,
                LoanAmount = application.LoanAmount,
                LoanPurpose = application.LoanPurpose,
                LoanTermMonths = application.LoanTermMonths,
                Status = application.Status,
                CreditScore = application.CreditScore,
                EmploymentStatus = application.EmploymentStatus,
                Notes = application.Notes,
                ApplicationDate = application.ApplicationDate,
                UpdatedDate = application.UpdatedDate
            };
        }
    }
}
