using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BillTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BillTracker.Repositories
{
    public interface IBillsRepository
    {
        public Task<IEnumerable<Bill>> GetAll();

        public Task<Bill> GetById(int id);

        public Task<Bill> Update(Bill bill);

        public Task<Bill> Create(Bill bill);

        public Task Delete(Bill bill);
    }

    public class BillsRepository : IBillsRepository
    {
        private readonly BillsContext _context;

        public BillsRepository(BillsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bill>> GetAll()
        {
            return await _context.Bills.AsQueryable().ToListAsync();
         }

        public async Task<Bill> GetById(int id)
        {
            return await _context.Bills.FindAsync(id);
        }

        public async Task<Bill> Update(Bill bill)
        {
            _context.GetDependencies().StateManager.ResetState();
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();

            return bill;
        }

        public async Task<Bill> Create(Bill bill)
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            return bill;
        }

        public async Task Delete(Bill bill)
        {
            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();
        }
    }
}
