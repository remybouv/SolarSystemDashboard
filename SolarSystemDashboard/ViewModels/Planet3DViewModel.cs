using Avalonia;
using Avalonia.Threading;
using HelixToolkit.Avalonia.SharpDX;
using HelixToolkit.Geometry;
using HelixToolkit.Maths;
using HelixToolkit.SharpDX;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace SolarSystemDashboard.ViewModels;

public class Planet3DViewModel : INotifyPropertyChanged, IDisposable
{
    private DispatcherTimer _animationTimer;
    private float _rotationAngle = 0f;
    private double _rotationSpeed = 0.5;

    public IEffectsManager EffectsManager { get; }
    public Camera Camera { get; }

    private MeshGeometry3D _planetGeometry;
    public MeshGeometry3D PlanetGeometry
    {
        get => _planetGeometry;
        set { _planetGeometry = value; OnPropertyChanged(); }
    }

    private PhongMaterial _planetMaterial;
    public PhongMaterial PlanetMaterial
    {
        get => _planetMaterial;
        set { _planetMaterial = value; OnPropertyChanged(); }
    }

    private MeshGeometry3D _ringGeometry;
    public MeshGeometry3D RingGeometry
    {
        get => _ringGeometry;
        set { _ringGeometry = value; OnPropertyChanged(); }
    }

    private PhongMaterial _ringMaterial;
    public PhongMaterial RingMaterial
    {
        get => _ringMaterial;
        set { _ringMaterial = value; OnPropertyChanged(); }
    }

    private Matrix _planetMatrix;
    public Matrix PlanetMatrix
    {
        get => _planetMatrix;
        set { _planetMatrix = value; OnPropertyChanged(); }
    }

    private bool _showRings;
    public bool ShowRings
    {
        get => _showRings;
        set { _showRings = value; OnPropertyChanged(); }
    }

    private string _backgroundColor = "#1a1a24";
    public string BackgroundColor
    {
        get => _backgroundColor;
        set { _backgroundColor = value; OnPropertyChanged(); }
    }

    public double RotationSpeed
    {
        get => _rotationSpeed;
        set => _rotationSpeed = value;
    }

    public Planet3DViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        Camera = new PerspectiveCamera
        {
            Position = new Vector3(0, 0, 8),
            LookDirection = new Vector3(0, 0, -1),
            UpDirection = new Vector3(0, 1, 0),
            FieldOfView = 45
        };

        CreatePlanetGeometry(2.0f);
        CreatePlanetMaterial(System.Drawing.Color.DodgerBlue);
        CreateRingGeometry();

        PlanetMatrix = Matrix.Identity;

        StartAnimation();
    }

    private void CreatePlanetGeometry(float size)
    {
        var builder = new MeshBuilder(true, true);
        builder.AddSphere(new Vector3(0, 0, 0), size, 60, 60);
        PlanetGeometry = builder.ToMeshGeometry3D();
    }

    private void CreatePlanetMaterial(System.Drawing.Color color)
    {
        PlanetMaterial = new PhongMaterial
        {
            DiffuseColor = new Color4(color.R / 255f, color.G / 255f, color.B / 255f, 1.0f),
            SpecularColor = new Color4(0.8f, 0.8f, 0.8f, 1.0f),
            SpecularShininess = 100f,
            EmissiveColor = new Color4(0.05f, 0.1f, 0.15f, 1.0f)
        };
    }

    private void CreateRingGeometry()
    {
        var builder = new MeshBuilder(true, true);
        builder.AddTorus(2.8f, 0.3f, 80, 20);
        RingGeometry = builder.ToMeshGeometry3D();

        RingMaterial = new PhongMaterial
        {
            DiffuseColor = new Color4(0.8f, 0.7f, 0.5f, 0.4f),
            SpecularColor = new Color4(0.6f, 0.6f, 0.6f, 0.3f),
            SpecularShininess = 50f
        };
    }

    public void UpdatePlanetColor(Avalonia.Media.Color color)
    {
        var sysColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        CreatePlanetMaterial(sysColor);
    }

    public void UpdatePlanetSize(double size)
    {
        CreatePlanetGeometry((float)size);
    }

    private void StartAnimation()
    {
        _animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
        };
        _animationTimer.Tick += (s, e) =>
        {
            _rotationAngle += (float)_rotationSpeed;
            if (_rotationAngle >= 360) _rotationAngle -= 360;

            // Créer les matrices de rotation avec SharpDX
            var rotationY = Matrix.CreateRotation(_rotationAngle * (float)Math.PI / 180f);
            var rotationZ = Matrix.CreateRotation(23.5f * (float)Math.PI / 180f); // Inclinaison

            // Combiner les transformations
            PlanetMatrix = rotationZ * rotationY;
        };
        _animationTimer.Start();
    }

    public void Dispose()
    {
        _animationTimer?.Stop();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
