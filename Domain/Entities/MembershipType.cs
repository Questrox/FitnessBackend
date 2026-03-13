using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MembershipType
    {
        public MembershipType()
        {
            Memberships = new HashSet<Membership>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int CashbackPercentage { get; set; }
        public int Duration { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
    }
}
