using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace Tests.ApplicationTests
{
    public class MembershipServiceTests
    {
        private readonly Mock<IMembershipRepository> _membershipRepMock;
        private readonly Mock<IMembershipTypeRepository> _membershipTypeRepMock;
        private readonly Mock<PaymentService> _paymentServiceMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ITransaction> _transactionMock;

        private readonly MembershipService _service;

        public MembershipServiceTests()
        {
            _membershipRepMock = new Mock<IMembershipRepository>();
            _membershipTypeRepMock = new Mock<IMembershipTypeRepository>();
            var paymentRepMock = new Mock<IPaymentRepository>();
            var clientRepMock = new Mock<IClientRepository>();
            _paymentServiceMock = new Mock<PaymentService>(paymentRepMock.Object, clientRepMock.Object);
            _uowMock = new Mock<IUnitOfWork>();
            _transactionMock = new Mock<ITransaction>();

            _uowMock
                .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);

            _service = new MembershipService(
                _uowMock.Object,
                _paymentServiceMock.Object,
                _membershipRepMock.Object,
                _membershipTypeRepMock.Object
            );
        }

        [Fact]
        public async Task AddMembershipAsync_ShouldCreateMembership_WhenDataIsValid()
        {
            // Arrange
            var dto = new CreateMembershipDTO
            {
                StartDate = DateTime.Now.Date.AddDays(1),
                ClientId = 1,
                MembershipTypeId = 2,
                PaidWithBonuses = 0,
                AdminId = "admin"
            };

            var membershipType = new MembershipType
            {
                Id = 2,
                Price = 5000,
                CashbackPercentage = 10,
                Duration = 3
            };

            var payment = new PaymentDTO
            {
                Id = 10
            };

            _membershipTypeRepMock
                .Setup(r => r.GetMembershipTypeById(dto.MembershipTypeId))
                .ReturnsAsync(membershipType);

            _paymentServiceMock
                .Setup(s => s.AddPaymentAsync(It.IsAny<CreatePaymentDTO>()))
                .ReturnsAsync(payment);

            _membershipRepMock
                .Setup(r => r.AddAsync(It.IsAny<Membership>()))
                .Callback<Membership>(m => m.Id = 15)
                .Returns(Task.CompletedTask);

            _membershipRepMock
                .Setup(r => r.GetMembershipByIdAsync(15))
                .ReturnsAsync((int id) => new Membership
                {
                    Id = id,
                    StartDate = dto.StartDate.Date,
                    EndDate = dto.StartDate.AddMonths(3).Date,
                    ClientId = dto.ClientId,
                    MembershipTypeId = dto.MembershipTypeId,
                    PaymentId = payment.Id
                });

            // Act
            var result = await _service.AddMembershipAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.ClientId, result.ClientId);

            _membershipRepMock.Verify(
                r => r.AddAsync(It.IsAny<Membership>()),
                Times.Once);

            _transactionMock.Verify(
                t => t.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AddMembershipAsync_ShouldThrow_WhenMembershipTypeNotFound()
        {
            // Arrange
            var dto = new CreateMembershipDTO
            {
                MembershipTypeId = 999,
                StartDate = DateTime.Now.AddDays(1)
            };

            _membershipTypeRepMock
                .Setup(r => r.GetMembershipTypeById(dto.MembershipTypeId))
                .ReturnsAsync((MembershipType)null);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AddMembershipAsync(dto));

            _transactionMock.Verify(
                t => t.RollbackAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AddMembershipAsync_ShouldThrow_WhenStartDateInPast()
        {
            // Arrange
            var dto = new CreateMembershipDTO
            {
                MembershipTypeId = 1,
                StartDate = DateTime.Now.AddDays(-1)
            };

            _membershipTypeRepMock
                .Setup(r => r.GetMembershipTypeById(dto.MembershipTypeId))
                .ReturnsAsync(new MembershipType
                {
                    Id = 1,
                    Duration = 1
                });

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AddMembershipAsync(dto));

            _transactionMock.Verify(
                t => t.RollbackAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AddMembershipAsync_ShouldRollback_WhenPaymentFails()
        {
            // Arrange
            var dto = new CreateMembershipDTO
            {
                MembershipTypeId = 1,
                ClientId = 1,
                StartDate = DateTime.Now.AddDays(1)
            };

            _membershipTypeRepMock
                .Setup(r => r.GetMembershipTypeById(dto.MembershipTypeId))
                .ReturnsAsync(new MembershipType
                {
                    Id = 1,
                    Duration = 1,
                    Price = 1000
                });

            _paymentServiceMock
                .Setup(s => s.AddPaymentAsync(It.IsAny<CreatePaymentDTO>()))
                .ThrowsAsync(new Exception("Payment error"));

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(
                () => _service.AddMembershipAsync(dto));

            _transactionMock.Verify(
                t => t.RollbackAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            _transactionMock.Verify(
                t => t.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task AddMembershipAsync_ShouldCreateCorrectPaymentDto()
        {
            // Arrange
            var dto = new CreateMembershipDTO
            {
                StartDate = DateTime.Now.AddDays(1),
                MembershipTypeId = 3,
                ClientId = 5,
                PaidWithBonuses = 100,
                AdminId = "admin123"
            };

            var mt = new MembershipType
            {
                Id = 3,
                Price = 4000,
                CashbackPercentage = 7,
                Duration = 6
            };

            _membershipTypeRepMock
                .Setup(r => r.GetMembershipTypeById(dto.MembershipTypeId))
                .ReturnsAsync(mt);

            _paymentServiceMock
                .Setup(s => s.AddPaymentAsync(
                    It.Is<CreatePaymentDTO>(p =>
                        p.Price == mt.Price &&
                        p.CashbackPercentage == mt.CashbackPercentage &&
                        p.ClientId == dto.ClientId &&
                        p.AdminId == dto.AdminId &&
                        p.PaidWithBonuses == dto.PaidWithBonuses)))
                .ReturnsAsync(new PaymentDTO { Id = 1 });

            _membershipRepMock
                .Setup(r => r.AddAsync(It.IsAny<Membership>()))
                .Callback<Membership>(m => m.Id = 1)
                .Returns(Task.CompletedTask);

            _membershipRepMock
                .Setup(r => r.GetMembershipByIdAsync(1))
                .ReturnsAsync(new Membership { Id = 1 });

            // Act
            await _service.AddMembershipAsync(dto);

            // Assert
            _paymentServiceMock.Verify(
                x => x.AddPaymentAsync(It.IsAny<CreatePaymentDTO>()),
                Times.Once);
        }
    }
}