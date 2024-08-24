using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public class ServerViewModel : ObservableObject
    {
        public ObservableCollection<ServerAPI>? ServerAPILst { get; set; }

        public ServerViewModel()
        {
            ServerAPILst = new ObservableCollection<ServerAPI>();
        }

        public async Task LoadServers()
        {
            ServerAPILst!.Clear();

            var responseHttp = await Repository.Get<List<ServerAPI>>("/api/servers/ServerList");
            if (responseHttp.Error)
            {
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            List<ServerAPI> ListaServer = responseHttp.Response;
            if (ListaServer != null || ListaServer!.Count > 0)
            {
                foreach (var item in ListaServer)
                {
                    ServerAPILst!.Add(item);
                }
            }
        }


    }
}
