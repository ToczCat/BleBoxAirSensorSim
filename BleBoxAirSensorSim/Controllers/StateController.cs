using BleBoxAirSensorSim.Services;
using BleBoxCommonSimLib.Services;
using Microsoft.AspNetCore.Mvc;

namespace BleBoxAirSensorSim.Controllers;

public class StateController(IDeviceStateService deviceState) : ControllerBase
{
    [HttpGet("state")]
    public IActionResult DeviceStateRequested()
    {
        try
        {
            return Ok(new { Air = deviceState.ReadDeviceState() });
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("state/extended")]
    public IActionResult DeviceExtendedStateRequested()
    {
        try
        {
            return Ok(new { Air = deviceState.ReadExtendedDeviceState() });
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("api/air/runtime")]
    public IActionResult DeviceRuntimeRequested(IDeviceInformationService deviceInformation)
    {
        try
        {
            var deviceUptime = deviceInformation.ReadUptime();

            return Ok(new { UpTimeS = ((int)deviceUptime.TotalHours).ToString() });
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("s/kick")]
    public IActionResult ForceMeasurement()
    {
        try
        {
            return Ok(new { Air = deviceState.ForceMeasurement() });
        }
        catch
        {
            return BadRequest();
        }
    }
}
