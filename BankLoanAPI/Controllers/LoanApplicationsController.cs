using BankLoanAPI.DTOs;
using BankLoanAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankLoanAPI.Controllers
{
    /// <summary>
    /// API controller for managing loan applications
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class LoanApplicationsController : ControllerBase
    {
        private readonly ILoanApplicationService _loanService;

        /// <summary>
        /// Initializes a new instance of LoanApplicationsController
        /// </summary>
        /// <param name="loanService">Loan application service</param>
        public LoanApplicationsController(ILoanApplicationService loanService)
        {
            _loanService = loanService;
        }

        /// <summary>
        /// Retrieves all loan applications
        /// </summary>
        /// <returns>List of loan applications</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LoanApplicationResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllApplications()
        {
            var applications = await _loanService.GetAllApplicationsAsync();
            return Ok(applications);
        }

        /// <summary>
        /// Retrieves loan applications filtered by status
        /// </summary>
        /// <param name="status">Status to filter by (Pending, Approved, Rejected, UnderReview)</param>
        /// <returns>List of filtered loan applications</returns>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<LoanApplicationResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationsByStatus(string status)
        {
            var applications = await _loanService.GetApplicationsByStatusAsync(status);
            return Ok(applications);
        }

        /// <summary>
        /// Retrieves a specific loan application by ID
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>Loan application details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LoanApplicationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetApplicationById(int id)
        {
            var application = await _loanService.GetApplicationByIdAsync(id);
            
            if (application == null)
                return NotFound(new { message = $"Loan application with ID {id} not found" });

            return Ok(application);
        }

        /// <summary>
        /// Creates a new loan application
        /// </summary>
        /// <param name="createDto">Loan application creation data</param>
        /// <returns>Created loan application</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LoanApplicationResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateApplication([FromBody] CreateLoanApplicationDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var application = await _loanService.CreateApplicationAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetApplicationById), 
                new { id = application.Id }, 
                application);
        }

        /// <summary>
        /// Updates the status of a loan application
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <param name="updateDto">Status update data</param>
        /// <returns>Updated loan application</returns>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(LoanApplicationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateApplicationStatus(int id, [FromBody] UpdateLoanStatusDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var application = await _loanService.UpdateApplicationStatusAsync(id, updateDto);
            
            if (application == null)
                return NotFound(new { message = $"Loan application with ID {id} not found" });

            return Ok(application);
        }

        /// <summary>
        /// Deletes a loan application
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var result = await _loanService.DeleteApplicationAsync(id);
            
            if (!result)
                return NotFound(new { message = $"Loan application with ID {id} not found" });

            return NoContent();
        }

        /// <summary>
        /// Checks loan eligibility based on applicant details
        /// </summary>
        /// <param name="annualIncome">Annual income</param>
        /// <param name="loanAmount">Loan amount requested</param>
        /// <param name="creditScore">Credit score (optional)</param>
        /// <returns>Eligibility result</returns>
        [HttpGet("check-eligibility")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult CheckEligibility([FromQuery] decimal annualIncome, [FromQuery] decimal loanAmount, [FromQuery] int? creditScore = null)
        {
            var (isEligible, reason) = _loanService.CheckLoanEligibility(annualIncome, loanAmount, creditScore);
            
            return Ok(new 
            { 
                isEligible, 
                reason,
                annualIncome,
                loanAmount,
                creditScore 
            });
        }
    }
}
