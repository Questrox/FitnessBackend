using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class TrainingTypeDTO
    {
        public TrainingTypeDTO()
        {
            Trainings = new HashSet<TrainingDTO>();
        }

        public TrainingTypeDTO(TrainingType t)
        {
            Id = t.Id;
            Price = t.Price;
            Name = t.Name;
            Description = t.Description;
            CashbackPercentage = t.CashbackPercentage;

            Trainings = t.Trainings
                .Select(x => new TrainingDTO(x))
                .ToList();
        }

        public TrainingTypeDTO(TrainingTypeDTO t)
        {
            Id = t.Id;
            Price = t.Price;
            Name = t.Name;
            Description = t.Description;
            CashbackPercentage = t.CashbackPercentage;
            Trainings = t.Trainings;
        }

        public int Id { get; set; }

        public decimal Price { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int CashbackPercentage { get; set; }

        public virtual ICollection<TrainingDTO> Trainings { get; set; }
    }
}
