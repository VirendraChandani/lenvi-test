using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionManager.Services.Transactions;
using Transaction = TransactionManager.Models.Transaction;

namespace TransactionManager.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    [Authorize] // Protect the entire controller with JWT authentication
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService) 
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        // GET api/transactions
        [HttpGet]
        public IActionResult Get()
        {
            var transactions = _transactionService.GetAllTransactions();
            return Ok(transactions);
        }

        // GET api/transactions/{id}
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var transaction = _transactionService.GetTransactionById(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        //GET api/transactions/GetByApplicationId/{applicationId
        [HttpGet("api/transactions/GetByApplicationId/{applicationId}")]
        public IActionResult GetByApplicationId(int applicationId)
        {
            var transactions = _transactionService.GetTransactionByApplicationId(applicationId);
            if (transactions == null)
            {
                return NotFound();
            }

            return Ok(transactions);
        }

        // GET api/transactions/GetByType/{type}
        [HttpGet("api/transactions/GetByType/{type}")]
        public IActionResult GetByType(string type)
        {
            var transactions = _transactionService.GetTransactionByType(type);
            if (transactions == null)
            {
                return NotFound();
            }

            return Ok(transactions);
        }

        //GET api/transactions/GetByPostingDate/{postingDate}
        [HttpGet("api/transactions/GetByPostingDate/{postingDate}")]
        public IActionResult GetByPostingDate(DateTime postingDate)
        {
            var transactions = _transactionService.GetTransactionByPostingDate(postingDate);
            if (transactions == null)
            {
                return NotFound();
            }

            return Ok(transactions);
        }

        // POST api/transactions
        [HttpPost]
        public IActionResult Post([FromBody] Transaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Invalid transaction data.");
            }

            var Id = _transactionService.CreateTransaction(transaction);
            return CreatedAtAction(nameof(Get), new { id = Id });
        }

        //PUT api/transactions/UpdateClearedStatusForTransaction
       [HttpPut("api/transactions/UpdateClearedStatusForTransaction")]
        public IActionResult UpdateClearedStatusForTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Invalid transaction data.");
            }

            var updatedTransaction = _transactionService.UpdateClearedStatusForTransaction(transaction);
            return Ok(updatedTransaction);
        }

        // PUT api/transactions
        [HttpPut]
        public IActionResult Put([FromBody] Transaction modifiedTransaction)
        {
            if (modifiedTransaction == null)
            {
                return BadRequest("Invalid transaction data.");
            }

            var updatedTransaction = _transactionService.UpdateTransaction(modifiedTransaction);

            if (updatedTransaction == null)
            {
                return NotFound();
            }

            return Ok(updatedTransaction);
        }

        // DELETE api/transactions/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var isDeleted = _transactionService.DeleteTransaction(id);

            return !isDeleted ? NotFound() : Ok(isDeleted);
        }
    }
}
