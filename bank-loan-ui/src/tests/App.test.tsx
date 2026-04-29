import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import App from '../App';
import * as api from '../services/api';
import type { LoanApplication } from '../types/LoanApplication';

/// <summary>
/// Mock the API module to isolate component tests
/// </summary>
vi.mock('../services/api');

/// <summary>
/// Test suite for App component
/// </summary>
describe('App Component', () => {
  const mockApplications: LoanApplication[] = [
    {
      id: 1,
      applicantName: 'John Doe',
      email: 'john@test.com',
      phoneNumber: '1234567890',
      annualIncome: 50000,
      loanAmount: 100000,
      loanPurpose: 'Home',
      loanTermMonths: 120,
      status: 'Pending',
      applicationDate: '2024-01-15T10:00:00Z'
    }
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    (api.getLoanApplications as any).mockResolvedValue(mockApplications);
  });

  /// <summary>
  /// Test: App should render header
  /// </summary>
  it('should render the header', () => {
    render(<App />);
    expect(screen.getByText('Bank Loan Application Management')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: App should fetch and display applications on load
  /// </summary>
  it('should fetch and display applications on load', async () => {
    render(<App />);
    
    await waitFor(() => {
      expect(api.getLoanApplications).toHaveBeenCalled();
    });
    
    expect(screen.getByText('John Doe')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: App should show loading state initially
  /// </summary>
  it('should show loading state initially', () => {
    (api.getLoanApplications as any).mockImplementation(() => new Promise(() => {}));
    render(<App />);
    
    expect(screen.getByText('Loading applications...')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: App should display error message when API fails
  /// </summary>
  it('should display error message when API fails', async () => {
    (api.getLoanApplications as any).mockRejectedValue(new Error('API Error'));
    
    render(<App />);
    
    await waitFor(() => {
      expect(screen.getByText(/Failed to fetch loan applications/)).toBeInTheDocument();
    });
  });

  /// <summary>
  /// Test: App should filter applications by status
  /// </summary>
  it('should filter applications by status', async () => {
    const user = userEvent.setup();
    (api.getLoanApplicationsByStatus as any).mockResolvedValue([]);
    
    render(<App />);
    
    const filterSelect = screen.getByLabelText(/Filter by Status/);
    await user.selectOptions(filterSelect, 'Approved');
    
    await waitFor(() => {
      expect(api.getLoanApplicationsByStatus).toHaveBeenCalledWith('Approved');
    });
  });

  /// <summary>
  /// Test: App should toggle form visibility
  /// </summary>
  it('should toggle form visibility when button is clicked', async () => {
    const user = userEvent.setup();
    
    render(<App />);
    
    const newAppButton = screen.getByText('New Application');
    await user.click(newAppButton);
    
    expect(screen.getByText('Cancel')).toBeInTheDocument();
    
    const cancelButton = screen.getByText('Cancel');
    await user.click(cancelButton);
    
    expect(screen.getByText('New Application')).toBeInTheDocument();
  });
});
