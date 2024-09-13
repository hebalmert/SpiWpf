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

        [RelayCommand]
        public void DetailCutControl(int idcutcontrol)
        {
            ContractCutAPI modelo = ListaContractCut!.Where(x => x.ContractCutId == idcutcontrol).FirstOrDefault()!;
            var mainWindow = Application.Current.MainWindow as MainPage;
            var viewModel = mainWindow!.DataContext as MainViewModel;
            viewModel!.LoadCutControlDetail(modelo);
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
            List<ContractCutAPI> NuevaLista = ListaContractCut.OrderByDescending(x => x.DateCut).ToList();

            if (NuevaLista != null || NuevaLista!.Count > 0)
            {
                foreach (var item in NuevaLista)
                {
                    ContractCutsLst!.Add(item);
                }
            }
            IsLoading = false;
        }
    }
}
