using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Todo.API.Dtos;
using Todo.API.Models;
using Todo.API.Services;

namespace Todo.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion(1)]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TodoController> _logger;
        private readonly PaginationMetadata _paginationMetadata;

        public TodoController(ITodoRepository todoRepository, IMapper mapper, ILogger<TodoController> logger)
        {
            _todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all Todos , searchQuery & paging is optional
        /// </summary>
        /// <param name="title"></param>
        /// <param name="searchQuery"></param>
        /// <param name="requestedPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoReadDto>>> GetTodos(string? title, string? searchQuery, int requestedPage = 1, int pageSize = 2)
        {
            _logger.LogInformation("Start Getting Todos");

            var (todos, paginationMetadata) = await _todoRepository.GetTodosAsync(title, searchQuery, requestedPage, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata)); 

            _logger.LogInformation("End Getting Todos");

            return Ok(_mapper.Map<IEnumerable<TodoReadDto>>(todos));
        }

        /// <summary>
        /// Get Todo By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns the request todo</response>
        [HttpGet("{id}", Name = "GetTodoById")]
        [ApiVersion(0.1, Deprecated = true)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoReadDto>> GetTodoById(int id)
        {
            _logger.LogInformation("Start GetTodoById");

            var todo = await _todoRepository.GetTodoAsync(id);

            if (todo == null)
            {
                _logger.LogInformation("GetTodoById not found");
                return NotFound();
            }

            _logger.LogInformation("End GetTodoById");
            
            return Ok(_mapper.Map<TodoReadDto>(todo));
        }

        /// <summary>
        /// Create a Todo
        /// </summary>
        /// <param name="todoCreateDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<TodoReadDto> CreateTodo(TodoCreateDto todoCreateDto)
        {
            _logger.LogInformation("Start CreateTodo");

            var todoEntity = _mapper.Map<TodoEntity>(todoCreateDto);
            _todoRepository.Create(todoEntity);
            _todoRepository.SaveChanges();

            var todoReadDto = _mapper.Map<TodoReadDto>(todoEntity);

            _logger.LogInformation("End CreateTodo");

            return CreatedAtRoute(nameof(GetTodoById), new { id = todoReadDto.Id }, todoReadDto);
        }

        /// <summary>
        /// update a todo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="todoCreateDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoReadDto>> UpdateTodo(int id, TodoCreateDto todoCreateDto)
        {
            _logger.LogInformation("Start UpdateTodo");

            var todoEntity = await _todoRepository.GetTodoAsync(id);
            if(todoEntity == null)
                return NotFound();

            todoEntity.UpdatedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            _mapper.Map(todoCreateDto, todoEntity);
            await _todoRepository.SaveChangesAsync();

            var todoReadDto = _mapper.Map<TodoReadDto>(todoEntity);

            _logger.LogInformation("End UpdateTodo");

            return CreatedAtRoute(nameof(GetTodoById), new { id = todoReadDto.Id }, todoReadDto);
        }

        /// <summary>
        /// partial update with JsonPatchDocument<TodoCreateDto>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoReadDto>> PartialUpdateTodo(int id, JsonPatchDocument<TodoCreateDto> patchDocument)
        {
            _logger.LogInformation("Start PartialUpdateTodo");

            var todoEntity = await _todoRepository.GetTodoAsync(id);
            if (todoEntity == null)
                return NotFound();

            var todoDtoToPatch = _mapper.Map<TodoCreateDto>(todoEntity);

            patchDocument.ApplyTo(todoDtoToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(todoDtoToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(todoDtoToPatch, todoEntity);
            await _todoRepository.SaveChangesAsync();

            var todoReadDto = _mapper.Map<TodoReadDto>(todoEntity);

            _logger.LogInformation("End PartialUpdateTodo");

            return CreatedAtRoute(nameof(GetTodoById), new { id = todoEntity.Id }, todoEntity);
        }

        /// <summary>
        /// Delete todo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteTodo(int id)
        {
            _logger.LogInformation("Start DeleteTodo");

            var todoEntity = await _todoRepository.GetTodoAsync(id);
            if (todoEntity == null)
                return NotFound();

            _todoRepository.Delete(todoEntity);
            await _todoRepository.SaveChangesAsync();
            
            _logger.LogInformation("End DeleteTodo");

            return NoContent();
        }
    }
}
