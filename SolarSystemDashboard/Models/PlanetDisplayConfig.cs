using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystemDashboard.Models;

public class PlanetDisplayConfig
{
    public string TexturePath { get; set; }
    public double PlanetSize { get; set; }
    public double RotationSpeed { get; set; }
    public bool ShowRings { get; set; }
}

public static class PlanetConfigurations
{
    // Échelles relatives basées sur les rayons réels
    // Terre = 100 comme référence
    private static readonly Dictionary<string, PlanetDisplayConfig> _configs = new()
    {
        ["Sun"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_sun.jpg",
            PlanetSize = 1000, // Le soleil est énorme
            RotationSpeed = 0.5,
            ShowRings = false
        },
        ["Mercury"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_mercury.jpg",
            PlanetSize = 38, // 0.38 fois la Terre
            RotationSpeed = 0.1,
            ShowRings = false
        },
        ["Venus"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_venus_surface.jpg",
            PlanetSize = 95, // 0.95 fois la Terre
            RotationSpeed = 0.05,
            ShowRings = false
        },
        ["Earth"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_earth_daymap.jpg",
            PlanetSize = 100, // Référence
            RotationSpeed = 1.0,
            ShowRings = false
        },
        ["Mars"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_mars.jpg",
            PlanetSize = 53, // 0.53 fois la Terre
            RotationSpeed = 1.5,
            ShowRings = false
        },
        ["Jupiter"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_jupiter.jpg",
            PlanetSize = 1120, // 11.2 fois la Terre
            RotationSpeed = 2.0,
            ShowRings = false
        },
        ["Saturn"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_saturn.jpg",
            PlanetSize = 945, // 9.45 fois la Terre
            RotationSpeed = 0.8,
            ShowRings = true
        },
        ["Uranus"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_uranus.jpg",
            PlanetSize = 400, // 4.0 fois la Terre
            RotationSpeed = 1.2,
            ShowRings = true
        },
        ["Neptune"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_neptune.jpg",
            PlanetSize = 388, // 3.88 fois la Terre
            RotationSpeed = 1.3,
            ShowRings = false
        },
        ["Moon"] = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_moon.jpg",
            PlanetSize = 27, // 0.27 fois la Terre
            RotationSpeed = 0.3,
            ShowRings = false
        }
    };

    public static PlanetDisplayConfig GetConfig(string bodyName)
    {
        if (_configs.TryGetValue(bodyName, out var config))
        {
            return config;
        }

        return new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_earth_daymap.jpg",
            PlanetSize = 50,
            RotationSpeed = 1.0,
            ShowRings = false
        };
    }

    public static double GetDisplaySize(string bodyName, double maxSize = 250)
    {
        var config = GetConfig(bodyName);

        if (config.PlanetSize > maxSize)
        {
            return maxSize;
        }

        if (config.PlanetSize < 50)
        {
            return config.PlanetSize * 2;
        }

        return config.PlanetSize;
    }
}
