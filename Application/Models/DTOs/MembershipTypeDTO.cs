using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class MembershipTypeDTO
    {
        public MembershipTypeDTO() { }

        public MembershipTypeDTO(MembershipType m)
        {
            Id = m.Id;
            Name = m.Name;
            Description = m.Description;
            Price = m.Price;
            CashbackPercentage = m.CashbackPercentage;
            Duration = m.Duration;
        }

        public MembershipTypeDTO(MembershipTypeDTO m)
        {
            Id = m.Id;
            Name = m.Name;
            Description = m.Description;
            Price = m.Price;
            CashbackPercentage = m.CashbackPercentage;
            Duration = m.Duration;
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int CashbackPercentage { get; set; }

        public int Duration { get; set; }
    }
}
