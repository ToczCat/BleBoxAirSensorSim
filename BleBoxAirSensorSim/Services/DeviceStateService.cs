using BleBoxModels.AirSensor.Enums;
using BleBoxModels.AirSensor.Models;

namespace BleBoxAirSensorSim.Services;

public interface IDeviceStateService : IDisposable
{
    Air ForceMeasurement();
    Air ReadDeviceState();
    Air ReadExtendedDeviceState();
}

public class DeviceStateService : IDeviceStateService
{
    private static readonly TimeSpan Infinite = Timeout.InfiniteTimeSpan;
    private readonly Timer _timer;
    private readonly TimeSpan _interval;

    private Random _rnd = new();
    private DateTime _lastMeasurementDateTime = DateTime.Now;
    private int _lastPm1Measurement = 0;
    private int _lastPm25Measurement = 0;
    private int _lastPm10Measurement = 0;
    private int _currentPm1Measurement = 0;
    private int _currentPm25Measurement = 0;
    private int _currentPm10Measurement = 0;

    public DeviceStateService()
    {
        _timer = new Timer(delegate { TimerElapsed(); });
        _interval = TimeSpan.FromSeconds(60);
        TimerElapsed();
        TimerStart();
    }

    public void TimerElapsed()
    {
        try
        {
            _lastMeasurementDateTime = DateTime.Now;
            _lastPm1Measurement = _currentPm1Measurement;
            _lastPm25Measurement = _currentPm25Measurement;
            _lastPm10Measurement = _currentPm10Measurement;

            _currentPm1Measurement = _rnd.Next(0, 200);
            _currentPm25Measurement = _rnd.Next(0, 200);
            _currentPm10Measurement = _rnd.Next(0, 200);
        }
        finally
        {
            TimerStart();
        }
    }

    public Air ReadDeviceState()
    {
        var state = new Air
        {
            AirQualityLevel = DetermineOverallQuality(_currentPm25Measurement, _currentPm10Measurement),
            Sensors = new Sensor[]
                {
                    new Sensor
                    {
                        Type = "pm1",
                        Value = _currentPm1Measurement,
                        QualityLevel = QualityLevel.NoScale,
                        Trend = DetermineTrend(_lastPm1Measurement, _currentPm1Measurement),
                        State = State.ActiveMode,
                        ElapsedTimeS = (int)(_lastMeasurementDateTime - DateTime.Now).Negate().TotalSeconds
                    },
                    new Sensor
                    {
                        Type = "pm2.5",
                        Value = _currentPm25Measurement,
                        QualityLevel = DeterminePm25Quality(_currentPm25Measurement),
                        Trend = DetermineTrend(_lastPm25Measurement, _currentPm25Measurement),
                        State = State.ActiveMode,
                        ElapsedTimeS = (int)(_lastMeasurementDateTime - DateTime.Now).Negate().TotalSeconds
                    },
                    new Sensor
                    {
                        Type = "pm10",
                        Value = _currentPm10Measurement,
                        QualityLevel = DeterminePm10Quality(_currentPm10Measurement),
                        Trend = DetermineTrend(_lastPm10Measurement, _currentPm10Measurement),
                        State = State.ActiveMode,
                        ElapsedTimeS = (int)(_lastMeasurementDateTime - DateTime.Now).Negate().TotalSeconds
                    }
                }
        };

        return state;
    }

    public Air ReadExtendedDeviceState() => ReadDeviceState();

    public Air ForceMeasurement()
    {
        TimerElapsed();
        return ReadDeviceState();
    }

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

    private void TimerStart()
    {
        _timer.Change(_interval, Infinite);
    }

    private void TimerStop()
    {
        _timer.Change(Infinite, Infinite);
    }

    public void Dispose()
    {
        TimerStop();
    }
}
