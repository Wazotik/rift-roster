var builder = WebApplication.CreateBuilder(args);

var riotApiKey = builder.Configuration["RiotApiKey"];
var databaseUrl = builder.Configuration["DatabaseConnectionString"];

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: allow your React dev server to call the API from a different origin
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("frontend", p =>
        p.WithOrigins("http://localhost:5173") 
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();



// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Endpoints
app.MapGet("/", () => "Server is up!");

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    timeUtc = DateTime.UtcNow
}));

// Get account (using tagline and game name) 
app.MapGet("/account/{gameName}/{tagLine}", async (string gameName, string tagLine) =>
{
    using var http = new HttpClient();

    Console.WriteLine(riotApiKey);
    http.DefaultRequestHeaders.Add("X-Riot-Token", riotApiKey);

    var url = $"https://americas.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{Uri.EscapeDataString(gameName)}/{Uri.EscapeDataString(tagLine)}";

    var res = await http.GetAsync(url);
    var body = await res.Content.ReadAsStringAsync();

    Console.WriteLine(body);

    return Results.Text(body, "applicatoin/json");

});

// Get all players
app.MapGet("/players", () =>
{



});




app.Run();



