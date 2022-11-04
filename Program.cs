// Copyright 2022 Ben Voß
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
// (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.CommandLine;
using System.Text.Json;
using ImpulseRocketry.StaticPressurePorts.Config;
using ImpulseRocketry.Units;
using ImpulseRocketry.Units.Json;

namespace ImpulseRocketry.StaticPressurePorts;

/// <summary>
/// </summary>
public class Program {

    public static int Main(string[] args) {
        var parametersOption = new Option<string>("-p", "Parameters file") {
            IsRequired = true
        };

        var cmd = new RootCommand();
        cmd.SetHandler((parametersFileName) => Run(parametersFileName), parametersOption);
        cmd.AddOption(parametersOption);
        
        return cmd.Invoke(args);
    }

    private static int Run(string file) {
        if (!File.Exists(file)) {
            Console.Error.WriteLine("File not found");
            return 1;
        }

        using var stream = File.OpenRead(file);

        var options = new JsonSerializerOptions {
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };
        options.Converters.Add(new UnitJsonConverterFactory());
        var parameters = JsonSerializer.Deserialize<Parameters>(stream, options);

        if (parameters is null) {
            Console.Error.WriteLine("Empty file");
            return 1;
        }

        parameters = new Parameters {
            NumberOfPorts = 3,
            BodyTubeLength = Length.Cm.Value(17),
            BodyTubeInsideRadius = Length.Mm.Value(12),
            MaxVelocity = Velocity.MSec.Value(184)
        };

        const double DischargeCoefficient = 0.62;
        const double AirSpecificHeatRatio = 1.401;
        const double OneAtmosphere = 101325;    // Pa
        const double LapseRate = -0.0065; // K/m^-1
        const double g = 9.80665; // m/s^2
        const double OneMoleAir = 0.0289645; // kg/mol^-1
        const double GasConstant = 8.31432;

        var temperatureKelvin = parameters.Temperature.To.K;
        var launchAltitude = parameters.LaunchAltitude.To.M;
        var altimeterBayRadius = parameters.BodyTubeInsideRadius.To.Cm;
        var altimeterBayLength = parameters.BodyTubeLength.To.Cm;
        var maxVelocity = parameters.MaxVelocity.To.MSec;
        var numberOfPorts = parameters.NumberOfPorts;

        // Calculate the volume of air in the altimiter bay - by modelling it as a cylinder and assuming there is nothing else taking up any space in the cylinder
        var altimeterBayVolume = Math.PI * Math.Pow(altimeterBayRadius, 2) * altimeterBayLength;    // cm ^ 2

        // Air pressure and density at the launch altitude
        var pressureLaunchAltitude = OneAtmosphere * Math.Pow(temperatureKelvin / (temperatureKelvin + LapseRate * launchAltitude), g * OneMoleAir / (GasConstant * LapseRate));
        var airDensity = pressureLaunchAltitude * OneMoleAir / (GasConstant * temperatureKelvin);

        // The mass of the air in the altimiter bay at the launch altitude
        var airMass = airDensity * Math.Pow(1/100d, 3) * altimeterBayVolume;

        // Rate that the atmospheric pressure will change
        var atmosphericPressureChange = (pressureLaunchAltitude - OneAtmosphere * Math.Pow(temperatureKelvin / (temperatureKelvin + LapseRate * (launchAltitude + 1)), g * OneMoleAir / (GasConstant * LapseRate))) / pressureLaunchAltitude;

        // Rate that the air will change at maximum velocity - assumes the worst case that we are always travelling
        // at maximum velocity
        var pressureEqualizationRate = 1 / maxVelocity;

        // Rate that the air needs to vent
        var massFlowRate = airMass * atmosphericPressureChange / pressureEqualizationRate;
        
        var portSize = Math.Sqrt(4 * massFlowRate / (
                Math.PI * numberOfPorts * DischargeCoefficient * Math.Sqrt(
                    2 * airDensity * pressureLaunchAltitude * (AirSpecificHeatRatio / (AirSpecificHeatRatio - 1)) *
                    (
                        Math.Pow(1 - atmosphericPressureChange, 2 / AirSpecificHeatRatio)
                        -
                        Math.Pow(1 - atmosphericPressureChange, (AirSpecificHeatRatio + 1) / AirSpecificHeatRatio)
                    )
                )
            )
        );

        // Convert from m to mm
        portSize *= 1000;

        // Round up to 1 decimal place
        portSize = Math.Round(portSize, 1, MidpointRounding.ToPositiveInfinity);

        Console.WriteLine(portSize);
        return 0;
    }
}