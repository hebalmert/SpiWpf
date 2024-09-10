using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Data;
using SpiWpf.Entities.DTOs;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public class HomeViewModel : ObservableObject
    {

        private DashBoardDTO? _Dashboard;
        public DashBoardDTO? Dashboard
        {
            get { return _Dashboard; }
            set { SetProperty(ref _Dashboard, value); }
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        public HomeViewModel()
        {
            Dashboard = new();
        }

        public async Task LoadDatos()
        {
            IsLoading = true;

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<DashBoardDTO>("/api/dashboard/DashBoardIndex");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Dashboard = responseHttp.Response;

            IsLoading = false;

        }
    }
}
