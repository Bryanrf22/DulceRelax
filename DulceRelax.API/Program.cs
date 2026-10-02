using DulceRelax.API.Repositories;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// --- Servicios (todo esto ANTES de builder.Build()) ---

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

var projectId = builder.Configuration["Firebase:ProjectId"];
var credentialsJson = builder.Configuration["Firebase:CredentialsJson"];

var firestoreDb = new FirestoreDbBuilder
{
    ProjectId = projectId,
    JsonCredentials = credentialsJson
}.Build();
builder.Services.AddSingleton(firestoreDb);

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

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(credentialsJson)
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<MasajeRepository>();
builder.Services.AddSingleton<CitaRepository>();

// --- Build ---

var app = builder.Build();

// --- Pipeline (orden importa) ---

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();