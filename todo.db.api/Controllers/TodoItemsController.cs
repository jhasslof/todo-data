using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using todo.db.api.Database;
using todo.db.api.Mappers;
using todo.db.api.Models;

namespace todo.db.api.Controllers
{
    [Route("api/TodoItems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoDbContext _context;

        public TodoItemsController(ITodoDbContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return TodoItemDTOMapper.Map(
                await _context.ListTodoItems()
                );
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItemDto = TodoItemDTOMapper.Map(
                                await _context.Get(id)
                                );

            if (todoItemDto == null)
            {
                return NotFound();
            }
            return todoItemDto;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }

            var todoItem = await _context.Get(id);
            if (todoItem == null)
            {
                return NotFound();
            }
            TodoItemMapper.Fill(todoItem, todoItemDTO);

            try
            {
                await _context.Update(todoItem);
            }
            catch (ApplicationException)
            {
                return NotFound();
            }
            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            if (0 != todoItemDTO.Id)
            {
                return BadRequest();
            }

            var todoItem = TodoItemMapper.Map(todoItemDTO);

            todoItem = await _context.Create(todoItem);

            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = todoItem.TodoItemId },
                TodoItemDTOMapper.Map(todoItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            try
            {
                await _context.Delete(id);
            }
            catch(ApplicationException)
            {
                return NotFound();
            }

            return NoContent();
        }


    }
}
