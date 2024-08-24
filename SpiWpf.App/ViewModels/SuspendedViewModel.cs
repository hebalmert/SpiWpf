using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public class SuspendedViewModel : ObservableObject
    {
        public ObservableCollection<SuspendedAPI>? SuspendedAPILst { get; set; }

        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText!, value); }
        }

        public SuspendedViewModel()
        {
            SuspendedAPILst = new ObservableCollection<SuspendedAPI>();
        }



        public async Task LoadSuspended()
        {
            SuspendedAPILst!.Clear();

            var responseHttp = await Repository.Get<List<SuspendedAPI>>("/api/suspended/suspendedList");
            if (responseHttp.Error)
            {
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Convertimos el List a un ObservableCollection
            List<SuspendedAPI> ListaSuspended = responseHttp.Response;
            if (ListaSuspended != null || ListaSuspended!.Count > 0)
            {
                foreach (var item in ListaSuspended)
                {
                    SuspendedAPILst!.Add(item);
                }
            }
        }

        public async Task SearchTxt(string txtbuscar)
        {
            if (string.IsNullOrEmpty(txtbuscar))
            {
                await LoadSuspended();
            }
            else
            {
                var ListaSuspended = SuspendedAPILst!.Where(x => x.NombreCliente!.ToLower().Contains(txtbuscar.ToLower())).ToList();
                SuspendedAPILst?.Clear();
                if (ListaSuspended != null || ListaSuspended!.Count > 0)
                {
                    foreach (var item in ListaSuspended)
                    {
                        SuspendedAPILst!.Add(item);
                    }
                }
            }
        }
    }
}
