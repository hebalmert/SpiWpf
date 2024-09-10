using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.DTOs;
using SpiWpf.Entities.Enum;
using SpiWpf.Entities.Models;
using SpiWpf.Wpf.Helpers;
using SpiWpf.Wpf.Views;
using System.Collections.ObjectModel;
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

        private CallPing _DoPing = new();

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

            ContractDTO UpdateContract = new() { Id = Convert.ToInt32(DatoPrueba), ControlContrato = ContractDetalle!.ControlContrato };
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
        public async Task NewQueues()
        {
            if (IpLst!.Count == 1 && MacLst!.Count == 1 && ServLst!.Count == 1 && PlanLst!.Count == 1)
            {
                IsLoading = true;

                var conCliente = ContractDetalle;
                var conServer = ServLst;
                var conPlan = PlanLst;
                var conIpClient = IpLst;

                var dato = new
                {
                    nameserver = ServLst.FirstOrDefault()!.NameServidor,
                    ipservidor = ServLst.FirstOrDefault()!.IpServidor,
                    us = ServLst.FirstOrDefault()!.Usuario,
                    pss = ServLst.FirstOrDefault()!.Clave,
                    puerto = ServLst.FirstOrDefault()!.ApiPort,
                    velocidad = $"{PlanLst!.FirstOrDefault()!.VelocidadUp}/{PlanLst.FirstOrDefault()!.VelocidadDown}",
                    ipcliente = IpLst.FirstOrDefault()!.IpCliente,
                    nomCliente = $"{ContractDetalle!.FullName} - ({ContractDetalle.ControlContrato})",
                    nomPlan = PlanLst.FirstOrDefault()!.PlanName
                };

                QueueParentDTO anyQueueParent = new();
                var responseHttp = await Repository.Get<QueueParentDTO>($"/api/contracts/AnyQueParent/{ServLst.FirstOrDefault()!.ServerId}/{PlanLst.FirstOrDefault()!.PlanId}");
                if (responseHttp.Error)
                {
                    IsLoading = false;
                    var msgerror = await responseHttp.GetErrorMessageAsync();
                    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                anyQueueParent = responseHttp.Response;
                if (anyQueueParent.QueueParentId == 0 && anyQueueParent.ServerId == 0 && anyQueueParent.PlanId == 0)
                {
                    anyQueueParent = null!;
                }

                List<CantClientDTO> CantCLientes = new();
                var responseHttp2 = await Repository.Get<List<CantClientDTO>>($"/api/contracts/CantidadClientes/{PlanLst.FirstOrDefault()!.PlanId}/{ServLst.FirstOrDefault()!.ServerId}");
                if (responseHttp2.Error)
                {
                    IsLoading = false;
                    var msgerror = await responseHttp2.GetErrorMessageAsync();
                    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                CantCLientes = responseHttp2.Response;

                int cantQueueClients = CantCLientes.Count;

                string ListaClientsIP = string.Empty;
                if (cantQueueClients == 0)
                {
                    ListaClientsIP = dato.ipcliente!;
                }
                if (cantQueueClients > 0)
                {
                    foreach (var item in CantCLientes)
                    {
                        ListaClientsIP += item.ips + ", ";
                    }
                    ListaClientsIP = ListaClientsIP.TrimEnd(',', ' ');
                    ListaClientsIP = $"{ListaClientsIP}, {dato.ipcliente}";
                }

                //Vamos a cargar en nombre del PCQ en las variables para poderlas agregar a la Mikrotik
                string PcqDown = string.Empty;
                string PcqUp = string.Empty;

                //calculamos todo en kbps para poder trabajar el Parent mejor
                int UpSpeed = 0;
                int DownSpeed = 0;
                UpSpeed = conPlan.FirstOrDefault()!.SpeedUpType == SpeedUpType.M ? conPlan.FirstOrDefault()!.SpeedUp * 1024 : conPlan.FirstOrDefault()!.SpeedUp;
                DownSpeed = conPlan.FirstOrDefault()!.SpeedDownType == SpeedDownType.M ? conPlan.FirstOrDefault()!.SpeedDown * 1024 : conPlan.FirstOrDefault()!.SpeedUp;
                int UpSpeedLimitAt = (UpSpeed / conPlan.FirstOrDefault()!.TasaReuso);
                int DownSpeedLimitAt = (DownSpeed / conPlan.FirstOrDefault()!.TasaReuso);
                int UpSpeedFather = cantQueueClients + 1 < conPlan.FirstOrDefault()!.TasaReuso + 1 ? UpSpeed : (UpSpeedLimitAt * (cantQueueClients + 1));
                int DownSpeedFather = cantQueueClients + 1 < conPlan.FirstOrDefault()!.TasaReuso + 1 ? DownSpeed : (DownSpeedLimitAt * (cantQueueClients + 1));


                List<QueueTypeDTO> quetype = new();
                var responseHttp3 = await Repository.Get<List<QueueTypeDTO>>($"/api/contracts/QueTypes");
                if (responseHttp3.Error)
                {
                    IsLoading = false;
                    var msgerror = await responseHttp3.GetErrorMessageAsync();
                    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                quetype = responseHttp3.Response;

                PcqDown = quetype.Where(x => x.Down == true).Select(x => x.TypeName).FirstOrDefault()!;
                PcqUp = quetype.Where(x => x.Up == true).Select(x => x.TypeName).FirstOrDefault()!;

                string nomQueues = $"{PcqUp}/{PcqDown}";
                string nomParent = $"Parent {dato.nomPlan} 1 a {conPlan.FirstOrDefault()!.TasaReuso}";

                ////////////////////////////////////////////////////////////
                MK mikrotik = new MK(dato.ipservidor!, dato.puerto);
                if (!mikrotik.Login(dato.us, dato.pss))
                {
                    IsLoading = false;
                    MessageBox.Show($"Error en la Conexion al Servidor Mikrotik", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int total = 0;
                int rest = 0;
                string idmk;
                string mikrotiIndex = string.Empty;

                //Esta es para ver si el Parent Existe en el servidor
                if (anyQueueParent == null)
                {
                    mikrotik.Send("/queue/simple/add");
                    mikrotik.Send("=limit-at=" + $"{UpSpeedFather}k/{DownSpeedFather}k");
                    mikrotik.Send("=max-limit=" + $"{UpSpeedFather}k/{DownSpeedFather}k");
                    mikrotik.Send("=name=" + nomParent);
                    mikrotik.Send("=queue=" + nomQueues);
                    mikrotik.Send("=target=" + ListaClientsIP);
                    mikrotik.Send("=priority=" + "5/5");
                    mikrotik.Send("/queue/simple/print", true);
                    foreach (var item in mikrotik.Read())
                    {
                        idmk = item;
                        total = idmk.Length;
                        rest = total - 10;
                        mikrotiIndex = idmk.Substring(10, rest);
                    }

                    mikrotik.Send("/queue/simple/move");
                    mikrotik.Send("=.id=" + mikrotiIndex);
                    mikrotik.Send("=destination=1");
                    mikrotik.Send("/queue/simple/print", true);
                    int sum1 = 0;
                    foreach (var item in mikrotik.Read())
                    {
                        sum1 += 1;
                    }

                    QueueParentDTO queueParent = new()
                    {
                        ParentName = nomParent,
                        ServerId = conServer.FirstOrDefault()!.ServerId,
                        PlanId = conPlan.FirstOrDefault()!.PlanId,
                        Down = $"{DownSpeed}k",
                        Up = $"{UpSpeed}k",
                        MkId = mikrotiIndex
                    };
                    var responseHttp4 = await Repository.Post<QueueParentDTO>($"/api/contracts/NewQueueParent", queueParent);
                    if (responseHttp4.Error)
                    {
                        IsLoading = false;
                        var msgerror = await responseHttp4.GetErrorMessageAsync();
                        MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    mikrotik.Send("/queue/simple/add");
                    mikrotik.Send("=limit-at=" + $"{UpSpeedLimitAt}k/{DownSpeedLimitAt}k");
                    mikrotik.Send("=max-limit=" + $"{UpSpeed}k/{DownSpeed}k");
                    mikrotik.Send("=name=" + dato.nomCliente);
                    mikrotik.Send("=target=" + dato.ipcliente);
                    mikrotik.Send("=parent=" + nomParent);
                    mikrotik.Send("=priority=" + "5/5");
                    mikrotik.Send("=queue=" + nomQueues);
                    mikrotik.Send("/queue/simple/print", true);

                    total = 0;
                    rest = 0;
                    idmk = string.Empty;
                    mikrotiIndex = string.Empty;

                    foreach (var item in mikrotik.Read())
                    {
                        idmk = item;
                        total = idmk.Length;
                        rest = total - 10;
                        mikrotiIndex = idmk.Substring(10, rest);
                    }

                    mikrotik.Send("/queue/simple/move");
                    mikrotik.Send("=.id=" + mikrotiIndex);
                    mikrotik.Send("=destination=1");
                    mikrotik.Send("/queue/simple/print", true);
                    int sum = 0;
                    foreach (var item in mikrotik.Read())
                    {
                        sum += 1;
                    }
                }
                else
                {
                    mikrotik.Send("/queue/simple/set");
                    mikrotik.Send("=.id=" + anyQueueParent.MkId);
                    mikrotik.Send("=limit-at=" + $"{UpSpeedFather}k/{DownSpeedFather}k");
                    mikrotik.Send("=max-limit=" + $"{UpSpeedFather}k/{DownSpeedFather}k");
                    mikrotik.Send("=target=" + ListaClientsIP);
                    mikrotik.Send("/queue/simple/print", true);
                    foreach (var item in mikrotik.Read())
                    {
                        idmk = item;
                        total = idmk.Length;
                        rest = total - 10;
                    }

                    anyQueueParent.Down = $"{DownSpeed}k";
                    anyQueueParent.Up = $"{UpSpeed}k";

                    var responseHttp4 = await Repository.Put<QueueParentDTO>($"/api/contracts/UpdateAnyQueueParent", anyQueueParent);
                    if (responseHttp4.Error)
                    {
                        IsLoading = false;
                        var msgerror = await responseHttp4.GetErrorMessageAsync();
                        MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    //_context.Update(anyQueueParent);
                    //await _context.SaveChangesAsync();

                    mikrotik.Send("/queue/simple/add");
                    mikrotik.Send("=limit-at=" + $"{UpSpeedLimitAt}k/{DownSpeedLimitAt}k");
                    mikrotik.Send("=max-limit=" + $"{UpSpeed}k/{DownSpeed}k");
                    mikrotik.Send("=name=" + dato.nomCliente);
                    mikrotik.Send("=target=" + dato.ipcliente);
                    mikrotik.Send("=parent=" + anyQueueParent.ParentName);
                    mikrotik.Send("=priority=" + "5/5");
                    mikrotik.Send("=queue=" + nomQueues);
                    mikrotik.Send("/queue/simple/print", true);

                    total = 0;
                    rest = 0;
                    idmk = string.Empty;
                    mikrotiIndex = string.Empty;

                    foreach (var item in mikrotik.Read())
                    {
                        idmk = item;
                        total = idmk.Length;
                        rest = total - 10;
                        mikrotiIndex = idmk.Substring(10, rest);
                    }

                    mikrotik.Send("/queue/simple/move");
                    mikrotik.Send("=.id=" + mikrotiIndex);
                    mikrotik.Send("=destination=1");
                    mikrotik.Send("/queue/simple/print", true);
                    int sum = 0;
                    foreach (var item in mikrotik.Read())
                    {
                        sum += 1;
                    }
                }

                mikrotik.Close();

                ContractQue ContractQueModel = new()
                {
                    ContractId = Convert.ToInt32(ContractDetalle.ContractId),
                    ServerId = Convert.ToInt32(ServLst.FirstOrDefault()!.ServerId),
                    PlanId = Convert.ToInt32(PlanLst.FirstOrDefault()!.PlanId),
                    IpNetId = Convert.ToInt32(IpLst.FirstOrDefault()!.IpNetId),
                    ServerName = ServLst.FirstOrDefault()!.NameServidor,
                    IpServer = ServLst.FirstOrDefault()!.IpServidor,
                    IpCliente = IpLst.FirstOrDefault()!.IpCliente,
                    PlanName = PlanLst.FirstOrDefault()!.PlanName,
                    TotalVelocidad = PlanLst.FirstOrDefault()!.VelocidadTotal,
                    MikrotikId = mikrotiIndex
                };

                var responseHttp5 = await Repository.Post<ContractQue>($"/api/contracts/NewContractQue", ContractQueModel);
                if (responseHttp5.Error)
                {
                    IsLoading = false;
                    var msgerror = await responseHttp5.GetErrorMessageAsync();
                    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                LoadQueueInfo();
                IsLoading = false;
            }
            else
            {
                MessageBox.Show($"No Existen Datos o hay mas de uno en Ip, Mac, Servidor o Plan para poder Crear el Ip Binding", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        [RelayCommand]
        public async Task DeleteContractQueues(int idcontractQueue)
        {
            IsLoading = true;

            var responseHttp = await Repository.Get<ContractQueDTO>($"/api/contracts/DatoContractQue/{idcontractQueue}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var NuevoDato = responseHttp.Response;

            var conCliente = ContractDetalle;
            var conServer = ServLst;
            var conPlan = PlanLst;
            var conIpClient = IpLst;

            var dato = new
            {
                nameserver = NuevoDato.ServerName,
                ipservidor = NuevoDato.IpServer,
                us = NuevoDato.Usuario,
                pss = NuevoDato.Clave,
                puerto = NuevoDato.Puerto,
                velocidad = $"{NuevoDato.VelocidadUp}/{NuevoDato.VelocidadDown}",
                ipcliente = NuevoDato.IpCliente,
                nomCliente = $"{ContractDetalle!.FullName} - ({ContractDetalle.ControlContrato})",
                nomPlan = NuevoDato.PlanName
            };

            QueueParentDTO anyQueueParent = new();
            var responseHttp2 = await Repository.Get<QueueParentDTO>($"/api/contracts/AnyQueParent/{NuevoDato.ServerId}/{NuevoDato.PlanId}");
            if (responseHttp2.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp2.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            anyQueueParent = responseHttp2.Response;
            if (anyQueueParent.QueueParentId == 0 && anyQueueParent.ServerId == 0 && anyQueueParent.PlanId == 0)
            {
                anyQueueParent = null!;
            }

            List<CantClientDTO> CantCLientes = new();
            var responseHttp3 = await Repository.Get<List<CantClientDTO>>($"/api/contracts/CantidadClientesToDelete/{NuevoDato.PlanId}/{NuevoDato.ServerId}/{idcontractQueue}");
            if (responseHttp3.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp2.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CantCLientes = responseHttp3.Response;

            int cantQueueClients = CantCLientes.Count;

            string ListaClientsIP = string.Empty;
            if (cantQueueClients == 0)
            {
                ListaClientsIP = "0.0.0.0/0";
            }
            if (cantQueueClients > 0)
            {
                foreach (var item in CantCLientes)
                {
                    ListaClientsIP += item.ips + ", ";
                }
                ListaClientsIP = ListaClientsIP.TrimEnd(',', ' ');
            }

            //Vamos a cargar en nombre del PCQ en las variables para poderlas agregar a la Mikrotik
            string PcqDown = string.Empty;
            string PcqUp = string.Empty;

            //calculamos todo en kbps para poder trabajar el Parent mejor
            int UpSpeed = 0;
            int DownSpeed = 0;
            UpSpeed = NuevoDato.SpeedUpType == SpeedUpType.M ? NuevoDato.SpeedUp * 1024 : NuevoDato.SpeedUp;
            DownSpeed = NuevoDato.SpeedDownType == SpeedDownType.M ? NuevoDato.SpeedDown * 1024 : NuevoDato.SpeedUp;
            int UpSpeedLimitAt = 0;
            int DownSpeedLimitAt = 0;
            if (NuevoDato.TasaReuso == 0)
            {
                UpSpeedLimitAt = (UpSpeed / 1);
                DownSpeedLimitAt = (DownSpeed / 1);
            }
            else
            {
                UpSpeedLimitAt = (UpSpeed / NuevoDato.TasaReuso);
                DownSpeedLimitAt = (DownSpeed / NuevoDato.TasaReuso);
            }
            int UpSpeedFather = cantQueueClients < NuevoDato.TasaReuso + 1 ? UpSpeed : (UpSpeedLimitAt * (cantQueueClients));
            int DownSpeedFather = cantQueueClients < NuevoDato.TasaReuso + 1 ? DownSpeed : (DownSpeedLimitAt * (cantQueueClients));

            List<QueueTypeDTO> quetype = new();
            var responseHttp4 = await Repository.Get<List<QueueTypeDTO>>($"/api/contracts/QueTypes");
            if (responseHttp4.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp4.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            quetype = responseHttp4.Response;

            PcqDown = quetype.Where(x => x.Down == true).Select(x => x.TypeName).FirstOrDefault()!;
            PcqUp = quetype.Where(x => x.Up == true).Select(x => x.TypeName).FirstOrDefault()!;

            string nomQueues = $"{PcqUp}/{PcqDown}";
            string nomParent = $"Parent {NuevoDato.PlanName} 1 a {NuevoDato.TasaReuso}";

            //Se hace con conexion a la Mikroti y se deja abierto
            ////////////////////////////////////////////////////////////
            MK mikrotik = new MK(NuevoDato.IpServer!, NuevoDato.Puerto);
            if (!mikrotik.Login(NuevoDato.Usuario!, NuevoDato.Clave!))
            {
                IsLoading = false;
                var msgerror = await responseHttp4.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            mikrotik.Send("/queue/simple/remove");
            mikrotik.Send("=.id=" + NuevoDato.MikrotikId, true);

            int total = 0;
            int rest = 0;
            string idmk;

            foreach (var item in mikrotik.Read())
            {
                idmk = item;
                total = idmk.Length;
                rest = total - 10;
            }

            //Se aplica cuando existe Parent, pero tiene por lo menos un Cliente asignado.
            if (anyQueueParent != null && ListaClientsIP != "0.0.0.0/0")
            {
                mikrotik.Send("/queue/simple/set");
                mikrotik.Send("=.id=" + anyQueueParent!.MkId);
                mikrotik.Send("=limit-at=" + $"{UpSpeedFather}k/{DownSpeedFather}k");
                mikrotik.Send("=max-limit=" + $"{UpSpeedFather}k/{DownSpeedFather}k");
                mikrotik.Send("=target=" + ListaClientsIP);
                mikrotik.Send("/queue/simple/print", true);
                foreach (var item in mikrotik.Read())
                {
                    idmk = item;
                    total = idmk.Length;
                    rest = total - 10;
                }

                anyQueueParent.Down = $"{DownSpeed}k";
                anyQueueParent.Up = $"{UpSpeed}k";

                var responseHttp5 = await Repository.Put<QueueParentDTO>($"/api/contracts/UpdateAnyQueueParent", anyQueueParent);
                if (responseHttp5.Error)
                {
                    IsLoading = false;
                    var msgerror = await responseHttp5.GetErrorMessageAsync();
                    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            //Fin Si existe algun anyQueuesParent

            //Se aplica cuando existe Parent, pero queda sin ningun Cliente asignado.
            if (anyQueueParent != null && ListaClientsIP == "0.0.0.0/0")
            {
                mikrotik.Send("/queue/simple/remove");
                mikrotik.Send("=.id=" + anyQueueParent!.MkId, true);

                foreach (var item in mikrotik.Read())
                {
                    idmk = item;
                    total = idmk.Length;
                    rest = total - 10;
                }
                var responseHttp6 = await Repository.Delete($"/api/contracts/DeleteAnyQueueParent/{anyQueueParent.QueueParentId}");
                if (responseHttp6.Error)
                {
                    IsLoading = false;
                    var msgerror = await responseHttp6.GetErrorMessageAsync();
                    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //_context.Remove(anyQueueParent);
                //await _context.SaveChangesAsync();
            }
            //Fin Si existe algun anyQueuesParent


            mikrotik.Close();
            ///////////////////////////////////////////////////////////////////////////////////
            /// 

            LoadQueueInfo();
            IsLoading = false;
        }


        [RelayCommand]
        public async Task NewBinding()
        {
            if (IpLst!.Count == 1 && MacLst!.Count == 1 && ServLst!.Count == 1 && PlanLst!.Count == 1)
            {
                IsLoading = true;

                var dato = new
                {
                    nameserver = ServLst.FirstOrDefault()!.NameServidor,
                    ipservidor = ServLst.FirstOrDefault()!.IpServidor,
                    us = ServLst.FirstOrDefault()!.Usuario,
                    pss = ServLst.FirstOrDefault()!.Clave,
                    puerto = ServLst.FirstOrDefault()!.ApiPort,
                    ipcliente = IpLst!.FirstOrDefault()!.IpCliente,
                    nomCliente = $"{ContractDetalle!.FullName} - ({ContractDetalle.ControlContrato})",
                    macCliente = $"{MacLst.FirstOrDefault()!.MacCliente}"
                };

                ////////////////////////////////////////////////////////////
                MK mikrotik = new MK(dato.ipservidor!, dato.puerto);
                if (!mikrotik.Login(dato.us, dato.pss))
                {
                    IsLoading = false;
                    MessageBox.Show($"Error en la Conexion al Servidor Mikrotik", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                mikrotik.Send("/ip/hotspot/ip-binding/add");
                mikrotik.Send("=address=" + dato.ipcliente);
                mikrotik.Send("=to-address=" + dato.ipcliente);
                mikrotik.Send("=comment=" + dato.nomCliente);
                mikrotik.Send("=mac-address=" + dato.macCliente);
                mikrotik.Send("=server=" + "all");
                mikrotik.Send("=type=" + HotSpotType.bypassed.ToString());
                mikrotik.Send("/ip/hotspot/ip-binding/print", true);

                int total = 0;
                int rest = 0;
                string idmk;
                string mikrotiIndex = string.Empty;

                foreach (var item in mikrotik.Read())
                {
                    idmk = item;
                    total = idmk.Length;
                    rest = total - 10;
                    mikrotiIndex = idmk.Substring(10, rest);
                }

                mikrotik.Close();
                ///////////////////////////////////////////////////////////////////////////////////
                /// 

                ContractBind modelo = new()
                {
                    ContractId = ContractDetalle.ContractId,
                    ServerId = Convert.ToInt32(ServLst.FirstOrDefault()!.ServerId),
                    CargueDetailId = Convert.ToInt32(MacLst.FirstOrDefault()!.CargueDetailId),
                    IpNetId = Convert.ToInt32(IpLst!.FirstOrDefault()!.IpNetId),
                    HotSpotTypeId = (int)HotSpotType.bypassed,
                    ServerName = ServLst!.FirstOrDefault()!.NameServidor,
                    IpServer = ServLst!.FirstOrDefault()!.IpServidor,
                    IpCliente = IpLst!.FirstOrDefault()!.IpCliente,
                    MacCliente = MacLst!.FirstOrDefault()!.MacCliente,
                    MikrotikId = mikrotiIndex
                };


                //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
                var responseHttp = await Repository.Post<ContractBind>($"/api/contracts/NewContractIpBinding", modelo);
                if (responseHttp.Error)
                {
                    IsLoading = false;
                    var msgerror = await responseHttp.GetErrorMessageAsync();
                    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                LoadBindInfo();

                IsLoading = false;
            }
            else
            {
                MessageBox.Show($"No Existen Datos o hay mas de uno en Ip, Mac, Servidor o Plan para poder Crear el Ip Binding", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        [RelayCommand]
        public void DeleteContractIp(int idcontractIp)
        {
            return;
        }

        [RelayCommand]
        public void DeleteContractMac(int idcontractMac)
        {
            return;
        }

        [RelayCommand]
        public void DeleteContractServ(int idcontractServ)
        {
            return;
        }

        [RelayCommand]
        public void DeleteContractPlan(int idcontractPlan)
        {
            return;
        }

        [RelayCommand]
        public void DeleteContractNode(int idcontractPlan)
        {
            return;
        }



        [RelayCommand]
        public async Task DeleteContractBinding(int idContractBind)
        {
            if (BinLst!.Count == 1)
            {
                var msgresult = MessageBox.Show("Deseas Eliminar el IpBinding?", "Vierificacion de Activacion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgresult == MessageBoxResult.No)
                {
                    return;
                }

                IsLoading = true;

                //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
                var responseHttp = await Repository.Get<ServerCon>($"/api/servers/ServerCon/{BinLst.FirstOrDefault()!.ServerId}");
                if (responseHttp.Error)
                {
                    IsLoading = false;
                    var msgerror = await responseHttp.GetErrorMessageAsync();
                    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var DatoServidor = responseHttp.Response;

                //Se hace con conexion a la Mikroti y se deja abierto
                ////////////////////////////////////////////////////////////
                MK mikrotik = new MK(DatoServidor.IpServidor!, DatoServidor.ApiPort);
                if (!mikrotik.Login(DatoServidor.Usuario!, DatoServidor.Clave!))
                {
                    IsLoading = false;
                    MessageBox.Show($"Error en la Conexion al Servidor Mikrotik", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                mikrotik.Send("/ip/hotspot/ip-binding/remove");
                mikrotik.Send("=.id=" + BinLst.FirstOrDefault()!.MkIndex, true);

                int total = 0;
                int rest = 0;
                string idmk;

                foreach (var item in mikrotik.Read())
                {
                    idmk = item;
                    total = idmk.Length;
                    rest = total - 10;
                }

                mikrotik.Close();
                ///////////////////////////////////////////////////////////////////////////////////
                /// 

                //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
                var responseHttp2 = await Repository.Delete($"/api/contracts/DeleteContractBin/{BinLst.FirstOrDefault()!.ContractBindId}");
                if (responseHttp2.Error)
                {
                    IsLoading = false;
                    var msgerror = await responseHttp2.GetErrorMessageAsync();
                    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                LoadBindInfo();

                IsLoading = false;
            }
            else
            {
                MessageBox.Show($"No Existen Datos o hay mas de uno en Ip, Mac, Servidor o Plan para poder Crear el Ip Binding", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        //Ping de Nodos
        [RelayCommand]
        public void PingToNode(string nodoIp)
        {
            _DoPing.PingIp(nodoIp);
        }

        //Ping de IP Cliente
        [RelayCommand]
        public void PingToIpClient(string ipCliente)
        {
            _DoPing.PingIp(ipCliente);
        }


        //Ping del Servidor
        [RelayCommand]
        public void PingToServer(string servidorIp)
        {
            _DoPing.PingIp(servidorIp);
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
