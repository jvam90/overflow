var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.AddServiceDefaults(); // para poder usar os métodos de extensão
builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer("keycloak", "overflow", options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = "overflow";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapDefaultEndpoints(); //mapeia endpoints dos serviços default, como health, alive, etc.

app.Run();