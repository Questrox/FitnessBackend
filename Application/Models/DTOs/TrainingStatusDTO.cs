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
        public TrainingStatusDTO() { }

        public TrainingStatusDTO(TrainingStatus t)
        {
            Id = t.Id;
            Name = t.Name;
        }

        public TrainingStatusDTO(TrainingStatusDTO t)
        {
            Id = t.Id;
            Name = t.Name;
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
