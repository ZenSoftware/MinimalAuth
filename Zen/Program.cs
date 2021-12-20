const string ALLOWED_ORIGINS_POLICY = "ZenOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: ALLOWED_ORIGINS_POLICY,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:4200",
                                              "http://localhost:7080")
                                                .AllowAnyHeader()
                                                .AllowAnyMethod();
                      });
});
builder.Services.AddSingleton<IJwtManager, JwtManager>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();
app.UseCors(ALLOWED_ORIGINS_POLICY);
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Sample JWT Bearer authentication");
app.MapGet("/doaction", [Authorize(Roles = "Super")] () => new { message = "Protected action succeeded" });
app.MapGet("/token", (IJwtManager jwtManager) => new { token = jwtManager.GetToken() });

app.Run();
