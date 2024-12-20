import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Container,
    Typography,
    Button,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    CircularProgress,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Grid
} from '@mui/material';
import DataGrid, { Column, Pager, Paging } from 'devextreme-react/data-grid';
import axios from 'axios';
import '../ViewMarks/viewmark.css';
import { STUDENT_URL, SEND_MARKS_EMAIL_URL, SEMESTER_URL } from '../Urls/apiurls';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const ViewMarks = ({ userRole }) => {
    const navigate = useNavigate();
    const [students, setStudents] = useState([]);
    const [selectedStudent, setSelectedStudent] = useState('');
    const [semesters, setSemesters] = useState([]);
    const [loading, setLoading] = useState(false);
    const [filterSemester, setFilterSemester] = useState('');
    const [selectedSemesterForEmail, setSelectedSemesterForEmail] = useState('');
    const [openFilterDialog, setOpenFilterDialog] = useState(false);

    useEffect(() => {
        const fetchStudents = async () => {
            try {
                const response = await axios.get(STUDENT_URL);
                setStudents(response.data || []);
            } catch (error) {
                console.error('Error fetching students:', error);
            }
        };

        const fetchSemesters = async () => {
            try {
                const response = await axios.get(SEMESTER_URL);
                setSemesters(response.data || []);
            } catch (error) {
                console.error('Error fetching semesters:', error);
            }
        };

        fetchStudents();
        fetchSemesters();
    }, []);

    const handleBack = () => {
        navigate('/users');
    };

    const handleSendEmails = async () => {
        if (!selectedStudent) {
            toast.error("Please select a student to send the email.");
            return;
        }

        if (!selectedSemesterForEmail) {
            toast.error("Please select a semester to send the email.");
            return;
        }

        const student = students.find(s => s.id === selectedStudent);
        if (!student) {
            toast.error("Selected student not found.");
            return;
        }

        setLoading(true);

        try {
            const studentMarksPayload = student.studentMarks
                .filter(mark => mark.semid === selectedSemesterForEmail)
                .map(mark => ({
                    Maths: mark.maths,
                    Science: mark.science,
                    Physics: mark.physics,
                    Chemistry: mark.chemistry,
                    English: mark.english,
                    semid: mark.semid
                }));

            const simplifiedPayload = {
                Id: student.id,
                Firstname: student.firstname,
                Lastname: student.lastname,
                Dob: student.dob,
                Createdon: new Date().toISOString(),
                Updatedon: new Date().toISOString(),
                Email: student.email,
                IsActive: student.isActive,
                Password: student.password,
                RoleId: student.roleId,
                StudentMarks: studentMarksPayload
            };

            await axios.post(SEND_MARKS_EMAIL_URL, simplifiedPayload);
            toast.success(`Email sent successfully to ${student.firstname} ${student.lastname}!`);
        } catch (error) {
            console.error('Error sending email:', error);
            toast.error(`Error sending email: ${error.response?.data || error.message}`);
        } finally {
            setLoading(false);
        }
    };

    const handleFilterOpen = () => {
        setOpenFilterDialog(true);
    };

    const handleFilterClose = () => {
        setOpenFilterDialog(false);
        setFilterSemester('');
    };

    const handleSemesterSelect = (semesterId) => {
        setFilterSemester(semesterId);
    };

    const handleSaveFilter = () => {
        setOpenFilterDialog(false);
    };

    const handleSemesterForEmailSelect = (semesterId) => {
        setSelectedSemesterForEmail(semesterId);
    };

    const handleStudentSelect = (studentId) => {
        setSelectedStudent(studentId);
        setSelectedSemesterForEmail(''); // Clear the semester selection when a new student is selected
    };

    const handleClearSelection = () => {
        setSelectedStudent('');
        setSelectedSemesterForEmail('');
    };

    const filteredStudentsWithMarks = students.flatMap(student => {
        return student.studentMarks
            .filter(mark => !filterSemester || mark.semid === filterSemester)
            .map(mark => {
                const semester = semesters.find(sem => sem.semid === mark.semid);
                const semesterName = semester ? semester.semname : 'N/A';

                const { maths, science, physics, chemistry, english } = mark;
                const total = (parseInt(maths, 10) || 0) + (parseInt(science, 10) || 0) +
                              (parseInt(physics, 10) || 0) + (parseInt(chemistry, 10) || 0) +
                              (parseInt(english, 10) || 0);
                const cgpa = (total / 50).toFixed(2);
                let grade;

                if (cgpa >= 9) grade = 'A+';
                else if (cgpa >= 8) grade = 'A';
                else if (cgpa >= 7) grade = 'B';
                else if (cgpa >= 6) grade = 'C';
                else grade = 'RA';

                return {
                    id: `${student.id}`,
                    name: `${student.firstname} ${student.lastname}`,
                    semester: semesterName,
                    maths,
                    science,
                    physics,
                    chemistry,
                    english,
                    total,
                    cgpa,
                    grade
                };
            });
    });

    return (
        <Container className="container">
            <ToastContainer />
            <Typography variant="h4" className="title">ALL STUDENT MARKS</Typography>
            {userRole !== 'student' && (
                <>
                    <Grid container spacing={2} alignItems="center">
                        <Grid item xs={12} sm={6} md={4}>
                            <FormControl variant="outlined" fullWidth>
                                <InputLabel id="select-student-label">Select Student To Send Mail</InputLabel>
                                <Select
                                    labelId="select-student-label"
                                    value={selectedStudent}
                                    onChange={(e) => handleStudentSelect(e.target.value)}
                                    label="Select Student"
                                >
                                    {students.map((student) => (
                                        <MenuItem key={student.id} value={student.id}>
                                            {student.firstname} {student.lastname}
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                        </Grid>
                        {selectedStudent && (
                            <Grid item xs={12} sm={6} md={4}>
                                <FormControl variant="outlined" fullWidth>
                                    <InputLabel id="select-semester-label">Select Semester</InputLabel>
                                    <Select
                                        labelId="select-semester-label"
                                        value={selectedSemesterForEmail}
                                        onChange={(e) => handleSemesterForEmailSelect(e.target.value)}
                                        label="Select Semester"
                                    >
                                        {semesters.map((semester) => (
                                            <MenuItem key={semester.semid} value={semester.semid}>
                                                {semester.semname}
                                            </MenuItem>
                                        ))}
                                    </Select>
                                </FormControl>
                            </Grid>
                        )}
                    </Grid>
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={handleSendEmails}
                        disabled={!selectedStudent || !selectedSemesterForEmail || loading}
                        style={{ marginTop: '10px' }}
                    >
                        {loading ? <CircularProgress size={24} /> : "Send Email"}
                    </Button>
                    <Button
                        variant="outlined"
                        color="secondary"
                        onClick={handleClearSelection}
                        style={{ marginLeft: '10px', marginTop: '10px' }}
                    >
                        Clear Selection
                    </Button>
                </>
            )}
            <DataGrid
                dataSource={filteredStudentsWithMarks}
                keyField="id"
                showBorders={true}
                allowColumnReordering={true}
                filterRow={{ visible: true }}
                allowColumnResizing
            >
                <Column dataField="id" caption="ID" width={100} />
                <Column dataField="name" caption="Name" />
                <Column dataField="semester" caption="Semester" />
                <Column dataField="maths" caption="Maths" />
                <Column dataField="science" caption="Science" />
                <Column dataField="physics" caption="Physics" />
                <Column dataField="chemistry" caption="Chemistry" />
                <Column dataField="english" caption="English" />
                <Column dataField="total" caption="Total" />
                <Column dataField="cgpa" caption="CGPA" />
                <Column dataField="grade" caption="Grade" />
                <Paging defaultPageSize={10} />
                <Pager showPageSizeSelector showInfo />
            </DataGrid>
            <Button variant="outlined" onClick={handleBack} className="button">
                Back
            </Button>

            <Dialog open={openFilterDialog} onClose={handleFilterClose}>
                <DialogTitle>Filter Marks</DialogTitle>
                <DialogContent>
                    <FormControl variant="outlined" fullWidth>
                        <InputLabel id="filter-semester-label">Select Semester</InputLabel>
                        <Select
                            labelId="filter-semester-label"
                            value={filterSemester}
                            onChange={(e) => handleSemesterSelect(e.target.value)}
                            label="Select Semester"
                        >
                            {semesters.map((semester) => (
                                <MenuItem key={semester.semid} value={semester.semid}>
                                    {semester.semname}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleFilterClose} color="primary">
                        Cancel
                    </Button>
                    <Button onClick={handleSaveFilter} color="primary">
                        Search
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

export default ViewMarks;
