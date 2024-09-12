using Microsoft.EntityFrameworkCore;
using Serilog;
using Todo.API.Data;
using Todo.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
                        .MinimumLevel.Debug()
                        .WriteTo.Console());

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<TodoContext>(option => option.UseInMemoryDatabase("InMem"));
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app);

app.Run();
