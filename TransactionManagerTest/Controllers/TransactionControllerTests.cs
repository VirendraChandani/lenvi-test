using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TransactionManager.Controllers;
using TransactionManager.Models;
using TransactionManager.Services.Transactions;
using Xunit;

public class TransactionControllerTests
{
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ILogger<TransactionController>> _loggerMock;
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ILogger<TransactionController>>();
        _controller = new TransactionController(_loggerMock.Object, _transactionServiceMock.Object);
    }

    [Fact]
    public void Get_ReturnsAllTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid() },
            new Transaction { Id = Guid.NewGuid() }
        };

        _transactionServiceMock.Setup(t => t.GetAllTransactions()).Returns(transactions);

        // Act
        var result = _controller.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<List<Transaction>>(result.Value);
        Assert.Equal(transactions.Count, model.Count);
    }

    [Fact]
    public void Get_ById_ReturnsTransaction()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction { Id = transactionId };

        _transactionServiceMock.Setup(t => t.GetTransactionById(transactionId.ToString())).Returns(transaction);

        // Act
        var result = _controller.Get(transactionId.ToString()) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<Transaction>(result.Value);
        Assert.Equal(transactionId, model.Id);
    }

    [Fact]
    public void Get_ById_NotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        _transactionServiceMock.Setup(t => t.GetTransactionById(transactionId.ToString())).Returns((Transaction)null);

        // Act
        var result = _controller.Get(transactionId.ToString()) as NotFoundResult;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetByApplicationId_ReturnsTransactions()
    {
        // Arrange
        var applicationId = 12345;
        var transactions = new List<Transaction>
        {
            new Transaction { ApplicationId = applicationId },
            new Transaction { ApplicationId = applicationId }
        };

        _transactionServiceMock.Setup(t => t.GetTransactionByApplicationId(applicationId)).Returns(transactions);

        // Act
        var result = _controller.GetByApplicationId(applicationId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<List<Transaction>>(result.Value);
        Assert.Equal(transactions.Count, model.Count);
    }

    [Fact]
    public void GetByApplicationId_NotFound()
    {
        // Arrange
        var applicationId = 12345;

        _transactionServiceMock.Setup(t => t.GetTransactionByApplicationId(applicationId)).Returns((List<Transaction>)null);

        // Act
        var result = _controller.GetByApplicationId(applicationId) as NotFoundResult;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetByType_ReturnsTransactions()
    {
        // Arrange
        var type = "Debit";
        var transactions = new List<Transaction>
        {
            new Transaction { Type = type },
            new Transaction { Type = type }
        };

        _transactionServiceMock.Setup(t => t.GetTransactionByType(type)).Returns(transactions);

        // Act
        var result = _controller.GetByType(type) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<List<Transaction>>(result.Value);
        Assert.Equal(transactions.Count, model.Count);
    }

    [Fact]
    public void GetByType_NotFound()
    {
        // Arrange
        var type = "Debit";

        _transactionServiceMock.Setup(t => t.GetTransactionByType(type)).Returns((List<Transaction>)null);

        // Act
        var result = _controller.GetByType(type) as NotFoundResult;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetByPostingDate_ReturnsTransactions()
    {
        // Arrange
        var postingDate = DateTime.Now;
        var transactions = new List<Transaction>
        {
            new Transaction { PostingDate = postingDate },
            new Transaction { PostingDate = postingDate }
        };

        _transactionServiceMock.Setup(t => t.GetTransactionByPostingDate(postingDate)).Returns(transactions);

        // Act
        var result = _controller.GetByPostingDate(postingDate) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<List<Transaction>>(result.Value);
        Assert.Equal(transactions.Count, model.Count);
    }

    [Fact]
    public void GetByPostingDate_NotFound()
    {
        // Arrange
        var postingDate = DateTime.Now;

        _transactionServiceMock.Setup(t => t.GetTransactionByPostingDate(postingDate)).Returns((List<Transaction>)null);

        // Act
        var result = _controller.GetByPostingDate(postingDate) as NotFoundResult;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Post_CreatesTransaction()
    {
        // Arrange
        var transaction = new Transaction { Id = Guid.NewGuid() };
        _transactionServiceMock.Setup(t => t.CreateTransaction(transaction)).Returns(transaction.Id);

        // Act
        var result = _controller.Post(transaction) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Get", result.ActionName);
    }

    [Fact]
    public void Post_InvalidTransaction_BadRequest()
    {
        // Arrange
        Transaction transaction = null;

        // Act
        var result = _controller.Post(transaction) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Invalid transaction data.", result.Value);
    }

    [Fact]
    public void UpdateClearedStatusForTransaction_UpdatesTransaction()
    {
        // Arrange
        var transaction = new Transaction { Id = Guid.NewGuid() };
        _transactionServiceMock.Setup(t => t.UpdateClearedStatusForTransaction(transaction)).Returns(transaction);

        // Act
        var result = _controller.UpdateClearedStatusForTransaction(transaction) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<Transaction>(result.Value);
        Assert.Equal(transaction.Id, model.Id);
    }

    [Fact]
    public void UpdateClearedStatusForTransaction_InvalidTransaction_BadRequest()
    {
        // Arrange
        Transaction transaction = null;

        // Act
        var result = _controller.UpdateClearedStatusForTransaction(transaction) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Invalid transaction data.", result.Value);
    }

    [Fact]
    public void Put_UpdatesTransaction()
    {
        // Arrange
        var transaction = new Transaction { Id = Guid.NewGuid() };
        _transactionServiceMock.Setup(t => t.UpdateTransaction(transaction)).Returns(transaction);

        // Act
        var result = _controller.Put(transaction) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<Transaction>(result.Value);
        Assert.Equal(transaction.Id, model.Id);
    }

    [Fact]
    public void Put_InvalidTransaction_BadRequest()
    {
        // Arrange
        Transaction transaction = null;

        // Act
        var result = _controller.Put(transaction) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Invalid transaction data.", result.Value);
    }

    [Fact]
    public void Delete_DeletesTransaction()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _transactionServiceMock.Setup(t => t.DeleteTransaction(transactionId.ToString())).Returns(true);

        // Act
        var result = _controller.Delete(transactionId.ToString()) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<bool>(result.Value);
        Assert.True(model);
    }

    [Fact]
    public void Delete_NotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _transactionServiceMock.Setup(t => t.DeleteTransaction(transactionId.ToString())).Returns(false);

        // Act
        var result = _controller.Delete(transactionId.ToString()) as NotFoundResult;

        // Assert
        Assert.NotNull(result);
    }
}
