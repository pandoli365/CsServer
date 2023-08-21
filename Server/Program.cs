using Server.System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

ProtocolProcessor.init();
app.MapPost("/", ProtocolProcessor.Process);

app.Run();
