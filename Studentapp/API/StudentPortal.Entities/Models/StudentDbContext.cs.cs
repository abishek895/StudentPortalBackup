using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Entities.DTO;

namespace StudentPortal.Entities.Models;

public partial class StudentdbContext : DbContext
{
    public StudentdbContext()
    {
    }

    public StudentdbContext(DbContextOptions<StudentdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentMark> StudentMarks { get; set; }

    public async Task AddLeaveRequestAsync(LeaveRequestDto leaveRequestDto)
    {
        throw new NotImplementedException();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ZOR;Database=studentdb;TrustServerCertificate=True;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Leavereq__3214EC0713ADF604");

            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Student).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.Studentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Leaverequ__Stude__0F624AF8");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("PK__Roles__8AF5CA32A52FF5C6");

            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.Semid).HasName("PK__Semester__16D5C3C25A068EF3");

            entity.ToTable("Semester", "master");

            entity.Property(e => e.Semid).ValueGeneratedNever();
            entity.Property(e => e.Semname)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__student__3213E83F7C302A45");

            entity.ToTable("Student");

            entity.HasIndex(e => e.Email, "UC_Student").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Createdon)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdon");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Updatedon)
                .HasColumnType("datetime")
                .HasColumnName("updatedon");

            entity.HasOne(d => d.Role).WithMany(p => p.Students)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("FK_Student_Roles");
        });

        modelBuilder.Entity<StudentMark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__studentm__3214EC273B6D147C");

            entity.ToTable("StudentMark");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Chemistry).HasColumnName("CHEMISTRY");
            entity.Property(e => e.English).HasColumnName("ENGLISH");
            entity.Property(e => e.Maths).HasColumnName("MATHS");
            entity.Property(e => e.Physics).HasColumnName("PHYSICS");
            entity.Property(e => e.Science).HasColumnName("SCIENCE");
            entity.Property(e => e.Semid).HasColumnName("SEMID");
            entity.Property(e => e.Studentid).HasColumnName("STUDENTID");

            entity.HasOne(d => d.Sem).WithMany(p => p.StudentMarks)
                .HasForeignKey(d => d.Semid)
                .HasConstraintName("FK_StudentMark_Semester");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentMarks)
                .HasForeignKey(d => d.Studentid)
                .HasConstraintName("FK__studentma__STUDE__5535A963");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
