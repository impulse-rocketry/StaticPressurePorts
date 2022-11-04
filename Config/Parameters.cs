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

using ImpulseRocketry.Units;

namespace ImpulseRocketry.StaticPressurePorts.Config;

/// <summary>
///
/// </summary>
public class Parameters {
    /// <summary>
    /// The desired number of pressure ports
    /// </summary>
    public int NumberOfPorts { get; init; } = 3;

    /// <summary>
    /// The inside radius of the rocket body tube containing the volume of air to vent
    /// </summary>
    public Length BodyTubeInsideRadius { get; init; } = Length.Mm.Value(7.5);

    /// <summary>
    /// The length of the rocket body tube containing the volume of air to vent
    /// </summary>
    public Length BodyTubeLength { get; init; } = Length.Cm.Value(14);

    /// <summary>
    /// The altitude of the launch site
    /// </summary>
    public Length LaunchAltitude { get; init; } = Length.M.Value(0);

    /// <summary>
    /// The maximum velocity over the duration of the flight
    /// </summary>
    public Velocity MaxVelocity { get; init; } = Velocity.MSec.Value(100);

    /// <summary>
    /// The ambient temperature at the launch site
    /// </summary>
    public Temperature Temperature { get; init; } = Temperature.C.Value(21);
}