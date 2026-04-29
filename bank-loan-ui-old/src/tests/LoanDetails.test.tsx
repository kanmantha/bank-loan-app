import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import LoanDetails from '../components/LoanDetails';
import * as api from '../services/api';
import type { LoanApplication } from '../types/LoanApplication';

/// <summary>
/// Mock the API module
/// </summary>
vi.mock('../services/api');

/// <summary>
/// Test suite for LoanDetails component
/// </summary>
describe('LoanDetails Component', () => {
  const mockApplication: LoanApplication = {
    id: 1,
    applicantName: 'John Doe',
    email: 'john@test.com',
    phoneNumber: '1234567890',
    annualIncome: 50000,
    loanAmount: 100000,
    loanPurpose: 'Home',
    loanTermMonths: 120,
    status: 'Pending',
    employmentStatus: 'Employed',
    notes: 'Test notes',
    applicationDate: '2024-01-15T10:00:00Z',
    creditScore: 700
  };

  const mockOnClose = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    (api.updateLoanApplicationStatus as any).mockResolvedValue({});
  });

  /// <summary>
  /// Test: Should render application details
  /// </summary>
  it('should render application details', () => {
    render(<LoanDetails application={mockApplication} onClose={mockOnClose} />);
    
    expect(screen.getByText('Loan Application Details')).toBeInTheDocument();
    expect(screen.getByText('John Doe')).toBeInTheDocument();
    expect(screen.getByText('john@test.com')).toBeInTheDocument();
    expect(screen.getByText('$100,000')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: Should call onClose when close button is clicked
  /// </summary>
  it('should call onClose when close button is clicked', () => {
    render(<LoanDetails application={mockApplication} onClose={mockOnClose} />);
    
    const closeButton = screen.getByText('×');
    fireEvent.click(closeButton);
    
    expect(mockOnClose).toHaveBeenCalled();
  });

  /// <summary>
  /// Test: Should call onClose when overlay is clicked
  /// </summary>
  it('should call onClose when overlay is clicked', () => {
    render(<LoanDetails application={mockApplication} onClose={mockOnClose} />);
    
    const overlay = screen.getByTestId('modal-overlay');
    if (overlay) {
      fireEvent.click(overlay);
      expect(mockOnClose).toHaveBeenCalled();
    }
  });

  /// <summary>
  /// Test: Should not close when modal content is clicked
  /// </summary>
  it('should not close when modal content is clicked', () => {
    render(<LoanDetails application={mockApplication} onClose={mockOnClose} />);
    
    const modalContent = screen.getByText('Loan Application Details').closest('.modal-content');
    if (modalContent) {
      fireEvent.click(modalContent);
      expect(mockOnClose).not.toHaveBeenCalled();
    }
  });

  /// <summary>
  /// Test: Should display formatted date
  /// </summary>
  it('should display formatted application date', () => {
    render(<LoanDetails application={mockApplication} onClose={mockOnClose} />);
    
    // Check that date is displayed (format may vary by locale)
    expect(screen.getByText(/1\/15\/2024/)).toBeInTheDocument();
  });

  /// <summary>
  /// Test: Should update status when update button is clicked
  /// </summary>
  it('should update status when update button is clicked', async () => {
    render(<LoanDetails application={mockApplication} onClose={mockOnClose} />);
    
    // Select new status
    const statusSelect = screen.getByLabelText(/New Status/);
    fireEvent.change(statusSelect, { target: { value: 'Approved' } });
    
    // Click update button
    const updateButton = screen.getByText('Update Status');
    fireEvent.click(updateButton);
    
    await waitFor(() => {
      expect(api.updateLoanApplicationStatus).toHaveBeenCalledWith(1, {
        status: 'Approved',
        notes: undefined
      });
    });
  });

  /// <summary>
  /// Test: Should disable update button while updating
  /// </summary>
  it('should disable update button while updating', async () => {
    (api.updateLoanApplicationStatus as any).mockImplementation(() => new Promise(() => {}));
    
    render(<LoanDetails application={mockApplication} onClose={mockOnClose} />);
    
    const statusSelect = screen.getByLabelText(/New Status/);
    fireEvent.change(statusSelect, { target: { value: 'Approved' } });
    
    const updateButton = screen.getByText('Update Status');
    fireEvent.click(updateButton);
    
    expect(screen.getByText('Updating...')).toBeDisabled();
  });

  /// <summary>
  /// Test: Should alert when status is not changed
  /// </summary>
  it('should alert when status is not changed', () => {
    vi.spyOn(window, 'alert').mockImplementation(() => {});
    
    render(<LoanDetails application={mockApplication} onClose={mockOnClose} />);
    
    const updateButton = screen.getByText('Update Status');
    fireEvent.click(updateButton);
    
    expect(window.alert).toHaveBeenCalledWith('Please select a different status to update');
  });

  /// <summary>
  /// Test: Should display credit score when provided
  /// </summary>
  it('should display credit score when provided', () => {
    render(<LoanDetails application={mockApplication} onClose={mockOnClose} />);
    
    expect(screen.getByText('700')).toBeInTheDocument();
  });

  /// <summary>
  /// Test: Should display "Not provided" when credit score is missing
  /// </summary>
  it('should display "Not provided" when credit score is missing', () => {
    const appWithoutCredit = { ...mockApplication, creditScore: undefined };
    render(<LoanDetails application={appWithoutCredit} onClose={mockOnClose} />);
    
    expect(screen.getByText('Not provided')).toBeInTheDocument();
  });
});
