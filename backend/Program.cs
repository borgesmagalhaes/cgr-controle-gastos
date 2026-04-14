using Cgr.Api.Services;
using Cgr.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra controllers e exploracao OpenAPI para facilitar testes locais.
// Em palavras simples: habilita as "portas" da API.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Habilita CORS para desenvolvimento local, sem bloquear por porta/host do Vite.
// Em palavras simples: permite que o front (outra porta) converse com o back.
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registra o DbContext SQLite com Foreign Keys ativas por conexao.
// Em palavras simples: liga a API no arquivo do banco SQLite.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Usa caminho absoluto para evitar erro de path relativo quando a API sobe pela raiz via npm.
    var sqlitePath = Path.GetFullPath(
        Path.Combine(builder.Environment.ContentRootPath, "..", "sqlite", "cgr.db"));

    var connectionString = $"Data Source={sqlitePath};Foreign Keys=True";
    options.UseSqlite(connectionString);
});

// Registra servicos de aplicacao mantendo SRP por caso de uso.
// Em palavras simples: diz para o .NET como criar cada serviço quando um Controller pedir.
builder.Services.AddScoped<PessoaService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<TransacaoService>();
builder.Services.AddScoped<ConsultaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Garante os registros fixos de domínio necessários para FKs de categoria/transação.
// Em palavras simples: cria valores básicos (despesa/receita/ambas) se ainda não existirem.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    dbContext.Database.ExecuteSqlRaw("""
        INSERT OR IGNORE INTO finalidade (id, descricao) VALUES
        (1, 'despesa'),
        (2, 'receita'),
        (3, 'ambas');
        """);

    dbContext.Database.ExecuteSqlRaw("""
        INSERT OR IGNORE INTO tipo_transacao (id, descricao) VALUES
        (1, 'despesa'),
        (2, 'receita');
        """);
}

app.UseCors("frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
