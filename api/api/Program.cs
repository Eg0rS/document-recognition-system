using Common;
using Common.Interfaces;

using Kafka.Interfaces;
using Kafka.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfigurationSettings, ConfigurationSettings>();
builder.Services.AddSingleton<IKafkaProducesService, KafkaProducesService>();
builder.Services.AddHostedService<KafkaConsumerService>();

var app = builder.Build();


app.UseHttpsRedirection();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthentication();

app.Run();
