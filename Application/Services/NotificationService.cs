using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationService(INotificationRepository _notificationRep)
    {
        public async Task<IEnumerable<CancellationNotificationDTO>> GetNotificationsAsync()
        {
            var notifications = await _notificationRep.GetNotificationsAsync();
            return notifications.Select(n => new CancellationNotificationDTO(n));
        }

        public async Task<IEnumerable<CancellationNotificationDTO>> GetActiveNotificationsAsync()
        {
            var notifications = await _notificationRep.GetActiveNotificationsAsync();
            return notifications.Select(n => new CancellationNotificationDTO(n));
        }

        public async Task<CancellationNotificationDTO?> GetNotificationByIdAsync(int id)
        {
            var notification = await _notificationRep.GetNotificationByIdAsync(id);
            return notification == null ? null : new CancellationNotificationDTO(notification);
        }

        public async Task<CancellationNotificationDTO> AddNotificationAsync(CreateCancellationNotificationDTO dto)
        {
            var notification = new CancellationNotification
            {
                IsNotified = dto.IsNotified,
                TrainingId = dto.TrainingId,
                ClientId = dto.ClientId
            };

            await _notificationRep.AddAsync(notification);
            notification = await _notificationRep.GetNotificationByIdAsync(notification.Id);

            return new CancellationNotificationDTO(notification);
        }

        public async Task<CancellationNotificationDTO> UpdateNotificationAsync(CancellationNotificationDTO dto)
        {
            var existing = await _notificationRep.GetNotificationByIdAsync(dto.Id) ??
                throw new KeyNotFoundException($"Уведомление с Id {dto.Id} не найдено");

            existing.IsNotified = dto.IsNotified;
            existing.TrainingId = dto.TrainingId;
            existing.ClientId = dto.ClientId;

            await _notificationRep.UpdateAsync(existing);

            return new CancellationNotificationDTO(existing);
        }

        public async Task DeleteNotification(int id)
        {
            var notification = await _notificationRep.GetNotificationByIdAsync(id) ??
                throw new KeyNotFoundException($"Уведомление с Id {id} не найдено");

            await _notificationRep.DeleteAsync(notification);
        }
    }
}
