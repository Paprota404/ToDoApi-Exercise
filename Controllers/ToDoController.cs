using ToDoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ToDoController : ControllerBase
{
    private readonly ToDoContext _context;

    public ToDoController(ToDoContext context)
    {
        _context = context;

        if (_context.ToDoItems.Count() == 0)
        {
            _context.ToDoItems.Add(new ToDoItem { Name = "Item1" });
            _context.SaveChanges();
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDoITem>>> GetToDoItems()
    {
        return await _context.ToDoItems.ToListAsync();
    }

    [HttpGet]
    public async Task<ActionResult<ToDoItem>> GetToDoItem(long id)
    {
        var ToDoItem = await _context.ToDoItems.FindAsync(id);

        return ToDoItem == null ? NotFound() : ToDoItem;
    }

    [HttpPost]
    public async 
}