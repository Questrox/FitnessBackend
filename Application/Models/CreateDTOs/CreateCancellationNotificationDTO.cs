using Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.CreateDTOs
{
    public class CreateCancellationNotificationDTO
    {
        public bool IsNotified { get; set; }

        public int TrainingId { get; set; }

        public int ClientId { get; set; }
        public string? AdminId { get; set; }
    }
}
