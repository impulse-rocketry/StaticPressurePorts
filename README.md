# Nose Cone Generator

Utility to calculate the size of static pressure ports for a given volume and number of ports.

# Usage

This console mode utility accepts a JSON formatted parameters file and produces the resulting port size output to the console.

``
StaticPressurePorts -p=paramtersFile
``

| Argument | Description |
| --- | --- |
| `parametersFile` | JSON formatted parameters file |

# Parameters File

The JSON formatted parameters file accepts the following parameters:

| Parameter | Description | Default Value |
| --- | --- | --- |
| NumberOfPorts | The desired number of pressure ports | 3 |
| BodyTubeInsideRadius | The inside radius of the rocket body tube containing the volume of air to vent | 7.5 mm |
| BodyTubeLength | The length of the rocket body tube containing the volume of air to vent | 14 cm |
| LaunchAltitude | The altitude of the launch site | 0 m |
| MaxVelocity | The maximum velocity over the duration of the flight | 100 m/sec |
| Temperature | The ambient temperature at the launch site | 21 c |


## Example

```
{
    "numberOfPorts": 3,
    "bodyTubeLength": {
        "value": 17.0,
        "unit": "mm"
    },
    "bodyTubeInsideRadius": {
        "value": 12.0,
        "unit": "mm"
    },
    "maxVelocity": {
        "value": 148.0,
        "unit": "m/sec"
    }
}
```
