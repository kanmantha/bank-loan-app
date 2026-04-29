import type { LoanApplication, LoanStatus } from '../types/LoanApplication';
import { updateLoanApplicationStatus } from '../services/api';
import { useState } from 'react';

/// <summary>
/// Props for LoanDetails component
/// </summary>
interface LoanDetailsProps {
  application: LoanApplication;
  onClose: () => void;
}

/// <summary>
/// Component to display detailed information about a loan application
/// </summary>
const LoanDetails = ({ application, onClose }: LoanDetailsProps) => {
  const [updating, setUpdating] = useState(false);
  const [newStatus, setNewStatus] = useState<LoanStatus>(application.status);
  const [notes, setNotes] = useState(application.notes || '');

  /// <summary>
  /// Handles status update for the loan application
  /// </summary>
  const handleUpdateStatus = async () => {
    if (newStatus === application.status) {
      alert('Please select a different status to update');
      return;
    }

    try {
      setUpdating(true);
      await updateLoanApplicationStatus(application.id, {
        status: newStatus,
        notes: notes || undefined
      });
      alert('Status updated successfully!');
      onClose();
      window.location.reload();
    } catch (err) {
      console.error('Error updating status:', err);
      alert('Failed to update status');
    } finally {
      setUpdating(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Loan Application Details</h2>
          <button className="close-btn" onClick={onClose}>×</button>
        </div>

        <div className="details-grid">
          <div className="detail-item">
            <strong>Applicant Name:</strong>
            <span>{application.applicantName}</span>
          </div>
          
          <div className="detail-item">
            <strong>Email:</strong>
            <span>{application.email}</span>
          </div>
          
          <div className="detail-item">
            <strong>Phone:</strong>
            <span>{application.phoneNumber}</span>
          </div>
          
          <div className="detail-item">
            <strong>Annual Income:</strong>
            <span>${application.annualIncome.toLocaleString()}</span>
          </div>
          
          <div className="detail-item">
            <strong>Loan Amount:</strong>
            <span>${application.loanAmount.toLocaleString()}</span>
          </div>
          
          <div className="detail-item">
            <strong>Purpose:</strong>
            <span>{application.loanPurpose}</span>
          </div>
          
          <div className="detail-item">
            <strong>Term:</strong>
            <span>{application.loanTermMonths} months</span>
          </div>
          
          <div className="detail-item">
            <strong>Status:</strong>
            <span className={`status-${application.status.toLowerCase()}`}>
              {application.status}
            </span>
          </div>
          
          <div className="detail-item">
            <strong>Employment:</strong>
            <span>{application.employmentStatus || 'Not specified'}</span>
          </div>
          
          <div className="detail-item">
            <strong>Credit Score:</strong>
            <span>{application.creditScore?.toString() || 'Not provided'}</span>
          </div>
          
          <div className="detail-item">
            <strong>Applied Date:</strong>
            <span>{new Date(application.applicationDate).toLocaleString()}</span>
          </div>
          
          {application.updatedDate && (
            <div className="detail-item">
              <strong>Last Updated:</strong>
              <span>{new Date(application.updatedDate).toLocaleString()}</span>
            </div>
          )}
          
          <div className="detail-item full-width">
            <strong>Notes:</strong>
            <span>{application.notes || 'No notes'}</span>
          </div>
        </div>

        <div className="status-update-section">
          <h3>Update Status</h3>
          <div className="form-group">
            <label htmlFor="status">New Status:</label>
            <select 
              id="status" 
              value={newStatus} 
              onChange={(e) => setNewStatus(e.target.value as LoanStatus)}
            >
              <option value="Pending">Pending</option>
              <option value="UnderReview">Under Review</option>
              <option value="Approved">Approved</option>
              <option value="Rejected">Rejected</option>
            </select>
          </div>
          
          <div className="form-group">
            <label htmlFor="updateNotes">Notes:</label>
            <textarea
              id="updateNotes"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              rows={2}
              placeholder="Add notes for status update..."
            />
          </div>
          
          <button 
            className="btn-primary"
            onClick={handleUpdateStatus}
            disabled={updating}
          >
            {updating ? 'Updating...' : 'Update Status'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default LoanDetails;
