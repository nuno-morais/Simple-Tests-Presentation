using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Repositories;
using BillTracker.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests.Integration.Repositories
{
    public class BillsRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly BillsContext _context;
        private readonly BillsRepository _subject;

        public BillsRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;

            var options = fixture.GetDbContextOptions<BillsContext>();
            _context = new BillsContext(options);
            _subject = new BillsRepository(_context);
        }

        [Fact]
        public async Task Create_Bill_ReturnsNewBill()
        {
            var result = await _subject.Create(new Bill() { Description = "Dummy", Price = 10 });

            _context.GetDependencies().StateManager.ResetState();
            await _fixture.ResetDatabaseAsync();

            Assert.Equal(
                new Bill() { Id = 1, Description = "Dummy", Price = 10},
                result
            );
        }

        [Theory]
        [MemberData(nameof(GetBills))]
        public async Task Get_Bills_ReturnsAllBills(List<Bill> bills)
        {            
            _context.Bills.AddRange(bills);
            await _context.SaveChangesAsync();
            
            var result = await _subject.GetAll();

            _context.GetDependencies().StateManager.ResetState();
            await _fixture.ResetDatabaseAsync();

            Assert.Equal(bills.Count, result.Count());
        }

        public static IEnumerable<object[]> GetBills()
        {
            yield return new object[] { GenerateBills(0) };
            yield return new object[] { GenerateBills(1) };
            yield return new object[] { GenerateBills(10) };
        }

        private static List<Bill> GenerateBills(int number)
        {
            var i = 0;
            var result = new List<Bill>();
            while (i < number)
            {
                result.Add(GenerateBill(i));
                i++;
            }
            return result;
        }

        private static Bill GenerateBill(int index)
        {
            return new Bill()
            {
                Description = index.ToString(),
                Price = index * 10
            };
        }

        [Theory]
        [MemberData(nameof(GetByIdBills))]
        public async Task GetById_ReturnsTheBill(List<Bill> bills, int id, Bill expectedResult)
        {
            _context.Bills.AddRange(bills);
            await _context.SaveChangesAsync();

            var result = await _subject.GetById(id);

            _context.GetDependencies().StateManager.ResetState();
            await _fixture.ResetDatabaseAsync();

            Assert.Equal(expectedResult, result);
        }

        public static IEnumerable<object[]> GetByIdBills()
        {
            yield return new object[] { GenerateBills(0), 2, null };
            yield return new object[] { GenerateBills(1), 2, null };
            yield return new object[] { GenerateBills(10), 2, new Bill() { Id = 2, Description = "1", Price = 10 } };
        }

        [Theory]
        [MemberData(nameof(UpdateBills))]
        public async Task Update_ReturnsTheBill(List<Bill> bills, Bill bill, Bill expectedResult)
        {
            _context.Bills.AddRange(bills);
            await _context.SaveChangesAsync();

            var result = await _subject.Update(bill);

            _context.GetDependencies().StateManager.ResetState();
            await _fixture.ResetDatabaseAsync();

            Assert.Equal(expectedResult, result);
        }

        public static IEnumerable<object[]> UpdateBills()
        {
            var bill = new Bill() { Id = 2, Description = "Dummy", Price = 1000 };
            yield return new object[] { GenerateBills(5), bill, bill };
        }

        [Fact]
        public async Task Delete_ShouldDeleteTheEntity()
        {
            _context.Bills.AddRange(GenerateBills(10));
            await _context.SaveChangesAsync();

            var bill = await _context.Bills.FindAsync(5);

            await _subject.Delete(bill);

            var elements = await _context.Bills.AsQueryable().ToListAsync();

            _context.GetDependencies().StateManager.ResetState();
            await _fixture.ResetDatabaseAsync();

            Assert.Equal(9, elements.Count);
        }
    }
}
