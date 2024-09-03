using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public class ContractViewModel : ObservableObject
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
                await LoadContratos();
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
