using ToDoApi.Controllers;
using ToDoApi.Models;
using Xunit;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ToDoApi.Tests
{
    public class ToDoTests
    {
        private readonly ILogger<ToDoController> _logger;
        private readonly ToDoContext _context;
        private readonly ToDoController _controller;

        public ToDoTests()
        {
            _context = Substitute.For<ToDoContext>();
            _logger = Substitute.For<ILogger<ToDoController>>();
            _controller = new ToDoController(_logger, _context);
        }

        [Fact]
        public async Task GetToDoItems_ReturnsOkResult_WithListOfToDoItems()
        {
            //Arrange
            var toDoItems = new List<ToDoItem>
            {
                new ToDoItem { Id = 1, Name = "Test Item 1 ", isComplete = false},
                new ToDoItem { Id = 2, Name = "Test Item 2" , isComplete = false},
            };

            var dbSetMock = Substitute.For<DbSet<ToDoItem>, IQueryable<ToDoItem>>();
            ((IQueryable<ToDoItem>)dbSetMock).Provider.Returns(toDoItems.AsQueryable().Provider);
            ((IQueryable<ToDoItem>)dbSetMock).Expression.Returns(toDoItems.AsQueryable().Expression);
            ((IQueryable<ToDoItem>)dbSetMock).ElementType.Returns(toDoItems.AsQueryable().ElementType);
            ((IQueryable<ToDoItem>)dbSetMock).GetEnumerator().Returns(toDoItems.GetEnumerator());

            _context.ToDoItems.Returns(dbSetMock);

            //Act
            var result = await _controller.GetToDoItems();

            //Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<ToDoItem>>>(result);
            var returnValue = Assert.IsType<List<ToDoItem>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
    }
}