using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Tests.InfrastructureTests
{
    public class TrainingReservationRepositoryTests
    {
        private readonly Mock<FitnessDb> _dbMock;

        public TrainingReservationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FitnessDb>()
                .Options;

            _dbMock = new Mock<FitnessDb>(options);
        }

        #region GetReservationsByTrainingIdAsync

        [Fact]
        public async Task GetReservationsByTrainingIdAsync_ShouldReturnOnlyActiveReservations_ForTraining()
        {
            // Arrange
            var reservations = new List<TrainingReservation>
            {
                new()
                {
                    Id = 1,
                    TrainingId = 1,
                    ReservationStatusId = (int)ReservationStatusEnum.Pending,
                    Client = new Client { User = new User() }
                },
                new()
                {
                    Id = 2,
                    TrainingId = 1,
                    ReservationStatusId = (int)ReservationStatusEnum.Cancelled,
                    Client = new Client { User = new User() }
                },
                new()
                {
                    Id = 3,
                    TrainingId = 2,
                    ReservationStatusId = (int)ReservationStatusEnum.Pending,
                    Client = new Client { User = new User() }
                }
            };

            _dbMock.Setup(x => x.Set<TrainingReservation>())
                .ReturnsDbSet(reservations);

            var repo = new TrainingReservationRepository(_dbMock.Object);

            // Act
            var result = await repo.GetReservationsByTrainingIdAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
            Assert.Equal(1, result.First().TrainingId);
        }

        #endregion
    }
}