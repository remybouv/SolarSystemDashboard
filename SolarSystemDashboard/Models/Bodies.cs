using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SolarSystemDashboard.Models;

public class Bodies
{
    [JsonPropertyName("bodies")]
    public List<Body> BodyList { get; set; }
}

public class AroundPlanet
{
    [JsonPropertyName("planet")]
    public string Planet { get; set; }

    [JsonPropertyName("rel")]
    public string Rel { get; set; }
}

public class Body
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("englishName")]
    public string EnglishName { get; set; }

    [JsonPropertyName("isPlanet")]
    public bool IsPlanet { get; set; }

    [JsonPropertyName("moons")]
    public object Moons { get; set; }

    [JsonPropertyName("semimajorAxis")]
    public double SemimajorAxis { get; set; }

    [JsonPropertyName("perihelion")]
    public double Perihelion { get; set; }

    [JsonPropertyName("aphelion")]
    public double Aphelion { get; set; }

    [JsonPropertyName("eccentricity")]
    public double Eccentricity { get; set; }

    [JsonPropertyName("inclination")]
    public double Inclination { get; set; }

    [JsonPropertyName("mass")]
    public Mass Mass { get; set; }

    [JsonPropertyName("vol")]
    public Vol Vol { get; set; }

    [JsonPropertyName("density")]
    public double Density { get; set; }

    [JsonPropertyName("gravity")]
    public double Gravity { get; set; }

    [JsonPropertyName("escape")]
    public double Escape { get; set; }

    [JsonPropertyName("meanRadius")]
    public double MeanRadius { get; set; }

    [JsonPropertyName("equaRadius")]
    public double EquaRadius { get; set; }

    [JsonPropertyName("polarRadius")]
    public double PolarRadius { get; set; }

    [JsonPropertyName("flattening")]
    public double Flattening { get; set; }

    [JsonPropertyName("dimension")]
    public string Dimension { get; set; }

    [JsonPropertyName("sideralOrbit")]
    public double SideralOrbit { get; set; }

    [JsonPropertyName("sideralRotation")]
    public double SideralRotation { get; set; }

    [JsonPropertyName("aroundPlanet")]
    public AroundPlanet AroundPlanet { get; set; }

    [JsonPropertyName("discoveredBy")]
    public string DiscoveredBy { get; set; }

    [JsonPropertyName("discoveryDate")]
    public string DiscoveryDate { get; set; }

    [JsonPropertyName("alternativeName")]
    public string AlternativeName { get; set; }

    [JsonPropertyName("axialTilt")]
    public double AxialTilt { get; set; }

    [JsonPropertyName("avgTemp")]
    public int AvgTemp { get; set; }

    [JsonPropertyName("mainAnomaly")]
    public double MainAnomaly { get; set; }

    [JsonPropertyName("argPeriapsis")]
    public double ArgPeriapsis { get; set; }

    [JsonPropertyName("longAscNode")]
    public double LongAscNode { get; set; }

    [JsonPropertyName("bodyType")]
    public string BodyType { get; set; }

    [JsonPropertyName("rel")]
    public string Rel { get; set; }
}

public class Mass
{
    [JsonPropertyName("massValue")]
    public double MassValue { get; set; }

    [JsonPropertyName("massExponent")]
    public int MassExponent { get; set; }
}

public class Vol
{
    [JsonPropertyName("volValue")]
    public double VolValue { get; set; }

    [JsonPropertyName("volExponent")]
    public int VolExponent { get; set; }
}


