using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors(p => p.WithOrigins("https://localhost:4200").AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager,roleManager);
}
catch(Exception ex) {
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError("Ошибка при создание миграции");
}

app.Run();
