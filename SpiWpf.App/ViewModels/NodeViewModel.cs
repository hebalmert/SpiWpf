using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using SpiWpf.Wpf.Helpers;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class NodeViewModel : ObservableObject
    {
        public ObservableCollection<NodeAPI>? NodeAPILst { get; set; }

        private List<NodeAPI>? ListaNodos;

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        private CallPing _DoPing = new();

        public NodeViewModel()
        {
            NodeAPILst = new ObservableCollection<NodeAPI>();
            ListaNodos = new List<NodeAPI>();
        }

        public async Task LoadNodes()
        {
            IsLoading = true;

            NodeAPILst!.Clear();

            var responseHttp = await Repository.Get<List<NodeAPI>>("/api/nodes/NodeLista");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            ListaNodos = responseHttp.Response;
            if (ListaNodos != null || ListaNodos!.Count > 0)
            {
                foreach (var item in ListaNodos)
                {
                    NodeAPILst!.Add(item);
                }
            }
            IsLoading = false;
        }

        public async Task SearchTxt(string txtbuscar)
        {
            if (string.IsNullOrEmpty(txtbuscar))
            {
                var ListaSuspended = ListaNodos!.ToList();
                NodeAPILst?.Clear();
                if (ListaSuspended != null || NodeAPILst!.Count > 0)
                {
                    foreach (var item in ListaSuspended!)
                    {
                        NodeAPILst!.Add(item);
                    }
                }
            }
            else
            {
                var ListaSuspended = ListaNodos!.Where(x => x.NodesName!.ToLower().Contains(txtbuscar.ToLower())).ToList();
                NodeAPILst?.Clear();
                if (ListaSuspended != null || NodeAPILst!.Count > 0)
                {
                    foreach (var item in ListaSuspended!)
                    {
                        NodeAPILst!.Add(item);
                    }
                }
            }
        }

        [RelayCommand]
        public void PingMK(string IdNode)
        {
            _DoPing.PingIp(IdNode);
        }
    }
}
