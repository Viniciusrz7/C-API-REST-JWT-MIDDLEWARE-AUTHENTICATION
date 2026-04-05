using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Livraria.Services;
using apirest.repository;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using SistemaLivros.Models;
using apirest.Data;
using Microsoft.EntityFrameworkCore;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); // just spacing

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// EF Core SQLite setup
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=banco.db"));

// Configuração Swagger
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "API REST Livros", 
        Version = "v1",
        Description = "API REST Livros",
        Contact = new OpenApiContact {
            Name = "Vinicius",
            Email = "vinirz17@gmail.com",
            Url = new Uri("https://github.com/Viniciusrz7/")
        }
    });

    c.EnableAnnotations(); // Habilitar anotações avançadas idênticas ao Spring Boot

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = "Cole apenas o seu token JWT aqui (sem 'Bearer ')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Registro das Dependências
builder.Services.AddScoped<LivroRepository>();
builder.Services.AddScoped<LivroService>();

// JWT
var key = Encoding.ASCII.GetBytes("SuaChaveSuperSecretaComMuitosCaracteres");
builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Executa e cria o banco/tabelas automaticamente na inicialização. Equivalent a spring.jpa.hibernate.ddl-auto = update
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // dbContext.Database.EnsureDeleted(); // Descomente caso queria dropar o banco a cada run
    dbContext.Database.EnsureCreated();
}

// Configuração do Pipeline HTTP
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();