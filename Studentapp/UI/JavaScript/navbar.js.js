import React from 'react';
import { useNavigate } from 'react-router-dom';
import '../Navbar/navbar.css'; 
import '@fortawesome/fontawesome-free/css/all.min.css'; 
import Swal from 'sweetalert2';

const Navbar = ({ onLogout, pendingRequestsCount, onOpenAddStudent }) => {
  const navigate = useNavigate();

  const handleLogout = () => {
    Swal.fire({
      title: 'Are you sure you want to log out?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, log out!',
    }).then((result) => {
      if (result.isConfirmed) {
        localStorage.removeItem('authToken'); 
        onLogout(); 
        navigate('/'); 
      }
    });
  };

  return (
    <div>
      <div className="sidebar">
        <ul className="sidebar-links">
          <li>
            <button onClick={() => navigate('/users')} className="nav-item">
              <i className="fas fa-users"></i>
              <span className="nav-text">Students</span>
            </button>
          </li>
          <li>
            <button onClick={() => navigate('/view-marks')} className="nav-item">
              <i className="fas fa-chart-bar"></i>
              <span className="nav-text">View Marks</span>
            </button>
          </li>
          <li>
            <button onClick={() => navigate('/teacher-leave-requests')} className="nav-item">
              <i className="fas fa-clipboard-list"></i>
              <span className="nav-text">
                Leave Requests 
                {pendingRequestsCount > 0 && (
                  <span className="pending-count">({pendingRequestsCount})</span>
                )}
              </span>
            </button>
          </li>
          
        </ul>
        
        <div className="logout-button">
          <button onClick={handleLogout} className="logout-btn">
            <i className="fas fa-sign-out-alt"></i>
            <span className="nav-text">Logout</span>
          </button>
        </div>
      </div>
    </div>
  );
};

export default Navbar;
