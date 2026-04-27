using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using System.Text;
using WebApplication2.Domain.Interfaces;
using WebApplication2.Infrastructure.Data;
using WebApplication2.Infrastructure.Repositories;
using WebApplication2.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка JWT аутентификации
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // Добавили ! после ["Jwt:Key"]
            IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// 2. Регистрация сервисов и репозиториев
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserRepository>(); // Твой репозиторий для Auth

// 3. Настройка Swagger для JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Авторизация. Введите 'Bearer {токен}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 4. Логирование и БД
builder.Logging.AddConsole();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 5. HttpClient (для Quartz)
builder.Services.AddHttpClient();

// 6. Настройка Quartz.NET
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("CurrencyUpdateJob");
    q.AddJob<CurrencyUpdateJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("CurrencyUpdateJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInHours(1) // Обновление раз в час
            .RepeatForever()));
});

// Добавляем Quartz как фоновую службу
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// 7. Контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Настройка Pipeline (Middlewares)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Сначала КТО ты
app.UseAuthorization();  // Потом ЧТО тебе можно

app.MapControllers();

// Автоматическое применение миграций при старте (удобно для разработки)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();