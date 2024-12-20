using System;
using System.Collections.Generic;

namespace StudentPortal.Entities.Models;

public partial class StudentMark
{
    public int Id { get; set; }

    public int? Studentid { get; set; }

    public double? Maths { get; set; }

    public double? Science { get; set; }

    public double? Physics { get; set; }

    public double? Chemistry { get; set; }

    public double? English { get; set; }

    public int? Semid { get; set; }

    public virtual Semester? Sem { get; set; }

    public virtual Student? Student { get; set; }
}
