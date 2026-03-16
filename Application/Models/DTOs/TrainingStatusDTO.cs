using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class TrainingStatusDTO
    {
        public TrainingStatusDTO()
        {
            Trainings = new HashSet<TrainingDTO>();
        }

        public TrainingStatusDTO(TrainingStatus t)
        {
            Id = t.Id;
            Name = t.Name;

            Trainings = t.Trainings
                .Select(x => new TrainingDTO(x))
                .ToList();
        }

        public TrainingStatusDTO(TrainingStatusDTO t)
        {
            Id = t.Id;
            Name = t.Name;
            Trainings = t.Trainings;
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<TrainingDTO> Trainings { get; set; }
    }
}
