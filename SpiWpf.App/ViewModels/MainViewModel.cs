using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Wpf.Views;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        private ObservableObject _CurrentChildView;

        public ObservableObject? CurrentChildView
        {
            get => _CurrentChildView;
            set { SetProperty(ref _CurrentChildView!, value); }
        }

        public MainViewModel()
        {
            
        }

        [RelayCommand]
        public async Task LoadHomeView()
        {
            CurrentChildView = new HomeViewModel();
        }

        [RelayCommand]
        public async Task LoadClientsView()
        {
            CurrentChildView = new ClientsViewModel();
        }
    }
}
