using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BillTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillsController : ControllerBase
    {
        private readonly ILogger<BillsController> _logger;
        private readonly BillService _billsService;

        public BillsController(ILogger<BillsController> logger, BillService billsService)
        {
            _logger = logger;
            _billsService = billsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> Get()
        {
            var bills = await _billsService.Get();
            return bills.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetById(int id)
        {
            return await _billsService.GetById(id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Bill>> Put(int id, Bill bill)
        {
            return await _billsService.Update(id, bill);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Bill>> Post(Bill bill)
        {
            var value = await _billsService.Create(bill);
           return CreatedAtAction(nameof(Post), value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _billsService.Delete(id);
            return new NoContentResult();
        }
    }
}
