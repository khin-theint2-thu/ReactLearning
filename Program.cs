using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder => builder.WithOrigins("http://localhost:5295", "http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()); // Specify AllowCredentials if needed
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = long.MaxValue;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MinRequestBodyDataRate = null;
    options.Limits.MinResponseDataRate = null;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting(); // UseRouting should be before UseCors and UseAuthorization

app.UseCors("AllowAnyOrigin"); // CORS middleware should be placed here

app.UseAuthorization();
app.UseStaticFiles();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
