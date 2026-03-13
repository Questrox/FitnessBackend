using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Old
{
    public class FoodEntry
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public int FoodId { get; set; }
        public int MealTypeId { get; set; }
        public string UserId { get; set; }
        public virtual Food Food { get; set; }
        public virtual MealType MealType { get; set; }
        public virtual User User { get; set; }
    }
}
