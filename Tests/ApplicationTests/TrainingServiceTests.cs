using Application.Models.CreateDTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace Tests.ApplicationTests
{
    public class TrainingServiceTests
    {
        #region Инициализация

        private readonly Mock<ITrainingRepository> _trainingRepMock = new();
        private readonly Mock<ITrainingTypeRepository> _typeRepMock = new();
        private readonly Mock<ICoachRepository> _coachRepMock = new();
        private readonly Mock<IClientRepository> _clientRepMock = new();
        private readonly Mock<ITrainingReservationRepository> _reservationRepMock = new();
        private readonly Mock<INotificationRepository> _notificationRepMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();

        private readonly Mock<ITransaction> _transactionMock;

        private readonly TrainingService _service;

        public TrainingServiceTests()
        {
            _transactionMock = new Mock<ITransaction>();

            _uowMock
                .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);

            _service = new TrainingService(
                _trainingRepMock.Object,
                _typeRepMock.Object,
                _coachRepMock.Object,
                _clientRepMock.Object,
                _reservationRepMock.Object,
                _notificationRepMock.Object,
                _uowMock.Object);
        }

        #endregion

        #region CancelTrainingAsync

        [Fact]
        public async Task CancelTrainingAsync_ShouldCancelTraining_WhenDataIsValid()
        {
            // Arrange
            int id = 1;

            var training = new Training
            {
                Id = id,
                CoachId = 5,
                StartDate = DateTime.Now.AddHours(2),
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
                TrainingType = new TrainingType
                {
                    MaxClients = 10
                },
                TrainingReservations =
                [
                    new TrainingReservation
                    {
                        ClientId = 1,
                        ReservationStatusId = (int)ReservationStatusEnum.Pending
                    }
                ]
            };

            _trainingRepMock
                .Setup(r => r.GetTrainingByIdAsync(id))
                .ReturnsAsync(training);

            _trainingRepMock
                .Setup(r => r.UpdateAsync(It.IsAny<Training>()))
                .Returns(Task.CompletedTask);

            _uowMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _notificationRepMock
                .Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<CancellationNotification>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CancelTrainingAsync(id, "", false);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                (int)TrainingStatusEnum.Cancelled,
                training.TrainingStatusId);

            Assert.Equal(
                (int)ReservationStatusEnum.TrainingCancelled,
                training.TrainingReservations.First().ReservationStatusId);

            _trainingRepMock.Verify(
                r => r.UpdateAsync(It.IsAny<Training>()),
                Times.Once);

            _notificationRepMock.Verify(
                r => r.AddRangeAsync(It.IsAny<IEnumerable<CancellationNotification>>()),
                Times.Once);

            _transactionMock.Verify(
                t => t.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CancelTrainingAsync_ShouldThrow_WhenTrainingNotFound()
        {
            // Arrange
            _trainingRepMock
                .Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync((Training?)null);

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CancelTrainingAsync(1, "", false));
        }

        [Fact]
        public async Task CancelTrainingAsync_ShouldThrow_WhenTrainingCompleted()
        {
            // Arrange
            var training = new Training
            {
                TrainingStatusId = (int)TrainingStatusEnum.Completed
            };

            _trainingRepMock
                .Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CancelTrainingAsync(1, "", false));
        }

        [Fact]
        public async Task CancelTrainingAsync_ShouldThrow_WhenTrainingAlreadyCancelled()
        {
            // Arrange
            var training = new Training
            {
                TrainingStatusId = (int)TrainingStatusEnum.Cancelled
            };

            _trainingRepMock
                .Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CancelTrainingAsync(1, "", false));
        }

        [Fact]
        public async Task CancelTrainingAsync_ShouldThrow_WhenTrainingAlreadyStarted()
        {
            // Arrange
            var training = new Training
            {
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
                StartDate = DateTime.Now.AddMinutes(-10)
            };

            _trainingRepMock
                .Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CancelTrainingAsync(1, "", false));
        }

        [Fact]
        public async Task CancelTrainingAsync_ShouldThrow_WhenCoachIsNotOwner()
        {
            // Arrange
            string userId = "coach";

            var training = new Training
            {
                CoachId = 10,
                StartDate = DateTime.Now.AddHours(1),
                TrainingStatusId = (int)TrainingStatusEnum.Pending
            };

            var coach = new Coach
            {
                Id = 20
            };

            _trainingRepMock
                .Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            _coachRepMock
                .Setup(r => r.GetCoachByUserIdAsync(userId))
                .ReturnsAsync(coach);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CancelTrainingAsync(1, userId, true));
        }

        [Fact]
        public async Task CancelTrainingAsync_ShouldNotCreateNotifications_ForIndividualTraining()
        {
            // Arrange
            var training = new Training
            {
                Id = 1,
                StartDate = DateTime.Now.AddHours(2),
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
                TrainingType = new TrainingType
                {
                    MaxClients = 1
                },
                TrainingReservations =
                [
                    new TrainingReservation
                    {
                        ClientId = 1,
                        ReservationStatusId = (int)ReservationStatusEnum.Pending
                    }
                ]
            };

            _trainingRepMock
                .Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            _trainingRepMock
                .Setup(r => r.UpdateAsync(It.IsAny<Training>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.CancelTrainingAsync(1, "", false);

            // Assert
            _notificationRepMock.Verify(
                r => r.AddRangeAsync(It.IsAny<IEnumerable<CancellationNotification>>()),
                Times.Never);
        }

        [Fact]
        public async Task CancelTrainingAsync_ShouldRollback_WhenExceptionOccurs()
        {
            // Arrange
            var training = new Training
            {
                StartDate = DateTime.Now.AddHours(2),
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
                TrainingType = new TrainingType
                {
                    MaxClients = 10
                }
            };

            _trainingRepMock
                .Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            _trainingRepMock
                .Setup(r => r.UpdateAsync(It.IsAny<Training>()))
                .ThrowsAsync(new Exception());

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.CancelTrainingAsync(1, "", false));

            _transactionMock.Verify(
                t => t.RollbackAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region CheckReservationPossibilityAsync

        [Fact]
        public async Task CheckReservationPossibilityAsync_ShouldThrow_WhenClientIdIsNull()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CheckReservationPossibilityAsync(1, null, true));
        }

        [Fact]
        public async Task CheckReservationPossibilityAsync_ShouldReturn_WhenTrainingCompleted()
        {
            var training = new Training
            {
                TrainingStatusId = (int)TrainingStatusEnum.Completed
            };

            _trainingRepMock.Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            var result = await _service.CheckReservationPossibilityAsync(1, 1, true);

            Assert.Equal("Тренировка уже была проведена", result);
        }

        [Fact]
        public async Task CheckReservationPossibilityAsync_ShouldReturn_WhenAlreadyReserved()
        {
            var training = new Training
            {
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
                StartDate = DateTime.Now.AddHours(2),
                TrainingReservations =
                [
                    new TrainingReservation
            {
                ClientId = 1,
                ReservationStatusId = (int)ReservationStatusEnum.Pending
            }
                ]
            };

            _trainingRepMock.Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            var result =
                await _service.CheckReservationPossibilityAsync(1, 1, true);

            Assert.Equal("Вы уже записаны на эту тренировку", result);
        }

        [Fact]
        public async Task CheckReservationPossibilityAsync_ShouldReturn_WhenNoPlaces()
        {
            var training = new Training
            {
                TrainingTypeId = 1,
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
                StartDate = DateTime.Now.AddHours(1),
                TrainingReservations =
                [
                    new TrainingReservation
            {
                ReservationStatusId =
                    (int)ReservationStatusEnum.Pending
            }
                ]
            };

            _trainingRepMock.Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            _typeRepMock.Setup(r => r.GetTrainingTypeByIdAsync(1))
                .ReturnsAsync(new TrainingType
                {
                    MaxClients = 1
                });

            var result =
                await _service.CheckReservationPossibilityAsync(1, 2, true);

            Assert.Equal("Нет мест", result);
        }

        [Fact]
        public async Task CheckReservationPossibilityAsync_ShouldReturn_WhenOverlapExists()
        {
            var training = new Training
            {
                TrainingTypeId = 1,
                StartDate = DateTime.Now.AddHours(2),
                EndDate = DateTime.Now.AddHours(3),
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
                TrainingReservations = []
            };

            var overlap = new TrainingReservation
            {
                ReservationStatusId =
                    (int)ReservationStatusEnum.Pending,
                Training = new Training
                {
                    TrainingStatusId =
                        (int)TrainingStatusEnum.Pending,
                    StartDate = DateTime.Now.AddHours(2.5),
                    EndDate = DateTime.Now.AddHours(3.5),
                    TrainingType = new TrainingType
                    {
                        Name = "Йога"
                    }
                }
            };

            var client = new Client
            {
                TrainingReservations = [overlap],
                Memberships = []
            };

            _trainingRepMock.Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            _typeRepMock.Setup(r => r.GetTrainingTypeByIdAsync(1))
                .ReturnsAsync(new TrainingType
                {
                    MaxClients = 10
                });

            _clientRepMock.Setup(r => r.GetClientByIdAsync(1))
                .ReturnsAsync(client);

            var result =
                await _service.CheckReservationPossibilityAsync(1, 1, true);

            Assert.Contains("Вы уже записаны на тренировку", result);
        }

        [Fact]
        public async Task CheckReservationPossibilityAsync_ShouldReturn_WhenNoMembership()
        {
            var training = new Training
            {
                TrainingTypeId = 1,
                StartDate = DateTime.Now.AddHours(2),
                EndDate = DateTime.Now.AddHours(3),
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
                TrainingReservations = []
            };

            _trainingRepMock.Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            _typeRepMock.Setup(r => r.GetTrainingTypeByIdAsync(1))
                .ReturnsAsync(new TrainingType
                {
                    MaxClients = 10
                });

            _clientRepMock.Setup(r => r.GetClientByIdAsync(1))
                .ReturnsAsync(new Client
                {
                    Memberships = [],
                    TrainingReservations = []
                });

            var result =
                await _service.CheckReservationPossibilityAsync(1, 1, true);

            Assert.Equal(
                "У вас нет абонемента на этот период",
                result);
        }

        [Fact]
        public async Task CheckReservationPossibilityAsync_ShouldReturnEmpty_WhenReservationAllowed()
        {
            var training = new Training
            {
                TrainingTypeId = 1,
                StartDate = DateTime.Now.AddHours(2),
                EndDate = DateTime.Now.AddHours(3),
                TrainingStatusId = (int)TrainingStatusEnum.Pending,
                TrainingReservations = []
            };

            _trainingRepMock.Setup(r => r.GetTrainingByIdAsync(1))
                .ReturnsAsync(training);

            _typeRepMock.Setup(r => r.GetTrainingTypeByIdAsync(1))
                .ReturnsAsync(new TrainingType
                {
                    MaxClients = 10
                });

            _clientRepMock.Setup(r => r.GetClientByIdAsync(1))
                .ReturnsAsync(new Client
                {
                    TrainingReservations = [],
                    Memberships =
                    [
                        new Membership
                {
                    StartDate = DateTime.Now.Date.AddDays(-1),
                    EndDate = DateTime.Now.Date.AddDays(10)
                }
                    ]
                });

            var result =
                await _service.CheckReservationPossibilityAsync(1, 1, true);

            Assert.Equal(string.Empty, result);
        }

        #endregion

        #region CheckIndividualTrainingCreationPossibilityAsync

        [Fact]
        public async Task CheckIndividualTrainingCreationPossibilityAsync_ShouldReturn_WhenTrainingTypeNotFound()
        {
            var dto = new CreateIndividualTrainingDTO
            {
                TrainingTypeId = 1
            };

            _typeRepMock.Setup(r => r.GetTrainingTypeByIdAsync(1))
                .ReturnsAsync((TrainingType?)null);

            var result =
                await _service.CheckIndividualTrainingCreationPossibilityAsync(
                    dto,
                    "coach");

            Assert.Contains("Не найден тип тренировки", result);
        }

        [Fact]
        public async Task CheckIndividualTrainingCreationPossibilityAsync_ShouldReturn_WhenNotIndividual()
        {
            _typeRepMock.Setup(r => r.GetTrainingTypeByIdAsync(1))
                .ReturnsAsync(new TrainingType
                {
                    MaxClients = 10
                });

            var result =
                await _service.CheckIndividualTrainingCreationPossibilityAsync(
                    new CreateIndividualTrainingDTO
                    {
                        TrainingTypeId = 1
                    },
                    "coach");

            Assert.Equal(
                "Тренировка не является индивидуальной",
                result);
        }

        [Fact]
        public async Task CheckIndividualTrainingCreationPossibilityAsync_ShouldReturn_WhenCoachBusy()
        {
            var start = DateTime.Now.AddHours(2);

            var coach = new Coach
            {
                CoachSchedules =
                [
                    new CoachSchedule
            {
                WeekDay = start.DayOfWeek,
                StartTime = TimeSpan.Zero,
                EndTime = new TimeSpan(23,59,59)
            }
                ],
                Trainings =
                [
                    new Training
            {
                StartDate = start.AddMinutes(-10),
                EndDate = start.AddMinutes(30),
                TrainingStatusId =
                    (int)TrainingStatusEnum.Pending,
                TrainingType = new TrainingType
                {
                    Name = "Силовая"
                }
            }
                ]
            };

            var client = new Client
            {
                Memberships =
                [
                    new Membership
            {
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(10)
            }
                ]
            };

            _typeRepMock.Setup(r => r.GetTrainingTypeByIdAsync(1))
                .ReturnsAsync(new TrainingType
                {
                    MaxClients = 1,
                    Duration = 60
                });

            _coachRepMock.Setup(r => r.GetCoachByUserIdAsync("coach"))
                .ReturnsAsync(coach);

            _clientRepMock.Setup(r => r.GetClientByIdAsync(1))
                .ReturnsAsync(client);

            var result =
                await _service.CheckIndividualTrainingCreationPossibilityAsync(
                    new CreateIndividualTrainingDTO
                    {
                        TrainingTypeId = 1,
                        ClientId = 1,
                        StartDate = start
                    },
                    "coach");

            Assert.Contains(
                "У вас уже есть тренировка в это время",
                result);
        }

        [Fact]
        public async Task CheckIndividualTrainingCreationPossibilityAsync_ShouldReturn_WhenClientHasNoMembership()
        {
            var start = DateTime.Now.AddHours(2);

            _typeRepMock.Setup(r => r.GetTrainingTypeByIdAsync(1))
                .ReturnsAsync(new TrainingType
                {
                    MaxClients = 1,
                    Duration = 60
                });

            _coachRepMock.Setup(r => r.GetCoachByUserIdAsync("coach"))
                .ReturnsAsync(new Coach
                {
                    CoachSchedules =
                    [
                        new CoachSchedule
                {
                    WeekDay = start.DayOfWeek,
                    StartTime = TimeSpan.Zero,
                    EndTime = new TimeSpan(23,59,59)
                }
                    ]
                });

            _clientRepMock.Setup(r => r.GetClientByIdAsync(1))
                .ReturnsAsync(new Client
                {
                    Memberships = [],
                    TrainingReservations = []
                });

            var result =
                await _service.CheckIndividualTrainingCreationPossibilityAsync(
                    new CreateIndividualTrainingDTO
                    {
                        ClientId = 1,
                        TrainingTypeId = 1,
                        StartDate = start
                    },
                    "coach");

            Assert.Equal(
                "У клиента нет абонемента на этот период",
                result);
        }

        [Fact]
        public async Task CheckIndividualTrainingCreationPossibilityAsync_ShouldReturnEmpty_WhenCreationAllowed()
        {
            var start = DateTime.Now.AddHours(2);

            _typeRepMock.Setup(r => r.GetTrainingTypeByIdAsync(1))
                .ReturnsAsync(new TrainingType
                {
                    MaxClients = 1,
                    Duration = 60
                });

            _coachRepMock.Setup(r => r.GetCoachByUserIdAsync("coach"))
                .ReturnsAsync(new Coach
                {
                    CoachSchedules =
                    [
                        new CoachSchedule
                {
                    WeekDay = start.DayOfWeek,
                    StartTime = TimeSpan.Zero,
                    EndTime = new TimeSpan(23,59,59)
                }
                    ],
                    Trainings = []
                });

            _clientRepMock.Setup(r => r.GetClientByIdAsync(1))
                .ReturnsAsync(new Client
                {
                    TrainingReservations = [],
                    Memberships =
                    [
                        new Membership
                {
                    StartDate = DateTime.Now.Date,
                    EndDate = DateTime.Now.Date.AddDays(10)
                }
                    ]
                });

            var result =
                await _service.CheckIndividualTrainingCreationPossibilityAsync(
                    new CreateIndividualTrainingDTO
                    {
                        ClientId = 1,
                        TrainingTypeId = 1,
                        StartDate = start
                    },
                    "coach");

            Assert.Equal(string.Empty, result);
        }

        #endregion
    }
}