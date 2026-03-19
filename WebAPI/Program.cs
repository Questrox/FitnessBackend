using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Domain.Interfaces;
using Infrastructure.Data;
using Application.Services;
using Infrastructure.Repositories;
using WebAPI.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<FitnessDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<FitnessDb>()
    .AddDefaultTokenProviders();

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
            IssuerSigningKey = new SymmetricSecurityKey( //Симметричный ключ для проверки и подписи jwt-токенов
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]))
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireRoleUser", policy => policy.RequireRole("User"));
    options.AddPolicy("RequireCoachRole", policy => policy.RequireRole("Coach"));
});

#region Регистрация зависимостей

//Регистрация зависимостей.
//Services - это объект IServiceCollection. IServiceCollection - список ServiceDescriptor,
//который содержит ServiceType (тип - IUserRepository), ImplementationType (реализация - UserRepository),
//LifeTime (время жизни - Scope, Singleton или Transient (каждый раз новый объект)) и Implementation Factory
builder.Services.AddScoped<IUserRepository, UserRepository>(); //Если кто-то попросит IUserRepository, нужно дать ему UserRepository
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ClientService>();

builder.Services.AddScoped<ICoachRepository, CoachRepository>();
builder.Services.AddScoped<CoachService>();

builder.Services.AddScoped<ICoachScheduleRepository, CoachScheduleRepository>();
builder.Services.AddScoped<CoachScheduleService>();

builder.Services.AddScoped<IDictionariesRepository, DictionariesRepository>();
builder.Services.AddScoped<DictionariesService>();

builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<MembershipService>();

builder.Services.AddScoped<IMembershipTypeRepository, MembershipTypeRepository>();
builder.Services.AddScoped<MembershipTypeService>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<PaymentService>();

builder.Services.AddScoped<ITrainingRepository, TrainingRepository>();
builder.Services.AddScoped<TrainingService>();

builder.Services.AddScoped<ITrainingReservationRepository, TrainingReservationRepository>();
builder.Services.AddScoped<TrainingReservationService>();

builder.Services.AddScoped<ITrainingTypeRepository, TrainingTypeRepository>();
builder.Services.AddScoped<TrainingTypeService>();

#endregion

builder.Services.AddControllers(); //Регистрирует контроллеры: приложение будет обрабатывать http-запросы через контроллеры
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); //Сканирует контроллеры и их маршруты для генерации OpenAPI-спецификации (aka Swagger).

builder.Services.AddSwaggerGen(); //Добавляет генератор Swagger-документации.
                                  //В dev-режиме она отображается на /swagger в виде визуального интерфейса для API (Swagger UI).

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Использование страницы разработки - подробные страницы ошибок

    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fitness API V1");
        c.RoutePrefix = "swagger";
        // Swagger будет доступен на главной странице.
    }); //Включают UI Swagger по адресу swagger (и UseSwagger, и UseSwaggerUI)
}


app.UseHttpsRedirection();

app.UseAuthorization(); //Подключает JWT-Токены

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers(); //Маршрутизирует (мапит) запросы к API-контроллерам (атрибуты [HttpGet], [Authorize] и т.д.)


// Вызов инициализации предопределенных данных
using (var scope = app.Services.CreateScope()) //Создает окружение для работы вне контроллеров
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<FitnessDb>();
    await context.Database.MigrateAsync();
}


//Статические файлы
app.UseStaticFiles();

app.Lifetime.ApplicationStarted.Register(() =>
{
    Task.Run(async () =>
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<FitnessDb>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await DbInitializer.Initialize(context, userManager, roleManager);
        }
    });
});

app.Run();