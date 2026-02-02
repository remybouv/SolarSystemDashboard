using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using SolarSystemDashboard.ViewModels;
using System;
using System.Collections.Generic;

namespace SolarSystemDashboard.Controls;

public partial class Planet3DControl : UserControl
{
    private Canvas _canvas;
    private DispatcherTimer _timer;
    private double _rotation = 0;
    private List<Ellipse> _sphereElements = new List<Ellipse>();

    public static readonly StyledProperty<IBrush> PlanetColorProperty =
        AvaloniaProperty.Register<Planet3DControl, IBrush>(
            nameof(PlanetColor),
            new SolidColorBrush(Colors.DodgerBlue));

    public static readonly StyledProperty<double> PlanetSizeProperty =
        AvaloniaProperty.Register<Planet3DControl, double>(nameof(PlanetSize), 150.0);

    public static readonly StyledProperty<bool> ShowRingsProperty =
        AvaloniaProperty.Register<Planet3DControl, bool>(nameof(ShowRings), false);

    public static readonly StyledProperty<double> RotationSpeedProperty =
        AvaloniaProperty.Register<Planet3DControl, double>(nameof(RotationSpeed), 1.0);

    public static readonly StyledProperty<string> TexturePathProperty =
        AvaloniaProperty.Register<Planet3DControl, string>(nameof(TexturePath), null);

    public IBrush PlanetColor
    {
        get => GetValue(PlanetColorProperty);
        set => SetValue(PlanetColorProperty, value);
    }

    public double PlanetSize
    {
        get => GetValue(PlanetSizeProperty);
        set => SetValue(PlanetSizeProperty, value);
    }

    public bool ShowRings
    {
        get => GetValue(ShowRingsProperty);
        set => SetValue(ShowRingsProperty, value);
    }

    public double RotationSpeed
    {
        get => GetValue(RotationSpeedProperty);
        set => SetValue(RotationSpeedProperty, value);
    }

    public string TexturePath
    {
        get => GetValue(TexturePathProperty);
        set => SetValue(TexturePathProperty, value);
    }

    public Planet3DControl()
    {
        InitializeComponent();
        PropertyChanged += OnControlPropertyChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _canvas = this.FindControl<Canvas>("PlanetCanvas");
    }

    private void OnControlPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == PlanetColorProperty ||
            e.Property == PlanetSizeProperty ||
            e.Property == ShowRingsProperty ||
            e.Property == TexturePathProperty)
        {
            CreatePlanet();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        CreatePlanet();
        StartAnimation();
    }

    private void CreatePlanet()
    {
        if (_canvas == null) return;

        _canvas.Children.Clear();
        _sphereElements.Clear();

        var size = PlanetSize;
        var color = PlanetColor;

        // Créer la planète principale avec texture ou couleur
        var planet = new Ellipse
        {
            Width = size,
            Height = size
        };

        // Si une texture est fournie, l'utiliser
        if (!string.IsNullOrEmpty(TexturePath))
        {
            try
            {
                var bitmap = new Avalonia.Media.Imaging.Bitmap(TexturePath);
                planet.Fill = new ImageBrush(bitmap)
                {
                    Stretch = Stretch.UniformToFill
                };

                // Ajouter un overlay de dégradé pour l'effet 3D
                planet.OpacityMask = new RadialGradientBrush
                {
                    GradientStops =
                        {
                            new GradientStop(Colors.White, 0),
                            new GradientStop(Colors.White, 0.7),
                            new GradientStop(Colors.Transparent, 1)
                        }
                };
            }
            catch
            {
                // Si l'image ne charge pas, utiliser la couleur par défaut
                planet.Fill = CreateDefaultFill(color, size);
            }
        }
        else
        {
            // Pas de texture, utiliser le dégradé de couleur
            planet.Fill = CreateDefaultFill(color, size);
        }

        Canvas.SetLeft(planet, -size / 2);
        Canvas.SetTop(planet, -size / 2);
        _canvas.Children.Add(planet);

        // Ajouter un effet d'ombre/lumière par dessus
        var lightOverlay = new Ellipse
        {
            Width = size,
            Height = size,
            Fill = new RadialGradientBrush
            {
                GradientStops =
                    {
                        new GradientStop(Color.FromArgb(80, 255, 255, 255), 0),
                        new GradientStop(Color.FromArgb(0, 255, 255, 255), 0.5),
                        new GradientStop(Color.FromArgb(120, 0, 0, 0), 1)
                    },
                Center = new RelativePoint(0.3, 0.3, RelativeUnit.Relative)
            }
        };
        Canvas.SetLeft(lightOverlay, -size / 2);
        Canvas.SetTop(lightOverlay, -size / 2);
        _canvas.Children.Add(lightOverlay);

        // Anneaux si activés
        if (ShowRings)
        {
            CreateRings(size);
        }
    }

    private IBrush CreateDefaultFill(IBrush color, double size)
    {
        return new RadialGradientBrush
        {
            GradientStops =
                {
                    new GradientStop(Colors.White, 0),
                    new GradientStop((color as SolidColorBrush)?.Color ?? Colors.DodgerBlue, 0.5),
                    new GradientStop(Colors.Black, 1)
                },
            Center = new RelativePoint(0.3, 0.3, RelativeUnit.Relative)
        };
    }

    private void CreateRings(double planetSize)
    {
        // Anneau externe
        var ringOuter = new Ellipse
        {
            Width = planetSize * 2,
            Height = planetSize * 0.4,
            Stroke = new SolidColorBrush(Color.FromArgb(100, 200, 180, 120)),
            StrokeThickness = 15,
            Fill = Brushes.Transparent
        };
        
        Canvas.SetLeft(ringOuter, -planetSize);
        Canvas.SetTop(ringOuter, -planetSize * 0.2);
        _canvas.Children.Add(ringOuter);

        // Anneau interne
        var ringInner = new Ellipse
        {
            Width = planetSize * 1.6,
            Height = planetSize * 0.32,
            Stroke = new SolidColorBrush(Color.FromArgb(150, 220, 200, 140)),
            StrokeThickness = 10,
            Fill = Brushes.Transparent
        };
        
        Canvas.SetLeft(ringInner, -planetSize * 0.8);
        Canvas.SetTop(ringInner, -planetSize * 0.16);

        _canvas.Children.Add(ringInner);
    }

    private void StartAnimation()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        _rotation += RotationSpeed;
        if (_rotation >= 360) _rotation -= 360;

        // Pour les textures, faire tourner l'image
        if (!string.IsNullOrEmpty(TexturePath) && _canvas.Children.Count > 0)
        {
            var planet = _canvas.Children[0] as Ellipse;
            if (planet?.Fill is ImageBrush imageBrush)
            {
                // Animer la position de la texture pour simuler la rotation
                imageBrush.Transform = new TranslateTransform(_rotation * 2, 0);
                imageBrush.TileMode = TileMode.Tile;
            }
        }

        // Animer les bandes de latitude pour simuler la rotation
        for (int i = 0; i < _sphereElements.Count; i++)
        {
            var band = _sphereElements[i];
            double phase = (_rotation + i * 20) * Math.PI / 180;
            double scale = 0.8 + 0.2 * Math.Cos(phase);

            if (band.RenderTransform is ScaleTransform transform)
            {
                transform.ScaleX = scale;
            }
            else
            {
                band.RenderTransform = new ScaleTransform(scale, 1);
            }
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _timer?.Stop();
        base.OnDetachedFromVisualTree(e);
    }
}