using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CancellationNotification : ISoftDeletable
    {
        [Key]
        public int Id { get; set; }
        public bool IsNotified { get; set; }
        public int TrainingId { get; set; }
        public virtual Training? Training { get; set; }
        public int ClientId { get; set; }
        public virtual Client? Client { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
