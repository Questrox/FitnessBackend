using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Tests.InfrastructureTests
{
    public class TrainingTypeRepositoryTests
    {
        private readonly Mock<FitnessDb> _dbMock;

        public TrainingTypeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FitnessDb>()
                .Options;

            _dbMock = new Mock<FitnessDb>(options);
        }

        #region GetTrainingTypesAsync

        [Fact]
        public async Task GetTrainingTypesAsync_ShouldReturnAll()
        {
            var types = new List<TrainingType>
            {
                new() { Id = 1, Name = "A", MaxClients = 1 },
                new() { Id = 2, Name = "B", MaxClients = 5 }
            };

            _dbMock.Setup(x => x.Set<TrainingType>())
                .ReturnsDbSet(types);

            var repo = new TrainingTypeRepository(_dbMock.Object);

            var result = await repo.GetTrainingTypesAsync();

            Assert.Equal(2, result.Count());
        }

        #endregion

        #region GetGroupTrainingTypesAsync

        [Fact]
        public async Task GetGroupTrainingTypesAsync_ShouldReturnOnlyGroupTypes()
        {
            var types = new List<TrainingType>
            {
                new() { Id = 1, MaxClients = 1 },
                new() { Id = 2, MaxClients = 5 },
                new() { Id = 3, MaxClients = 10 }
            };

            _dbMock.Setup(x => x.Set<TrainingType>())
                .ReturnsDbSet(types);

            var repo = new TrainingTypeRepository(_dbMock.Object);

            var result = await repo.GetGroupTrainingTypesAsync();

            Assert.Equal(2, result.Count());
            Assert.All(result, t => Assert.True(t.MaxClients > 1));
        }

        #endregion

        #region GetIndividualTrainingTypesAsync

        [Fact]
        public async Task GetIndividualTrainingTypesAsync_ShouldReturnOnlyIndividualTypes()
        {
            var types = new List<TrainingType>
            {
                new() { Id = 1, MaxClients = 1 },
                new() { Id = 2, MaxClients = 5 }
            };

            _dbMock.Setup(x => x.Set<TrainingType>())
                .ReturnsDbSet(types);

            var repo = new TrainingTypeRepository(_dbMock.Object);

            var result = await repo.GetIndividualTrainingTypesAsync();

            Assert.Single(result);
            Assert.Equal(1, result.First().MaxClients);
        }

        #endregion
    }
}