using BleBoxModels.AirSensor.Enums;
using BleBoxModels.AirSensor.Models;
using BleBoxModels.Common.Enums;
using BleBoxModels.Common.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BleBoxAirSensorSim.Services;

public interface IAirSensorSettingsService
{
    object CompleteFullSettings(SettingsBase settingsBase);
    SettingsBase UpdateFullSettings(JsonObject fullSettings);
}

public class AirSensorSettingsService : IAirSensorSettingsService
{
    private Geolocation _geolocation = Geolocation.Accurate;
    private Mounting _mounting = Mounting.Outside;
    private Toggle _view = Toggle.Enabled;

    public object CompleteFullSettings(SettingsBase settingsBase)
    {
        var fullSettings = new Settings
        {
            DeviceName = settingsBase.DeviceName,
            Tunnel = settingsBase.Tunnel,
            StatusLed = settingsBase.StatusLed,
            SensorApi  = new SensorApi
            {
                MakeGeolocationCoarse = _geolocation
            },
            Air = new AirSettings
            {
                MountingPlace = _mounting,
                DetailedView = _view
            }
        };

        return fullSettings;
    }

    public SettingsBase UpdateFullSettings(JsonObject fullSettings)
    {
        var deserializedSettings = JsonSerializer.Deserialize<SettingsRequest>(fullSettings, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        deserializedSettings ??= new SettingsRequest { Settings = new Settings() };
        deserializedSettings.Settings ??= new Settings();

        _geolocation = deserializedSettings.Settings.SensorApi?.MakeGeolocationCoarse ?? Geolocation.Accurate;
        _mounting = deserializedSettings.Settings.Air?.MountingPlace ?? Mounting.Outside;
        _view = deserializedSettings.Settings.Air?.DetailedView ?? Toggle.Enabled;

        return deserializedSettings.Settings;
    }

    public record SettingsRequest
    {
        public Settings? Settings { get; set; }
    }
}
