using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.ApplicationTests
{
    public class TrainingReservationServiceTests
    {
        #region Инициализация
        private readonly Mock<IUnitOfWork> _uowMock = new();

        private readonly Mock<ITrainingReservationRepository> _reservationRepMock = new();
        private readonly Mock<ICoachRepository> _coachRepMock = new();

        private readonly Mock<ITrainingRepository> _trainingRepMock = new();
        private readonly Mock<ITrainingTypeRepository> _trainingTypeRepMock = new();
        private readonly Mock<IClientRepository> _clientRepMock = new();
        private readonly Mock<INotificationRepository> _notificationRepMock = new();

        private readonly Mock<IPaymentRepository> _paymentRepMock = new();

        private readonly Mock<ITransaction> _transactionMock;

        private readonly Mock<TrainingService> _trainingServiceMock;
        private readonly Mock<PaymentService> _paymentServiceMock;

        private readonly TrainingReservationService _service;

        public TrainingReservationServiceTests()
        {
            _transactionMock = new Mock<ITransaction>();

            _uowMock
                .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);

            _paymentServiceMock = new Mock<PaymentService>(
                _paymentRepMock.Object,
                _clientRepMock.Object);

            _trainingServiceMock = new Mock<TrainingService>(
                _trainingRepMock.Object,
                _trainingTypeRepMock.Object,
                _coachRepMock.Object,
                _clientRepMock.Object,
                _reservationRepMock.Object,
                _notificationRepMock.Object,
                _uowMock.Object);

            _service = new TrainingReservationService(
                _uowMock.Object,
                _reservationRepMock.Object,
                _coachRepMock.Object,
                _trainingServiceMock.Object,
                _paymentServiceMock.Object);
        }
        #endregion

        #region ConfirmTrainingAttendanceAsync

        [Fact]
        public async Task ConfirmTrainingAttendanceAsync_ShouldConfirmAttendance_WhenDataIsValid()
        {
            // Arrange
            int resId = 1;
            string userId = "coachUser";

            var reservation = new TrainingReservation
            {
                Id = resId,
                ReservationStatusId = (int)ReservationStatusEnum.Pending,
                Training = new Training
                {
                    CoachId = 10,
                    TrainingStatusId = (int)TrainingStatusEnum.Pending
                }
            };

            var coach = new Coach
            {
                Id = 10
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(resId))
                .ReturnsAsync(reservation);

            _coachRepMock
                .Setup(r => r.GetCoachByUserIdAsync(userId))
                .ReturnsAsync(coach);

            _reservationRepMock
                .Setup(r => r.UpdateAsync(It.IsAny<TrainingReservation>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service
                .ConfirmTrainingAttendanceAsync(resId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ReservationStatusEnum.Visited,
                reservation.ReservationStatusId);

            _reservationRepMock.Verify(
                r => r.UpdateAsync(It.IsAny<TrainingReservation>()),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmTrainingAttendanceAsync_ShouldThrow_WhenCoachNotFound()
        {
            // Arrange
            int resId = 1;
            string userId = "coachUser";

            _coachRepMock
                .Setup(r => r.GetCoachByUserIdAsync(userId))
                .ReturnsAsync((Coach?)null);

            // Act + Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmTrainingAttendanceAsync(resId, userId));

            Assert.Equal("Не найден тренер", ex.Message);
        }

        [Fact]
        public async Task ConfirmTrainingAttendanceAsync_ShouldThrow_WhenReservationNotFound()
        {
            // Arrange
            int resId = 1;
            string userId = "coachUser";

            _coachRepMock
                .Setup(r => r.GetCoachByUserIdAsync(userId))
                .ReturnsAsync(new Coach { Id = 10 });

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(resId))
                .ReturnsAsync((TrainingReservation?)null);

            // Act + Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmTrainingAttendanceAsync(resId, userId));

            Assert.Equal($"Запись с Id {resId} не найдена", ex.Message);
        }

        [Fact]
        public async Task ConfirmTrainingAttendanceAsync_ShouldThrow_WhenCoachIsNotResponsible()
        {
            // Arrange
            int resId = 1;
            string userId = "coachUser";

            var reservation = new TrainingReservation
            {
                Training = new Training
                {
                    CoachId = 20
                }
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(resId))
                .ReturnsAsync(reservation);

            _coachRepMock
                .Setup(r => r.GetCoachByUserIdAsync(userId))
                .ReturnsAsync(new Coach { Id = 10 });

            // Act + Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmTrainingAttendanceAsync(resId, userId));

            Assert.Equal(
                "Подтвердить посещение может только ответственный за тренировку тренер",
                ex.Message);
        }

        [Fact]
        public async Task ConfirmTrainingAttendanceAsync_ShouldThrow_WhenAttendanceAlreadyConfirmed()
        {
            // Arrange
            int resId = 1;
            string userId = "coachUser";

            var reservation = new TrainingReservation
            {
                ReservationStatusId = (int)ReservationStatusEnum.Visited,
                Training = new Training
                {
                    CoachId = 10
                }
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(resId))
                .ReturnsAsync(reservation);

            _coachRepMock
                .Setup(r => r.GetCoachByUserIdAsync(userId))
                .ReturnsAsync(new Coach { Id = 10 });

            // Act + Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmTrainingAttendanceAsync(resId, userId));

            Assert.Equal("Посещение уже подтверждено", ex.Message);
        }

        [Fact]
        public async Task ConfirmTrainingAttendanceAsync_ShouldThrow_WhenReservationCancelled()
        {
            // Arrange
            int resId = 1;
            string userId = "coachUser";

            var reservation = new TrainingReservation
            {
                ReservationStatusId = (int)ReservationStatusEnum.Cancelled,
                Training = new Training
                {
                    CoachId = 10
                }
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(resId))
                .ReturnsAsync(reservation);

            _coachRepMock
                .Setup(r => r.GetCoachByUserIdAsync(userId))
                .ReturnsAsync(new Coach { Id = 10 });

            // Act + Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmTrainingAttendanceAsync(resId, userId));

            Assert.Equal("Запись отменена", ex.Message);
        }

        [Fact]
        public async Task ConfirmTrainingAttendanceAsync_ShouldThrow_WhenTrainingCancelled()
        {
            // Arrange
            int resId = 1;
            string userId = "coachUser";

            var reservation = new TrainingReservation
            {
                ReservationStatusId = (int)ReservationStatusEnum.Pending,
                Training = new Training
                {
                    CoachId = 10,
                    TrainingStatusId = (int)TrainingStatusEnum.Cancelled
                }
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(resId))
                .ReturnsAsync(reservation);

            _coachRepMock
                .Setup(r => r.GetCoachByUserIdAsync(userId))
                .ReturnsAsync(new Coach { Id = 10 });

            // Act + Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmTrainingAttendanceAsync(resId, userId));

            Assert.Equal(
                "Нельзя подтвердить посещение у отмененной тренировки",
                ex.Message);
        }

        [Fact]
        public async Task ConfirmTrainingAttendanceAsync_ShouldThrow_WhenTrainingCompleted()
        {
            // Arrange
            int resId = 1;
            string userId = "coachUser";

            var reservation = new TrainingReservation
            {
                ReservationStatusId = (int)ReservationStatusEnum.Pending,
                Training = new Training
                {
                    CoachId = 10,
                    TrainingStatusId = (int)TrainingStatusEnum.Completed
                }
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(resId))
                .ReturnsAsync(reservation);

            _coachRepMock
                .Setup(r => r.GetCoachByUserIdAsync(userId))
                .ReturnsAsync(new Coach { Id = 10 });

            // Act + Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmTrainingAttendanceAsync(resId, userId));

            Assert.Equal("Тренировка уже проведена", ex.Message);
        }

        #endregion

        #region CancelReservationAsync

        [Fact]
        public async Task CancelReservationAsync_ShouldCancelGroupReservation()
        {
            // Arrange
            int id = 1;

            var reservation = new TrainingReservation
            {
                Id = id,
                ReservationStatusId = (int)ReservationStatusEnum.Pending,
                Training = new Training
                {
                    TrainingType = new TrainingType
                    {
                        MaxClients = 10
                    }
                }
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(id))
                .ReturnsAsync(reservation);

            _reservationRepMock
                .Setup(r => r.UpdateAsync(It.IsAny<TrainingReservation>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CancelReservationAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(
                (int)ReservationStatusEnum.Cancelled,
                reservation.ReservationStatusId);

            _reservationRepMock.Verify(
                r => r.UpdateAsync(It.IsAny<TrainingReservation>()),
                Times.Once);
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldCancelTraining_ForIndividualTraining()
        {
            // Arrange
            int id = 1;

            var reservation = new TrainingReservation
            {
                Id = id,
                ReservationStatusId = (int)ReservationStatusEnum.Pending,
                Training = new Training
                {
                    Id = 15,
                    TrainingType = new TrainingType
                    {
                        MaxClients = 1
                    }
                }
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(id))
                .ReturnsAsync(reservation);

            _trainingServiceMock
                .Setup(s => s.CancelTrainingAsync(15, "", false))
                .ReturnsAsync(new TrainingDTO());

            // Act
            await _service.CancelReservationAsync(id);

            // Assert
            _trainingServiceMock.Verify(
                s => s.CancelTrainingAsync(15, "", false),
                Times.Once);
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldThrow_WhenReservationNotFound()
        {
            // Arrange
            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(1))
                .ReturnsAsync((TrainingReservation?)null);

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CancelReservationAsync(1));
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldThrow_WhenStatusInvalid()
        {
            // Arrange
            var reservation = new TrainingReservation
            {
                ReservationStatusId =
                    (int)ReservationStatusEnum.Cancelled
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(1))
                .ReturnsAsync(reservation);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CancelReservationAsync(1));
        }

        #endregion

        #region ConfirmPaymentAsync
        [Fact]
        public async Task ConfirmPaymentAsync_ShouldConfirmPayment_WhenDataValid()
        {
            // Arrange
            int reservationId = 1;

            var dto = new CreatePaymentDTO
            {
                Price = 1000,
                CashbackPercentage = 5,
                PaidWithBonuses = 0,
                ClientId = 1,
                AdminId = "admin"
            };

            var reservation = new TrainingReservation
            {
                Id = reservationId,
                ReservationStatusId =
                    (int)ReservationStatusEnum.Visited
            };

            var payment = new PaymentDTO
            {
                Id = 50
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(reservationId))
                .ReturnsAsync(reservation);

            _paymentServiceMock
                .Setup(s => s.AddPaymentAsync(
                    It.IsAny<CreatePaymentDTO>()))
                .ReturnsAsync(payment);

            _reservationRepMock
                .Setup(r => r.UpdateAsync(
                    It.IsAny<TrainingReservation>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service
                .ConfirmPaymentAsync(reservationId, dto);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                (int)ReservationStatusEnum.Paid,
                reservation.ReservationStatusId);

            Assert.Equal(
                payment.Id,
                reservation.PaymentId);

            _transactionMock.Verify(
                t => t.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmPaymentAsync_ShouldRollback_WhenPaymentFails()
        {
            // Arrange
            var dto = new CreatePaymentDTO();

            var reservation = new TrainingReservation
            {
                ReservationStatusId =
                    (int)ReservationStatusEnum.Visited
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(1))
                .ReturnsAsync(reservation);

            _paymentServiceMock
                .Setup(s => s.AddPaymentAsync(
                    It.IsAny<CreatePaymentDTO>()))
                .ThrowsAsync(new Exception());

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.ConfirmPaymentAsync(1, dto));

            _transactionMock.Verify(
                t => t.RollbackAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmPaymentAsync_ShouldThrow_WhenReservationNotFound()
        {
            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(1))
                .ReturnsAsync((TrainingReservation?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmPaymentAsync(
                    1,
                    new CreatePaymentDTO()));
        }

        [Fact]
        public async Task ConfirmPaymentAsync_ShouldThrow_WhenReservationPending()
        {
            var reservation = new TrainingReservation
            {
                ReservationStatusId =
                    (int)ReservationStatusEnum.Pending
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(1))
                .ReturnsAsync(reservation);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmPaymentAsync(
                    1,
                    new CreatePaymentDTO()));
        }

        [Fact]
        public async Task ConfirmPaymentAsync_ShouldThrow_WhenAlreadyPaid()
        {
            var reservation = new TrainingReservation
            {
                ReservationStatusId =
                    (int)ReservationStatusEnum.Paid
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(1))
                .ReturnsAsync(reservation);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmPaymentAsync(
                    1,
                    new CreatePaymentDTO()));
        }

        [Fact]
        public async Task ConfirmPaymentAsync_ShouldThrow_WhenReservationCancelled()
        {
            var reservation = new TrainingReservation
            {
                ReservationStatusId =
                    (int)ReservationStatusEnum.Cancelled
            };

            _reservationRepMock
                .Setup(r => r.GetReservationByIdAsync(1))
                .ReturnsAsync(reservation);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ConfirmPaymentAsync(
                    1,
                    new CreatePaymentDTO()));
        }

        #endregion
    }
}
