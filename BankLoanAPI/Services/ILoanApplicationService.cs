using BankLoanAPI.DTOs;

namespace BankLoanAPI.Services
{
    /// <summary>
    /// Interface defining the business logic operations for loan applications
    /// </summary>
    public interface ILoanApplicationService
    {
        /// <summary>
        /// Retrieves all loan applications
        /// </summary>
        /// <returns>List of loan application response DTOs</returns>
        Task<IEnumerable<LoanApplicationResponseDto>> GetAllApplicationsAsync();

        /// <summary>
        /// Retrieves loan applications filtered by status
        /// </summary>
        /// <param name="status">Status to filter by</param>
        /// <returns>List of filtered loan application response DTOs</returns>
        Task<IEnumerable<LoanApplicationResponseDto>> GetApplicationsByStatusAsync(string status);

        /// <summary>
        /// Retrieves a specific loan application by ID
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>Loan application response DTO or null if not found</returns>
        Task<LoanApplicationResponseDto?> GetApplicationByIdAsync(int id);

        /// <summary>
        /// Creates a new loan application
        /// </summary>
        /// <param name="createDto">Loan application creation data</param>
        /// <returns>Created loan application response DTO</returns>
        Task<LoanApplicationResponseDto> CreateApplicationAsync(CreateLoanApplicationDto createDto);

        /// <summary>
        /// Updates the status of a loan application
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <param name="updateDto">Status update data</param>
        /// <returns>Updated loan application response DTO or null if not found</returns>
        Task<LoanApplicationResponseDto?> UpdateApplicationStatusAsync(int id, UpdateLoanStatusDto updateDto);

        /// <summary>
        /// Deletes a loan application
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>True if deleted successfully, false if not found</returns>
        Task<bool> DeleteApplicationAsync(int id);

        /// <summary>
        /// Calculates the loan eligibility based on applicant details
        /// </summary>
        /// <param name="annualIncome">Annual income of applicant</param>
        /// <param name="loanAmount">Requested loan amount</param>
        /// <param name="creditScore">Credit score of applicant</param>
        /// <returns>Eligibility result with reason</returns>
        (bool IsEligible, string Reason) CheckLoanEligibility(decimal annualIncome, decimal loanAmount, int? creditScore);
    }
}
