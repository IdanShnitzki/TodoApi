using AutoMapper;
using Todo.API.Dtos;
using Todo.API.Models;

namespace Todo.API.Profiles
{
    public class TodoProfiles : Profile
    {
        public TodoProfiles()
        {
            CreateMap<TodoEntity, TodoReadDto>();
            CreateMap<TodoCreateDto, TodoEntity>();
        }
    }
}
