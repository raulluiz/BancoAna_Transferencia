using System.Reflection;
using Microsoft.Data.Sqlite;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// ============================
// Configurações
// ============================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=transferencia.db";

// ============================
// Serviços
// ============================

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "BancoAna - API Transferência", Version = "v1" });
});

// MediatR (escaneia a camada Application)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.Load("Application.Transferencia")));

// SQLite (registrar para uso com Dapper ou EF)
builder.Services.AddSingleton(new SqliteConnection(connectionString));

// Autenticação JWT (básico, ainda sem validação)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Jwt:Authority"];
        options.TokenValidationParameters.ValidateAudience = false;
    });

// ============================
// App
// ============================
var app = builder.Build();

// Executar script SQL de inicialização (opcional)
EnsureDatabaseCreated(connectionString);

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


// ============================
// Método auxiliar
// ============================
void EnsureDatabaseCreated(string connString)
{
    var sqlFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "Scripts", "transferencia.sql");

    if (!File.Exists(sqlFilePath))
    {
        Console.WriteLine($"⚠️ Script SQL não encontrado: {sqlFilePath}");
        return;
    }

    var script = File.ReadAllText(sqlFilePath);

    using var connection = new SqliteConnection(connString);
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = script;
    command.ExecuteNonQuery();

    Console.WriteLine("✅ Script transferencia.sql executado com sucesso.");
}
