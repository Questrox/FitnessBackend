using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.CreateDTOs
{
    public class CreateTrainingDTO
    {
        public int MaxClients { get; set; }

        public DateTime Date { get; set; }

        public int CoachId { get; set; }

        public int TrainingTypeId { get; set; }
    }
}
