using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class MembershipDTO
    {
        public MembershipDTO() { }

        public MembershipDTO(Membership m)
        {
            Id = m.Id;
            StartDate = m.StartDate;
            EndDate = m.EndDate;
            ClientId = m.ClientId;

            MembershipTypeId = m.MembershipTypeId;
            MembershipType = m.MembershipType == null
                ? null
                : new MembershipTypeDTO(m.MembershipType);

            PaymentId = m.PaymentId;
            Payment = m.Payment == null
                ? null
                : new PaymentDTO(m.Payment);
        }

        public MembershipDTO(MembershipDTO m)
        {
            Id = m.Id;
            StartDate = m.StartDate;
            EndDate = m.EndDate;
            ClientId = m.ClientId;
            MembershipTypeId = m.MembershipTypeId;
            MembershipType = m.MembershipType;
            PaymentId = m.PaymentId;
            Payment = m.Payment;
        }

        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ClientId { get; set; }

        public int MembershipTypeId { get; set; }

        public virtual MembershipTypeDTO? MembershipType { get; set; }

        public int PaymentId { get; set; }

        public virtual PaymentDTO? Payment { get; set; }
    }
}
