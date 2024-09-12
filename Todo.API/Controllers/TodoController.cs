using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Todo.API.Dtos;
using Todo.API.Services;

namespace Todo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoRepository todoRepository, IMapper mapper, ILogger<TodoController> logger)
        {
            _todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoReadDto>>> GetTodos()
        {
            _logger.LogInformation("Start Getting Todos");

            var todos = await _todoRepository.GetTodosAsync();

            _logger.LogInformation("End Getting Todos");

            return Ok(_mapper.Map<IEnumerable<TodoReadDto>>(todos));
        }

        [HttpGet("{id}", Name = "GetTodoById")]
        public ActionResult<TodoReadDto> GetTodoById(int id)
        {
            _logger.LogInformation("Start GetTodoById");

            var todos = new List<TodoReadDto>
            {
                new TodoReadDto
                {
                    Id = 1,
                    Title = "take out the dog",
                    Description="do it at the evening",
                },
                new TodoReadDto
                {
                    Id = 2,
                    Title = "take my girl swimming",
                    Description = "talk to Inbal"
                },
            };

            var todo = todos.Where(t => t.Id == id).FirstOrDefault();

            _logger.LogInformation("End GetTodoById");
            
            return Ok(todo);
        }

        [HttpPost]
        public ActionResult<TodoReadDto> CreateTodo(TodoCreateDto todoCreateDto)
        {
            _logger.LogInformation("Start CreateTodo");


            var todo = new TodoCreateDto
            {
                Id = 3,
                Title = todoCreateDto.Title,
                Description = todoCreateDto.Description,
            };

            _logger.LogInformation("End CreateTodo");

            return CreatedAtRoute(nameof(GetTodoById), new { id = todo.Id }, todo);
        }
    }
}
