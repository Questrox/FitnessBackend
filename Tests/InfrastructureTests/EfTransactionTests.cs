using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Tests.InfrastructureTests;

public class EfTransactionTests
{
    [Fact]
    public async Task CommitAsync_ShouldCallDbTransactionCommit()
    {
        // Arrange
        var dbTransactionMock =
            new Mock<IDbContextTransaction>();

        var transaction =
            new EfTransaction(dbTransactionMock.Object);

        // Act
        await transaction.CommitAsync();

        // Assert
        dbTransactionMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RollbackAsync_ShouldCallDbTransactionRollback()
    {
        // Arrange
        var dbTransactionMock =
            new Mock<IDbContextTransaction>();

        var transaction =
            new EfTransaction(dbTransactionMock.Object);

        // Act
        await transaction.RollbackAsync();

        // Assert
        dbTransactionMock.Verify(
            x => x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DisposeAsync_ShouldCallDispose()
    {
        // Arrange
        var dbTransactionMock =
            new Mock<IDbContextTransaction>();

        var transaction =
            new EfTransaction(dbTransactionMock.Object);

        // Act
        await transaction.DisposeAsync();

        // Assert
        dbTransactionMock.Verify(
            x => x.DisposeAsync(),
            Times.Once);
    }
}
