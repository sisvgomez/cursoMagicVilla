using MagicVilla_API;
using MagicVilla_API.Datos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(); // se agrego con las librerias nuggets
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// se agrego para la comunicaci�n con la BD
builder.Services.AddDbContext<AplicationDbcontext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"));

});

builder.Services.AddAutoMapper(typeof(MappingConfig)); // se agrego para el tema de  convertir modelo a objetos

var app = builder.Build();

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
