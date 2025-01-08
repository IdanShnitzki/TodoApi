using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using Todo.API.Data;
using Todo.API.Services;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/Todolog.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (environment == Environments.Development)
{
    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
                            .MinimumLevel.Debug()
                            .WriteTo.Console()
                            .WriteTo.File("logs/ToodLogDevelopment.log", rollingInterval: RollingInterval.Day));
}
else
{
    var secretClient = new SecretClient(
        new Uri("https://tododotnetkeyvalut.vault.azure.net/"),
        new DefaultAzureCredential());

    builder.Configuration.AddAzureKeyVault(secretClient,
        new KeyVaultSecretManager());

    builder.Host.UseSerilog(
        (context, loggerConfiguration) => loggerConfiguration
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/TodoLog.log", rollingInterval: Serilog.RollingInterval.Day)
            .WriteTo.ApplicationInsights(new TelemetryConfiguration
            {
                InstrumentationKey = builder.Configuration["ApplicationInsightsInstrumentationKey"]
            }, TelemetryConverter.Traces));
}


// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();


//add options.CustomizeProblemDetails to add extra information on when returning an error 
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = ctx =>
        ctx.ProblemDetails.Extensions.Add("MachineName", Environment.MachineName));

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddTransient<PaginationMetadata>();


builder.Services.AddDbContext<TodoContext>(option => option.UseInMemoryDatabase("InMem"));
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc()
.AddApiExplorer(setupAction =>
{
    setupAction.SubstituteApiVersionInUrl = true;
});

var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

builder.Services.AddSwaggerGen(setupAction =>
{
    foreach(var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        setupAction.SwaggerDoc(
            $"{description.GroupName}",
            new()
            {
                Title = "Todo API",
                Version = description.ApiVersion.ToString(),
                Description = "Through this API you can access all todos"
            });
    }

    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);

    setupAction.IncludeXmlComments(xmlCommentsFullPath);

    setupAction.AddSecurityDefinition("TodoApiBearerAuth", new() { Type = SecuritySchemeType.Http, Scheme = "Bearer", Description = "Input a valid token to access this API" });

    setupAction.AddSecurityRequirement(new ()
    {
        {
            new ()
            {
                Reference = new OpenApiReference 
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "TodoApiBearerAuth" 
                }                
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]))
        };
    });

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            setupAction.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
//}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app);

app.Run();
