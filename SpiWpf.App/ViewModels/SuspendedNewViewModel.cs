using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.DTOs;
using SpiWpf.Entities.Models;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class SuspendedNewViewModel : ObservableObject
    {
        
      
        public ObservableCollection<ClientDTO> ClientsLst { get; set; }
        public ObservableCollection<ContractDTO> ContractsLst { get; set; }


        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                SearchTextCommand.Execute(null);
                IsPopupOpen = true;
                //ListBoxVisibility = string.IsNullOrEmpty(_searchText) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private DateTime _Fecha;
        public DateTime Fecha
        {
            get => _Fecha;
            set => SetProperty(ref _Fecha, value);
        }

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }

        private ContracDetailsNewSuspention? _ContractNewSuspention;
        public ContracDetailsNewSuspention? ContractNewSuspention
        {
            get { return _ContractNewSuspention; }
            set
            {
                SetProperty(ref _ContractNewSuspention, value);
            }
        }

        private ClientDTO? _selectedClient;
        public ClientDTO? SelectedClient
        {
            get { return _selectedClient; }
            set 
            { 
                SetProperty(ref _selectedClient, value); 
                LoadContractCommand.Execute(null);
                IsPopupOpen = false;
                //ListBoxVisibility = Visibility.Collapsed;
            }
        }

        private ContractDTO? _selectedContract;
        public ContractDTO? SelectedContract
        {
            get { return _selectedContract; }
            set 
            { 
                SetProperty(ref _selectedContract, value);
                LoadContractDetailsCommand.Execute(null);
            }
        }

        private Visibility _listBoxVisibility;
        public Visibility ListBoxVisibility
        {
            get => _listBoxVisibility;
            set => SetProperty(ref _listBoxVisibility, value);
        }

        private List<ClientsAPI>? ListaClientes;
        private List<ClientDTO>? ListaFiltrar;

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        public RelayCommand SearchTextCommand { get; }
        public RelayCommand LoadContractCommand { get; }
        public RelayCommand LoadContractDetailsCommand { get; }

        public SuspendedNewViewModel()
        {
            ClientsLst = new ObservableCollection<ClientDTO>();
            ContractsLst = new ObservableCollection<ContractDTO>();
            ListaClientes = new List<ClientsAPI>();
            SearchTextCommand = new RelayCommand(SearchClient);
            LoadContractCommand = new RelayCommand(LoadContratos);
            LoadContractDetailsCommand = new RelayCommand(LoadContractDetails);
            Fecha = DateTime.Now;
            LoadCLients();
        }

        private void SearchClient()
        {

            var lista = ListaFiltrar!.Where(x => x.Name!.ToLower().Contains(SearchText!.ToLower())).ToList();
            ClientsLst?.Clear();
            if (lista != null || lista!.Count > 0)
            {
                foreach (var item in lista!)
                {
                    ClientsLst!.Add(item);
                }
            }
        }

        private async void LoadContractDetails()
        {
            var detalle = SelectedContract;
            if (detalle == null)
            {
                return;
            }
            var responseHttp = await Repository.Get<ContracDetailsNewSuspention>($"/api/contracts/ContractDetailsNewSuspen/{SelectedClient!.Id}/{SelectedContract!.Id}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ContractNewSuspention = responseHttp.Response;

        }

        private async void LoadContratos() 
        {
            var datocliente = SelectedClient;
            if (datocliente != null)
            {
                SearchText = datocliente!.Name;
            }
            else
            {
                return;
            }
            var responseHttp = await Repository.Get<List<ContractDTO>>($"/api/contracts/SelectContract/{datocliente!.Id}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            List<ContractDTO> lista = responseHttp.Response;
            ContractsLst?.Clear();
            if (lista != null || lista!.Count > 0)
            {
                foreach (var item in lista!)
                {
                    ContractsLst!.Add(item);
                }
            }
        }

        public async void LoadCLients()
        {
            IsLoading = true;

            var responseHttp = await Repository.Get<List<ClientsAPI>>("/api/clients/ClienteList");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            ListaClientes = responseHttp.Response;
            ListaFiltrar = ListaClientes.Select(x => new ClientDTO() { Id = x.ClientId, Name = x.FullName! }).ToList();

            IsLoading = false;
        }
    }
}
