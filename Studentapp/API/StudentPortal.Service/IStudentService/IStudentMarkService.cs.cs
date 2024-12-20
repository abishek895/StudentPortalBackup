using System.Collections.Generic;
using System.Threading.Tasks;
using StudentPortal.Entities.DTO;

namespace StudentPortal.Services.IServices
{
    public interface IStudentMarkService
    {
        Task<IEnumerable<StudentMarkDto>> GetStudentMarksAsync();
        Task<StudentMarkDto> GetStudentMarkAsync(int id);
        Task AddStudentMarkAsync(StudentMarkDto studentMarkDto);
        Task UpdateStudentMarkAsync(int id, StudentMarkDto studentMarkDto);
        Task DeleteStudentMarkAsync(int id);
        
    }
}
