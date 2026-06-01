using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Tests.InfrastructureTests
{
    public class CoachRepositoryTests
    {
        private readonly Mock<FitnessDb> _dbMock;

        public CoachRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FitnessDb>()
                .Options;

            _dbMock = new Mock<FitnessDb>(options);
        }

        [Fact]
        public async Task GetAvailableCoachesAsync_ShouldReturnAvailableCoach()
        {
            // Arrange
            var start = new DateTime(2026, 6, 1, 10, 0, 0);
            var end = new DateTime(2026, 6, 1, 11, 0, 0);

            var coaches = new List<Coach>
            {
                // свободный тренер
                new Coach
                {
                    Id = 1,
                    User = new User
                    {
                        FullName = "Иванов Иван"
                    },
                    CoachSchedules =
                    [
                        new CoachSchedule
                        {
                            WeekDay = DayOfWeek.Monday,
                            StartTime = new TimeSpan(8,0,0),
                            EndTime = new TimeSpan(20,0,0)
                        }
                    ],
                    Trainings = []
                },

                // занят другой тренировкой
                new Coach
                {
                    Id = 2,
                    CoachSchedules =
                    [
                        new CoachSchedule
                        {
                            WeekDay = DayOfWeek.Monday,
                            StartTime = new TimeSpan(8,0,0),
                            EndTime = new TimeSpan(20,0,0)
                        }
                    ],
                    Trainings =
                    [
                        new Training
                        {
                            TrainingStatusId =
                                (int)TrainingStatusEnum.Pending,

                            StartDate = new DateTime(2026,6,1,10,30,0),
                            EndDate = new DateTime(2026,6,1,11,30,0)
                        }
                    ]
                },

                // не работает в это время
                new Coach
                {
                    Id = 3,
                    CoachSchedules =
                    [
                        new CoachSchedule
                        {
                            WeekDay = DayOfWeek.Monday,
                            StartTime = new TimeSpan(15,0,0),
                            EndTime = new TimeSpan(18,0,0)
                        }
                    ],
                    Trainings = []
                }
            };

            _dbMock.Setup(x => x.Set<Coach>()).ReturnsDbSet(coaches);

            var repository = new CoachRepository(_dbMock.Object);

            // Act
            var result = await repository.GetAvailableCoachesAsync(start, end);

            // Assert
            var resultList = result.ToList();

            Assert.Single(resultList);

            Assert.Equal(1, resultList[0].Id);
        }

        [Fact]
        public async Task GetAvailableCoachesAsync_ShouldIgnoreCancelledTrainings()
        {
            // Arrange
            var start = new DateTime(2026, 6, 1, 10, 0, 0);
            var end = new DateTime(2026, 6, 1, 11, 0, 0);

            var coaches = new List<Coach>
            {
                new Coach
                {
                    Id = 1,

                    CoachSchedules =
                    [
                        new CoachSchedule
                        {
                            WeekDay = DayOfWeek.Monday,
                            StartTime = new TimeSpan(8,0,0),
                            EndTime = new TimeSpan(20,0,0)
                        }
                    ],

                    Trainings =
                    [
                        new Training
                        {
                            TrainingStatusId =
                                (int)TrainingStatusEnum.Cancelled,

                            StartDate = new DateTime(2026,6,1,10,0,0),
                            EndDate = new DateTime(2026,6,1,11,0,0)
                        }
                    ]
                }
            };

            _dbMock.Setup(x => x.Set<Coach>()).ReturnsDbSet(coaches);

            var repository = new CoachRepository(_dbMock.Object);

            // Act
            var result = await repository.GetAvailableCoachesAsync(start, end);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAvailableCoachesAsync_ShouldReturnEmpty_WhenNoAvailableCoaches()
        {
            // Arrange
            var start = new DateTime(2026, 6, 1, 10, 0, 0);
            var end = new DateTime(2026, 6, 1, 11, 0, 0);

            var coaches = new List<Coach>
            {
                new Coach
                {
                    Id = 1,

                    CoachSchedules =
                    [
                        new CoachSchedule
                        {
                            WeekDay = DayOfWeek.Monday,
                            StartTime = new TimeSpan(15,0,0),
                            EndTime = new TimeSpan(18,0,0)
                        }
                    ],

                    Trainings = []
                }
            };

            _dbMock.Setup(x => x.Set<Coach>()).ReturnsDbSet(coaches);

            var repository = new CoachRepository(_dbMock.Object);

            // Act
            var result = await repository.GetAvailableCoachesAsync(start, end);

            // Assert
            Assert.Empty(result);
        }
    }
}