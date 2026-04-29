import type { LoanApplication } from '../types/LoanApplication';
import { deleteLoanApplication } from '../services/api';
import { useState } from 'react';
import LoanDetails from './LoanDetails';

/// <summary>
/// Props for LoanList component
/// </summary>
interface LoanListProps {
  applications: LoanApplication[];
  onRefresh: () => void;
}

/// <summary>
/// Component to display list of loan applications with delete functionality
/// </summary>
const LoanList = ({ applications, onRefresh }: LoanListProps) => {
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [selectedApp, setSelectedApp] = useState<LoanApplication | null>(null);

  /// <summary>
  /// Handles deletion of a loan application with confirmation
  /// </summary>
  /// <param name="id">Application ID to delete</param>
  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this application?')) {
      return;
    }

    try {
      setDeletingId(id);
      await deleteLoanApplication(id);
      onRefresh();
    } catch (err) {
      console.error('Error deleting application:', err);
      alert('Failed to delete application');
    } finally {
      setDeletingId(null);
    }
  };

  /// <summary>
  /// Returns CSS class based on loan status
  /// </summary>
  const getStatusClass = (status: string) => {
    switch (status) {
      case 'Approved': return 'status-approved';
      case 'Rejected': return 'status-rejected';
      case 'UnderReview': return 'status-review';
      default: return 'status-pending';
    }
  };

  if (applications.length === 0) {
    return <div className="no-data">No loan applications found.</div>;
  }

  return (
    <div className="loan-list">
      <h2>Loan Applications ({applications.length})</h2>
      
      <div className="loan-cards">
        {applications.map((app) => (
          <div key={app.id} className="loan-card">
            <div className="loan-header">
              <h3>{app.applicantName}</h3>
              <span className={`status-badge ${getStatusClass(app.status)}`}>
                {app.status}
              </span>
            </div>
            
            <div className="loan-info">
              <p><strong>Email:</strong> {app.email}</p>
              <p><strong>Loan Amount:</strong> ${app.loanAmount.toLocaleString()}</p>
              <p><strong>Purpose:</strong> {app.loanPurpose}</p>
              <p><strong>Term:</strong> {app.loanTermMonths} months</p>
              <p><strong>Applied:</strong> {new Date(app.applicationDate).toLocaleDateString()}</p>
            </div>

            <div className="loan-actions">
              <button 
                className="btn-view"
                onClick={() => setSelectedApp(app)}
              >
                View Details
              </button>
              <button 
                className="btn-delete"
                onClick={() => handleDelete(app.id)}
                disabled={deletingId === app.id}
              >
                {deletingId === app.id ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </div>
        ))}
      </div>

      {selectedApp && (
        <LoanDetails 
          application={selectedApp} 
          onClose={() => setSelectedApp(null)} 
        />
      )}
    </div>
  );
};

export default LoanList;
