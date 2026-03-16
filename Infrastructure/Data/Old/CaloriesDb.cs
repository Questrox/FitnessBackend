using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.Entities.Old;

namespace Infrastructure.Data.Old
{
    public class CaloriesDb : IdentityDbContext<User>
    {
        public CaloriesDb(DbContextOptions<CaloriesDb> options)
            : base(options)
        { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Food> Foods { get; set; }
        public virtual DbSet<FoodEntry> FoodEntries { get; set; }
        public virtual DbSet<MealPlan> MealPlans { get; set; }
        public virtual DbSet<MealPlanDay> MealPlanDays { get; set; }
        public virtual DbSet<MealType> MealTypes { get; set; }
        public virtual DbSet<WaterEntry> WaterEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- USER ----------
            modelBuilder.Entity<User>()
                .HasOne(u => u.MealPlan)
                .WithMany(p => p.User)
                .HasForeignKey(u => u.MealPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            // ---------- FOOD ----------
            modelBuilder.Entity<Food>()
                .HasOne(f => f.User)
                .WithMany(u => u.Food)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- FOOD ENTRY ----------
            modelBuilder.Entity<FoodEntry>()
                .HasOne(e => e.Food)
                .WithMany(f => f.FoodEntry)
                .HasForeignKey(e => e.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FoodEntry>()
                .HasOne(e => e.User)
                .WithMany(u => u.FoodEntry)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FoodEntry>()
                .HasOne(e => e.MealType)
                .WithMany(m => m.FoodEntry)
                .HasForeignKey(e => e.MealTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- MEAL PLAN DAY ----------
            modelBuilder.Entity<MealPlanDay>()
                .HasOne(d => d.MealPlan)
                .WithMany(p => p.MealPlanDay)
                .HasForeignKey(d => d.MealPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- WATER ----------
            modelBuilder.Entity<WaterEntry>()
                .HasOne(w => w.User)
                .WithMany(u => u.WaterEntry)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- MEAL TYPE INIT (optional) ----------
            modelBuilder.Entity<MealType>().HasData(
                new MealType { Id = 1, Name = "breakfast" },
                new MealType { Id = 2, Name = "lunch" },
                new MealType { Id = 3, Name = "dinner" }
            );
        }
    }
}
