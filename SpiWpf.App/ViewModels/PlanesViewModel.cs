using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public class PlanesViewModel : ObservableObject
    {
        public ObservableCollection<PlanAPI>? PlanAPILst { get; set; }

        private List<PlanAPI>? ListaPlanes;

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        public PlanesViewModel()
        {
            PlanAPILst = new ObservableCollection<PlanAPI>();
            ListaPlanes = new List<PlanAPI>();
        }

        public async Task LoadPlanes()
        {
            IsLoading = true;

            PlanAPILst!.Clear();

            var responseHttp = await Repository.Get<List<PlanAPI>>("/api/plans/PlanList");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            ListaPlanes = responseHttp.Response;
            if (ListaPlanes != null || ListaPlanes!.Count > 0)
            {
                foreach (var item in ListaPlanes)
                {
                    PlanAPILst!.Add(item);
                }
            }
            IsLoading = false;
        }

        public async Task SearchTxt(string txtbuscar)
        {
            if (string.IsNullOrEmpty(txtbuscar))
            {
                await LoadPlanes();
            }
            else
            {
                var ListaSuspended = ListaPlanes!.Where(x => x.PlanName!.ToLower().Contains(txtbuscar.ToLower())).ToList();
                PlanAPILst?.Clear();
                if (ListaSuspended != null || PlanAPILst!.Count > 0)
                {
                    foreach (var item in ListaSuspended!)
                    {
                        PlanAPILst!.Add(item);
                    }
                }
            }
        }
    }
}
