using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<Payment>> GetClientPaymentsAsync(int clientId)
        {
            return await _dbSet.Where(p =>
            (p.Membership != null && p.Membership.ClientId == clientId) ||
            (p.TrainingReservation != null && p.TrainingReservation.ClientId == clientId)).ToListAsync();
        }
        public async Task<Payment?> GetPaymentByIdAsync(int id)
            => await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
    }
}
