using Microsoft.EntityFrameworkCore;
using StudentPortal.Entities.DTO;
using StudentPortal.Entities.Models;
using StudentPortal.Services.IServices;

namespace StudentPortal.Services.Services
{
    public class StudentMarkService : IStudentMarkService
    {
        private readonly StudentdbContext _context;

        public StudentMarkService(StudentdbContext context)
        {
            _context = context;
        }

        // HTTP GET: Retrieve all student marks along with student details
        public async Task<IEnumerable<StudentMarkDto>> GetStudentMarksAsync()
        {
            return await _context.StudentMarks
                .Include(sm => sm.Student) // Include related Student entity
                .Select(sm => new StudentMarkDto
                {
                    Id = sm.Id,
                    Studentid = (int)sm.Studentid,
                    Maths = (double)sm.Maths,
                    Science = (double)sm.Science,
                    Physics = (double)sm.Physics,
                    Chemistry = (double)sm.Chemistry,
                    English = (double)sm.English,
                    Semid = (int)sm.Semid,
                    Student = new StudentDto // Map student details
                    {
                        Id = sm.Student.Id,
                        Firstname = sm.Student.Firstname,
                        Lastname = sm.Student.Lastname,
                        Dob = (DateOnly)sm.Student.Dob,
                        Createdon = (DateTime)sm.Student.Createdon,
                        Updatedon = (DateTime)sm.Student.Updatedon,
                        Email = sm.Student.Email,
                        IsActive = (bool)sm.Student.IsActive
                    }
                })
                .ToListAsync();
        }

        // HTTP GET BY ID: Retrieve a specific student mark along with student details
        public async Task<StudentMarkDto> GetStudentMarkAsync(int id)
        {
            var studentMark = await _context.StudentMarks
                .Include(sm => sm.Student) // Include related Student entity
                .FirstOrDefaultAsync(sm => sm.Id == id);

            if (studentMark == null) return null;

            return new StudentMarkDto
            {
                Id = studentMark.Id,
                Studentid = (int)studentMark.Studentid,
                Maths = (double)studentMark.Maths,
                Science = (double)studentMark.Science,
                Physics = (double)studentMark.Physics,
                Chemistry = (double)studentMark.Chemistry,
                English = (double)studentMark.English,
                Semid = (int)studentMark.Semid,
                Student = new StudentDto // Map student details
                {
                    Id = studentMark.Student.Id,
                    Firstname = studentMark.Student.Firstname,
                    Lastname = studentMark.Student.Lastname,
                    Dob = (DateOnly)studentMark.Student.Dob,
                    Createdon = (DateTime)studentMark.Student.Createdon,
                    Updatedon = (DateTime)studentMark.Student.Updatedon,
                    Email = studentMark.Student.Email,
                    IsActive = (bool)studentMark.Student.IsActive
                }
            };
        }

        // HTTP POST: Add a new student mark
        public async Task AddStudentMarkAsync(StudentMarkDto studentMarkDto)
        {
            Console.WriteLine($"Received Studentid:{studentMarkDto.Studentid}");
            var newId = _context.StudentMarks.Any() ? _context.StudentMarks.Max(m => m.Id) + 1 : 1;
            var studentMark = new StudentMark
            {
                Id=newId,
                Studentid = studentMarkDto.Studentid,
                Maths = studentMarkDto.Maths,
                Science = studentMarkDto.Science,
                Physics = studentMarkDto.Physics,
                Chemistry = studentMarkDto.Chemistry,
                English = studentMarkDto.English,
                Semid = studentMarkDto.Semid,
            };
            _context.StudentMarks.Add(studentMark);
            await _context.SaveChangesAsync();
        }
        // HTTP PUT: Update an existing student
        public async Task UpdateStudentMarkAsync(int id, StudentMarkDto studentmarkDto)
        {
            try
            {
                var studentmark = await _context.StudentMarks
                    .Include(s => s.Student)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (studentmark == null)
                {
                    throw new KeyNotFoundException("Student not found");
                }

                // Update student properties
                studentmark.Studentid = studentmarkDto.Studentid;
                studentmark.Maths = studentmarkDto.Maths;
                studentmark.Science = studentmarkDto.Science;
                studentmark.Physics = studentmarkDto.Physics;
                studentmark.Chemistry = studentmarkDto.Chemistry;
                studentmark.English = studentmarkDto.English;

                // Update existing marks or add new marks
                if (studentmarkDto.StudentMarks != null)
                {
                    foreach (var mark in studentmarkDto.StudentMarks)
                    {
                        // Create a new mark and assign a new ID
                        var newMark = new StudentMark
                        {
                            // Assign a new ID manually
                            Id = _context.StudentMarks.Any() ? _context.StudentMarks.Max(sm => sm.Id) + 1 : 1,
                            Studentid = mark.Id, // Ensure to assign the student ID
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


        // HTTP DELETE: Remove a student mark
        public async Task DeleteStudentMarkAsync(int id)
        {
            var studentMark = await _context.StudentMarks.FindAsync(id);
            if (studentMark == null)
            {
                throw new KeyNotFoundException("Student mark not found");
            }

            _context.StudentMarks.Remove(studentMark);
            await _context.SaveChangesAsync();
        }
    }
}
