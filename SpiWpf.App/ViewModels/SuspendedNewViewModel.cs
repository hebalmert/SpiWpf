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
        private string? _searchText;
        private ClientDTO? _selectedClient;
        private ContractDTO? _selectedContract;
        private Visibility _listBoxVisibility;

        // Colecciones para clientes y contratos
        public ObservableCollection<ClientDTO> ClientsLst { get; set; }
        public ObservableCollection<ContractDTO> ContractsLst { get; set; }

        // Propiedades para binding
        public string? SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                SearchTextCommand.Execute(null);
                ListBoxVisibility = string.IsNullOrEmpty(_searchText) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public ClientDTO? SelectedClient
        {
            get { return _selectedClient; }
            set 
            { 
                SetProperty(ref _selectedClient, value); 
                LoadContractCommand.Execute(null);
                ListBoxVisibility = Visibility.Collapsed;
            }
        }

        public ContractDTO? SelectedContract
        {
            get { return _selectedContract; }
            set { SetProperty(ref _selectedContract, value); }
        }

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

        public SuspendedNewViewModel()
        {
            ClientsLst = new ObservableCollection<ClientDTO>();
            ContractsLst = new ObservableCollection<ContractDTO>();
            ListaClientes = new List<ClientsAPI>();
            SearchTextCommand = new RelayCommand(SearchClient);
            LoadContractCommand = new RelayCommand(LoadContratos);
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

        private void LoadContratos() 
        {
            var datocliente = SelectedClient;
            if (datocliente != null)
            {
                SearchText = datocliente!.Name;
            }
            return;
        }

        public async Task LoadCLients()
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
