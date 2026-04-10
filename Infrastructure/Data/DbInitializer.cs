using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(FitnessDb context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            #region Роли и пользователи

            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
                await roleManager.CreateAsync(new IdentityRole("Coach"));
            }

            // Админы
            if (!userManager.Users.Any(u => u.UserName == "admin1"))
            {
                var admin1 = new User
                {
                    UserName = "admin1",
                    Email = "admin1@test.com",
                    FullName = "Иванов Александр Иванович",
                    PhoneNumber = "+79633346795"
                };
                await userManager.CreateAsync(admin1, "Admin@123");
                await userManager.AddToRoleAsync(admin1, "Admin");
            }

            if (!userManager.Users.Any(u => u.UserName == "admin2"))
            {
                var admin2 = new User
                {
                    UserName = "admin2",
                    Email = "admin2@test.com",
                    FullName = "Иванова Елена Ивановна",
                    PhoneNumber = "+79044213968"
                };
                await userManager.CreateAsync(admin2, "Admin@123");
                await userManager.AddToRoleAsync(admin2, "Admin");
            }

            // Клиенты
            User clientUser1 = null!;
            User clientUser2 = null!;

            if (!userManager.Users.Any(u => u.UserName == "client1"))
            {
                clientUser1 = new User
                {
                    UserName = "client1",
                    Email = "client1@test.com",
                    FullName = "Афонин Михаил Михайлович",
                    PhoneNumber = "+79082876591"
                };
                await userManager.CreateAsync(clientUser1, "Client@123");
                await userManager.AddToRoleAsync(clientUser1, "User");
            }
            else clientUser1 = await userManager.FindByNameAsync("client1");

            if (!userManager.Users.Any(u => u.UserName == "client2"))
            {
                clientUser2 = new User
                {
                    UserName = "client2",
                    Email = "client2@test.com",
                    FullName = "Егоров Алексей Юрьевич",
                    PhoneNumber = "+79658126366"
                };
                await userManager.CreateAsync(clientUser2, "Client@123");
                await userManager.AddToRoleAsync(clientUser2, "User");
            }
            else clientUser2 = await userManager.FindByNameAsync("client2");

            // Тренеры
            User coachUser1 = null!;
            User coachUser2 = null!;

            if (!userManager.Users.Any(u => u.UserName == "coach1"))
            {
                coachUser1 = new User
                {
                    UserName = "coach1",
                    Email = "coach1@test.com",
                    FullName = "Шаров Александр Александрович",
                    PhoneNumber = "+79053767851"
                };
                await userManager.CreateAsync(coachUser1, "Coach@123");
                await userManager.AddToRoleAsync(coachUser1, "Coach");
            }
            else coachUser1 = await userManager.FindByNameAsync("coach1");

            if (!userManager.Users.Any(u => u.UserName == "coach2"))
            {
                coachUser2 = new User
                {
                    UserName = "coach2",
                    Email = "coach2@test.com",
                    FullName = "Смирнов Антон Георгиевич",
                    PhoneNumber = "+79651850415"
                };
                await userManager.CreateAsync(coachUser2, "Coach@123");
                await userManager.AddToRoleAsync(coachUser2, "Coach");
            }
            else coachUser2 = await userManager.FindByNameAsync("coach2");

            #endregion

            #region TrainingTypes

            if (!context.TrainingTypes.Any())
            {
                context.TrainingTypes.AddRange(
                    new TrainingType
                    {
                        Name = "Йога",
                        Description = "Классическая Хатха-йога в облегченном варианте, направлена на укрепление мышц позвоночного столба, " +
                        "восполнение амплитуды движения в крупных суставах, улучшение кровообращения, освоение базовых дыхательных техник.",
                        Price = 600,
                        CashbackPercentage = 3,
                        MaxClients = 15,
                        Duration = 60,
                        PhotoPath = "images/TrainingTypes/Yoga.jpg"
                    },
                    new TrainingType
                    {
                        Name = "Пилатес",
                        Description = "Комплекс физических упражнений, направленный на укрепление мышечного корсета, развитие гибкости, улучшение координации движений, исправление осанки. " +
                        "Тренировка выполняется в спокойном темпе, не провоцируя перенапряжения и учащенного сердцебиения. " +
                        "Во время занятий пилатесом задействованы мышцы и разум, под контролем которого выполняются движения.",
                        Price = 1000,
                        CashbackPercentage = 7,
                        MaxClients = 5,
                        Duration = 90,
                        PhotoPath = "images/TrainingTypes/Pilates.jpg"
                    },
                    new TrainingType
                    {
                        Name = "Cycle",
                        Description = "Занятие проходит в малых группах на стационарных велосипедах — спин байках. " +
                        "Это мощная и энергичная кардиотренировка. " +
                        "Сюда приходят те, кто хочет похудеть, зарядиться энергией и начать вести активный образ жизни.",
                        Price = 700,
                        CashbackPercentage = 5,
                        MaxClients = 10,
                        Duration = 75,
                        PhotoPath = "images/TrainingTypes/Cycle.jpg"
                    },
                    new TrainingType
                    {
                        Name = "Кроссфит",
                        Description = "Высокоинтенсивный вид спорта, включающий элементы кардио, аэробики, тяжелой атлетики, спортивной гимнастики и гребли. " +
                        "Цель кроссфита – гармоничное развитие тела человека в сочетании с качественной физической подготовкой и выносливостью.",
                        Price = 700,
                        CashbackPercentage = 3,
                        MaxClients = 10,
                        Duration = 60,
                        PhotoPath = "images/TrainingTypes/Crossfit.jpeg"
                    },
                    new TrainingType
                    {
                        Name = "Индивидуальная тренировка (тренажерный зал)",
                        Description = "Индивидуальная тренировка с тренером в тренажерном зале.",
                        Price = 1500,
                        CashbackPercentage = 7,
                        MaxClients = 1,
                        Duration = 60,
                        PhotoPath = "images/TrainingTypes/Тренажерный зал.jpg"
                    },
                    new TrainingType
                    {
                        Name = "Индивидуальная тренировка (бассейн)",
                        Description = "Индивидуальная тренировка с тренером в бассейне.",
                        Price = 1500,
                        CashbackPercentage = 7,
                        MaxClients = 1,
                        Duration = 60,
                        PhotoPath = "images/TrainingTypes/Бассейн.jpg"
                    }
                );

                await context.SaveChangesAsync();
            }

            #endregion

            #region MembershipTypes

            if (!context.MembershipTypes.Any())
            {
                context.MembershipTypes.AddRange(
                    new MembershipType
                    {
                        Name = "Месячный",
                        Description = "Самый базовый вариант: 30 дней безлимитного фитнеса",
                        Price = 5000,
                        CashbackPercentage = 5,
                        Duration = 1
                    },
                    new MembershipType
                    {
                        Name = "Квартальный",
                        Description = "Выгоднее месячного абонемента: меньше цена за месяц и больше бонусов",
                        Price = 13000,
                        CashbackPercentage = 7,
                        Duration = 3
                    },
                    new MembershipType
                    {
                        Name = "Годовой",
                        Description = "Самый выгодный",
                        Price = 48000,
                        CashbackPercentage = 10,
                        Duration = 12
                    }
                );

                await context.SaveChangesAsync();
            }

            #endregion

            #region Clients

            if (!context.Clients.Any())
            {
                context.Clients.AddRange(
                    new Client
                    {
                        Bonuses = 0,
                        UserId = clientUser1.Id
                    },
                    new Client
                    {
                        Bonuses = 50,
                        UserId = clientUser2.Id
                    }
                );

                await context.SaveChangesAsync();
            }

            #endregion

            #region Coaches

            if (!context.Coaches.Any())
            {
                context.Coaches.AddRange(
                    new Coach
                    {
                        Experience = 5,
                        PhotoPath = null,
                        UserId = coachUser1.Id
                    },
                    new Coach
                    {
                        Experience = 7,
                        PhotoPath = null,
                        UserId = coachUser2.Id
                    }
                );

                await context.SaveChangesAsync();
            }

            if (!context.CoachSchedules.Any())
            {
                var coaches = await context.Coaches.ToListAsync();

                if (coaches.Count < 2)
                    throw new Exception("Недостаточно тренеров для инициализации расписания");

                var coach1 = coaches[0];
                var coach2 = coaches[1];

                var schedules = new List<CoachSchedule>();

                // Тренер 1: выходные - суббота, воскресенье
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                        continue;

                    schedules.Add(new CoachSchedule
                    {
                        CoachId = coach1.Id,
                        WeekDay = day,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0)
                    });
                }

                // Тренер 2: выходные - среда, четверг
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (day == DayOfWeek.Wednesday || day == DayOfWeek.Thursday)
                        continue;

                    schedules.Add(new CoachSchedule
                    {
                        CoachId = coach2.Id,
                        WeekDay = day,
                        StartTime = new TimeSpan(12, 0, 0),
                        EndTime = new TimeSpan(20, 0, 0)
                    });
                }

                context.CoachSchedules.AddRange(schedules);
                await context.SaveChangesAsync();
            }

            #endregion
        }
    }
}