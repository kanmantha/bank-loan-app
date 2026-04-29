import { useState, useEffect } from 'react';
import { getLoanApplications, getLoanApplicationsByStatus } from './services/api';
import type { LoanApplication, LoanStatus } from './types/LoanApplication';
import LoanList from './components/LoanList';
import LoanForm from './components/LoanForm';
import './App.css';

/// <summary>
/// Main application component for Bank Loan Management System
/// </summary>
function App() {
  const [applications, setApplications] = useState<LoanApplication[]>([]);
  const [filterStatus, setFilterStatus] = useState<string>('all');
  const [showForm, setShowForm] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  /// <summary>
  /// Fetches loan applications on component mount and when filter changes
  /// </summary>
  useEffect(() => {
    fetchApplications();
  }, [filterStatus]);

  /// <summary>
  /// Fetches loan applications from the API based on current filter
  /// </summary>
  const fetchApplications = async () => {
    try {
      setLoading(true);
      setError('');
      
      let data: LoanApplication[];
      if (filterStatus === 'all') {
        data = await getLoanApplications();
      } else {
        data = await getLoanApplicationsByStatus(filterStatus);
      }
      
      setApplications(data);
    } catch (err) {
      setError('Failed to fetch loan applications. Please ensure the API is running.');
      console.error('Error fetching applications:', err);
    } finally {
      setLoading(false);
    }
  };

  /// <summary>
  /// Handles successful loan application creation
  /// </summary>
  const handleApplicationCreated = () => {
    setShowForm(false);
    fetchApplications();
  };

  return (
    <div className="app">
      <header className="app-header">
        <h1>Bank Loan Application Management</h1>
      </header>

      <main className="app-main">
        <div className="controls">
          <div className="filter-controls">
            <label htmlFor="status-filter">Filter by Status: </label>
            <select 
              id="status-filter"
              value={filterStatus} 
              onChange={(e) => setFilterStatus(e.target.value)}
            >
              <option value="all">All Applications</option>
              <option value="Pending">Pending</option>
              <option value="UnderReview">Under Review</option>
              <option value="Approved">Approved</option>
              <option value="Rejected">Rejected</option>
            </select>
          </div>

          <button 
            className="btn-primary"
            onClick={() => setShowForm(!showForm)}
          >
            {showForm ? 'Cancel' : 'New Application'}
          </button>
        </div>

        {error && <div className="error-message">{error}</div>}

        {showForm && (
          <LoanForm onSuccess={handleApplicationCreated} />
        )}

        {loading ? (
          <div className="loading">Loading applications...</div>
        ) : (
          <LoanList applications={applications} onRefresh={fetchApplications} />
        )}
      </main>
    </div>
  );
}

export default App;
