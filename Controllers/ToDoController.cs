using ToDoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ToDoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoContext _context;

        private readonly ILogger<ToDoController> _logger;
       public ToDoController(ILogger<ToDoController> logger, ToDoContext context)
        {
            _context = context;
            _logger = logger;

            // Call the asynchronous initialization method
            InitializeAsync().ConfigureAwait(false);
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (await _context.ToDoItems.CountAsync() == 0)
                {
                    await _context.ToDoItems.AddAsync(new ToDoItem { Name = "Item1", isComplete = false });
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding initial ToDoItem.");
            }
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoItem>>> GetToDoItems()
        {
            return await _context.ToDoItems.AsNoTracking().ToListAsync();
        }

        //Theory
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItem>> GetToDoItem(long id)
        {
            var ToDoItem = await _context.ToDoItems.FindAsync(id);

            return ToDoItem == null ? NotFound() : ToDoItem;
        }


        [HttpPost]
        public async Task<IActionResult> PostToDoItem(ToDoItem todoitem)
        {
            if (todoitem == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid todo item data.");
            }

            try
            {
                await _context.ToDoItems.AddAsync(todoitem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the ToDo item.");

                if (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
                {
                    return Conflict(new { message = $"ToDoItem with ID {todoitem.Id} already exists." });
                }
                return StatusCode(500, new { message = "An error occurred while saving the ToDo item." });
            }

            return CreatedAtAction(nameof(GetToDoItem), new { id = todoitem.Id }, todoitem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToDoItem>> EditToDoItem(long id, ToDoItem todoitem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ToDoItem item = await _context.ToDoItems.FindAsync(id);

            if (item != null)
            {
                item.Name = todoitem.Name;
                await _context.SaveChangesAsync();
                return Ok(item);
            }
            else
            {
                return NotFound(new { message = $"ToDoItem with {id} doesn't exist" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDo(long id)
        {
            ToDoItem item = await _context.ToDoItems.FindAsync(id);

            if (item != null)
            {
                _context.ToDoItems.Remove(item);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"ToDoItem with {id} has been deleted" });
            }
            else
            {
                return NotFound(new { message = $"ToDoItem with {id} doesn't exist" });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> MarkAsDone(long id)
        {
            ToDoItem item = await _context.ToDoItems.FindAsync(id);

            if (item != null)
            {
                item.isComplete = true;
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }

}