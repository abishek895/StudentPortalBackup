using Microsoft.EntityFrameworkCore;
using StudentPortal.Entities.DTO;
using StudentPortal.Entities.Models;
using StudentPortal.Services.IServices;
using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.AspNetCore.Mvc;
using System.Text;
namespace StudentPortal.Services.Services
{
    public class StudentService : IStudentService
    {
        private readonly StudentdbContext _context;

        public StudentService(StudentdbContext context)
        {
            _context = context;
        }

        // HTTP GET: Retrieve all students
        public async Task<IEnumerable<StudentDto>> GetStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.StudentMarks)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Firstname = s.Firstname,
                    Lastname = s.Lastname,
                    Dob = (DateOnly)s.Dob,
                    Createdon = (DateTime)s.Createdon,
                    Updatedon = (DateTime)s.Updatedon,
                    Email = s.Email,
                    IsActive = (bool)s.IsActive,
                    StudentMarks = s.StudentMarks.Select(sm => new StudentMarkDto
                    {
                        Id = sm.Id,
                        Studentid = (int)sm.Studentid,
                        Maths = (double)sm.Maths,
                        Science = (double)sm.Science,
                        Physics = (double)sm.Physics,
                        Chemistry = (double)sm.Chemistry,
                        English = (double)sm.English,
                        Semid = (int)sm.Semid
                    }).ToList()
                })
                .ToListAsync();
        }

        // HTTP GET: Retrieve a student by ID
        public async Task<StudentDto> GetStudentAsync(int id)
        {
            var student = await _context.Students
                .Include(s => s.StudentMarks)
                .ThenInclude(sm => sm.Sem) // Include the Semester details
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return null;

            return new StudentDto
            {
                Id = student.Id,
                Firstname = student.Firstname,
                Lastname = student.Lastname,
                Dob = (DateOnly)student.Dob,
                Createdon = (DateTime)student.Createdon,
                Updatedon = (DateTime)student.Updatedon,
                Email = student.Email,
                IsActive = (bool)student.IsActive,
                StudentMarks = student.StudentMarks.Select(sm => new StudentMarkDto
                {
                    Id = sm.Id,
                    Studentid = (int)sm.Studentid,
                    Maths = (double)sm.Maths,
                    Science = (double)sm.Science,
                    Physics = (double)sm.Physics,
                    Chemistry = (double)sm.Chemistry,
                    English = (double)sm.English,
                    Semid = (int)sm.Semid,
                    SemesterName = sm.Sem != null ? sm.Sem.Semname : null // Fetch semester name
                }).ToList()
            };
        }

        //HHTP PUT : update an student details only
        public async Task UpdateStudentsAsync(int id, StudentDto studentDto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found");
            }

            // Update student details
            student.Firstname = studentDto.Firstname;
            student.Lastname = studentDto.Lastname;
            student.Dob = studentDto.Dob; 
            student.Email = studentDto.Email;
            student.IsActive = studentDto.IsActive;
            student.Updatedon = DateTime.Now;

            await _context.SaveChangesAsync();
        }


        // HTTP PUT: Update an existing student
        public async Task UpdateStudentAsync(int id, StudentDto studentDto)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.StudentMarks)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (student == null)
                {
                    throw new KeyNotFoundException("Student not found");
                }

                // Update student properties
                student.Firstname = studentDto.Firstname;
                student.Lastname = studentDto.Lastname;
                student.Dob = studentDto.Dob;
                student.Email = studentDto.Email;
                student.IsActive = studentDto.IsActive;
                student.Updatedon = DateTime.Now;

                // Update existing marks or add new marks
                if (studentDto.StudentMarks != null)
                {
                    foreach (var mark in studentDto.StudentMarks)
                    {
                        // Create a new mark and assign a new ID
                        var newMark = new StudentMark
                        {
                            // Assign a new ID manually
                            Id = _context.StudentMarks.Any() ? _context.StudentMarks.Max(sm => sm.Id) + 1 : 1,
                            Studentid = student.Id, // Ensure to assign the student ID
                            Maths = mark.Maths,
                            Science = mark.Science,
                            Physics = mark.Physics,
                            Chemistry = mark.Chemistry,
                            English = mark.English,
                            //Semid = mark.Semid,
                        };

                        // Add the new mark
                        _context.StudentMarks.Add(newMark);
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging library)
                throw new Exception($"An error occurred while updating the student: {ex.Message}");
            }
        }


        // HTTP POST: Add a new student
        public async Task AddStudentAsync(StudentDto studentDto)
        {
            var newId = _context.Students.Any() ? _context.Students.Max(s => s.Id) + 1 : 1;
            var student = new Student
            {
                Id = newId,
                Firstname = studentDto.Firstname,
                Lastname = studentDto.Lastname,
                Dob = studentDto.Dob,
                Email = studentDto.Email,
                IsActive = true, // Assuming new students are active by default
                Createdon = DateTime.Now,
                Updatedon = DateTime.Now,
                Password = "password",
                Roleid = 2
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        // HTTP POST(EMAIL): Send marks email to a student
        public async Task SendMarksEmailAsync(string email, StudentDto student)
        {
            if (string.IsNullOrEmpty(email) || student == null)
            {
                throw new ArgumentException("Invalid email or student data");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Institution Name", "your-email@example.com"));
            message.To.Add(new MailboxAddress($"{student.Firstname} {student.Lastname}", email));
            message.Subject = "Student Marks";

            // Ensure we are working with a list of marks
            var marksList = student.StudentMarks.ToList(); // Get all marks

            if (!marksList.Any())
            {
                throw new InvalidOperationException("No marks available for the student.");
            }

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = GenerateEmailBody(student, marksList) // Use the updated method
            };
            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.your-email-provider.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtpClient.Authenticate("your-email@example.com", "your-email-password");

                await smtpClient.SendAsync(message);
                smtpClient.Disconnect(true);
            }
        }
        private (double total, double cgpa, string grade) CalculateTotalCgpaGrade(StudentMarkDto marks)
        {
            double total = marks.Maths + marks.Science + marks.Physics + marks.Chemistry + marks.English;
            double cgpa = total / 5; // Assuming CGPA is calculated out of 5
            string grade = GetGrade(cgpa);
            return (total, cgpa, grade);
        }

        private string GetGrade(double cgpa)
        {
            if (cgpa >= 4.5) return "A+";
            if (cgpa >= 4.0) return "A";
            if (cgpa >= 3.5) return "B+";
            if (cgpa >= 3.0) return "B";
            if (cgpa >= 2.5) return "C+";
            if (cgpa >= 2.0) return "C";
            return "F";
        }


        private string GenerateEmailBody(StudentDto student, IEnumerable<StudentMarkDto> marksList)
        {
            var bodyBuilder = new StringBuilder();
            bodyBuilder.Append($"<h1>Marks for {student.Firstname} {student.Lastname}</h1>");

            foreach (var marks in marksList)
            {
                // Ensure CalculateTotalCgpaGrade is defined to handle a StudentMarkDto
                var (total, cgpa, grade) = CalculateTotalCgpaGrade(marks);

                // Check for Semid and handle the case where it's not present
                var semesterId = marks.Semid.HasValue ? marks.Semid.Value.ToString() : "Not specified";

                bodyBuilder.Append($"<h2>Semester: {semesterId}</h2>"); // Use the semester ID
                bodyBuilder.Append($"<p>Maths: {marks.Maths}</p>");
                bodyBuilder.Append($"<p>Science: {marks.Science}</p>");
                bodyBuilder.Append($"<p>Physics: {marks.Physics}</p>");
                bodyBuilder.Append($"<p>Chemistry: {marks.Chemistry}</p>");
                bodyBuilder.Append($"<p>English: {marks.English}</p>");
                bodyBuilder.Append($"<p>Total: {total}</p>");
                bodyBuilder.Append($"<p>CGPA: {cgpa}</p>");
                bodyBuilder.Append($"<p>Grade: {grade}</p>");
            }

            return bodyBuilder.ToString();
        }



        //RETRIVE STUDENT MAIL BY ID 
        public async Task<StudentDto> GetStudentByIdAsync(int studentId)
        {
            // Retrieve the student from the database using their ID
            return await _context.Students
                .Where(s => s.Id == studentId)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Firstname = s.Firstname,
                    Lastname = s.Lastname,
                    Email = s.Email,
                    // Map other properties as necessary
                })
                .FirstOrDefaultAsync();
        }


        //HTTP LOGIN
        public async Task<object> LoginAsync(LoginDto loginDto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                throw new ArgumentException("Email and password cannot be empty.");
            }

            // Check for user (students and teachers in the same table)
            var user = await _context.Students // Assuming both roles are in the Students table
                .Include(s => s.Role) // Make sure you have Role included
                .FirstOrDefaultAsync(s => s.Email == loginDto.Email);

            if (user == null || user.Password != loginDto.Password)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Return role name and user ID
            return new
            {
                RoleName = user.Role.Rolename, // Assuming you have a Role property
                UserId = user.Id // User ID from the student entity
            };
        }



        // HTTP DELETE: Toggle the active status of a student
        public async Task ToggleDeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found");
            }

            student.IsActive = !student.IsActive;
            _context.Entry(student).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
