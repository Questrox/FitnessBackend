using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.CreateDTOs
{
    public class CreateTrainingDTO
    {
        public DateTime StartDate { get; set; }

        public int CoachId { get; set; }

        public int TrainingTypeId { get; set; }
    }
}
