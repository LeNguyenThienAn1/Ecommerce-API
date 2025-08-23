using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Application; // 👈 để gọi AddApplicationServices

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký toàn bộ service trong Application
builder.Services.AddApplicationServices();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
