using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackgroundServices
{
    public class NotificationCleanerService(IServiceScopeFactory _scopeFactory, 
        ILogger<NotificationCleanerService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<FitnessDb>();

                    var notifications = await db.CancellationNotifications
                        .Where(n => !n.IsNotified && n.Training.EndDate <= DateTime.Now)
                        .ToListAsync(cancellationToken);
                    foreach (var notification in notifications)
                    {
                        notification.IsNotified = true;
                    }
                    if (notifications.Count > 0)
                    {
                        await db.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation($"Было обновлено уведомлений: {notifications.Count}");
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
