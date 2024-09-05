using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public class ClientsViewModel : ObservableObject
    {
        public ObservableCollection<ClientsAPI>? ClientAPILst { get; set; }

        private List<ClientsAPI>? ListaClientes;

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        public ClientsViewModel()
        {
            ClientAPILst = new ObservableCollection<ClientsAPI>();
            ListaClientes = new List<ClientsAPI>();
        }

        public async Task LoadCLients() 
        {
            IsLoading = true;


            ClientAPILst!.Clear();

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
            if (ListaClientes != null || ListaClientes!.Count > 0)
            {
                foreach (var item in ListaClientes)
                {
                    ClientAPILst!.Add(item);
                }
            }

            IsLoading = false;
        }

        public async Task SearchTxt(string txtbuscar)
        {
            if (string.IsNullOrEmpty(txtbuscar))
            {
                var ListaClient = ListaClientes!.ToList();
                ClientAPILst?.Clear();
                if (ListaClient != null || ListaClient!.Count > 0)
                {
                    foreach (var item in ListaClient!)
                    {
                        ClientAPILst!.Add(item);
                    }
                }
            }
            else
            {
                var ListaSuspended = ListaClientes!.Where(x => x.FullName!.ToLower().Contains(txtbuscar.ToLower())).ToList();
                ClientAPILst?.Clear();
                if (ListaSuspended != null || ClientAPILst!.Count > 0)
                {
                    foreach (var item in ListaSuspended!)
                    {
                        ClientAPILst!.Add(item);
                    }
                }
            }
        }
    }
}
