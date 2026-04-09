using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class TrainingDTO
    {
        public TrainingDTO()
        {
            TrainingReservations = new HashSet<TrainingReservationDTO>();
        }

        public TrainingDTO(Training t)
        {
            Id = t.Id;
            StartDate = t.StartDate;
            EndDate = t.EndDate;
            Price = t.Price;
            CashbackPercentage = t.CashbackPercentage;
            CoachId = t.CoachId;
            TrainingTypeId = t.TrainingTypeId;
            TrainingStatusId = t.TrainingStatusId;

            Coach = t.Coach == null ? null : new CoachDTO(t.Coach);
            TrainingType = t.TrainingType == null ? null : new TrainingTypeDTO(t.TrainingType);
            TrainingStatus = t.TrainingStatus == null ? null : new TrainingStatusDTO(t.TrainingStatus);

            TrainingReservations = t.TrainingReservations
                .Select(x => new TrainingReservationDTO(x))
                .ToList();
        }

        public TrainingDTO(TrainingDTO t)
        {
            Id = t.Id;
            StartDate = t.StartDate;
            EndDate = t.EndDate;
            Price = t.Price;
            CashbackPercentage = t.CashbackPercentage;
            CoachId = t.CoachId;
            TrainingTypeId = t.TrainingTypeId;
            TrainingStatusId = t.TrainingStatusId;
            Coach = t.Coach;
            TrainingType = t.TrainingType;
            TrainingStatus = t.TrainingStatus;
            TrainingReservations = t.TrainingReservations;
        }

        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public int CashbackPercentage { get; set; }

        public int CoachId { get; set; }

        public int TrainingTypeId { get; set; }

        public int TrainingStatusId { get; set; }

        public virtual CoachDTO? Coach { get; set; }

        public virtual TrainingTypeDTO? TrainingType { get; set; }

        public virtual TrainingStatusDTO? TrainingStatus { get; set; }

        public virtual ICollection<TrainingReservationDTO> TrainingReservations { get; set; }
    }
}
