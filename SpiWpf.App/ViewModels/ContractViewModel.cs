using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using SpiWpf.Wpf.Views;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Security.RightsManagement;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class ContractViewModel : ObservableObject
    {
        public ObservableCollection<ContractCls>? ContractLst { get; set; }

        private List<ContractCls>? ListaContract;

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        public ContractViewModel()
        {
            ContractLst = new ObservableCollection<ContractCls>();
            ListaContract = new List<ContractCls>();
        }

        [RelayCommand]
        public void DetailContract(int idcontract) 
        {
            var mainWindow = Application.Current.MainWindow as MainPage;
            var viewModel = mainWindow!.DataContext as MainViewModel;
            viewModel!.LoadContractDetail(idcontract);
        }

        public async Task LoadContratos()
        {
            IsLoading = true;

            ContractLst!.Clear();

            var responseHttp = await Repository.Get<List<ContractCls>>("/api/contracts/SelectList");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            ListaContract = responseHttp.Response;
            ListaContract = ListaContract
                .Where(x => x.StateType == "Activo" || x.StateType == "Procesando")
                .OrderByDescending(x=> x.StateType).ThenBy(x=> x.FullName)
                .ToList();
            if (ListaContract != null || ListaContract!.Count > 0)
            {
                foreach (var item in ListaContract)
                {
                    ContractLst!.Add(item);
                }
            }
            IsLoading = false;
        }

        public async Task SearchTxt(string txtbuscar)
        {
            if (string.IsNullOrEmpty(txtbuscar))
            {
                var ListaContratos = ListaContract!.ToList();
                ContractLst?.Clear();
                if (ListaContratos != null || ListaContratos!.Count > 0)
                {
                    foreach (var item in ListaContratos!)
                    {
                        ContractLst!.Add(item);
                    }
                }
            }
            else
            {
                var ListaContratos = ListaContract!.Where(x => x.FullName!.ToLower().Contains(txtbuscar.ToLower())).ToList();
                ContractLst?.Clear();
                if (ListaContratos != null || ListaContratos!.Count > 0)
                {
                    foreach (var item in ListaContratos!)
                    {
                        ContractLst!.Add(item);
                    }
                }
            }
        }
    }
}
