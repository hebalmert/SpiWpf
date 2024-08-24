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

        public ClientsViewModel()
        {
            ClientAPILst = new ObservableCollection<ClientsAPI>();
        }

        public async Task LoadCLients() 
        {
            ClientAPILst!.Clear();

            var responseHttp = await Repository.Get<List<ClientsAPI>>("/api/clients/ClienteList");
            if (responseHttp.Error)
            { 
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            List<ClientsAPI> ListaCliente = responseHttp.Response;
            if (ListaCliente != null || ListaCliente!.Count > 0)
            {
                foreach (var item in ListaCliente)
                {
                    ClientAPILst!.Add(item);
                }
            }
        }

    }
}
