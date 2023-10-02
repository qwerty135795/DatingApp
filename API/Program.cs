using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Default"))); 

builder.Services.AddControllers();
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors(option => {
    option.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
});
app.UseAuthorization();

app.MapControllers();

app.Run();
