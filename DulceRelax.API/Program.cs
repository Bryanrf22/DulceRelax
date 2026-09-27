using DulceRelax.API.Repositories;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Program.cs
var credPath = builder.Configuration["Firebase:CredentialsPath"];
var client = new FirestoreDbBuilder
{
    ProjectId = builder.Configuration["Firebase:ProjectId"],
    JsonCredentials = File.ReadAllText(credPath)
}.Build();
builder.Services.AddSingleton(client);


var projectId = builder.Configuration["Firebase:ProjectId"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidAudience = projectId,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

app.UseAuthentication();
app.UseAuthorization();


builder.Services.AddScoped<UsuarioRepository>(); 


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
