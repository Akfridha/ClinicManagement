using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using UnicornProject.Helper;

namespace UnicornProject.Models
{
    public partial class Hospital_Appointment_DBContext : DbContext
    {
        public Hospital_Appointment_DBContext()
        {
        }

        public Hospital_Appointment_DBContext(DbContextOptions<Hospital_Appointment_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppionmentBooking> AppionmentBooking { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserMasterTable> UserMasterTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString.HospitalAppionmentConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppionmentBooking>(entity =>
            {
                entity.ToTable("appionment_booking");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BookingConformation).HasColumnName("booking_conformation");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.DoctorId).HasColumnName("doctorId");

                entity.Property(e => e.EndDateTime)
                    .HasColumnName("end_dateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.PatientsId).HasColumnName("patientsID");

                entity.Property(e => e.StartDateTime)
                    .HasColumnName("start_dateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CretaedDate)
                    .HasColumnName("cretaed_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.RoleName)
                    .HasColumnName("role_name")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<UserMasterTable>(entity =>
            {
                entity.ToTable("User_master_table");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Adress)
                    .HasColumnName("adress")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserMasterTable)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__User_mast__role___267ABA7A");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
