using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Tests.InfrastructureTests
{
    public class MembershipRepositoryTests
    {
        private readonly Mock<FitnessDb> _dbMock;

        public MembershipRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FitnessDb>()
                .Options;

            _dbMock = new Mock<FitnessDb>(options);
        }

        [Fact]
        public async Task GetOverlappingMembershipsAsync_ShouldReturnMatches()
        {
            // Arrange
            var start = new DateTime(2026, 05, 10);
            var end = new DateTime(2026, 05, 20);

            var memberships = new List<Membership>
            {
                new()
                {
                    Id = 1,
                    ClientId = 1,
                    StartDate = new DateTime(2026, 05, 01),
                    EndDate = new DateTime(2026, 05, 15),
                    MembershipType = new MembershipType { Id = 1, Name = "Type1" },
                    Payment = new Payment { Id = 1 }
                },
                new()
                {
                    Id = 2,
                    ClientId = 1,
                    StartDate = new DateTime(2026, 05, 18),
                    EndDate = new DateTime(2026, 05, 25),
                    MembershipType = new MembershipType { Id = 2, Name = "Type2" },
                    Payment = new Payment { Id = 2 }
                },
                new()
                {
                    Id = 3,
                    ClientId = 2,
                    StartDate = new DateTime(2026, 05, 10),
                    EndDate = new DateTime(2026, 05, 20),
                    MembershipType = new MembershipType { Id = 3, Name = "Type3" },
                    Payment = new Payment { Id = 3 }
                }
            };

            _dbMock.Setup(x => x.Set<Membership>())
                .ReturnsDbSet(memberships);

            var repository = new MembershipRepository(_dbMock.Object);

            // Act
            var result = await repository.GetOverlappingMembershipsAsync(
                clientId: 1,
                startDate: start,
                endDate: end);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.All(result, m => Assert.Equal(1, m.ClientId));
        }

        [Fact]
        public async Task GetOverlappingMembershipsAsync_ShouldReturnEmpty_WhenNoMatches()
        {
            // Arrange
            var memberships = new List<Membership>
            {
                new()
                {
                    Id = 1,
                    ClientId = 1,
                    StartDate = new DateTime(2026, 01, 01),
                    EndDate = new DateTime(2026, 01, 10)
                }
            };

            _dbMock.Setup(x => x.Set<Membership>())
                .ReturnsDbSet(memberships);

            var repository = new MembershipRepository(_dbMock.Object);

            var start = new DateTime(2026, 05, 10);
            var end = new DateTime(2026, 05, 20);

            // Act
            var result = await repository.GetOverlappingMembershipsAsync(
                clientId: 1,
                startDate: start,
                endDate: end);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetOverlappingMembershipsAsync_ShouldFilterByClientId()
        {
            // Arrange
            var memberships = new List<Membership>
            {
                new()
                {
                    Id = 1,
                    ClientId = 1,
                    StartDate = new DateTime(2026, 05, 10),
                    EndDate = new DateTime(2026, 05, 20)
                },
                new()
                {
                    Id = 2,
                    ClientId = 2,
                    StartDate = new DateTime(2026, 05, 10),
                    EndDate = new DateTime(2026, 05, 20)
                }
            };

            _dbMock.Setup(x => x.Set<Membership>())
                .ReturnsDbSet(memberships);

            var repository = new MembershipRepository(_dbMock.Object);

            var start = new DateTime(2026, 05, 10);
            var end = new DateTime(2026, 05, 20);

            // Act
            var result = await repository.GetOverlappingMembershipsAsync(
                clientId: 1,
                startDate: start,
                endDate: end);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().ClientId);
        }
    }
}