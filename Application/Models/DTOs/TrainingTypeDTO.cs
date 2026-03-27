using Domain.Entities;
using Microsoft.AspNetCore.Http;
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
            MaxClients = t.MaxClients;
            Price = t.Price;
            Name = t.Name;
            Description = t.Description;
            CashbackPercentage = t.CashbackPercentage;

            Trainings = t.Trainings
                .Select(x => new TrainingDTO(x))
                .ToList();
            PhotoPath = t.PhotoPath;
            Duration = t.Duration;
        }

        public TrainingTypeDTO(TrainingTypeDTO t)
        {
            Id = t.Id;
            MaxClients = t.MaxClients;
            Price = t.Price;
            Name = t.Name;
            Description = t.Description;
            CashbackPercentage = t.CashbackPercentage;
            Trainings = t.Trainings;
            PhotoPath = t.PhotoPath;
            Duration = t.Duration;
            Image = t.Image;
        }

        public int Id { get; set; }

        public int MaxClients { get; set; }

        public decimal Price { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string PhotoPath { get; set; } = null!;
        public IFormFile? Image { get; set; }
        public int Duration { get; set; }

        public int CashbackPercentage { get; set; }

        public virtual ICollection<TrainingDTO> Trainings { get; set; }
    }
}
