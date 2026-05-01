using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITrainingTypeRepository : IRepository<TrainingType>
    {
        Task<IEnumerable<TrainingType>> GetTrainingTypesAsync();
        Task<IEnumerable<TrainingType>> GetGroupTrainingTypesAsync();
        Task<IEnumerable<TrainingType>> GetIndividualTrainingTypesAsync();
        Task<TrainingType?> GetTrainingTypeByIdAsync(int id);
    }
}
