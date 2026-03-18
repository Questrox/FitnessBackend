using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(FitnessDb db) : base(db) { }
        public async Task<IEnumerable<Payment>> GetClientPayments(int clientId)
        {
            return await _dbSet.Where(p =>
            (p.Membership != null && p.Membership.ClientId == clientId) ||
            (p.TrainingReservation != null && p.TrainingReservation.ClientId == clientId)).ToListAsync();
        }
        public async Task<Payment?> GetPaymentById(int id)
            => await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
    }
}
