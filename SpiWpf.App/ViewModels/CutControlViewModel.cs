using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.DTOs;
using SpiWpf.Wpf.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class CutControlViewModel : ObservableObject
    {

        public ObservableCollection<ContractCutAPI>? ContractCutsLst { get; set; }

        private List<ContractCutAPI>? ListaContractCut;

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        private bool _IsVisibly;
        public bool IsVisibly
        {
            get => _IsVisibly;
            set => SetProperty(ref _IsVisibly, value);
        }

        public CutControlViewModel()
        {
            ContractCutsLst = new ObservableCollection<ContractCutAPI>();
            ListaContractCut = new List<ContractCutAPI>();
        }

        [RelayCommand]
        public void LoadCutControlNew()
        {
            var mainWindow = Application.Current.MainWindow as MainPage;
            var viewModel = mainWindow!.DataContext as MainViewModel;
            viewModel!.LoadCutControlNewView();
        }

        [RelayCommand]
        public async Task DeleteCutControl(int idcutcontrol) 
        {
            var msgresult = MessageBox.Show("Realmente Desea Eliminar Este Control de Suspension General?", "Vierificacion de Activacion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgresult == MessageBoxResult.No)
            {
                return;
            }

            IsLoading = true;

            var responseHttp2 = await Repository.Delete($"/api/cutcontrol/toDelete/{idcutcontrol}");
            if (responseHttp2.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp2.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await LoadCutControl();

            IsLoading = false;
            MessageBox.Show($"El Registro se Elimino con Exito", "Confirmacion Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public async Task LoadCutControl()
        {
            IsLoading = true;

            ContractCutsLst!.Clear();

            var responseHttp = await Repository.Get<List<ContractCutAPI>>("/api/cutcontrol/cutcontrolList");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            ListaContractCut = responseHttp.Response;
            if (ListaContractCut != null || ListaContractCut!.Count > 0)
            {
                foreach (var item in ListaContractCut)
                {
                    ContractCutsLst!.Add(item);
                }
            }
            IsLoading = false;
        }
    }
}
