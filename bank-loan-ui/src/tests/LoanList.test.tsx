import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import LoanList from '../components/LoanList';
import * as api from '../services/api';
import type { LoanApplication } from '../types/LoanApplication';

/// <summary>
/// Mock the API module
/// </summary>
vi.mock('../services/api');

/// <summary>
/// Test suite for LoanList component
/// </summary>
describe('LoanList Component', () => {
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
    },
    {
      id: 2,
      applicantName: 'Jane Smith',
      email: 'jane@test.com',
      phoneNumber: '0987654321',
      annualIncome: 60000,
      loanAmount: 150000,
      loanPurpose: 'Car',
      loanTermMonths: 60,
      status: 'Approved',
      applicationDate: '2024-01-16T10:00:00Z'
    }
  ];

  const mockOnRefresh = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  /// <summary>
  /// Test: Should render list of applications
  /// </summary>
  it('should render list of applications', () => {
    render(<LoanList applications={mockApplications} onRefresh={mockOnRefresh} />);
    
    expect(screen.getByText('John Doe')).toBeInTheDocument();
    expect(screen.getByText('Jane Smith')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: Should display correct count in header
  /// </summary>
  it('should display correct application count', () => {
    render(<LoanList applications={mockApplications} onRefresh={mockOnRefresh} />);
    
    expect(screen.getByText('Loan Applications (2)')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: Should show no data message when empty
  /// </summary>
  it('should show no data message when applications array is empty', () => {
    render(<LoanList applications={[]} onRefresh={mockOnRefresh} />);
    
    expect(screen.getByText('No loan applications found.')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: Should display correct status badges
  /// </summary>
  it('should display correct status badges', () => {
    render(<LoanList applications={mockApplications} onRefresh={mockOnRefresh} />);
    
    expect(screen.getByText('Pending')).toBeInTheDocument();
    expect(screen.getByText('Approved')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: Should call delete API when delete button clicked
  /// </summary>
  it('should call delete API when delete button is clicked', async () => {
    (api.deleteLoanApplication as any).mockResolvedValue(undefined);
    
    render(<LoanList applications={mockApplications} onRefresh={mockOnRefresh} />);
    
    const deleteButtons = screen.getAllByText('Delete');
    fireEvent.click(deleteButtons[0]);
    
    // Handle confirmation dialog
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    
    await waitFor(() => {
      expect(api.deleteLoanApplication).toHaveBeenCalledWith(1);
    });
  });

  /// <summary>
  /// Test: Should not delete when confirmation is cancelled
  /// </summary>
  it('should not delete when confirmation is cancelled', () => {
    render(<LoanList applications={mockApplications} onRefresh={mockOnRefresh} />);
    
    vi.spyOn(window, 'confirm').mockReturnValue(false);
    
    const deleteButtons = screen.getAllByText('Delete');
    fireEvent.click(deleteButtons[0]);
    
    expect(api.deleteLoanApplication).not.toHaveBeenCalled();
  });

  /// <summary>
  /// Test: Should format currency correctly
  /// </summary>
  it('should format loan amount as currency', () => {
    render(<LoanList applications={mockApplications} onRefresh={mockOnRefresh} />);
    
    expect(screen.getByText('$100,000')).toBeInTheDocument();
    expect(screen.getByText('$150,000')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: Should show LoanDetails when View Details is clicked
  /// </summary>
  it('should show LoanDetails modal when View Details is clicked', () => {
    render(<LoanList applications={mockApplications} onRefresh={mockOnRefresh} />);
    
    const viewButtons = screen.getAllByText('View Details');
    fireEvent.click(viewButtons[0]);
    
    expect(screen.getByText('Loan Application Details')).toBeInTheDocument();
  });
});
