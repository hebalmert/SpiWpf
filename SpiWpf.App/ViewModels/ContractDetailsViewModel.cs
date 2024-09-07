using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.DTOs;
using SpiWpf.Entities.Models;
using SpiWpf.Wpf.Views;
using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class ContractDetailsViewModel : ObservableObject
    {
        private static int? DatoPrueba;

        public ObservableCollection<QueuesInfoCls>? QueLst { get; set; }

        public ObservableCollection<BindingInfoCls>? BinLst { get; set; }

        public ObservableCollection<ContractIpDTO>? IpLst { get; set; }

        public ObservableCollection<ContractMacDTO>? MacLst { get; set; }

        public ObservableCollection<ContractServDTO>? ServLst { get; set; }

        public ObservableCollection<ContractPlanDTO>? PlanLst { get; set; }

        public ObservableCollection<ContractNodeDTO>? NodeLst { get; set; }

        private ContractDetailCls? _ContractDetalle;

        public ContractDetailCls? ContractDetalle
        {
            get { return _ContractDetalle; }
            set
            {
                SetProperty(ref _ContractDetalle, value);
            }
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        private bool _ExistNode;
        public bool ExistNode
        {
            get { return _ExistNode; }
            set { SetProperty(ref _ExistNode, value); }
        }

        private bool _ExistPlan;
        public bool ExistPlan
        {
            get { return _ExistPlan; }
            set { SetProperty(ref _ExistPlan, value); }
        }

        private bool _ExistServ;
        public bool ExistServ
        {
            get { return _ExistServ; }
            set { SetProperty(ref _ExistServ, value); }
        }

        private bool _ExistMac;
        public bool ExistMac
        {
            get { return _ExistMac; }
            set { SetProperty(ref _ExistMac, value); }
        }

        private bool _ExistIp;
        public bool ExistIp
        {
            get { return _ExistIp; }
            set { SetProperty(ref _ExistIp, value); }
        }


        private bool _ExistQueues;
        public bool ExistQueues
        {
            get { return _ExistQueues; }
            set { SetProperty(ref _ExistQueues, value); }
        }

        private bool _ExistBind;
        public bool ExistBind
        {
            get { return _ExistBind; }
            set { SetProperty(ref _ExistBind, value); }
        }

        public ContractDetailsViewModel()
        {
            QueLst = new ObservableCollection<QueuesInfoCls>();
            BinLst = new ObservableCollection<BindingInfoCls>();
            IpLst = new ObservableCollection<ContractIpDTO>();
            MacLst = new ObservableCollection<ContractMacDTO>();
            ServLst = new ObservableCollection<ContractServDTO>();
            PlanLst = new ObservableCollection<ContractPlanDTO>();
            NodeLst = new ObservableCollection<ContractNodeDTO>();
            ContractDetalle = new ContractDetailCls();
        }

        public void PaseParametro(int value)
        {
            DatoPrueba = value;
        }

        [RelayCommand]
        public async Task ActiveContract()
        {
            if (ExistBind == false || ExistQueues == false)
            {
                MessageBox.Show("Para Activar un Contrato, debes tener Queues y Binding activos", "Vierificacion de Activacion", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var msgresult = MessageBox.Show("Deseas Activar el Contrato?", "Vierificacion de Activacion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgresult == MessageBoxResult.No)
            {
                return;
            }

            IsLoading = true;

            ContractDTO UpdateContract = new() { Id =Convert.ToInt32(DatoPrueba) , ControlContrato = ContractDetalle!.ControlContrato};
            //actualizamos el contrato de Proceso a Activo

            var responseHttp = await Repository.Put<ContractDTO>($"/api/contracts/UpdateContractActive", UpdateContract);
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadContractDetails();
        }

        [RelayCommand]
        public void VolverBoton()
        {
            var mainWindow = Application.Current.MainWindow as MainPage;
            var viewModel = mainWindow!.DataContext as MainViewModel;
            viewModel!.LoadContracts();
        }

        [RelayCommand]
        public void DeleteContractIp(int idcontractIp)
        {

        }

        [RelayCommand]
        public void DeleteContractMac(int idcontractMac)
        {

        }

        [RelayCommand]
        public void DeleteContractServ(int idcontractServ)
        {

        }

        [RelayCommand]
        public void DeleteContractPlan(int idcontractPlan)
        {

        }

        [RelayCommand]
        public void DeleteContractNode(int idcontractPlan)
        {

        }

        [RelayCommand]
        public async Task PingToNode(string nodoIp)
        {
            IsLoading = true;

            if (nodoIp != null)
            {
                Ping ping = new();
                PingReply reply = await ping.SendPingAsync(nodoIp, 3000);
                if (reply.Status == IPStatus.Success)
                {
                    // El ping fue exitoso
                    IsLoading = false;
                    MessageBox.Show($"Ping a {nodoIp} exitoso. Tiempo de respuesta: {reply.RoundtripTime} ms.", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    // El ping no fue exitoso
                    IsLoading = false;
                    MessageBox.Show($"Fallo el ping a {nodoIp}. Estado: {reply.Status}", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            IsLoading = false;
            MessageBox.Show($"Erro en los Datos para hacer Ping al Nodo", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        [RelayCommand]
        public async Task PingToIpClient(string ipCliente)
        {
            IsLoading = true;

            if (ipCliente != null)
            {
                Ping ping = new();
                PingReply reply = await ping.SendPingAsync(ipCliente, 3000);
                if (reply.Status == IPStatus.Success)
                {
                    // El ping fue exitoso
                    IsLoading = false;
                    MessageBox.Show($"Ping a {ipCliente} exitoso. Tiempo de respuesta: {reply.RoundtripTime} ms.", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    // El ping no fue exitoso
                    IsLoading = false;
                    MessageBox.Show($"Fallo el ping a {ipCliente}. Estado: {reply.Status}", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            IsLoading = false;
            MessageBox.Show($"Erro en los Datos para hacer Ping al Cliente", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        [RelayCommand]
        public async Task PingToServer(string servidorIp)
        {
            IsLoading = true;

            if (servidorIp != null)
            {
                Ping ping = new();
                PingReply reply = await ping.SendPingAsync(servidorIp, 3000);
                if (reply.Status == IPStatus.Success)
                {
                    // El ping fue exitoso
                    IsLoading = false;
                    MessageBox.Show($"Ping a {servidorIp} exitoso. Tiempo de respuesta: {reply.RoundtripTime} ms.", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    // El ping no fue exitoso
                    IsLoading = false;
                    MessageBox.Show($"Fallo el ping a {servidorIp}. Estado: {reply.Status}", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            IsLoading = false;
            MessageBox.Show($"Erro en los Datos para hacer Ping al Servidor", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        public async void LoadContractDetails()
        {
            IsLoading = true;

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<ContractDetailCls>($"/api/contracts/GetContractDetails/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ContractDetalle = responseHttp.Response;

            LoadQueueInfo();
            LoadBindInfo();
            LoadIpInfo();
            LoadMacInfo();
            LoadServInfo();
            LoadPlanInfo();
            LoadNodeInfo();
            IsLoading = false;

            return;
        }

        public async void LoadQueueInfo()
        {

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<List<QueuesInfoCls>>($"/api/contracts/GetContractQue/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var listaQue = responseHttp.Response;
            ExistQueues = listaQue.Count > 0 ? true : false;
            QueLst?.Clear();
            if (listaQue != null || listaQue!.Count > 0)
            {
                foreach (var item in listaQue!)
                {
                    QueLst!.Add(item);
                }
            }

            return;
        }

        public async void LoadBindInfo()
        {

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<List<BindingInfoCls>>($"/api/contracts/GetContractBin/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var listaBin = responseHttp.Response;
            ExistBind = listaBin.Count > 0 ? true : false;
            BinLst?.Clear();
            if (listaBin != null || listaBin!.Count > 0)
            {
                foreach (var item in listaBin!)
                {
                    BinLst!.Add(item);
                }
            }

            return;
        }

        public async void LoadIpInfo()
        {

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<List<ContractIpDTO>>($"/api/contracts/ContracIpCliente/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<ContractIpDTO> listaIp = responseHttp.Response;
            ExistIp = listaIp.Count > 0 ? true : false;
            IpLst?.Clear();
            if (listaIp != null || listaIp!.Count > 0)
            {
                foreach (var item in listaIp!)
                {
                    IpLst!.Add(item);
                }
            }

            return;
        }

        public async void LoadMacInfo()
        {

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<List<ContractMacDTO>>($"/api/contracts/ContracMacCliente/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<ContractMacDTO> listaMac = responseHttp.Response;
            ExistMac = listaMac.Count > 0 ? true : false;
            MacLst?.Clear();
            if (listaMac != null || listaMac!.Count > 0)
            {
                foreach (var item in listaMac!)
                {
                    MacLst!.Add(item);
                }
            }

            return;
        }

        public async void LoadServInfo()
        {

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<List<ContractServDTO>>($"/api/contracts/ContracServCliente/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<ContractServDTO> listaServ = responseHttp.Response;
            ExistServ = listaServ.Count > 0 ? true : false;
            ServLst?.Clear();
            if (listaServ != null || listaServ!.Count > 0)
            {
                foreach (var item in listaServ!)
                {
                    ServLst!.Add(item);
                }
            }

            return;
        }

        public async void LoadPlanInfo()
        {

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<List<ContractPlanDTO>>($"/api/contracts/ContracPlanCliente/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<ContractPlanDTO> listaPlan = responseHttp.Response;
            ExistPlan = listaPlan.Count > 0 ? true : false;
            PlanLst?.Clear();
            if (listaPlan != null || listaPlan!.Count > 0)
            {
                foreach (var item in listaPlan!)
                {
                    PlanLst!.Add(item);
                }
            }

            return;
        }

        public async void LoadNodeInfo()
        {

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<List<ContractNodeDTO>>($"/api/contracts/ContracNodeCliente/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<ContractNodeDTO> listaNode = responseHttp.Response;
            ExistNode = listaNode.Count > 0 ? true : false;
            NodeLst?.Clear();
            if (listaNode != null || listaNode!.Count > 0)
            {
                foreach (var item in listaNode!)
                {
                    NodeLst!.Add(item);
                }
            }

            return;
        }
    }
}
