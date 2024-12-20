using StudentPortal.Entities.DTO;
using StudentPortal.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentPortal.Services.IServices
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetStudentsAsync();
        Task<StudentDto> GetStudentAsync(int id);
        Task UpdateStudentsAsync(int id, StudentDto studentDto);
        Task UpdateStudentAsync(int id, StudentDto studentDto);
        Task AddStudentAsync(StudentDto studentDto);
        Task ToggleDeleteStudentAsync(int id);
        Task SendMarksEmailAsync(string email, StudentDto student);
        Task<object> LoginAsync(LoginDto loginDto);
        Task<StudentDto> GetStudentByIdAsync(int studentId);
    }
}
