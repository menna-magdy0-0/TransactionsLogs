using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;
using TransactionLogs.Domain.Entities;
using TransactionLogs.Domain.Interfaces;

namespace TransactionLogs.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
              
            var transactions = await _transactionRepository.GetAllTransactionsAsync();

            
            if (transactions == null || !transactions.Any())
            {
                return NotFound("No transactions found.");
            }

            return Ok(transactions);
        }

        [HttpPost("test-rabbit")]
        public async Task<IActionResult> TestRabbitMQ([FromServices] IBus bus)
        {
            var testLog = new List<Transaction>
            {
                new()
                {
                    TableName = "Test",
                    OperationType = "Create",
                    PrimaryKeyValue = "1",
                    EntityData = "{}",
                    TimeStamp = DateTime.UtcNow
                }
            };

            await bus.Send(testLog);
            return Ok("Test message sent to RabbitMQ");
        }
    }
}
