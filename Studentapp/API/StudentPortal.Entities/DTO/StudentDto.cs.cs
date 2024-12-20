using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPortal.Entities.DTO
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateOnly? Dob { get; set; }
        public DateTime? Createdon { get; set; }
        public DateTime? Updatedon { get; set; }
        public string Email { get; set; }

        public bool? IsActive { get; set; }
        public string? Password {  get; set; }
        public int? RoleId { get; set; } // Add this line
       

        public IEnumerable<StudentMarkDto>? StudentMarks { get; set; }

       
    }
}
