using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Entities.DTOs;
using System.Runtime.Serialization.DataContracts;
using System.Windows;
using System.Windows.Controls;

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

        [RelayCommand]
        public void LoadCutControl()
        {
            CurrentChildView = new CutControlViewModel();
        }

        //una ViewModel llamada desde otro User Control
        public void LoadSuspendedNewView()
        {
            CurrentChildView = new SuspendedNewViewModel();
        }

        //una ViewModel llamada desde otro User Control
        public void LoadCutControlNewView()
        {
            CurrentChildView = new CutControlNewViewModel();
        }

        //una ViewModel llamada desde otro User Control
        //Forma para pasar parametros de un UserControl a Otro UserControl
        public void LoadCutControlDetail(ContractCutAPI value)
        {
            var viewmododel = new CutControlDetailViewModel();
            viewmododel.PaseParametro(value);
            CurrentChildView = viewmododel;
            
        }
    }
}
