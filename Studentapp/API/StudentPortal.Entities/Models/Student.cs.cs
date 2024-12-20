using System;
using System.Collections.Generic;

namespace StudentPortal.Entities.Models;

public partial class Student
{
    public int Id { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public DateOnly? Dob { get; set; }

    public DateTime? Createdon { get; set; }

    public DateTime? Updatedon { get; set; }

    public string? Email { get; set; }

    public bool? IsActive { get; set; }

    public string? Password { get; set; }

    public int? Roleid { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<StudentMark> StudentMarks { get; set; } = new List<StudentMark>();
}
