using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Tests.InfrastructureTests
{
    public class NotificationRepositoryTests
    {
        private readonly Mock<FitnessDb> _dbMock;

        public NotificationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FitnessDb>()
                .Options;

            _dbMock = new Mock<FitnessDb>(options);
        }

        [Fact]
        public async Task GetExpiredNotificationsAsync_ShouldReturnOnlyExpiredAndUnnotified()
        {
            // Arrange
            var now = DateTime.Now;

            var notifications = new List<CancellationNotification>
            {
                new()
                {
                    Id = 1,
                    IsNotified = false,
                    Training = new Training
                    {
                        Id = 1,
                        EndDate = now.AddMinutes(-10)
                    }
                },
                new()
                {
                    Id = 2,
                    IsNotified = true,
                    Training = new Training
                    {
                        Id = 2,
                        EndDate = now.AddMinutes(-10)
                    }
                },
                new()
                {
                    Id = 3,
                    IsNotified = false,
                    Training = new Training
                    {
                        Id = 3,
                        EndDate = now.AddMinutes(10)
                    }
                }
            };

            _dbMock.Setup(x => x.Set<CancellationNotification>())
                .ReturnsDbSet(notifications);

            var repository = new NotificationRepository(_dbMock.Object);

            // Act
            var result = await repository.GetExpiredNotificationsAsync(CancellationToken.None);

            // Assert
            Assert.Single(result);

            var notification = result.First();

            Assert.Equal(1, notification.Id);
            Assert.False(notification.IsNotified);
            Assert.True(notification.Training.EndDate <= now);
        }

        [Fact]
        public async Task GetExpiredNotificationsAsync_ShouldReturnEmpty_WhenNoMatches()
        {
            // Arrange
            var notifications = new List<CancellationNotification>
            {
                new()
                {
                    Id = 1,
                    IsNotified = true,
                    Training = new Training
                    {
                        EndDate = DateTime.Now.AddMinutes(-10)
                    }
                }
            };

            _dbMock.Setup(x => x.Set<CancellationNotification>())
                .ReturnsDbSet(notifications);

            var repository = new NotificationRepository(_dbMock.Object);

            // Act
            var result = await repository.GetExpiredNotificationsAsync(CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }
    }
}