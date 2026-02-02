using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarSystemDashboard.Interfaces;
using SolarSystemDashboard.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SolarSystemDashboard.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";

        private readonly IBodiesService _bodiesService;

        [ObservableProperty]
        private Bodies _bodiesList = new();

        [ObservableProperty]
        private Body? _currentBody = new();

        [ObservableProperty]
        private int _currentBodyIndex = 0;

        [ObservableProperty]
        private PlanetDisplayConfig? _currentPlanetConfig = new PlanetDisplayConfig
        {
            TexturePath = "Resources/Textures/2k_earth_daymap.jpg",
            PlanetSize = 50,
            RotationSpeed = 1.0,
            ShowRings = false
        };

        [ObservableProperty]
        private double _currentDisplaySize = 100;

        public MainWindowViewModel(IBodiesService bodiesService)
        {
            _bodiesService = bodiesService;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await GetBodies();
        }

        public async Task GetBodies()
        {
            try
            {
                BodiesList = await _bodiesService.GetBodiesAsync();

                // Filter to show only planets
                if (BodiesList.BodyList != null)
                {
                    BodiesList.BodyList = BodiesList.BodyList.Where(b => b.IsPlanet).ToList();
                }

                if (BodiesList.BodyList != null && BodiesList.BodyList.Count > 0)
                {
                    CurrentBodyIndex = 0;
                    CurrentBody = BodiesList.BodyList[CurrentBodyIndex];
                }
            }
            catch (Exception ex)
            {
                // Gérer les erreurs (afficher un message à l'utilisateur, etc.)
                Console.WriteLine($"Error fetching bodies: {ex.Message}");
            }
        }

        [RelayCommand]
        private void NextBody()
        {
            if (BodiesList.BodyList == null || BodiesList.BodyList.Count == 0) return;

            CurrentBodyIndex = (CurrentBodyIndex + 1) % BodiesList.BodyList.Count;
            UpdateCurrentBody();
        }

        [RelayCommand]
        private void PreviousBody()
        {
            if (BodiesList == null || BodiesList.BodyList.Count == 0) return;

            CurrentBodyIndex = (CurrentBodyIndex - 1 + BodiesList.BodyList.Count) % BodiesList.BodyList.Count;
            UpdateCurrentBody();
        }

        private void UpdateCurrentBody()
        {
            CurrentBody = BodiesList.BodyList[CurrentBodyIndex];

            CurrentPlanetConfig = PlanetConfigurations.GetConfig(CurrentBody.EnglishName);
            CurrentDisplaySize = PlanetConfigurations.GetDisplaySize(CurrentBody.EnglishName);

            OnPropertyChanged(nameof(HasBodies));
            OnPropertyChanged(nameof(BodyCounter));
        }

        public bool HasBodies => BodiesList.BodyList != null && BodiesList.BodyList.Count > 0;
        public string BodyCounter => HasBodies ? $"{CurrentBodyIndex + 1} / {BodiesList.BodyList.Count}" : "0 / 0";
    }
}
