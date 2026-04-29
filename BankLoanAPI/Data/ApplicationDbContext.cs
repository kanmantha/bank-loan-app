using BankLoanAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankLoanAPI.Data
{
    /// <summary>
    /// Entity Framework database context for the Bank Loan Application system
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext
        /// </summary>
        /// <param name="options">Database context options</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet for LoanApplications table
        /// </summary>
        public DbSet<LoanApplication> LoanApplications { get; set; }

        /// <summary>
        /// Configures the model and entity relationships
        /// </summary>
        /// <param name="modelBuilder">Model builder for entity configuration</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure LoanApplication entity
            modelBuilder.Entity<LoanApplication>(entity =>
            {
                entity.ToTable("LoanApplications");
                
                // Configure primary key
                entity.HasKey(e => e.Id);
                
                // Configure column properties
                entity.Property(e => e.ApplicantName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.Property(e => e.LoanPurpose)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");
                
                entity.Property(e => e.ApplicationDate)
                    .HasDefaultValueSql("GETUTCDATE()");
                
                // Create index on Email for faster lookups
                entity.HasIndex(e => e.Email);
                
                // Create index on Status for filtering
                entity.HasIndex(e => e.Status);
            });
        }
    }
}
