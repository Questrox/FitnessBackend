using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class FitnessDb : IdentityDbContext<User>
    {
        public FitnessDb(DbContextOptions<FitnessDb> options)
            : base(options)
        { }
        public virtual DbSet<CancellationNotification> CancellationNotifications { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Coach> Coaches { get; set; }
        public virtual DbSet<CoachSchedule> CoachSchedules { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<MembershipType> MembershipTypes { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<ReservationStatus> ReservationStatuses { get; set; }
        public virtual DbSet<Training> Trainings { get; set; }
        public virtual DbSet<TrainingReservation> TrainingsReservations { get; set; }
        public virtual DbSet<TrainingStatus> TrainingStatuses { get; set; }
        public virtual DbSet<TrainingType> TrainingTypes { get; set; }

        // для мягкого удаления
        private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(FitnessDb)
                        .GetMethod(nameof(SetSoftDeleteFilter), 
                         System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(null, new object[] { modelBuilder });
                }
            }
        }
        private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, ISoftDeletable
        {
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(e => !e.IsDeleted);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ApplySoftDeleteFilter(modelBuilder);

            // Client -> User
            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithOne(u => u.Client)
                .HasForeignKey<Client>(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Coach -> User
            modelBuilder.Entity<Coach>()
                .HasOne(c => c.User)
                .WithOne(u => u.Coach)
                .HasForeignKey<Coach>(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // CoachSchedule -> Coach
            modelBuilder.Entity<CoachSchedule>()
                .HasOne(cs => cs.Coach)
                .WithMany(c => c.CoachSchedules)
                .HasForeignKey(cs => cs.CoachId)
                .OnDelete(DeleteBehavior.Cascade);

            // Membership -> Client
            modelBuilder.Entity<Membership>()
                .HasOne(m => m.Client)
                .WithMany(c => c.Memberships)
                .HasForeignKey(m => m.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Membership -> MembershipType
            modelBuilder.Entity<Membership>()
                .HasOne(m => m.MembershipType)
                .WithMany(mt => mt.Memberships)
                .HasForeignKey(m => m.MembershipTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Membership -> Payment
            modelBuilder.Entity<Membership>()
                .HasOne(m => m.Payment)
                .WithOne(p => p.Membership)
                .HasForeignKey<Membership>(m => m.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Training -> Coach
            modelBuilder.Entity<Training>()
                .HasOne(t => t.Coach)
                .WithMany(c => c.Trainings)
                .HasForeignKey(t => t.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            // Training -> TrainingType
            modelBuilder.Entity<Training>()
                .HasOne(t => t.TrainingType)
                .WithMany(tt => tt.Trainings)
                .HasForeignKey(t => t.TrainingTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Training -> TrainingStatus
            modelBuilder.Entity<Training>()
                .HasOne(t => t.TrainingStatus)
                .WithMany(ts => ts.Trainings)
                .HasForeignKey(t => t.TrainingStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainingReservation -> Client
            modelBuilder.Entity<TrainingReservation>()
                .HasOne(tr => tr.Client)
                .WithMany(c => c.TrainingReservations)
                .HasForeignKey(tr => tr.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // TrainingReservation -> Training
            modelBuilder.Entity<TrainingReservation>()
                .HasOne(tr => tr.Training)
                .WithMany(t => t.TrainingReservations)
                .HasForeignKey(tr => tr.TrainingId)
                .OnDelete(DeleteBehavior.Cascade);

            // TrainingReservation -> Payment
            modelBuilder.Entity<TrainingReservation>()
                .HasOne(tr => tr.Payment)
                .WithOne(p => p.TrainingReservation)
                .HasForeignKey<TrainingReservation>(tr => tr.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainingReservation -> ReservationStatus
            modelBuilder.Entity<TrainingReservation>()
                .HasOne(tr => tr.ReservationStatus)
                .WithMany(rs => rs.TrainingReservations)
                .HasForeignKey(tr => tr.ReservationStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // CancellationNotification -> Client
            modelBuilder.Entity<CancellationNotification>()
                .HasOne(cn => cn.Client)
                .WithMany(c => c.CancellationNotifications)
                .HasForeignKey(cn => cn.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // CancellationNotification -> Training
            modelBuilder.Entity<CancellationNotification>()
                .HasOne(cn => cn.Training)
                .WithMany(t => t.CancellationNotifications)
                .HasForeignKey(cn => cn.TrainingId)
                .OnDelete(DeleteBehavior.Cascade);

            // TimeSpan -> SQL time
            modelBuilder.Entity<CoachSchedule>()
                .Property(cs => cs.StartTime)
                .HasColumnType("time");

            modelBuilder.Entity<CoachSchedule>()
                .Property(cs => cs.EndTime)
                .HasColumnType("time");

            // заполнение статусов
            modelBuilder.Entity<ReservationStatus>().HasData(
                new ReservationStatus { Id = 1, Name = "Ожидание" },
                new ReservationStatus { Id = 2, Name = "Посещена" },
                new ReservationStatus { Id = 3, Name = "Оплачена" },
                new ReservationStatus { Id = 4, Name = "Отменена" }
            );

            modelBuilder.Entity<TrainingStatus>().HasData(
                new TrainingStatus { Id = 1, Name = "Ожидание" },
                new TrainingStatus { Id = 2, Name = "Проведена" },
                new TrainingStatus { Id = 3, Name = "Отменена" }
            );
        }
    }
}
