import React, { useState } from 'react';
import { Routes, Route, Navigate, useLocation } from 'react-router-dom';
import StudentTable from './Components/StudentTable/studenttable';
import EditStudent from './Components/EditStudent/editstudent';
import ViewMarks from './Components/ViewMarks/viewmark';
import TeacherNavbar from './Components/Navbar/navbar'; // Teacher navbar
import StudentNavbar from './Components/StudentNavbar/studentnavbar'; // Import student navbar
import Login from './Components/LoginPage/login';
import Home from './Components/HomePage/home';
import StudentDetail from './Components/StudentDetails/studentdetail';
import LeaveRequests from './Components/LeaveRequest/leaverequest';
import './App.css';

const PrivateRoute = ({ children, isLoggedIn }) => {
  return isLoggedIn ? children : <Navigate to="/login" replace />;
};

const App = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(() => {
    const token = localStorage.getItem('authToken');
    return !!token; // Check if the token exists
  });

  const [userRole, setUserRole] = useState(() => {
    return localStorage.getItem('userRole') || 'student'; // Default to student
  });

  const location = useLocation(); // Get current location

  const handleLogin = (role) => {
    setIsLoggedIn(true);
    const normalizedRole = role.toLowerCase(); // Normalize the role to lowercase
    setUserRole(normalizedRole);
    localStorage.setItem('authToken', 'your_token_here'); // Replace with actual token
    localStorage.setItem('userRole', normalizedRole); // Store user role
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
    localStorage.removeItem('authToken');
    localStorage.removeItem('userRole'); // Remove user role on logout
  };

  const isHomeOrLoginPage = location.pathname === '/' || location.pathname === '/login';

  return (
    <div className="App">
      {!isHomeOrLoginPage && (
        userRole === 'teacher' ? (
          <TeacherNavbar onLogout={handleLogout} />
        ) : (
          <StudentNavbar onLogout={handleLogout} />
        )
      )}
      <div className={`main-content ${isHomeOrLoginPage ? 'full-width' : ''}`}>
        <main>
          <Routes>
            <Route path="/" element={<Home isLoggedIn={isLoggedIn} />} />
            <Route path="/login" element={<Login onLogin={handleLogin} />} />
            {/* Protected Routes */}
            <Route path="/users" element={<PrivateRoute isLoggedIn={isLoggedIn}><StudentTable /></PrivateRoute>} />
            <Route path="/edit/:id" element={<PrivateRoute isLoggedIn={isLoggedIn}><EditStudent /></PrivateRoute>} />
            <Route path="/view-marks" element={<PrivateRoute isLoggedIn={isLoggedIn}><ViewMarks /></PrivateRoute>} />
            <Route path="/teacher-leave-requests" element={<PrivateRoute isLoggedIn={isLoggedIn}><LeaveRequests /></PrivateRoute>} />
            <Route path="/students/:studentId" element={<PrivateRoute isLoggedIn={isLoggedIn}><StudentDetail /></PrivateRoute>} />
           
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </main>
      </div>
    </div>
  );  
};  

export default App;
