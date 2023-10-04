using API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors(option => {
    option.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
