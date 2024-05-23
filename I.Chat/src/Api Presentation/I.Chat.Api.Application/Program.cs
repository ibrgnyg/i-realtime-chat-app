using I.Chat.Api;
using I.Chat.Core;

var builder = Engine.CreateAsBuilder(args);

Engine.ConfigurationFiles(builder.Configuration);

builder.Services.SetGraphQLInitialize(builder.Configuration);

Engine.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Engine.ConfigureRequestPipeline(app, builder.Environment);

app.BuildRun();