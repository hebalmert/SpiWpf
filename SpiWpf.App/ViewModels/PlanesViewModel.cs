using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public class PlanesViewModel : ObservableObject
    {
        public ObservableCollection<PlanAPI>? PlanAPILst { get; set; }

        public PlanesViewModel()
        {
            PlanAPILst = new ObservableCollection<PlanAPI>();   
        }

        public async Task LoadPlanes()
        {
            PlanAPILst!.Clear();

            var responseHttp = await Repository.Get<List<PlanAPI>>("/api/plans/PlanList");
            if (responseHttp.Error)
            {
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            List<PlanAPI> ListaPlanes = responseHttp.Response;
            if (ListaPlanes != null || ListaPlanes!.Count > 0)
            {
                foreach (var item in ListaPlanes)
                {
                    PlanAPILst!.Add(item);
                }
            }
        }
    }
}
