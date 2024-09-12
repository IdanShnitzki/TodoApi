using Todo.API.Models;

namespace Todo.API.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<TodoContext>());
            }
        }


        public static void SeedData(TodoContext todoContext)
        {
            if (!todoContext.Todos.Any())
            {
                Console.WriteLine("Seeding data...");

                todoContext.Todos.AddRange
                    (
                        new Todos
                        {
                            Id = 1,
                            Title = "take out the dog",
                            Description = "do it at the evening"
                        },
                        new Todos
                        {
                            Id = 2,
                            Title = "take my girl swimming",
                            Description = "talk to Inbal"
                        },
                        new Todos
                        {
                            Id = 3,
                            Title = "Go to Gym",
                            Description = ""
                        },
                        new Todos
                        {
                            Id = 4,
                            Title = "Talk to mom",
                            Description = ""
                        }
                    );

                todoContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("Data already populated");
            }
        }
    }
}
