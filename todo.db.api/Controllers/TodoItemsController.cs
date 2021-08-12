using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using todo.db.api.Mappers;
using todo.db.api.Models;

namespace todo.db.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoDbContext _context;
        private readonly TodoItemDTOMapper _todoItemDTOMapper;
        private readonly TodoItemMapper _todoItemMapper;

        public TodoItemsController(ITodoDbContext context, IFeatureFlags featureFlags)
        {
            _context = context;
            _todoItemDTOMapper = new TodoItemDTOMapper(featureFlags);
            _todoItemMapper = new TodoItemMapper(featureFlags);
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return _todoItemDTOMapper.Map(
                await _context.ListTodoItems()
                );
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItemDto = _todoItemDTOMapper.Map(
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
            _todoItemMapper.Fill(todoItem, todoItemDTO);

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

            var todoItem = _todoItemMapper.Map(todoItemDTO);

            todoItem = await _context.Create(todoItem);

            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = todoItem.TodoItemId },
                _todoItemDTOMapper.Map(todoItem));
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
