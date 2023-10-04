using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TransactionManager.Models;

namespace TransactionManager.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private string jsonFilePath;
        private readonly IConfiguration _configuration;
        public TransactionService(IConfiguration configuration) 
        { 
            _configuration = configuration;
            jsonFilePath = _configuration["JsonDataFilePath"];
        }
        public Guid? CreateTransaction(Transaction transaction)
        {
            var transactions = ReadTransactionsFromJson();
            transaction.Id = Guid.NewGuid();
            transactions?.Add(transaction);
            if (transactions != null)
            {
                WriteTransactionsToJson(transactions);
            }
            return transaction.Id;
        }

        public bool DeleteTransaction(string Id)
        {
            var transactions = ReadTransactionsFromJson();
            var transactionToRemove = transactions?.FirstOrDefault(t => t.Id.ToString() == Id);

            if (transactionToRemove == null)
            {
                return false;
            }

            transactions?.Remove(transactionToRemove);

            if (transactions != null)
            {
                WriteTransactionsToJson(transactions);
            }
            return true;
        }

        public List<Transaction>? GetAllTransactions()
        {
            return ReadTransactionsFromJson();
        }

        public List<Transaction>? GetTransactionByApplicationId(int applicationId)
        {
            var transactions = ReadTransactionsFromJson();
            return transactions?.Where(t => t.ApplicationId == applicationId).ToList();
        }

        public Transaction? GetTransactionById(string Id)
        {
            var transactions = ReadTransactionsFromJson();
            return transactions?.FirstOrDefault(t => t.Id.ToString() == Id);
        }

        public List<Transaction>? GetTransactionByPostingDate(DateTime postingDate)
        {
            var transactions = ReadTransactionsFromJson();
            return transactions?.Where(t => t.PostingDate == postingDate).ToList();
        }

        public List<Transaction>? GetTransactionByType(string type)
        {
            var transactions = ReadTransactionsFromJson();
            return transactions?.Where(t => t.Type == type).ToList();
        }

        public Transaction UpdateClearedStatusForTransaction(Transaction updatedTransaction)
        {
            var transactions = ReadTransactionsFromJson();
            var existingTransaction = transactions?.Find(t => t.Id == updatedTransaction.Id);

            if (existingTransaction == null)
            {
                return null;
            }

            // Update the existing transaction with the Cleared Status
            existingTransaction.IsCleared = true;
            existingTransaction.ClearedDate = DateTime.Now;

            if (transactions != null)
            {
                WriteTransactionsToJson(transactions);
            }

            return existingTransaction;
        }

        public Transaction UpdateTransaction(Transaction updatedTransaction)
        {
            var transactions = ReadTransactionsFromJson();
            var existingTransaction = transactions?.Find(t => t.Id == updatedTransaction.Id);

            if (existingTransaction == null)
            {
                return null;
            }

            // Update the existing transaction with the new data
            existingTransaction.ApplicationId = updatedTransaction.ApplicationId;
            existingTransaction.Type = updatedTransaction.Type;
            existingTransaction.Summary = updatedTransaction.Summary;
            existingTransaction.Amount = updatedTransaction.Amount;
            existingTransaction.PostingDate = updatedTransaction.PostingDate;
            existingTransaction.IsCleared = updatedTransaction.IsCleared;
            existingTransaction.ClearedDate = updatedTransaction.ClearedDate;

            if (transactions != null)
            {
                WriteTransactionsToJson(transactions);
            }

            return existingTransaction;
        }

        // Helper method to read transactions from JSON file
        public List<Transaction>? ReadTransactionsFromJson()
        {
            try
            {
                var json = File.ReadAllText(jsonFilePath);

                return JsonConvert.DeserializeObject<List<Transaction>>(json);
            }
            catch (Exception)
            {
                return new List<Transaction>();
            }
        }

        // Helper method to write transactions to JSON file
        public void WriteTransactionsToJson(List<Transaction> transactions)
        {
            var json = JsonConvert.SerializeObject(transactions, Formatting.Indented);
            File.WriteAllText(jsonFilePath, json);
        }

    }
}
