import { useState } from 'react';
import { createLoanApplication, checkLoanEligibility } from '../services/api';
import type { CreateLoanApplicationDto, EligibilityResponse } from '../types/LoanApplication';

/// <summary>
/// Props for LoanForm component
/// </summary>
interface LoanFormProps {
  onSuccess: () => void;
}

/// <summary>
/// Component for creating new loan applications with eligibility check
/// </summary>
const LoanForm = ({ onSuccess }: LoanFormProps) => {
  const [formData, setFormData] = useState<CreateLoanApplicationDto>({
    applicantName: '',
    email: '',
    phoneNumber: '',
    annualIncome: 0,
    loanAmount: 0,
    loanPurpose: '',
    loanTermMonths: 60,
    employmentStatus: '',
    notes: ''
  });
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [eligibility, setEligibility] = useState<EligibilityResponse | null>(null);

  /// <summary>
  /// Handles input changes for form fields
  /// </summary>
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    
    setFormData(prev => ({
      ...prev,
      [name]: name === 'annualIncome' || name === 'loanAmount' || name === 'loanTermMonths' 
        ? Number(value) 
        : value
    }));

    // Clear eligibility when form changes
    setEligibility(null);
  };

  /// <summary>
  /// Checks loan eligibility based on current form data
  /// </summary>
  const handleCheckEligibility = async () => {
    if (formData.annualIncome <= 0 || formData.loanAmount <= 0) {
      alert('Please enter valid income and loan amount');
      return;
    }

    try {
      const result = await checkLoanEligibility(
        formData.annualIncome,
        formData.loanAmount,
        undefined
      );
      setEligibility(result);
    } catch (err) {
      console.error('Error checking eligibility:', err);
    }
  };

  /// <summary>
  /// Handles form submission to create new loan application
  /// </summary>
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await createLoanApplication(formData);
      onSuccess();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to create application');
      console.error('Error creating application:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="loan-form-container">
      <h2>New Loan Application</h2>
      
      {error && <div className="error-message">{error}</div>}

      <form onSubmit={handleSubmit} className="loan-form">
        <div className="form-group">
          <label htmlFor="applicantName">Applicant Name *</label>
          <input
            type="text"
            id="applicantName"
            name="applicantName"
            value={formData.applicantName}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="email">Email *</label>
          <input
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="phoneNumber">Phone Number *</label>
          <input
            type="tel"
            id="phoneNumber"
            name="phoneNumber"
            value={formData.phoneNumber}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="annualIncome">Annual Income *</label>
          <input
            type="number"
            id="annualIncome"
            name="annualIncome"
            value={formData.annualIncome || ''}
            onChange={handleChange}
            min="0"
            step="1000"
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="loanAmount">Loan Amount *</label>
          <input
            type="number"
            id="loanAmount"
            name="loanAmount"
            value={formData.loanAmount || ''}
            onChange={handleChange}
            min="1000"
            step="1000"
            required
          />
          <button 
            type="button" 
            className="btn-secondary"
            onClick={handleCheckEligibility}
            style={{ marginTop: '0.5rem' }}
          >
            Check Eligibility
          </button>
          
          {eligibility && (
            <div className={`eligibility-result ${eligibility.isEligible ? 'eligible' : 'not-eligible'}`}>
              {eligibility.isEligible ? '✓ Eligible' : '✗ Not Eligible'}: {eligibility.reason}
            </div>
          )}
        </div>

        <div className="form-group">
          <label htmlFor="loanPurpose">Loan Purpose *</label>
          <select
            id="loanPurpose"
            name="loanPurpose"
            value={formData.loanPurpose}
            onChange={handleChange}
            required
          >
            <option value="">Select Purpose</option>
            <option value="Home">Home</option>
            <option value="Car">Car</option>
            <option value="Education">Education</option>
            <option value="Personal">Personal</option>
            <option value="Business">Business</option>
          </select>
        </div>

        <div className="form-group">
          <label htmlFor="loanTermMonths">Loan Term (months) *</label>
          <input
            type="number"
            id="loanTermMonths"
            name="loanTermMonths"
            value={formData.loanTermMonths}
            onChange={handleChange}
            min="6"
            max="360"
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="employmentStatus">Employment Status</label>
          <select
            id="employmentStatus"
            name="employmentStatus"
            value={formData.employmentStatus}
            onChange={handleChange}
          >
            <option value="">Select Status</option>
            <option value="Employed">Employed</option>
            <option value="Self-Employed">Self-Employed</option>
            <option value="Unemployed">Unemployed</option>
            <option value="Retired">Retired</option>
          </select>
        </div>

        <div className="form-group">
          <label htmlFor="notes">Notes</label>
          <textarea
            id="notes"
            name="notes"
            value={formData.notes}
            onChange={handleChange}
            rows={3}
          />
        </div>

        <button type="submit" className="btn-primary" disabled={loading}>
          {loading ? 'Submitting...' : 'Submit Application'}
        </button>
      </form>
    </div>
  );
};

export default LoanForm;
