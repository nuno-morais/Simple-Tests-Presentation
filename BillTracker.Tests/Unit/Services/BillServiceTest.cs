using System.Collections.Generic;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Exceptions;
using BillTracker.Repositories;
using BillTracker.Services;
using Moq;
using Xunit;

namespace BillTracker.Tests.Services
{
    public class BillServiceTest
    {
        private readonly BillService _subject;
        private readonly Mock<IBillsRepository> _mockRepository;

        public BillServiceTest()
        {
            _mockRepository = new Mock<IBillsRepository>();
            _subject = new BillService(_mockRepository.Object);
        }

        [Theory]
        [MemberData(nameof(GetAllBills))]
        public async void Get_NoInput_ReturnsRepositoryResult(IList<Bill> mockResult, IList<Bill> expected)
        {
            _mockRepository.Setup(repo => repo.GetAll()).ReturnsAsync(mockResult);
            var result = await _subject.Get();

            Assert.Equal(result, expected);
        }

        public static IEnumerable<object[]> GetAllBills()
        {
            var empty = GenerateBills(0);
            yield return new object[] { empty, empty };
            var oneResult = GenerateBills(1);
            yield return new object[] { oneResult, oneResult };
            var multipleResults = GenerateBills(5);
            yield return new object[] { multipleResults, multipleResults };
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
                Id = index,
                Description = index.ToString(),
                Price = index * 10
            };
        }

        [Theory]
        [MemberData(nameof(CreateBill))]
        public async void Create_ValidInput_ReturnsNewResult(Bill input, Bill outputMock, Bill expected)
        {
            _mockRepository.Setup(repo => repo.Create(input)).ReturnsAsync(outputMock);
            var result = await _subject.Create(input);

            Assert.Equal(result, expected);
        }

        public static IEnumerable<object[]> CreateBill()
        {
            var bill = new Bill() { Description = "1", Price = 10 };
            var mockBill = GenerateBill(1);
            yield return new object[] { bill, mockBill, mockBill };
        }

        [Fact]
        public void Create_InvalidInput_ReturnsBadRequestException()
        {
            Assert.ThrowsAsync<BadRequestException>(async () => await _subject.Create(new Bill() { }));
        }

        [Fact]
        public void GetById_UnexistentId_ReturnsEntityNotFoundException()
        {
            var id = 1;
            Bill mockResult = null;
            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(mockResult);
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _subject.GetById(id));
        }

        [Fact]
        public async void GetById_ExistentId_ReturnsTheEntity()
        {
            var id = 1;
            Bill expected = GenerateBill(1);
            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(expected);
            var result = await _subject.GetById(id);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Upadate_InvalidInput_ReturnsBadRequestException()
        {
            Assert.ThrowsAsync<BadRequestException>(async () => await _subject.Update(1, new Bill() { }));
        }

        [Fact]
        public void Update_UnexistentId_ReturnsEntityNotFoundException()
        {
            var id = 1;
            Bill mockResult = null;
            Bill updateBill = GenerateBill(1);
            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(mockResult);
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _subject.GetById(id));
        }

        [Fact]
        public async void Update_ValidBill_ReturnsTheUpdatedBill()
        {
            var id = 1;
            Bill mockResult = new Bill() { Id = 1, Description = "Dummy", Price = 1 };
            Bill updateBill = GenerateBill(1);
            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(mockResult);
            _mockRepository.Setup(repo => repo.Update(updateBill)).ReturnsAsync(updateBill);
            var result = await _subject.Update(id, updateBill);
            Assert.Equal(updateBill, result);
        }

        [Fact]
        public void Delete_UnexistentId_ReturnsEntityNotFoundException()
        {
            var id = 1;
            Bill mockResult = null;
            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(mockResult);
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _subject.Delete(id));
        }

        [Fact]
        public async void Delete_ExistentId_ReturnsTheEntity()
        {
            var id = 1;
            Bill existentBill = GenerateBill(1);
            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(existentBill);
            _mockRepository.Setup(repo => repo.Delete(existentBill)).Returns(Task.FromResult(0));

            await _subject.Delete(id);
        }
    }
}
