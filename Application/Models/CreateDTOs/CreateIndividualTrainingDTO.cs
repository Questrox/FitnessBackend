using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.CreateDTOs
{
    public class CreateIndividualTrainingDTO
    {
        public DateTime StartDate { get; set; }

        public int TrainingTypeId { get; set; }

        public int ClientId { get; set; }
    }
}
