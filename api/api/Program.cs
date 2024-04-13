using api.ApiServices;
using Common;
using Common.Interfaces;
using Database;
using Database.Interfaces;
using Database.Repositories;
using Kafka.Interfaces;
using Kafka.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DataContext>();
builder.Services.AddSingleton<IConfigurationSettings, ConfigurationSettings>();
builder.Services.AddSingleton<IKafkaProducesService, KafkaProducesService>();
builder.Services.AddSingleton<KafkaEventHandler>();
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<FileService>();
builder.Services.AddHostedService<KafkaConsumerService>();

var app = builder.Build();


app.UseHttpsRedirection();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthentication();

app.Run();
