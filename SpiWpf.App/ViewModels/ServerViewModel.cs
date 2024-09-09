using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using SpiWpf.Wpf.Helpers;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class ServerViewModel : ObservableObject
    {
        public ObservableCollection<ServerAPI>? ServerAPILst { get; set; }

        private List<ServerAPI>? ListaServidores;

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        private CallPing _DoPing = new();


        public ServerViewModel()
        {
            ServerAPILst = new ObservableCollection<ServerAPI>();
            ListaServidores = new List<ServerAPI>();
        }

        public async Task LoadServers()
        {
            IsLoading = true;

            ServerAPILst!.Clear();

            var responseHttp = await Repository.Get<List<ServerAPI>>("/api/servers/ServerList");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            ListaServidores = responseHttp.Response;
            if (ListaServidores != null || ListaServidores!.Count > 0)
            {
                foreach (var item in ListaServidores)
                {
                    ServerAPILst!.Add(item);
                }
            }

            IsLoading = false;
        }

        public async Task SearchTxt(string txtbuscar)
        {
            if (string.IsNullOrEmpty(txtbuscar))
            {
                var ListaSuspended = ListaServidores!.ToList();
                ServerAPILst?.Clear();
                if (ListaSuspended != null || ServerAPILst!.Count > 0)
                {
                    foreach (var item in ListaSuspended!)
                    {
                        ServerAPILst!.Add(item);
                    }
                }
            }
            else
            {
                var ListaSuspended = ListaServidores!.Where(x => x.ServerName!.ToLower().Contains(txtbuscar.ToLower())).ToList();
                ServerAPILst?.Clear();
                if (ListaSuspended != null || ServerAPILst!.Count > 0)
                {
                    foreach (var item in ListaSuspended!)
                    {
                        ServerAPILst!.Add(item);
                    }
                }
            }
        }

        [RelayCommand]
        public async Task ConectMk(int Idserver)
        {
            IsLoading = true;
            var Servidor = ListaServidores!.First(x => x.ServerId == Idserver);
            Ping ping = new Ping();
            PingReply reply = await ping.SendPingAsync(Servidor.IpNetwork!, 3000);
            if (reply.Status == IPStatus.Success)
            {
                string mensaje = $"Conexion a:{Servidor!.IpNetwork}\n";

                MK mikrotik = new MK(Servidor.IpNetwork!, Servidor.ApiPort);
                if (!mikrotik.Login(Servidor.Usuario!, Servidor.Clave!))
                {
                    IsLoading = false;
                    MessageBox.Show($"Verificar Mikrotik o Datos de Conexion", "Respuesta Conexion", MessageBoxButton.OK);
                    return;
                }

                mikrotik.Send("/system/identity/getall");
                mikrotik.Send("/system/identity/print", true);
                List<string> listArray = new List<string>();
                foreach (string s in mikrotik.Read())
                {
                    listArray.Add(s);
                }
                var listArrayCount = listArray.Count;
                listArray.RemoveAt(listArrayCount - 1);
                var PrimerRegistro = listArray.FirstOrDefault();
                var NameServidor = PrimerRegistro!.Substring(9);


                mikrotik.Send("/ip/hotspot/ip-binding/getall");
                mikrotik.Send("/ip/hotspot/ip-binding/print");
                mikrotik.Send("=.proplist=address", true);
                List<string> list = new List<string>();
                foreach (var item in mikrotik.Read())
                {
                    list.Add(item);
                }
                var listAcount = list.Count;

                mikrotik.Close();

                mensaje += $"Conexion Exitosa a {NameServidor}\n";
                mensaje += $"Servidor Con {listAcount - 1} Registros en Ip Binding";

                IsLoading = false;

                MessageBox.Show($"{mensaje}", "Respuesta Conexion", MessageBoxButton.OK);
            }
            else
            {
                IsLoading = false;
                MessageBox.Show($"Ping Exitos a {Servidor.IpNetwork}.  Respuesta {reply.RoundtripTime} ms.", "Confirmacion", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        [RelayCommand]
        public void PingMK(string Idserver)
        {
            _DoPing.PingIp(Idserver);
        }
    }
}
