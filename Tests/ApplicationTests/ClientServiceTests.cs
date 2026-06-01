using Application.Models.CreateDTOs;
using Application.Models;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Tests.ApplicationTests
{
    public class ClientServiceTests
    {
        private readonly Mock<IClientRepository> _clientRepMock = new();
        private readonly Mock<AuthService> _authServiceMock;

        private readonly ClientService _service;
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<ITransaction> _transactionMock = new();

        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;

        public ClientServiceTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(),
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<User>>>()
            );

            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<User>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<User>>()
            );

            _authServiceMock = new Mock<AuthService>(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                Mock.Of<IConfiguration>(),
                Mock.Of<ILogger<AuthService>>()
            );

            _uowMock
                .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);

            _service = new ClientService(
                _uowMock.Object,
                _authServiceMock.Object,
                _clientRepMock.Object
            );
        }

        #region AddClientAsync_ShouldCreateClient_WhenDataIsValid

        [Fact]
        public async Task AddClientAsync_ShouldCreateClient_WhenDataIsValid()
        {
            // Arrange
            var dto = new CreateClientDTO
            {
                FullName = "Иван Иванов",
                PhoneNumber = "+79991234567"
            };

            _authServiceMock.Setup(x => x.GenerateUsernameAsync(6))
                .ReturnsAsync("ivan123");

            _authServiceMock.Setup(x => x.GeneratePassword(6))
                .Returns("pass");

            _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<RegisterModel>(), "User"))
                .ReturnsAsync("user-id");

            _clientRepMock.Setup(x => x.AddAsync(It.IsAny<Client>()))
                .Returns(Task.CompletedTask)
                .Callback<Client>(c => c.Id = 1);

            // Act
            var result = await _service.AddClientAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ClientId);

            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region AddClientAsync_ShouldThrow_WhenFullNameInvalid

        [Fact]
        public async Task AddClientAsync_ShouldThrow_WhenFullNameInvalid()
        {
            var dto = new CreateClientDTO
            {
                FullName = "Ivan123",
                PhoneNumber = "+79991234567"
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddClientAsync(dto));
        }

        #endregion

        #region AddClientAsync_ShouldThrow_WhenPhoneInvalid

        [Fact]
        public async Task AddClientAsync_ShouldThrow_WhenPhoneInvalid()
        {
            var dto = new CreateClientDTO
            {
                FullName = "Иван Иванов",
                PhoneNumber = "12345"
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddClientAsync(dto));
        }

        #endregion
    }
}