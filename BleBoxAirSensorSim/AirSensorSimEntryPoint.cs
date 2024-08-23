using BleBoxAirSensorSim.Services;
using BleBoxCommonSimLib;
using BleBoxCommonSimLib.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCommonSim();
builder.Services.AddControllers();
builder.Services.AddSingleton<IDeviceStateService, DeviceStateService>();
builder.Services.AddSingleton<IAirSensorSettingsService, AirSensorSettingsService>();

var app = builder.Build();
app.MapControllers();

var deviceService = app.Services.GetRequiredService<IDeviceInformationService>();
deviceService.InitializeDevice("AirSensor", "airSensor", "20200831", "airSensor");

var settingsService = app.Services.GetRequiredService<ISettingsService>();
var airSensorSettingsService = app.Services.GetRequiredService<IAirSensorSettingsService>();

settingsService.ObtainFullSettings = airSensorSettingsService.CompleteFullSettings;
settingsService.UpdateFullSettings = airSensorSettingsService.UpdateFullSettings;

app.Run();