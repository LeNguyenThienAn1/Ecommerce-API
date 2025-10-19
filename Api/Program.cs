using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Application.EntityHandler.Services;
using Application.EntityHandler.Queries.Interface;
using Application.EntityHandler.Queries;
using Application.Services;
using EntityHandler.Services.Interface;
using EntityHandler.Services;
using EntityHandler.Queries.Interface;
using EntityHandler.Queries;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Hubs;
using Application.Interfaces;
using Infrastructure.Services;
using Application.EntityHandler.Services.Implementations;
using Infrastructure.Queries;
using Application.Interfaces.Services;
using Application.Interfaces.Queries;
using Application.Queries;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ======================= Khởi tạo WebApplication =======================
var builder = WebApplication.CreateBuilder(args);

// ======================= SignalR =======================
builder.Services.AddSignalR();

// ======================= DbContext =======================
builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================= Đăng ký toàn bộ service trong Application =======================
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient<IMomoService, MomoService>();

builder.Services.AddLogging();
builder.Services.AddScoped<IChatQueries, ChatQueries>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddHttpClient<IChatService, ChatService>();
builder.Services.AddScoped<IChatQueries, ChatQueries>();


builder.Services.AddScoped<IUserQueries, UserQueries>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IProductQueries, ProductQueries>();
builder.Services.AddScoped<IProductService, ProductService>();

// HttpClient cho ChatService
builder.Services.AddHttpClient<IChatService, ChatService>();
builder.Services.AddScoped<IChatMessageQueries, ChatMessageQueries>();
builder.Services.AddScoped<IRealTimeChatService, RealTimeChatService>();

// ======================= Đăng ký Services bổ sung (nếu chưa có trong AddApplicationServices) =======================
builder.Services.AddScoped<IFeaturedProductService, FeaturedProductService>();
builder.Services.AddScoped<IFeaturedProductQueries, FeaturedProductQueries>();

builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IWishlistQueries, WishlistQueries>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IOrderQueries, OrderQueries>();
builder.Services.AddScoped<IProductQueries, ProductQueries>(); // nếu có ProductQueries

// ======================= ✅ Thêm mới: Category & Brand =======================

// Category
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Brand
builder.Services.AddScoped<IBrandQueries, BrandQueries>();
builder.Services.AddScoped<IBrandService, BrandService>();

// ======================= Real-Time Chat =======================
builder.Services.AddScoped<IRealTimeChatService, RealTimeChatService>();
builder.Services.AddScoped<IChatMessageQueries, ChatMessageQueries>();

// ======================= Controllers =======================
builder.Services.AddControllers();

// ======================= Swagger =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================= CORS =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // FE URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // ✅ Cho phép gửi credentials (cookies, auth headers)
    });
});

builder.Services.AddAuthentication("Bearer")
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });



var app = builder.Build();

// ======================= Data Seeding =======================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EcommerceDbContext>();
        await DataSeeder.SeedAdminUserAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during data seeding.");
    }
}




// ======================= Middleware =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Bật CORS ngay trước Authorization
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// ======================= SignalR Hub Endpoint =======================
app.MapHub<ChatHub>("/chathub");

app.Run();
