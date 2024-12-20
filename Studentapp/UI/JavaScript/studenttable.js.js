import React, { useState, useEffect } from 'react';
import '../StudentTable/studenttable.css'; // Ensure this points to the correct CSS file
import axios from 'axios';
import moment from 'moment';
import * as XLSX from 'xlsx';
import swal from 'sweetalert2';
import { 
  Container, 
  Button, 
  Typography, 
  Modal, 
  TextField, 
  Checkbox, 
  InputAdornment 
} from '@mui/material';
import Tooltip from '@mui/material/Tooltip';
import { DataGrid, Column, Pager, Paging } from 'devextreme-react/data-grid';
import 'devextreme/dist/css/dx.light.css';
import { useNavigate } from 'react-router-dom';
import SearchIcon from '@mui/icons-material/Search'; 
import AddIcon from '@mui/icons-material/Add'; 
import GetAppIcon from '@mui/icons-material/GetApp'; 
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import RestoreIcon from '@mui/icons-material/Restore';
import { STUDENT_URL, LEAVE_REQUEST_COUNT_URL } from '../Urls/apiurls'; // Update the import for leave request URL

const StudentTable = ({ userRole }) => {
  const navigate = useNavigate();
  const [users, setUsers] = useState([]);
  const [leaveRequestCount, setLeaveRequestCount] = useState(0);
  const [formData, setFormData] = useState({
    firstname: '',
    lastname: '',
    dob: '',
    email: '',
    createdon: '',
    updatedon: '',
    IsActive: true
  });
  const [editingIndex, setEditingIndex] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [modalOpen, setModalOpen] = useState(false);

  const fetchUsers = async () => {
    try {
      const response = await axios.get(STUDENT_URL);
      console.log('API Response:', response.data); // Log the response
      const formattedUsers = response.data.map(user => ({
        ...user,
        createdon: moment(user.createdon).format('DD-MM-YYYY hh:mm A'),
        updatedon: moment(user.updatedon).format('DD-MM-YYYY hh:mm A'),
        IsActive: user.isActive === true // Use user.isActive
      }));
      console.log('Formatted Users:', formattedUsers); // Log formatted users
      setUsers(formattedUsers);
    } catch (error) {
      console.error('Error fetching users:', error);
    }
  };

  useEffect(() => {
    const fetchLeaveRequestCount = async () => {
      try {
        const response = await axios.get(LEAVE_REQUEST_COUNT_URL);
        setLeaveRequestCount(response.data.count);
      } catch (error) {
        console.error('Error fetching leave request count:', error);
      }
    };

    fetchUsers();
    fetchLeaveRequestCount();
  }, []);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({ ...formData, [name]: type === 'checkbox' ? checked : value });
  };

  const resetForm = () => {
    setFormData({
      firstname: '',
      lastname: '',
      dob: '',
      email: '',
      createdon: '',
      updatedon: '',
      IsActive: true
    });
    setEditingIndex(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const newUser = {
      ...formData,
      createdon: moment().format(),
      updatedon: moment().format(),
    };

    try {
      if (editingIndex !== null) {
        await axios.put(`${STUDENT_URL}/${users[editingIndex].id}`, newUser);
        const updatedUsers = users.map((user, index) => 
          index === editingIndex ? newUser : user
        );
        setUsers(updatedUsers);
      } else {
        const response = await axios.post(STUDENT_URL, newUser);
        fetchUsers();
        resetForm();
      }
      setModalOpen(false);
    } catch (error) {
      console.error('Error saving user:', error.response ? error.response.data : error.message);
    }
  };

  const handleEdit = (data) => {
    navigate(`/edit/${data.id}`); 
  };

  const handleDelete = async (data) => {
    const user = users.find(u => u.id === data.id);
    const action = user.IsActive ? 'delete' : 'restore';
    const { isConfirmed } = await swal.fire({
      title: `Are you sure you want to ${action} this user?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: `Yes, ${action} it!`,
      cancelButtonText: 'No, cancel!',
    });
    if (isConfirmed) {
      try {
        await axios.delete(`${STUDENT_URL}${user.id}`);
        const updatedUsers = users.map(u => 
          u.id === user.id ? { ...u, IsActive: !u.IsActive } : u
        );
        setUsers(updatedUsers);
        swal.fire(`${action.charAt(0).toUpperCase() + action.slice(1)}d!`, `The user has been ${action}d.`, 'success');
      } catch (error) {
        console.error('Error deleting/restoring user:', error);
        swal.fire('Error!', 'There was a problem processing your request.', 'error');
      }
    }
  };

  const filteredUsers = users.filter((user) => 
    Object.values(user).some(value => 
      String(value).toLowerCase().includes(searchTerm.toLowerCase())
    )
  );
  
  const exportToExcel = async () => {
    const { value: filename } = await swal.fire({
      title: "Enter name of file",
      input: "text",
      icon: "question",
      confirmButtonText: "Confirm",
      showCancelButton: true
    });
  
    if (filename) {
      // Map filteredUsers to only include the specified fields
      const exportData = filteredUsers.map(user => ({
        id: user.id,
        firstname: user.firstname,
        lastname: user.lastname,
        email: user.email,
        dob: user.dob,
        createdon: user.createdon,
        updatedon: user.updatedon,
        status: user.IsActive ? "Active" : "Inactive", // Convert boolean to string
      }));
  
      const ws = XLSX.utils.json_to_sheet(exportData);
      const wb = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(wb, ws, 'Student');
      XLSX.writeFile(wb, `${filename}.xlsx`);
    }
  };
  
  const handleLeaveRequests = () => {
    navigate('/teacher-leave-requests');
  };
   
  return (
    <Container>
      <center>
        <h1>TEACHER PORTAL</h1>
      </center>
      
      <div className="button-container">
        {/* <Button 
          variant="outlined" 
          onClick={() => navigate('/')} // Navigate back to Home
        >
          Back to Home
        </Button> */}
        
        <Tooltip title="View Leave Requests" arrow>
          <Button 
            variant="contained" 
            color="secondary" 
            onClick={handleLeaveRequests} // Navigate to Leave Requests
          >
            Leave Requests {leaveRequestCount > 0 && `(${leaveRequestCount})`}
          </Button>
        </Tooltip>

        <Tooltip title="View Marks" arrow>
          <Button 
            variant="contained" 
            onClick={() => navigate('/view-marks')}
          >
            View Marks
          </Button>
        </Tooltip>
      </div>

      <div className="search-export-container">
        <TextField
          label="Search Users"
          variant="outlined"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            )
          }}
        />
        
        <Typography variant="subtitle1" className="total-records">
          Total Records: {filteredUsers.length}
        </Typography>

        <div className="export-container">
          <Tooltip title="Export to Excel" arrow>
            <Button variant="contained" onClick={exportToExcel} className='export-button' startIcon={<GetAppIcon />}>
               Export to Excel 
            </Button>
          </Tooltip>
          
          {userRole !== 'student' && (
            <Tooltip title="Add Student" arrow>
              <Button variant="contained" color="primary" onClick={() => { resetForm(); setModalOpen(true); }} className='add-button' startIcon={<AddIcon />}>
                Add Student 
              </Button>
            </Tooltip>
          )}
        </div>
      </div>

      <DataGrid
        dataSource={filteredUsers}
        showBorders={true}
        allowColumnReordering={true}
        keyField="id"
        columnAutoWidth={true}
        allowColumnResizing
        filterRow={{ visible: true }}
      >
        <Column dataField="id" caption="ID" width={30} />
        <Column dataField="firstname" caption="First Name" />
        <Column dataField="lastname" caption="Last Name" />
        <Column dataField="email" caption="Email"/>
        <Column dataField="dob" caption="DOB" />
        <Column dataField="createdon" caption="Created On" />
        <Column dataField="updatedon" caption="Updated On" />
        <Column 
          caption="Status" 
          cellRender={(cellData) => (
            <Checkbox 
              className='checkbox'
              checked={cellData.data.IsActive} // This references the correct property
              disabled 
            />
          )}
        />
        <Paging defaultPageSize={10} />
        <Pager className='custom-pagination' showPageSizeSelector showInfo />
        <Column 
          caption="Actions" 
          cellRender={(cellData) => (
            <div>
              <Tooltip title="Edit" arrow>
                <Button variant="contained" onClick={() => handleEdit(cellData.data)} startIcon={<EditIcon />} />
              </Tooltip>
              <Tooltip title={cellData.data.IsActive ? "Delete" : "Restore"} arrow>
                <Button 
                  variant="contained" 
                  color={cellData.data.IsActive ? "secondary" : "primary"} 
                  onClick={() => handleDelete(cellData.data)}
                  startIcon={cellData.data.IsActive ? <DeleteIcon /> : <RestoreIcon />}
                />
              </Tooltip>
            </div>
          )}
        />
      </DataGrid>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)}>
        <div style={{ padding: '20px', maxWidth: '500px', margin: 'auto', backgroundColor: 'white', borderRadius: '8px', boxShadow: '0 4px 10px rgba(0, 0, 0, 0.1)' }}>
          <h2>{editingIndex !== null ? 'Edit User' : 'Add User'}</h2>
          <form onSubmit={handleSubmit} className="user-form">
            <TextField
              name="firstname"
              label="First Name"
              value={formData.firstname}
              onChange={handleChange}
              fullWidth
              margin="normal"
              required
            />
            <TextField
              name="lastname"
              label="Last Name"
              value={formData.lastname}
              onChange={handleChange}
              fullWidth
              margin="normal"
              required
            />
            <TextField
              name="dob"
              label="Date of Birth"
              type="date"
              value={formData.dob}
              onChange={handleChange}
              fullWidth
              margin="normal"
              InputLabelProps={{ shrink: true }}
              required
            />
            <TextField
              name="email"
              label="Email"
              type="email"
              value={formData.email}
              onChange={handleChange}
              fullWidth
              margin="normal"
              required
            />
            {/* <FormControlLabel
              control={
                <Checkbox 
                  name="IsActive"
                  checked={formData.IsActive}
                  onChange={handleChange}
                />
              }
              label="Active"
            /> */}
            <Button type="submit" variant="contained" color="primary">
              {editingIndex !== null ? 'Update User' : 'Add User'}
            </Button>
          </form>
        </div>
      </Modal>
    </Container>
  );
};

export default StudentTable;
