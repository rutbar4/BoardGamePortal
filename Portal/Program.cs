using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Portal;
using Portal.DBMethods;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddSingleton<BoardGameDBOperations>(new BoardGameDBOperations(new UserDBOperations()));
builder.Services.AddSingleton<OrganisationDBOperations>(new OrganisationDBOperations());
builder.Services.AddSingleton<UserDBOperations>(new UserDBOperations());
builder.Services.AddSingleton<BoardGamePlayDBOperations>(new BoardGamePlayDBOperations());
builder.Services.AddSingleton<TournamentDBO>(new TournamentDBO());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("https://localhost:44351", "http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.UseAuthorization();

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/profile"), appBuilder =>
{
    appBuilder.UseMiddleware<JwtMiddleware>();
}); 

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/BoardGamePlay"), appBuilder =>
{
    appBuilder.UseMiddleware<JwtMiddleware>();
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Organisation"), appBuilder =>
{
    appBuilder.UseMiddleware<JwtMiddleware>();
});

app.MapControllers();
app.Run();
