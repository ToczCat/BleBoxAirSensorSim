using BleBoxAirSensorSim.Services;
using BleBoxCommonSimLib;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCommonSim();
builder.Services.AddControllers();
builder.Services.AddSingleton<IDeviceStateService, DeviceStateService>();
builder.Services.AddSingleton<IAirSensorSettingsService, AirSensorSettingsService>();

var AllowAllPolicy = "_allowAllPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowAllPolicy,
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                      });
});

var app = builder.Build();
app.MapControllers();
app.UseCors(AllowAllPolicy);

var airSensorSettingsService = app.Services.GetRequiredService<IAirSensorSettingsService>();

app.Services.ConfigureCommonSim(
    "AirSensor",
    "airSensor",
    "20200831",
    airSensorSettingsService.CompleteFullSettings,
    airSensorSettingsService.UpdateFullSettings,
    "airSensor");

app.Run();