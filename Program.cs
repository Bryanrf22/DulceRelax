using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DulceRelax.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Firebase
var credentialsJson = builder.Configuration["Firebase:CredentialsJson"];
var projectId = builder.Configuration["Firebase:ProjectId"];

// Firestore
var firestoreDb = new FirestoreDbBuilder
{
    ProjectId = projectId,
    JsonCredentials = credentialsJson
}.Build();
builder.Services.AddSingleton(firestoreDb);

// FirebaseAdmin (para operaciones administrativas: crear usuarios, custom claims)
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(credentialsJson)
});

// Autenticación con Firebase Auth (JWT Bearer)
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

// Repositorios
builder.Services.AddScoped<UsuarioRepository>();

// Controllers, Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();