using PostInterviewAI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi(); // for Swagger/OpenAPI docs
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ChatGPTService>();
builder.Services.AddSingleton<TranscriptionService>();
// builder.Services.AddSingleton<AWSService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();