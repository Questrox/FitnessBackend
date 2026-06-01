using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Tests.InfrastructureTests
{
    public class ClientRepositoryTests
    {
        private readonly Mock<FitnessDb> _dbMock;

        public ClientRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FitnessDb>().Options;
            _dbMock = new Mock<FitnessDb>(options);
        }

        [Fact]
        public async Task GetPagedFilteredClientsAsync_ShouldReturnFilteredClients()
        {
            // Arrange
            var clients = new List<Client>
            {
                new()
                {
                    Id = 1,
                    User = new User
                    {
                        FullName = "Иван Иванов",
                        PhoneNumber = "123456",
                        UserName = "ivan"
                    },
                    Memberships = [],
                    TrainingReservations = []
                },
                new()
                {
                    Id = 2,
                    User = new User
                    {
                        FullName = "Петр Петров",
                        PhoneNumber = "999999",
                        UserName = "petr"
                    },
                    Memberships = [],
                    TrainingReservations = []
                }
            };

            _dbMock.Setup(x => x.Set<Client>())
                .ReturnsDbSet(clients);

            var repository = new ClientRepository(_dbMock.Object);

            // Act
            var result = await repository.GetPagedFilteredClientsAsync(
                1,
                10,
                "Иван");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal("Иван Иванов",
                result.Items.First().User.FullName);
        }

        [Fact]
        public async Task GetPagedFilteredClientsAsync_ShouldApplyPagination()
        {
            // Arrange
            var clients = Enumerable.Range(1, 20)
                .Select(i => new Client
                {
                    Id = i,
                    User = new User
                    {
                        FullName = $"Client {i}",
                        PhoneNumber = $"123{i}",
                        UserName = $"user{i}"
                    },
                    Memberships = [],
                    TrainingReservations = []
                })
                .ToList();

            _dbMock.Setup(x => x.Set<Client>()).ReturnsDbSet(clients);

            var repository = new ClientRepository(_dbMock.Object);

            // Act
            var result = await repository.GetPagedFilteredClientsAsync(
                page: 2,
                pageSize: 5,
                filter: "Client");

            // Assert
            Assert.Equal(20, result.TotalCount);

            Assert.Equal(5, result.Items.Count);

            Assert.Equal(6,
                result.Items.First().Id);
        }

        [Fact]
        public async Task GetPagedFilteredClientsAsync_ShouldReturnEmpty_WhenNoMatches()
        {
            // Arrange
            var clients = new List<Client>
            {
                new()
                {
                    Id = 1,
                    User = new User
                    {
                        FullName = "Иван Иванов",
                        PhoneNumber = "123456",
                        UserName = "ivan"
                    },
                    Memberships = [],
                    TrainingReservations = []
                }
            };

            _dbMock.Setup(x => x.Set<Client>()).ReturnsDbSet(clients);

            var repository = new ClientRepository(_dbMock.Object);

            // Act
            var result = await repository.GetPagedFilteredClientsAsync(
                1,
                10,
                "ААААААААААААААААА");

            // Assert
            Assert.Empty(result.Items);

            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetPagedFilteredClientsAsync_ShouldFilterByPhoneOrUsername()
        {
            // Arrange
            var clients = new List<Client>
            {
                new()
                {
                    Id = 1,
                    User = new User
                    {
                        FullName = "Иван Иванов",
                        PhoneNumber = "777777",
                        UserName = "specialUser"
                    },
                    Memberships = [],
                    TrainingReservations = []
                }
            };

            _dbMock.Setup(x => x.Set<Client>()).ReturnsDbSet(clients);

            var repository = new ClientRepository(_dbMock.Object);

            // Act
            var result1 =
                await repository.GetPagedFilteredClientsAsync(
                    1, 10, "777777");

            var result2 =
                await repository.GetPagedFilteredClientsAsync(
                    1, 10, "specialUser");

            // Assert
            Assert.Single(result1.Items);

            Assert.Single(result2.Items);
        }
    }
}