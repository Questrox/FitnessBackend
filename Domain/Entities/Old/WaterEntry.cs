using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Old
{
    public class WaterEntry
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
