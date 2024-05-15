using Api.Controllers;
using Api.HtmlEncoder;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<SqlController>();
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddControllers().AddControllersAsServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.MapGrpcService<ChatService>();

app.Map("/",
    () => Results.Extensions.Html(
        "<!DOCTYPE html><html lang=\"de\"><head><meta charset=\"UTF-8\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Server</title><style>body{background-color: black;}.c{font-size: xx-large;font-family: Verdana, Geneva, Tahoma, sans-serif; }.rainbow{animation: rainbow 2.5s linear;animation-iteration-count: infinite;}@keyframes rainbow{100%,0%{color: rgb(255,0,0);}8%{color: rgb(255,127,0);}16%{color: rgb(255,255,0);}25%{color: rgb(127,255,0);} 33%{color: rgb(0,255,0);}41%{color: rgb(0,255,127);}50%{color: rgb(0,255,255);}58%{color: rgb(0,127,255);}66%{color: rgb(0,0,255);}75%{color: rgb(127,0,255);}83%{color: rgb(255,0,255);}91%{color: rgb(255,0,127);}}</style></head><body><h1 class=\"c rainbow\">Server is Online </h1><h2 class=\"c rainbow\"> :) </h2></body></html>"));

app.Run();