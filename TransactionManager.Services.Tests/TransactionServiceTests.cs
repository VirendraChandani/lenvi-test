using Moq;
using Newtonsoft.Json;
using TransactionManager.Models;
using TransactionManager.Services.Transactions;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

public class TransactionServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly TransactionService _transactionService;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly string _testDataFilePath = "test-data.json";

    public TransactionServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.SetupGet(x => x["JsonDataFilePath"]).Returns(_testDataFilePath);

        _transactionService = new TransactionService(_configurationMock.Object);
        _transactionServiceMock = new Mock<ITransactionService>();
    }

    [Fact]
    public void CreateTransaction_ValidTransaction_ReturnsTransactionId()
    {
        // Arrange
        var transaction = new Transaction
        {
            ApplicationId = 123,
            Type = "Debit",
            Summary = "Test Transaction",
            Amount = 50.0M,
            PostingDate = DateTime.Now,
            IsCleared = false
        };

        // Act
        var transactionId = _transactionService.CreateTransaction(transaction);

        // Assert
        Assert.NotNull(transactionId);
        Assert.IsType<Guid>(transactionId);
    }

    [Fact]
    public void GetAllTransactions_EmptyFile_ReturnsNullTransactions()
    {
        // Arrange
        File.WriteAllText(_testDataFilePath, "");

        // Act
        var transactions = _transactionService.GetAllTransactions();

        // Assert
        Assert.Null(transactions);
    }

    [Fact]
    public void GetAllTransactions_ValidFile_ReturnsTransactionList()
    {
        // Arrange
        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 2 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 }
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var transactions = _transactionService.GetAllTransactions();

        // Assert
        Assert.NotNull(transactions);
        Assert.Equal(testData.Count, transactions.Count);
    }

    [Fact]
    public void GetTransactionByApplicationId_ValidApplicationId_ReturnsTransactions()
    {
        // Arrange
        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 2 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 }
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var transactions = _transactionService.GetTransactionByApplicationId(1);

        // Assert
        Assert.NotNull(transactions);
        Assert.Equal(2, transactions.Count);
        Assert.All(transactions, t => Assert.Equal(1, t.ApplicationId));
    }

    [Fact]
    public void GetTransactionByApplicationId_InvalidApplicationId_ReturnsNullTransactions()
    {
        // Arrange
        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 2 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 }
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var transactions = _transactionService.GetTransactionByApplicationId(3);

        // Assert
        Assert.NotNull(transactions);
        Assert.Empty(transactions);
    }

    [Fact]
    public void GetTransactionByType_ValidType_ReturnsTransactions()
    {
        // Arrange
        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), Type = "Credit" },
            new Transaction { Id = Guid.NewGuid(), Type = "Credit" },
            new Transaction { Id = Guid.NewGuid(), Type = "Debit" }
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var transactions = _transactionService.GetTransactionByType("Credit");

        // Assert
        Assert.NotNull(transactions);
        Assert.Equal(2, transactions.Count);
        Assert.All(transactions, t => Assert.Equal("Credit", t.Type));
    }

    [Fact]
    public void GetTransactionByType_InvalidType_ReturnsEmptyList()
    {
        // Arrange
        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), Type = "Credit" },
            new Transaction { Id = Guid.NewGuid(), Type = "Credit" },
            new Transaction { Id = Guid.NewGuid(), Type = "Debit" }
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var transactions = _transactionService.GetTransactionByType("Invalid");

        // Assert
        Assert.NotNull(transactions);
        Assert.Empty(transactions);
    }

    [Fact]
    public void GetTransactionByPostingDate_ValidPostingDate_ReturnsTransactions()
    {
        // Arrange
        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), PostingDate = DateTime.Parse("2017-01-02T00:00:00") },
            new Transaction { Id = Guid.NewGuid(), PostingDate = DateTime.Parse("2017-01-02T00:00:00") },
            new Transaction { Id = Guid.NewGuid(), PostingDate = DateTime.Parse("2017-02-02T00:00:00") }
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var transactions = _transactionService.GetTransactionByPostingDate(DateTime.Parse("2017-01-02T00:00:00"));

        // Assert
        Assert.NotNull(transactions);
        Assert.Equal(2, transactions.Count);
        Assert.All(transactions, t => Assert.Equal(DateTime.Parse("2017-01-02T00:00:00"), t.PostingDate));
    }

    [Fact]
    public void GetTransactionByPostingDate_InvalidPostingDate_ReturnsEmptyList()
    {
        // Arrange
        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), PostingDate = DateTime.Parse("2017-01-02T00:00:00") },
            new Transaction { Id = Guid.NewGuid(), PostingDate = DateTime.Parse("2017-01-02T00:00:00") },
            new Transaction { Id = Guid.NewGuid(), PostingDate = DateTime.Parse("2017-02-02T00:00:00") }
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var transactions = _transactionService.GetTransactionByPostingDate(DateTime.Parse("2017-03-02T00:00:00"));

        // Assert
        Assert.NotNull(transactions);
        Assert.Empty(transactions);
    }

    [Fact]
    public void DeleteTransaction_ExistingTransactionId_ReturnsTrueAndDeletesTransaction()
    {
        // Arrange
        var transactionIdToDelete = Guid.NewGuid();
        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 },
            new Transaction { Id = transactionIdToDelete, ApplicationId = 2 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 }
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var result = _transactionService.DeleteTransaction(transactionIdToDelete.ToString());

        // Assert
        Assert.True(result);
        var updatedTransactions = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(_testDataFilePath));
        Assert.DoesNotContain(updatedTransactions, t => t.Id == transactionIdToDelete);
    }

    [Fact]
    public void DeleteTransaction_NonExistingTransactionId_ReturnsFalse()
    {
        // Arrange
        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 2 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 }
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var result = _transactionService.DeleteTransaction(Guid.NewGuid().ToString());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateClearedStatusForTransaction_ExistingTransaction_SuccessfullyUpdates()
    {
        // Arrange
        var existingTransactionId = Guid.NewGuid();
        var updatedTransaction = new Transaction
        {
            Id = existingTransactionId,
            IsCleared = true,
            ClearedDate = DateTime.Now
        };

        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 2 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 },
            updatedTransaction
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var result = _transactionService.UpdateClearedStatusForTransaction(updatedTransaction);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsCleared);
        Assert.NotNull(result.ClearedDate);
    }

    [Fact]
    public void UpdateClearedStatusForTransaction_NonExistentTransaction_ReturnsNull()
    {
        // Arrange
        var updatedTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            IsCleared = true,
            ClearedDate = DateTime.Now
        };

        var transactions = new List<Transaction>();

        _transactionServiceMock.Setup(t => t.ReadTransactionsFromJson()).Returns(transactions);

        // Act
        var result = _transactionService.UpdateClearedStatusForTransaction(updatedTransaction);

        // Assert
        Assert.Null(result);

        _transactionServiceMock.Verify(t => t.WriteTransactionsToJson(It.IsAny<List<Transaction>>()), Times.Never);
    }

    [Fact]
    public void UpdateTransaction_ExistingTransaction_SuccessfullyUpdates()
    {
        // Arrange
        var existingTransactionId = Guid.NewGuid();
        var updatedTransaction = new Transaction
        {
            Id = existingTransactionId,
            ApplicationId = 12345,
            Type = "Credit",
            Summary = "Updated Summary",
            Amount = 99.99M,
            PostingDate = DateTime.Now,
            IsCleared = true,
            ClearedDate = DateTime.Now
        };

        var testData = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 2 },
            new Transaction { Id = Guid.NewGuid(), ApplicationId = 1 },
            updatedTransaction
        };
        File.WriteAllText(_testDataFilePath, JsonConvert.SerializeObject(testData));

        // Act
        var result = _transactionService.UpdateTransaction(updatedTransaction);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedTransaction.ApplicationId, result.ApplicationId);
        Assert.Equal(updatedTransaction.Type, result.Type);
        Assert.Equal(updatedTransaction.Summary, result.Summary);
        Assert.Equal(updatedTransaction.Amount, result.Amount);
        Assert.Equal(updatedTransaction.PostingDate, result.PostingDate);
        Assert.Equal(updatedTransaction.IsCleared, result.IsCleared);
        Assert.Equal(updatedTransaction.ClearedDate, result.ClearedDate);
    }

    [Fact]
    public void UpdateTransaction_NonExistentTransaction_ReturnsNull()
    {
        // Arrange
        var updatedTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ApplicationId = 12345,
            Type = "Credit",
            Summary = "Updated Summary",
            Amount = 99.99M,
            PostingDate = DateTime.Now,
            IsCleared = true,
            ClearedDate = DateTime.Now
        };

        var transactions = new List<Transaction>();

        _transactionServiceMock.Setup(t => t.ReadTransactionsFromJson()).Returns(transactions);

        // Act
        var result = _transactionService.UpdateTransaction(updatedTransaction);

        // Assert
        Assert.Null(result);

        _transactionServiceMock.Verify(t => t.WriteTransactionsToJson(It.IsAny<List<Transaction>>()), Times.Never);
    }

    // Clean up after tests
    public void Dispose()
    {
        if (File.Exists(_testDataFilePath))
        {
            File.Delete(_testDataFilePath);
        }
    }
}
