using System.Collections.Generic;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Exceptions;
using BillTracker.Repositories;

namespace BillTracker.Services
{
    public class BillService
    {
        private readonly IBillsRepository _repository;

        public BillService(IBillsRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Bill>> Get()
        {
            return await _repository.GetAll();

        }

        public async Task<Bill> GetById(int id)
        {
            var bill = await _repository.GetById(id);
            if (bill == null)
            {
                throw new EntityNotFoundException();
            }
            return bill;
        }

        public async Task<Bill> Update(int id, Bill bill)
        {
            ValidateBill(bill);
            await GetById(id);
            return await _repository.Update(bill);
        }

        public async Task<Bill> Create(Bill bill)
        {
            ValidateBill(bill);
            return await _repository.Create(bill);
        }

        private void ValidateBill(Bill bill)
        {
            if (bill.Description == null || bill.Description == string.Empty)
            {
                throw new BadRequestException("Description");
            } 
        }

        public async Task Delete(int id)
        {
            var bill = await GetById(id);
            await _repository.Delete(bill);
        }
    }
}
