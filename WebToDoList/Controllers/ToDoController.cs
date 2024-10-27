using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using WebToDoList.DTOs;

namespace WebToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly TodoContext _context;

        public ToDoController(TodoContext context)
        {
            _context = context;
        }
      
        // GET: api/ToDo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/ToDo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/ToDo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoItemDto)
        {
            var existingTodoItem = await _context.TodoItems.FindAsync(id);
            if (existingTodoItem == null)
            {
                return NotFound();
            }
            if (!Enum.IsDefined(typeof(TaskStatusNow), todoItemDto.Status))
            {
                return BadRequest("Invalid status value. Allowed values are ToDo, InProgress, and Done.");
            }
            int currentStatusIndex = (int)existingTodoItem.Status;
            int newStatusIndex = (int)todoItemDto.Status;

            if (Math.Abs(newStatusIndex - currentStatusIndex) != 1)
            {
                return BadRequest("Status can only be changed to the next or previous state.");
            }
                
            existingTodoItem.Name = todoItemDto.Name;
            existingTodoItem.Status = todoItemDto.Status;
           

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ToDo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItemDTO todoItemDto)
        {
            if (!Enum.IsDefined(typeof(TaskStatusNow), todoItemDto.Status))
            {
                return BadRequest("Invalid status value. Allowed values are ToDo, InProgress, and Done.");
            }
            var todoItem = new TodoItem
            {
                Name = todoItemDto.Name,
                Status = todoItemDto.Status
            };
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/ToDo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/ToDo/statuses
        [HttpGet("statuses")]
        public ActionResult<IEnumerable<string>> GetTaskStatuses()
        {
            // Получаем все имена из TaskStatusNow и преобразуем в список строк
            var statuses = Enum.GetNames(typeof(TaskStatusNow)).ToList();
            return Ok(statuses);
        }
        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
