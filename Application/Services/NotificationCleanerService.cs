using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class NotificationCleanerService(IServiceScopeFactory _scopeFactory, ILogger<NotificationCleanerService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                    var updatedCount = await notificationService.MarkExpiredNotificationsAsync(cancellationToken);

                    if (updatedCount > 0)
                    {
                        _logger.LogInformation("Было обновлено уведомлений: {Count}", updatedCount);
                    }
                    else
                    {
                        _logger.LogInformation("Не найдены уведомления для обновления");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении уведомлений");
                }
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
        }
    }
}
