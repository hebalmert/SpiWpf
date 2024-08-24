using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Wpf.Views;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        private ObservableObject? _CurrentChildView;

        public ObservableObject? CurrentChildView
        {
            get => _CurrentChildView;
            set { SetProperty(ref _CurrentChildView!, value); }
        }

        public MainViewModel()
        {
            
        }

        [RelayCommand]
        public void LoadHomeView()
        {
            CurrentChildView = new HomeViewModel();
        }

        [RelayCommand]
        public void LoadClientsView()
        {
            CurrentChildView = new ClientsViewModel();
        }

        [RelayCommand]
        public void LoadServersView()
        {
            CurrentChildView = new ServerViewModel();
        }

        [RelayCommand]
        public void LoadPlansView()
        {
            CurrentChildView = new PlanesViewModel();
        }

        [RelayCommand]
        public void LoadNodesView()
        {
            CurrentChildView = new NodeViewModel();
        }

        [RelayCommand]
        public void LoadSuspendedView()
        {
            CurrentChildView = new SuspendedViewModel();
        }
    }
}
