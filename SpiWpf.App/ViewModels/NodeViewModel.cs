using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public class NodeViewModel : ObservableObject
    {
        public ObservableCollection<NodeAPI>? NodeAPILst { get; set; }

        public NodeViewModel()
        {
            NodeAPILst = new ObservableCollection<NodeAPI>();
        }

        public async Task LoadNodes()
        {
            NodeAPILst!.Clear();

            var responseHttp = await Repository.Get<List<NodeAPI>>("/api/nodes/NodeLista");
            if (responseHttp.Error)
            {
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            List<NodeAPI> ListaNodes = responseHttp.Response;
            if (ListaNodes != null || ListaNodes!.Count > 0)
            {
                foreach (var item in ListaNodes)
                {
                    NodeAPILst!.Add(item);
                }
            }
        }
    }
}
