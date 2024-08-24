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
    }
}
