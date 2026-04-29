/// <summary>
/// Represents a loan application in the system
/// </summary>
export interface LoanApplication {
  id: number;
  applicantName: string;
  email: string;
  phoneNumber: string;
  annualIncome: number;
  loanAmount: number;
  loanPurpose: string;
  loanTermMonths: number;
  status: LoanStatus;
  creditScore?: number;
  employmentStatus?: string;
  notes?: string;
  applicationDate: string;
  updatedDate?: string;
}

/// <summary>
/// Status values for loan applications
/// </summary>
export type LoanStatus = 'Pending' | 'UnderReview' | 'Approved' | 'Rejected';

/// <summary>
/// DTO for creating a new loan application
/// </summary>
export interface CreateLoanApplicationDto {
  applicantName: string;
  email: string;
  phoneNumber: string;
  annualIncome: number;
  loanAmount: number;
  loanPurpose: string;
  loanTermMonths: number;
  employmentStatus?: string;
  notes?: string;
}

/// <summary>
/// DTO for updating loan application status
/// </summary>
export interface UpdateLoanStatusDto {
  status: LoanStatus;
  notes?: string;
}

/// <summary>
/// DTO for loan eligibility check
/// </summary>
export interface LoanEligibilityDto {
  annualIncome: number;
  loanAmount: number;
  creditScore?: number;
}

/// <summary>
/// Response from eligibility check
/// </summary>
export interface EligibilityResponse {
  isEligible: boolean;
  reason: string;
  annualIncome: number;
  loanAmount: number;
  creditScore?: number;
}
