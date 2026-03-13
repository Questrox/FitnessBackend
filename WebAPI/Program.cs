using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CaloriesDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<CaloriesDb>()
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
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
    options.AddPolicy("RequireRoleUser", policy => policy.RequireRole("User"));
});


//Регистрация зависимостей.
//Services - это объект IServiceCollection. IServiceCollection - список ServiceDescriptor,
//который содержит ServiceType (тип - IUserRepository), ImplementationType (реализация - UserRepository),
//LifeTime (время жизни - Scope, Singleton или Transient (каждый раз новый объект)) и Implementation Factory
builder.Services.AddScoped<IUserRepository, UserRepository>(); //Если кто-то попросит IUserRepository, нужно дать ему UserRepository
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IFoodEntryRepository, FoodEntryRepository>();
builder.Services.AddScoped<FoodEntryService>();

builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<FoodService>();

builder.Services.AddScoped<IMealPlanRepository, MealPlanRepository>();
builder.Services.AddScoped<MealPlanService>();

builder.Services.AddScoped<IWaterEntryRepository, WaterEntryRepository>();
builder.Services.AddScoped<WaterEntryService>();

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel API V1");
        c.RoutePrefix = "swagger";
        // Swagger будет доступен на главной странице.
    }); //Включают UI Swagger по адресу swagger (и UseSwagger, и UseSwaggerUI)
}


//app.UseHttpsRedirection();

app.UseAuthorization(); //Подключает JWT-Токены

app.MapControllers(); //Маршрутизирует (мапит) запросы к API-контроллерам (атрибуты [HttpGet], [Authorize] и т.д.)


// Вызов инициализации предопределенных данных
using (var scope = app.Services.CreateScope()) //Создает окружение для работы вне контроллеров
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CaloriesDb>();
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
            var context = services.GetRequiredService<CaloriesDb>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await DbInitializer.Initialize(context, userManager, roleManager);
        }
    });
});

app.Run();