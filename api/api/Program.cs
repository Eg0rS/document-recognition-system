using System.Reflection;
using api.ApiServices;
using Common;
using Common.Interfaces;
using Database;
using Database.Interfaces;
using Kafka.Interfaces;
using Kafka.Services;
using FluentMigrator.Runner;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnection, Connection>();
builder.Services.AddSingleton<IConfigurationSettings, ConfigurationSettings>();
builder.Services.AddSingleton<IKafkaProducesService, KafkaProducesService>();
builder.Services.AddSingleton<KafkaEventHandler>();

builder.Services.AddScoped<FileService>();
builder.Services.AddHostedService<KafkaConsumerService>();

var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb.AddPostgres().WithGlobalConnectionString(connectionString).ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
    .AddLogging(rb => rb.AddFluentMigratorConsole());


var app = builder.Build();
var serviceProvider = app.Services.CreateScope().ServiceProvider;

var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
//runner.MigrateUp();

app.UseHttpsRedirection();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthentication();

app.Run();
