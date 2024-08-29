using BleBoxAirSensorSim.Services;
using BleBoxCommonSimLib;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCommonSim();
builder.Services.AddControllers();
builder.Services.AddSingleton<IDeviceStateService, DeviceStateService>();
builder.Services.AddSingleton<IAirSensorSettingsService, AirSensorSettingsService>();

var app = builder.Build();
app.MapControllers();

var airSensorSettingsService = app.Services.GetRequiredService<IAirSensorSettingsService>();

app.Services.ConfigureCommonSim(
    "AirSensor",
    "airSensor",
    "20200831",
    airSensorSettingsService.CompleteFullSettings,
    airSensorSettingsService.UpdateFullSettings,
    "airSensor");

app.Run();