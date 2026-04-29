using BankLoanAPI.Data;
using BankLoanAPI.DTOs;
using BankLoanAPI.Models;
using BankLoanAPI.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BankLoanAPI.Tests.Services
{
    /// <summary>
    /// Unit tests for LoanApplicationService
    /// </summary>
    public class LoanApplicationServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly LoanApplicationService _service;

        /// <summary>
        /// Initializes test context with InMemory database
        /// </summary>
        public LoanApplicationServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new LoanApplicationService(_context);
        }

        /// <summary>
        /// Cleans up the InMemory database after each test
        /// </summary>
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        /// <summary>
        /// Test: GetAllApplicationsAsync should return empty list when no applications exist
        /// </summary>
        [Fact]
        public async Task GetAllApplicationsAsync_ShouldReturnEmptyList_WhenNoApplications()
        {
            // Act
            var result = await _service.GetAllApplicationsAsync();

            // Assert
            Assert.Empty(result);
        }

        /// <summary>
        /// Test: GetAllApplicationsAsync should return all applications ordered by date
        /// </summary>
        [Fact]
        public async Task GetAllApplicationsAsync_ShouldReturnAllApplications_WhenApplicationsExist()
        {
            // Arrange
            var applications = new List<LoanApplication>
            {
                new() { ApplicantName = "John Doe", Email = "john@test.com", PhoneNumber = "1234567890", 
                       AnnualIncome = 50000, LoanAmount = 100000, LoanPurpose = "Home", 
                       LoanTermMonths = 120, ApplicationDate = DateTime.UtcNow.AddDays(-1) },
                new() { ApplicantName = "Jane Smith", Email = "jane@test.com", PhoneNumber = "0987654321", 
                       AnnualIncome = 60000, LoanAmount = 150000, LoanPurpose = "Car", 
                       LoanTermMonths = 60, ApplicationDate = DateTime.UtcNow }
            };

            _context.LoanApplications.AddRange(applications);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllApplicationsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Jane Smith", result.First().ApplicantName); // Most recent first
        }

        /// <summary>
        /// Test: GetApplicationByIdAsync should return null for non-existent ID
        /// </summary>
        [Fact]
        public async Task GetApplicationByIdAsync_ShouldReturnNull_WhenApplicationNotFound()
        {
            // Act
            var result = await _service.GetApplicationByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Test: GetApplicationByIdAsync should return correct application for valid ID
        /// </summary>
        [Fact]
        public async Task GetApplicationByIdAsync_ShouldReturnApplication_WhenIdExists()
        {
            // Arrange
            var application = new LoanApplication
            {
                ApplicantName = "Test User",
                Email = "test@test.com",
                PhoneNumber = "1234567890",
                AnnualIncome = 50000,
                LoanAmount = 100000,
                LoanPurpose = "Home",
                LoanTermMonths = 120
            };

            _context.LoanApplications.Add(application);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetApplicationByIdAsync(application.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test User", result.ApplicantName);
            Assert.Equal("test@test.com", result.Email);
        }

        /// <summary>
        /// Test: CreateApplicationAsync should create application with valid data
        /// </summary>
        [Fact]
        public async Task CreateApplicationAsync_ShouldCreateApplication_WithValidData()
        {
            // Arrange
            var createDto = new CreateLoanApplicationDto
            {
                ApplicantName = "New Applicant",
                Email = "applicant@test.com",
                PhoneNumber = "1111111111",
                AnnualIncome = 75000,
                LoanAmount = 200000,
                LoanPurpose = "Home",
                LoanTermMonths = 180,
                EmploymentStatus = "Employed",
                Notes = "Test application"
            };

            // Act
            var result = await _service.CreateApplicationAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Applicant", result.ApplicantName);
            Assert.Equal("applicant@test.com", result.Email);
            Assert.Equal("UnderReview", result.Status); // Eligible application goes to UnderReview
            Assert.True(result.Id > 0);

            // Verify it was saved to database
            var saved = await _context.LoanApplications.FindAsync(result.Id);
            Assert.NotNull(saved);
        }

        /// <summary>
        /// Test: CreateApplicationAsync should set status to Pending for ineligible applications
        /// </summary>
        [Fact]
        public async Task CreateApplicationAsync_ShouldSetPendingStatus_ForIneligibleApplication()
        {
            // Arrange
            var createDto = new CreateLoanApplicationDto
            {
                ApplicantName = "Low Income Applicant",
                Email = "low@test.com",
                PhoneNumber = "2222222222",
                AnnualIncome = 15000, // Below minimum
                LoanAmount = 100000,
                LoanPurpose = "Personal",
                LoanTermMonths = 60
            };

            // Act
            var result = await _service.CreateApplicationAsync(createDto);

            // Assert
            Assert.Equal("Pending", result.Status); // Not eligible, goes to Pending
        }

        /// <summary>
        /// Test: UpdateApplicationStatusAsync should return null for non-existent application
        /// </summary>
        [Fact]
        public async Task UpdateApplicationStatusAsync_ShouldReturnNull_WhenApplicationNotFound()
        {
            // Arrange
            var updateDto = new UpdateLoanStatusDto { Status = "Approved" };

            // Act
            var result = await _service.UpdateApplicationStatusAsync(999, updateDto);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Test: UpdateApplicationStatusAsync should update status successfully
        /// </summary>
        [Fact]
        public async Task UpdateApplicationStatusAsync_ShouldUpdateStatus_WhenApplicationExists()
        {
            // Arrange
            var application = new LoanApplication
            {
                ApplicantName = "Test User",
                Email = "test@test.com",
                PhoneNumber = "1234567890",
                AnnualIncome = 50000,
                LoanAmount = 100000,
                LoanPurpose = "Home",
                LoanTermMonths = 120,
                Status = "Pending"
            };

            _context.LoanApplications.Add(application);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateLoanStatusDto 
            { 
                Status = "Approved", 
                Notes = "Approved after review" 
            };

            // Act
            var result = await _service.UpdateApplicationStatusAsync(application.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Approved", result.Status);
            Assert.Equal("Approved after review", result.Notes);
            Assert.NotNull(result.UpdatedDate);

            // Verify update in database
            var updated = await _context.LoanApplications.FindAsync(application.Id);
            Assert.Equal("Approved", updated.Status);
        }

        /// <summary>
        /// Test: DeleteApplicationAsync should return false for non-existent application
        /// </summary>
        [Fact]
        public async Task DeleteApplicationAsync_ShouldReturnFalse_WhenApplicationNotFound()
        {
            // Act
            var result = await _service.DeleteApplicationAsync(999);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test: DeleteApplicationAsync should delete application successfully
        /// </summary>
        [Fact]
        public async Task DeleteApplicationAsync_ShouldDeleteApplication_WhenIdExists()
        {
            // Arrange
            var application = new LoanApplication
            {
                ApplicantName = "To Delete",
                Email = "delete@test.com",
                PhoneNumber = "1234567890",
                AnnualIncome = 50000,
                LoanAmount = 100000,
                LoanPurpose = "Home",
                LoanTermMonths = 120
            };

            _context.LoanApplications.Add(application);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteApplicationAsync(application.Id);

            // Assert
            Assert.True(result);

            // Verify deletion from database
            var deleted = await _context.LoanApplications.FindAsync(application.Id);
            Assert.Null(deleted);
        }

        /// <summary>
        /// Test: CheckLoanEligibility should return eligible for valid application
        /// </summary>
        [Theory]
        [InlineData(60000, 200000, 700, true)] // Good income, reasonable loan, good credit
        [InlineData(100000, 500000, 750, true)] // High income, reasonable loan, excellent credit
        public async Task CheckLoanEligibility_ShouldReturnEligible_ForValidApplications(
            decimal annualIncome, decimal loanAmount, int creditScore, bool expectedEligible)
        {
            // Act
            var (isEligible, reason) = _service.CheckLoanEligibility(annualIncome, loanAmount, creditScore);

            // Assert
            Assert.Equal(expectedEligible, isEligible);
        }

        /// <summary>
        /// Test: CheckLoanEligibility should return ineligible for invalid applications
        /// </summary>
        [Theory]
        [InlineData(15000, 100000, 650, false)] // Income too low
        [InlineData(50000, 300000, 580, false)] // Credit score too low
        [InlineData(50000, 2000000, 700, false)] // Loan amount too high
        [InlineData(30000, 200000, null, false)] // Income too low, no credit score
        public void CheckLoanEligibility_ShouldReturnIneligible_ForInvalidApplications(
            decimal annualIncome, decimal loanAmount, int? creditScore, bool expectedEligible)
        {
            // Act
            var (isEligible, reason) = _service.CheckLoanEligibility(annualIncome, loanAmount, creditScore);

            // Assert
            Assert.Equal(expectedEligible, isEligible);
            Assert.False(string.IsNullOrEmpty(reason));
        }

        /// <summary>
        /// Test: GetApplicationsByStatusAsync should filter by status correctly
        /// </summary>
        [Fact]
        public async Task GetApplicationsByStatusAsync_ShouldReturnFilteredResults()
        {
            // Arrange
            var applications = new List<LoanApplication>
            {
                new() { ApplicantName = "User1", Email = "user1@test.com", PhoneNumber = "1", 
                       AnnualIncome = 50000, LoanAmount = 100000, LoanPurpose = "Home", 
                       LoanTermMonths = 120, Status = "Pending" },
                new() { ApplicantName = "User2", Email = "user2@test.com", PhoneNumber = "2", 
                       AnnualIncome = 60000, LoanAmount = 150000, LoanPurpose = "Car", 
                       LoanTermMonths = 60, Status = "Approved" },
                new() { ApplicantName = "User3", Email = "user3@test.com", PhoneNumber = "3", 
                       AnnualIncome = 70000, LoanAmount = 200000, LoanPurpose = "Home", 
                       LoanTermMonths = 180, Status = "Pending" }
            };

            _context.LoanApplications.AddRange(applications);
            await _context.SaveChangesAsync();

            // Act
            var pendingApps = await _service.GetApplicationsByStatusAsync("Pending");
            var approvedApps = await _service.GetApplicationsByStatusAsync("Approved");

            // Assert
            Assert.Equal(2, pendingApps.Count());
            Assert.Single(approvedApps);
        }
    }
}
