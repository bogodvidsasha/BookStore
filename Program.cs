using BookStore.Repositories;
using BookStore.Services;
using FluentMigrator.Runner;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Додавання логування у консоль для демонстрації роботи кешування
builder.Services.AddLogging(c => c.AddConsole());

// Реєстрація репозиторію та сервісу
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<BookService>();

// Додавання LazyCache
builder.Services.AddLazyCache();

// Налаштування FluentMigrator
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

var app = builder.Build();

// Запуск міграцій при старті додатка
using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();
    // Запуск всіх міграцій, які ще не були застосовані
    migrator.MigrateUp();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();