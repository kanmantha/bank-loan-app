import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import LoanForm from '../components/LoanForm';
import * as api from '../services/api';

/// <summary>
/// Mock the API module
/// </summary>
vi.mock('../services/api');

/// <summary>
/// Test suite for LoanForm component
/// </summary>
describe('LoanForm Component', () => {
  const mockOnSuccess = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    (api.createLoanApplication as any).mockResolvedValue({});
    (api.checkLoanEligibility as any).mockResolvedValue({ isEligible: true, reason: 'Eligible' });
  });

  /// <summary>
  /// Test: Should render all form fields
  /// </summary>
  it('should render all form fields', () => {
    render(<LoanForm onSuccess={mockOnSuccess} />);
    
    expect(screen.getByLabelText(/Applicant Name/)).toBeInTheDocument();
    expect(screen.getByLabelText(/Email/)).toBeInTheDocument();
    expect(screen.getByLabelText(/Phone Number/)).toBeInTheDocument();
    expect(screen.getByLabelText(/Annual Income/)).toBeInTheDocument();
    expect(screen.getByLabelText(/Loan Amount/)).toBeInTheDocument();
    expect(screen.getByLabelText(/Loan Purpose/)).toBeInTheDocument();
    expect(screen.getByLabelText(/Loan Term/)).toBeInTheDocument();
  });

  /// <summary>
  /// Test: Should update form data on input change
  /// </summary>
  it('should update form data on input change', async () => {
    const user = userEvent.setup();
    render(<LoanForm onSuccess={mockOnSuccess} />);
    
    const nameInput = screen.getByLabelText(/Applicant Name/);
    await user.type(nameInput, 'John Doe');
    
    expect(nameInput).toHaveValue('John Doe');
  });

  /// <summary>
  /// Test: Should call createLoanApplication on form submit
  /// </summary>
  it('should call createLoanApplication on form submit', async () => {
    const user = userEvent.setup();
    render(<LoanForm onSuccess={mockOnSuccess} />);
    
    // Fill required fields
    await user.type(screen.getByLabelText(/Applicant Name/), 'John Doe');
    await user.type(screen.getByLabelText(/Email/), 'john@test.com');
    await user.type(screen.getByLabelText(/Phone Number/), '1234567890');
    await user.type(screen.getByLabelText(/Annual Income/), '50000');
    await user.type(screen.getByLabelText(/Loan Amount/), '100000');
    
    // Select loan purpose
    await user.selectOptions(screen.getByLabelText(/Loan Purpose/), 'Home');
    
    // Submit form
    const submitButton = screen.getByText('Submit Application');
    await user.click(submitButton);
    
    await waitFor(() => {
      expect(api.createLoanApplication).toHaveBeenCalled();
    });
  });

  /// <summary>
  /// Test: Should call onSuccess after successful submission
  /// </summary>
  it('should call onSuccess after successful submission', async () => {
    const user = userEvent.setup();
    (api.createLoanApplication as any).mockResolvedValue({ id: 1 });
    
    render(<LoanForm onSuccess={mockOnSuccess} />);
    
    // Fill form
    await user.type(screen.getByLabelText(/Applicant Name/), 'John Doe');
    await user.type(screen.getByLabelText(/Email/), 'john@test.com');
    await user.type(screen.getByLabelText(/Phone Number/), '1234567890');
    await user.type(screen.getByLabelText(/Annual Income/), '50000');
    await user.type(screen.getByLabelText(/Loan Amount/), '100000');
    await user.selectOptions(screen.getByLabelText(/Loan Purpose/), 'Home');
    
    // Submit
    await user.click(screen.getByText('Submit Application'));
    
    await waitFor(() => {
      expect(mockOnSuccess).toHaveBeenCalled();
    });
  });

  /// <summary>
  /// Test: Should display error message on API failure
  /// </summary>
  it('should display error message on API failure', async () => {
    const user = userEvent.setup();
    (api.createLoanApplication as any).mockRejectedValue({
      response: { data: { message: 'Failed to create' } }
    });
    
    render(<LoanForm onSuccess={mockOnSuccess} />);
    
    // Fill form
    await user.type(screen.getByLabelText(/Applicant Name/), 'John Doe');
    await user.type(screen.getByLabelText(/Email/), 'john@test.com');
    await user.type(screen.getByLabelText(/Phone Number/), '1234567890');
    await user.type(screen.getByLabelText(/Annual Income/), '50000');
    await user.type(screen.getByLabelText(/Loan Amount/), '100000');
    await user.selectOptions(screen.getByLabelText(/Loan Purpose/), 'Home');
    
    // Submit
    await user.click(screen.getByText('Submit Application'));
    
    await waitFor(() => {
      expect(screen.getByText('Failed to create')).toBeInTheDocument();
    });
  });

  /// <summary>
  /// Test: Should check eligibility when button is clicked
  /// </summary>
  it('should check eligibility when button is clicked', async () => {
    const user = userEvent.setup();
    render(<LoanForm onSuccess={mockOnSuccess} />);
    
    // Enter income and loan amount
    await user.type(screen.getByLabelText(/Annual Income/), '50000');
    await user.type(screen.getByLabelText(/Loan Amount/), '100000');
    
    // Click check eligibility
    const checkButton = screen.getByText('Check Eligibility');
    await user.click(checkButton);
    
    await waitFor(() => {
      expect(api.checkLoanEligibility).toHaveBeenCalledWith(50000, 100000, undefined);
    });
  });

  /// <summary>
  /// Test: Should display eligibility result
  /// </summary>
  it('should display eligibility result', async () => {
    const user = userEvent.setup();
    (api.checkLoanEligibility as any).mockResolvedValue({ 
      isEligible: false, 
      reason: 'Income too low' 
    });
    
    render(<LoanForm onSuccess={mockOnSuccess} />);
    
    // Enter income and loan amount
    await user.type(screen.getByLabelText(/Annual Income/), '15000');
    await user.type(screen.getByLabelText(/Loan Amount/), '100000');
    
    // Check eligibility
    await user.click(screen.getByText('Check Eligibility'));
    
    await waitFor(() => {
      expect(screen.getByText(/Not Eligible/)).toBeInTheDocument();
      expect(screen.getByText('Income too low')).toBeInTheDocument();
    });
  });

  /// <summary>
  /// Test: Should disable submit button while loading
  /// </summary>
  it('should disable submit button while loading', async () => {
    const user = userEvent.setup();
    (api.createLoanApplication as any).mockImplementation(() => new Promise(() => {}));
    
    render(<LoanForm onSuccess={mockOnSuccess} />);
    
    // Fill form
    await user.type(screen.getByLabelText(/Applicant Name/), 'John Doe');
    await user.type(screen.getByLabelText(/Email/), 'john@test.com');
    await user.type(screen.getByLabelText(/Phone Number/), '1234567890');
    await user.type(screen.getByLabelText(/Annual Income/), '50000');
    await user.type(screen.getByLabelText(/Loan Amount/), '100000');
    await user.selectOptions(screen.getByLabelText(/Loan Purpose/), 'Home');
    
    // Submit
    await user.click(screen.getByText('Submit Application'));
    
    expect(screen.getByText('Submitting...')).toBeInTheDocument();
    expect(screen.getByText('Submitting...')).toBeDisabled();
  });
});
