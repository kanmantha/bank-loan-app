using BankLoanAPI.Controllers;
using BankLoanAPI.DTOs;
using BankLoanAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BankLoanAPI.Tests.Controllers
{
    /// <summary>
    /// Unit tests for LoanApplicationsController
    /// </summary>
    public class LoanApplicationsControllerTests
    {
        private readonly Mock<ILoanApplicationService> _mockService;
        private readonly LoanApplicationsController _controller;

        /// <summary>
        /// Initializes test with mocked service
        /// </summary>
        public LoanApplicationsControllerTests()
        {
            _mockService = new Mock<ILoanApplicationService>();
            _controller = new LoanApplicationsController(_mockService.Object);
        }

        /// <summary>
        /// Test: GetAllApplications should return Ok with applications
        /// </summary>
        [Fact]
        public async Task GetAllApplications_ShouldReturnOk_WithApplications()
        {
            // Arrange
            var applications = new List<LoanApplicationResponseDto>
            {
                new() { Id = 1, ApplicantName = "John Doe", Email = "john@test.com" },
                new() { Id = 2, ApplicantName = "Jane Smith", Email = "jane@test.com" }
            };

            _mockService.Setup(s => s.GetAllApplicationsAsync())
                .ReturnsAsync(applications);

            // Act
            var result = await _controller.GetAllApplications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedApps = Assert.IsAssignableFrom<IEnumerable<LoanApplicationResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedApps.Count());
        }

        /// <summary>
        /// Test: GetApplicationById should return NotFound for non-existent ID
        /// </summary>
        [Fact]
        public async Task GetApplicationById_ShouldReturnNotFound_WhenApplicationDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetApplicationByIdAsync(999))
                .ReturnsAsync((LoanApplicationResponseDto?)null);

            // Act
            var result = await _controller.GetApplicationById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        /// <summary>
        /// Test: GetApplicationById should return Ok with application for valid ID
        /// </summary>
        [Fact]
        public async Task GetApplicationById_ShouldReturnOk_WhenApplicationExists()
        {
            // Arrange
            var application = new LoanApplicationResponseDto
            {
                Id = 1,
                ApplicantName = "Test User",
                Email = "test@test.com"
            };

            _mockService.Setup(s => s.GetApplicationByIdAsync(1))
                .ReturnsAsync(application);

            // Act
            var result = await _controller.GetApplicationById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedApp = Assert.IsType<LoanApplicationResponseDto>(okResult.Value);
            Assert.Equal(1, returnedApp.Id);
        }

        /// <summary>
        /// Test: CreateApplication should return CreatedAtAction for valid data
        /// </summary>
        [Fact]
        public async Task CreateApplication_ShouldReturnCreatedAtAction_WithValidData()
        {
            // Arrange
            var createDto = new CreateLoanApplicationDto
            {
                ApplicantName = "New User",
                Email = "new@test.com",
                PhoneNumber = "1234567890",
                AnnualIncome = 50000,
                LoanAmount = 100000,
                LoanPurpose = "Home",
                LoanTermMonths = 120
            };

            var createdApp = new LoanApplicationResponseDto
            {
                Id = 1,
                ApplicantName = "New User",
                Email = "new@test.com",
                Status = "UnderReview"
            };

            _mockService.Setup(s => s.CreateApplicationAsync(createDto))
                .ReturnsAsync(createdApp);

            // Act
            var result = await _controller.CreateApplication(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedApp = Assert.IsType<LoanApplicationResponseDto>(createdResult.Value);
            Assert.Equal(1, returnedApp.Id);
            Assert.Equal("New User", returnedApp.ApplicantName);
        }

        /// <summary>
        /// Test: CreateApplication should return BadRequest for invalid model
        /// </summary>
        [Fact]
        public async Task CreateApplication_ShouldReturnBadRequest_ForInvalidModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("ApplicantName", "Required");
            var createDto = new CreateLoanApplicationDto();

            // Act
            var result = await _controller.CreateApplication(createDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// Test: UpdateApplicationStatus should return NotFound for non-existent application
        /// </summary>
        [Fact]
        public async Task UpdateApplicationStatus_ShouldReturnNotFound_WhenApplicationDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateLoanStatusDto { Status = "Approved" };

            _mockService.Setup(s => s.UpdateApplicationStatusAsync(999, updateDto))
                .ReturnsAsync((LoanApplicationResponseDto?)null);

            // Act
            var result = await _controller.UpdateApplicationStatus(999, updateDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        /// <summary>
        /// Test: UpdateApplicationStatus should return Ok with updated application
        /// </summary>
        [Fact]
        public async Task UpdateApplicationStatus_ShouldReturnOk_WhenApplicationExists()
        {
            // Arrange
            var updateDto = new UpdateLoanStatusDto 
            { 
                Status = "Approved", 
                Notes = "Approved after review" 
            };

            var updatedApp = new LoanApplicationResponseDto
            {
                Id = 1,
                Status = "Approved",
                Notes = "Approved after review"
            };

            _mockService.Setup(s => s.UpdateApplicationStatusAsync(1, updateDto))
                .ReturnsAsync(updatedApp);

            // Act
            var result = await _controller.UpdateApplicationStatus(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedApp = Assert.IsType<LoanApplicationResponseDto>(okResult.Value);
            Assert.Equal("Approved", returnedApp.Status);
        }

        /// <summary>
        /// Test: DeleteApplication should return NotFound for non-existent application
        /// </summary>
        [Fact]
        public async Task DeleteApplication_ShouldReturnNotFound_WhenApplicationDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteApplicationAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteApplication(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        /// <summary>
        /// Test: DeleteApplication should return NoContent when deleted successfully
        /// </summary>
        [Fact]
        public async Task DeleteApplication_ShouldReturnNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteApplicationAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteApplication(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Test: CheckEligibility should return Ok with eligibility result
        /// </summary>
        [Theory]
        [InlineData(60000, 200000, 700)]
        [InlineData(50000, 100000, null)]
        public async Task CheckEligibility_ShouldReturnOk_WithEligibilityResult(
            decimal annualIncome, decimal loanAmount, int? creditScore)
        {
            // Act - Calling the method directly as it's not async
            var result = _controller.CheckEligibility(annualIncome, loanAmount, creditScore);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        /// <summary>
        /// Test: GetApplicationsByStatus should return Ok with filtered applications
        /// </summary>
        [Fact]
        public async Task GetApplicationsByStatus_ShouldReturnOk_WithFilteredApplications()
        {
            // Arrange
            var applications = new List<LoanApplicationResponseDto>
            {
                new() { Id = 1, Status = "Pending" },
                new() { Id = 2, Status = "Pending" }
            };

            _mockService.Setup(s => s.GetApplicationsByStatusAsync("Pending"))
                .ReturnsAsync(applications);

            // Act
            var result = await _controller.GetApplicationsByStatus("Pending");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedApps = Assert.IsAssignableFrom<IEnumerable<LoanApplicationResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedApps.Count());
        }
    }
}
