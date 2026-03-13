using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Membership
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientId { get; set; }
        public virtual Client? Client { get; set; }
        public int MembershipTypeId { get; set; }
        public virtual MembershipType? MembershipType { get; set; }
        public int PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }
    }
}
