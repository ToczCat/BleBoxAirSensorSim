using BleBoxModels.AirSensor.Enums;
using BleBoxModels.AirSensor.Models;

namespace BleBoxAirSensorSim.Services;

public interface IDeviceStateService
{
    Air ForceMeasurement();
    Air ReadDeviceState();
    Air ReadExtendedDeviceState();
}

public class DeviceStateService : IDeviceStateService
{
    private Random _rnd = new();
    private DateTime _lastMeasurementDateTime = DateTime.Now;
    private int _lastPm1Measurement = 0;
    private int _lastPm25Measurement = 0;
    private int _lastPm10Measurement = 0;

    public Air ReadDeviceState()
    {
        var measurementTime = DateTime.Now;
        var currentPm1Measurement = _rnd.Next(0, 200);
        var currentPm25Measurement = _rnd.Next(0, 200);
        var currentPm10Measurement = _rnd.Next(0, 200);

        var state = new Air
        {
            AirQualityLevel = DetermineOverallQuality(currentPm25Measurement, currentPm10Measurement),
            Sensors = new Sensor[]
                {
                new Sensor
                {
                    Type = "pm1",
                    Value = currentPm1Measurement,
                    QualityLevel = QualityLevel.NoScale,
                    Trend = DetermineTrend(_lastPm1Measurement, currentPm1Measurement),
                    State = State.ActiveMode,
                    ElapsedTimeS = (int)(_lastMeasurementDateTime - measurementTime).Negate().TotalSeconds
                },
                new Sensor
                {
                    Type = "pm2.5",
                    Value = currentPm25Measurement,
                    QualityLevel = DeterminePm25Quality(currentPm25Measurement),
                    Trend = DetermineTrend(_lastPm25Measurement, currentPm25Measurement),
                    State = State.ActiveMode,
                    ElapsedTimeS = (int)(_lastMeasurementDateTime - measurementTime).Negate().TotalSeconds
                },
                new Sensor
                {
                    Type = "pm10",
                    Value = currentPm10Measurement,
                    QualityLevel = DeterminePm10Quality(currentPm10Measurement),
                    Trend = DetermineTrend(_lastPm10Measurement, currentPm10Measurement),
                    State = State.ActiveMode,
                    ElapsedTimeS = (int)(_lastMeasurementDateTime - measurementTime).Negate().TotalSeconds
                }
                }
        };

        _lastMeasurementDateTime = measurementTime;
        _lastPm1Measurement = currentPm1Measurement;
        _lastPm25Measurement = currentPm25Measurement;
        _lastPm10Measurement = currentPm10Measurement;

        return state;
    }

    public Air ReadExtendedDeviceState() => ReadDeviceState();

    public Air ForceMeasurement() => ReadDeviceState();

    private QualityLevel? DetermineOverallQuality(int pm25, int pm10)
    {
        QualityLevel? qualityLevel = true switch
        {
            true when (pm10 < 20) && (pm25 < 13) => QualityLevel.VeryGood,
            true when (pm10 < 50) && (pm25 < 35) => QualityLevel.Good,
            true when (pm10 < 80) && (pm25 < 55) => QualityLevel.Moderate,
            true when (pm10 < 110) && (pm25 < 75) => QualityLevel.Sufficient,
            true when (pm10 < 150) && (pm25 < 110) => QualityLevel.Bad,
            true when (pm10 >= 150) || (pm25 >= 110) => QualityLevel.VeryBad,
            _ => null
        };

        return qualityLevel;
    }

    private QualityLevel? DeterminePm25Quality(int airQuality)
    {
        QualityLevel? qualityLevel = true switch
        {
            true when airQuality < 13 => QualityLevel.VeryGood,
            true when airQuality < 35 => QualityLevel.Good,
            true when airQuality < 55 => QualityLevel.Moderate,
            true when airQuality < 75 => QualityLevel.Sufficient,
            true when airQuality < 110 => QualityLevel.Bad,
            true when airQuality >= 110 => QualityLevel.VeryBad,
            _ => null
        };

        return qualityLevel;
    }

    private QualityLevel? DeterminePm10Quality(int airQuality)
    {
        QualityLevel? qualityLevel = true switch
        {
            true when airQuality < 20 => QualityLevel.VeryGood,
            true when airQuality < 50 => QualityLevel.Good,
            true when airQuality < 80 => QualityLevel.Moderate,
            true when airQuality < 110 => QualityLevel.Sufficient,
            true when airQuality < 150 => QualityLevel.Bad,
            true when airQuality >= 150 => QualityLevel.VeryBad,
            _ => null
        };

        return qualityLevel;
    }

    private static Trend DetermineTrend(int last, int current)
    {
        try
        {
            if (last == current)
                return Trend.Sidewave;

            return last < current ? Trend.Upward : Trend.Downward;
        }
        catch
        {
            return Trend.NoData;
        }
    }
}
