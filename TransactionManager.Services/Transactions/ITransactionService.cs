using TransactionManager.Models;

namespace TransactionManager.Services.Transactions
{
    public interface ITransactionService
    {
        List<Transaction>? GetAllTransactions();
        Transaction? GetTransactionById(string Id);
        List<Transaction>? GetTransactionByApplicationId(int applicationId);
        List<Transaction>? GetTransactionByType(string type);
        List<Transaction>? GetTransactionByPostingDate(DateTime postingDate);
        Guid? CreateTransaction(Transaction transaction);
        Transaction? UpdateTransaction(Transaction transaction);
        Transaction? UpdateClearedStatusForTransaction(Transaction transaction);
        bool DeleteTransaction(string Id);
        List<Transaction>? ReadTransactionsFromJson();
        void WriteTransactionsToJson(List<Transaction> transactions);
    }
}
