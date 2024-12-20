using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPortal.Entities.DTO
{
    public partial class StudentMarkDto
    {
        public int Id { get; set; }
        public int Studentid { get; set; }
        public double Maths { get; set; } 
        public double Science { get; set; }
        public double Physics { get; set; }
        public double Chemistry { get; set; }
        public double English { get; set; }
        public int? Semid { get; set; }
        public string? SemesterName { get; set; }
        public virtual StudentDto? Student { get; set; }
        //public List<StudentDto>? Students { get; set; }  
        public List<StudentMarkDto>? StudentMarks { get; set; } 
    }

}


