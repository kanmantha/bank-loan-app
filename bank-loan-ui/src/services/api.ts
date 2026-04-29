import axios from 'axios';
import type { LoanApplication, CreateLoanApplicationDto, UpdateLoanStatusDto, EligibilityResponse } from '../types/LoanApplication';

/// <summary>
/// Axios instance configured for the Bank Loan API
/// </summary>
const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

/// <summary>
/// Fetches all loan applications from the API
/// </summary>
/// <returns>Promise with array of loan applications</returns>
export const getLoanApplications = async (): Promise<LoanApplication[]> => {
  const response = await api.get<LoanApplication[]>('/LoanApplications');
  return response.data;
};

/// <summary>
/// Fetches loan applications filtered by status
/// </summary>
/// <param name="status">Status to filter by</param>
/// <returns>Promise with filtered loan applications</returns>
export const getLoanApplicationsByStatus = async (status: string): Promise<LoanApplication[]> => {
  const response = await api.get<LoanApplication[]>(`/LoanApplications/status/${status}`);
  return response.data;
};

/// <summary>
/// Fetches a single loan application by ID
/// </summary>
/// <param name="id">Application ID</param>
/// <returns>Promise with loan application data</returns>
export const getLoanApplicationById = async (id: number): Promise<LoanApplication> => {
  const response = await api.get<LoanApplication>(`/LoanApplications/${id}`);
  return response.data;
};

/// <summary>
/// Creates a new loan application
/// </summary>
/// <param name="application">Application data to create</param>
/// <returns>Promise with created loan application</returns>
export const createLoanApplication = async (application: CreateLoanApplicationDto): Promise<LoanApplication> => {
  const response = await api.post<LoanApplication>('/LoanApplications', application);
  return response.data;
};

/// <summary>
/// Updates the status of a loan application
/// </summary>
/// <param name="id">Application ID</param>
/// <param name="statusUpdate">Status update data</param>
/// <returns>Promise with updated loan application</returns>
export const updateLoanApplicationStatus = async (
  id: number, 
  statusUpdate: UpdateLoanStatusDto
): Promise<LoanApplication> => {
  const response = await api.put<LoanApplication>(`/LoanApplications/${id}/status`, statusUpdate);
  return response.data;
};

/// <summary>
/// Deletes a loan application
/// </summary>
/// <param name="id">Application ID to delete</param>
/// <returns>Promise indicating success</returns>
export const deleteLoanApplication = async (id: number): Promise<void> => {
  await api.delete(`/LoanApplications/${id}`);
};

/// <summary>
/// Checks loan eligibility based on applicant details
/// </summary>
/// <param name="annualIncome">Annual income</param>
/// <param name="loanAmount">Loan amount requested</param>
/// <param name="creditScore">Credit score (optional)</param>
/// <returns>Promise with eligibility result</returns>
export const checkLoanEligibility = async (
  annualIncome: number,
  loanAmount: number,
  creditScore?: number
): Promise<EligibilityResponse> => {
  const params = new URLSearchParams({
    annualIncome: annualIncome.toString(),
    loanAmount: loanAmount.toString(),
  });
  
  if (creditScore !== undefined) {
    params.append('creditScore', creditScore.toString());
  }

  const response = await api.get<EligibilityResponse>(`/LoanApplications/check-eligibility?${params}`);
  return response.data;
};
