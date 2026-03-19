using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TrainingType : ISoftDeletable
    {
        public TrainingType()
        {
            Trainings = new HashSet<Training>();
        }
        [Key]
        public int Id { get; set; }
        public int MaxClients { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CashbackPercentage { get; set; }
        public virtual ICollection<Training> Trainings { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
