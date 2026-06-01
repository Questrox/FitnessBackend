using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Tests.InfrastructureTests
{
    public class TrainingRepositoryTests
    {
        private readonly Mock<FitnessDb> _dbMock;

        public TrainingRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FitnessDb>()
                .Options;

            _dbMock = new Mock<FitnessDb>(options);
        }

        #region GetTrainingsForPeriodAsync

        [Fact]
        public async Task GetTrainingsForPeriodAsync_ShouldReturnTrainings_InPeriod_ForCoach()
        {
            // Arrange
            var now = DateTime.Now;

            var start = now.AddDays(-10);
            var end = now.AddDays(10);

            var coachId = 1;

            var trainings = new List<Training>
            {
                new()
                {
                    Id = 1,
                    StartDate = now.AddDays(-5),
                    EndDate = now.AddDays(-5).AddHours(1),
                    CoachId = 1,
                    TrainingType = new TrainingType { MaxClients = 1 },
                    Coach = new Coach { Id = 1 },
                    TrainingStatusId = (int)TrainingStatusEnum.Pending,
                    TrainingReservations = []
                },
                new()
                {
                    Id = 2,
                    StartDate = now.AddDays(-3),
                    EndDate = now.AddDays(-3).AddHours(1),
                    CoachId = 2,
                    TrainingType = new TrainingType { MaxClients = 5 },
                    Coach = new Coach { Id = 2 },
                    TrainingStatusId = (int)TrainingStatusEnum.Pending,
                    TrainingReservations = []
                },
                new()
                {
                    Id = 3,
                    StartDate = now.AddDays(20), // вне диапазона
                    EndDate = now.AddDays(20).AddHours(1),
                    CoachId = 1,
                    TrainingType = new TrainingType { MaxClients = 5 },
                    Coach = new Coach { Id = 1 },
                    TrainingStatusId = (int)TrainingStatusEnum.Pending,
                    TrainingReservations = []
                }
            };

            _dbMock.Setup(x => x.Set<Training>())
                .ReturnsDbSet(trainings);

            var repo = new TrainingRepository(_dbMock.Object);

            // Act
            var result = await repo.GetTrainingsForPeriodAsync(start, end, coachId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, t =>
                Assert.True(t.StartDate.Date >= start.Date && t.EndDate.Date <= end.Date));

            Assert.Contains(result, t => t.Id == 1);
            Assert.Contains(result, t => t.Id == 2);
        }

        [Fact]
        public async Task GetTrainingsForPeriodAsync_ShouldReturnOnlyGroup_WhenCoachIsNull()
        {
            // Arrange
            var now = DateTime.Now;

            var start = now.AddDays(-10);
            var end = now.AddDays(10);

            var trainings = new List<Training>
            {
                new()
                {
                    Id = 1,
                    StartDate = now.AddDays(-5),
                    EndDate = now.AddDays(-5).AddHours(1),
                    TrainingType = new TrainingType { MaxClients = 1 },
                    CoachId = 1,
                    TrainingReservations = []
                },
                new()
                {
                    Id = 2,
                    StartDate = now.AddDays(-5),
                    EndDate = now.AddDays(-5).AddHours(1),
                    TrainingType = new TrainingType { MaxClients = 5 },
                    CoachId = 2,
                    TrainingReservations = []
                }
            };

            _dbMock.Setup(x => x.Set<Training>())
                .ReturnsDbSet(trainings);

            var repo = new TrainingRepository(_dbMock.Object);

            // Act
            var result = await repo.GetTrainingsForPeriodAsync(start, end, null);

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result.First().Id);
        }

        #endregion

        #region GetTrainingsWithNotificationsAsync

        [Fact]
        public async Task GetTrainingsWithNotificationsAsync_ShouldReturnOnlyCancelledTrainings()
        {
            // Arrange
            var now = DateTime.Now;

            var trainings = new List<Training>
            {
                new()
                {
                    Id = 1,
                    StartDate = now.AddDays(-5),
                    TrainingStatusId = (int)TrainingStatusEnum.Cancelled,
                    TrainingType = new TrainingType(),
                    Coach = new Coach(),
                    CancellationNotifications = new List<CancellationNotification>()
                },
                new()
                {
                    Id = 2,
                    StartDate = now.AddDays(-4),
                    TrainingStatusId = (int)TrainingStatusEnum.Pending,
                    TrainingType = new TrainingType(),
                    Coach = new Coach(),
                    CancellationNotifications = new List<CancellationNotification>()
                },
                new()
                {
                    Id = 3,
                    StartDate = now.AddDays(-3),
                    TrainingStatusId = (int)TrainingStatusEnum.Cancelled,
                    TrainingType = new TrainingType(),
                    Coach = new Coach(),
                    CancellationNotifications = new List<CancellationNotification>()
                }
            };

            _dbMock.Setup(x => x.Set<Training>())
                .ReturnsDbSet(trainings);

            var repo = new TrainingRepository(_dbMock.Object);

            // Act
            var result = await repo.GetTrainingsWithNotificationsAsync();

            // Assert
            Assert.Equal(2, result.Count());

            Assert.All(result, t =>
                Assert.Equal((int)TrainingStatusEnum.Cancelled, t.TrainingStatusId));

            Assert.Equal(1, result.First().Id); // OrderBy StartDate
        }

        #endregion
    }
}