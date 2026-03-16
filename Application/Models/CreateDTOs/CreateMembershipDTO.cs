using Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.CreateDTOs
{
    public class CreateMembershipDTO
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ClientId { get; set; }

        public int MembershipTypeId { get; set; }

        public int PaymentId { get; set; }
    }
}
