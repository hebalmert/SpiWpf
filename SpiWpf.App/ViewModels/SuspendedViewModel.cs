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
    public partial class SuspendedViewModel : ObservableObject
    {
        public ObservableCollection<SuspendedAPI>? SuspendedAPILst { get; set; }

        private List<SuspendedAPI>? ListaSuspended;

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        public SuspendedViewModel()
        {
            SuspendedAPILst = new ObservableCollection<SuspendedAPI>();
            ListaSuspended = new List<SuspendedAPI>();
        }



        public async Task LoadSuspended()
        {
            IsLoading = true;

            SuspendedAPILst!.Clear();

            var responseHttp = await Repository.Get<List<SuspendedAPI>>("/api/suspended/suspendedList");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            ListaSuspended = responseHttp.Response;
            if (ListaSuspended != null || ListaSuspended!.Count > 0)
            {
                foreach (var item in ListaSuspended)
                {
                    SuspendedAPILst!.Add(item);
                }
            }

            IsLoading = false;
        }

        public async Task SearchTxt(string txtbuscar)
        {
            if (string.IsNullOrEmpty(txtbuscar))
            {
                await LoadSuspended();
            }
            else
            {
                var Lista = ListaSuspended!.Where(x => x.NombreCliente!.ToLower().Contains(txtbuscar.ToLower())).ToList();
                SuspendedAPILst?.Clear();
                if (Lista != null || Lista!.Count > 0)
                {
                    foreach (var item in Lista)
                    {
                        SuspendedAPILst!.Add(item);
                    }
                }
            }
        }

        [RelayCommand]
        public async Task SuspendedToActive(int IdSuspended)
        {
            IsLoading = true;

            var responseHttp = await Repository.Get<SuspendedActiveAPI>($"/api/suspended/toactive/{IdSuspended}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SuspendedActiveAPI DatoEliminar = responseHttp.Response;

            //Verificamos el Ping al servidor para ver si responde y continuar
            Ping ping = new();
            PingReply reply = await ping.SendPingAsync(DatoEliminar.IpNetwork!, 3000);
            if (reply.Status != IPStatus.Success)
            {
                // El ping fue exitoso
                IsLoading = false;
                MessageBox.Show($"Fallo el ping a {DatoEliminar.IpNetwork}. Estado: {reply.Status}", "Respuesta Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Termina prueba del Ping


            ////////////////////////////////////////////////////////////
            MK mikrotik = new MK(DatoEliminar.IpNetwork!, DatoEliminar.ApiPort);
            if (!mikrotik.Login(DatoEliminar.Usuario!, DatoEliminar.Clave!))
            {
                IsLoading = false;
                MessageBox.Show($"Error en la Conexion al Servidor Mikrotik", "Error Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            mikrotik.Send("/ip/hotspot/ip-binding/set");
            mikrotik.Send("=.id=" + DatoEliminar.IdIpBinding);
            mikrotik.Send("=type=" + "bypassed");
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

            var responseHttp2 = await Repository.Delete($"/api/suspended/toDelete/{IdSuspended}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
           
            await LoadSuspended();

            IsLoading = false;
            MessageBox.Show($"El Cliente se ha Activado con Exitro", "Confirmacion Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
