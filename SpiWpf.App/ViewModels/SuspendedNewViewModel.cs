using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.DTOs;
using SpiWpf.Entities.Models;
using SpiWpf.Wpf.Helpers;
using SpiWpf.Wpf.Views;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
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

        private string _Motivo;
        public string Motivo
        {
            get => _Motivo;
            set => SetProperty(ref _Motivo, value);
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

        [RelayCommand]
        public async void GuardarBoton() 
        {
            var cliente = SelectedClient;
            var contrato = SelectedContract;
            var motivoSuspension = Motivo;
            if (string.IsNullOrEmpty(cliente!.Name) || string.IsNullOrEmpty(contrato!.ControlContrato) || string.IsNullOrEmpty(motivoSuspension))
            {
                MessageBox.Show("Cliente, Contrato y Motivo son Datos Obligatorios", "Error para Continuar", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsLoading = true;

            SuspendeCliente modelo = new() 
            { 
                ClientId = cliente.Id,
                ContractId = contrato.Id,
                ControlContrato = contrato.ControlContrato,
                Motivo = motivoSuspension,
                PlanName = ContractNewSuspention!.PlanName,
                MontoPlan = ContractNewSuspention.Monto
            };
            var responseHttp = await Repository.Get<SuspendedActiveAPI>($"/api/suspended/toSuspendclient/{SelectedClient!.Id}/{SelectedContract!.Id}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SuspendedActiveAPI DatoAsuspender = responseHttp.Response;
            //Verificamos el Ping al servidor para ver si responde y continuar
            Ping ping = new();
            PingReply reply = await ping.SendPingAsync(DatoAsuspender.IpNetwork!, 3000);
            if (reply.Status != IPStatus.Success)
            {
                // El ping fue exitoso
                IsLoading = false;
                MessageBox.Show($"Fallo el ping a {DatoAsuspender.IpNetwork}. Estado: {reply.Status}", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Termina prueba del Ping

            ////////////////////////////////////////////////////////////
            MK mikrotik = new MK(DatoAsuspender.IpNetwork!, DatoAsuspender.ApiPort);
            if (!mikrotik.Login(DatoAsuspender.Usuario!, DatoAsuspender.Clave!))
            {
                IsLoading = false;
                MessageBox.Show($"Error en la Conexion al Servidor Mikrotik", "Error Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            mikrotik.Send("/ip/hotspot/ip-binding/set");
            mikrotik.Send("=.id=" + DatoAsuspender.IdIpBinding);
            mikrotik.Send("=type=" + "regular");
            mikrotik.Send("/ip/hotspot/ip-binding/print", true);

            int total = 0;
            int rest = 0;
            string idmk;

            foreach (var item2 in mikrotik.Read())
            {
                idmk = item2;
                total = idmk.Length;
                rest = total - 10;
            }

            mikrotik.Close();

            var responseHttp2 = await Repository.Post<SuspendeCliente>($"/api/suspended/CreateSuspended/", modelo);
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IsLoading = false;

            var mainWindow = Application.Current.MainWindow as MainPage;
            var viewModel = mainWindow!.DataContext as MainViewModel;
            viewModel!.LoadSuspendedView();
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
