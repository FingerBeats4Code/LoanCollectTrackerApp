using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LoanDb>(opt =>
    opt.UseInMemoryDatabase("LoanDb"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/agents", async (LoanDb db, Agent agent) =>
{
    db.Agents.Add(agent);
    await db.SaveChangesAsync();
    return Results.Created($"/api/agents/{agent.Id}", agent);
});

app.MapGet("/api/agents", async (LoanDb db) => await db.Agents.ToListAsync());

app.MapPost("/api/loans", async (LoanDb db, Loan loan) =>
{
    db.Loans.Add(loan);
    await db.SaveChangesAsync();
    return Results.Created($"/api/loans/{loan.Id}", loan);
});

app.MapGet("/api/loans/overdue", async (LoanDb db) =>
    await db.Loans.Where(l => l.DueDate < DateTime.UtcNow && l.IsCollected == false).ToListAsync());

app.Run();

class LoanDb : DbContext
{
    public LoanDb(DbContextOptions<LoanDb> options) : base(options) { }
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Loan> Loans => Set<Loan>();
}

class Agent
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Region { get; set; }
}

class Loan
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCollected { get; set; }
    public int AgentId { get; set; }
}
