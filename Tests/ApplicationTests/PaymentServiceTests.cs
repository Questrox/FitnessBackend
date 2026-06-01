using Application.Models.CreateDTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace Tests.ApplicationTests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepMock;
        private readonly Mock<IClientRepository> _clientRepMock;
        private readonly PaymentService _service;

        public PaymentServiceTests()
        {
            _paymentRepMock = new Mock<IPaymentRepository>();
            _clientRepMock = new Mock<IClientRepository>();

            _service = new PaymentService(
                _paymentRepMock.Object,
                _clientRepMock.Object
            );
        }

        [Fact]
        public async Task AddPaymentAsync_ShouldCreatePayment_WhenDataIsValid()
        {
            // Arrange
            var client = new Client
            {
                Id = 1,
                Bonuses = 100
            };

            var dto = new CreatePaymentDTO
            {
                Price = 1000,
                CashbackPercentage = 10,
                PaidWithBonuses = 50,
                ClientId = 1,
                AdminId = "admin"
            };

            _clientRepMock
                .Setup(r => r.GetClientByIdAsync(dto.ClientId))
                .ReturnsAsync(client);

            _paymentRepMock
                .Setup(r => r.AddAsync(It.IsAny<Payment>()))
                .Callback<Payment>(p => p.Id = 10)
                .Returns(Task.CompletedTask);

            _paymentRepMock
                .Setup(r => r.GetPaymentByIdAsync(10))
                .ReturnsAsync((int id) => new Payment
                {
                    Id = id,
                    Price = dto.Price,
                    CashbackPercentage = dto.CashbackPercentage,
                    PaidWithBonuses = dto.PaidWithBonuses,
                    AdminId = dto.AdminId
                });

            // Act
            var result = await _service.AddPaymentAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Price, result.Price);

            Assert.Equal(150, client.Bonuses);

            _paymentRepMock.Verify(
                r => r.AddAsync(It.IsAny<Payment>()),
                Times.Once);
        }

        [Fact]
        public async Task AddPaymentAsync_ShouldThrow_WhenClientNotFound()
        {
            // Arrange
            var dto = new CreatePaymentDTO
            {
                ClientId = 1,
                AdminId = "admin"
            };

            _clientRepMock
                .Setup(r => r.GetClientByIdAsync(dto.ClientId))
                .ReturnsAsync((Client)null);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AddPaymentAsync(dto));
        }

        [Fact]
        public async Task AddPaymentAsync_ShouldThrow_WhenAdminIdIsNull()
        {
            // Arrange
            var client = new Client { Bonuses = 100 };

            var dto = new CreatePaymentDTO
            {
                ClientId = 1,
                AdminId = null
            };

            _clientRepMock
                .Setup(r => r.GetClientByIdAsync(dto.ClientId))
                .ReturnsAsync(client);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AddPaymentAsync(dto));
        }

        [Fact]
        public async Task AddPaymentAsync_ShouldThrow_WhenPaidWithBonusesNegative()
        {
            // Arrange
            var client = new Client { Bonuses = 100 };

            var dto = new CreatePaymentDTO
            {
                ClientId = 1,
                AdminId = "admin",
                PaidWithBonuses = -10
            };

            _clientRepMock
                .Setup(r => r.GetClientByIdAsync(dto.ClientId))
                .ReturnsAsync(client);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AddPaymentAsync(dto));
        }

        [Fact]
        public async Task AddPaymentAsync_ShouldThrow_WhenBonusesExceedPrice()
        {
            // Arrange
            var client = new Client { Bonuses = 1000 };

            var dto = new CreatePaymentDTO
            {
                Price = 100,
                PaidWithBonuses = 200,
                ClientId = 1,
                AdminId = "admin"
            };

            _clientRepMock
                .Setup(r => r.GetClientByIdAsync(dto.ClientId))
                .ReturnsAsync(client);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AddPaymentAsync(dto));
        }

        [Fact]
        public async Task AddPaymentAsync_ShouldThrow_WhenClientHasNotEnoughBonuses()
        {
            // Arrange
            var client = new Client
            {
                Bonuses = 10
            };

            var dto = new CreatePaymentDTO
            {
                Price = 1000,
                PaidWithBonuses = 50,
                ClientId = 1,
                AdminId = "admin"
            };

            _clientRepMock
                .Setup(r => r.GetClientByIdAsync(dto.ClientId))
                .ReturnsAsync(client);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AddPaymentAsync(dto));
        }
    }
}
