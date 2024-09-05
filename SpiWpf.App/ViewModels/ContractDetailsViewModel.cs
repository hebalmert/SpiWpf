using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using SpiWpf.Wpf.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class ContractDetailsViewModel : ObservableObject
    {
        private static int? DatoPrueba;

        public ObservableCollection<QueuesInfoCls>? QueLst { get; set; }

        public ObservableCollection<BindingInfoCls>? BinLst { get; set; }

        private ContractDetailCls? _ContractDetalle;
        public ContractDetailCls? ContractDetalle
        {
            get { return _ContractDetalle; }
            set
            {
                SetProperty(ref _ContractDetalle, value);
            }
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        private bool _ExistQueues;
        public bool ExistQueues
        {
            get { return _ExistQueues; }
            set { SetProperty(ref _ExistQueues, value); }
        }

        private bool _ExistBind;
        public bool ExistBind
        {
            get { return _ExistBind; }
            set { SetProperty(ref _ExistBind, value); }
        }

        public ContractDetailsViewModel()
        {
            QueLst = new ObservableCollection<QueuesInfoCls>();
            BinLst = new ObservableCollection<BindingInfoCls>();
            ContractDetalle = new ContractDetailCls();
        }

        public void PaseParametro(int value)
        {
            DatoPrueba = value;
        }

        [RelayCommand]
        public void VolverBoton()
        {
            var mainWindow = Application.Current.MainWindow as MainPage;
            var viewModel = mainWindow!.DataContext as MainViewModel;
            viewModel!.LoadContracts();
        }

        public async void LoadContractDetails()
        {
            IsLoading = true;

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<ContractDetailCls>($"/api/contracts/GetContractDetails/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ContractDetalle = responseHttp.Response;

            LoadQueueInfo();
            LoadBindInfo();

            IsLoading = false;

            return;
        }

        public async void LoadQueueInfo()
        {

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<List<QueuesInfoCls>>($"/api/contracts/GetContractQue/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var listaQue = responseHttp.Response;
            ExistQueues = listaQue.Count > 0 ? true : false;
            QueLst?.Clear();
            if (listaQue != null || listaQue!.Count > 0)
            {
                foreach (var item in listaQue!)
                {
                    QueLst!.Add(item);
                }
            }

            return;
        }

        public async void LoadBindInfo()
        {

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<List<BindingInfoCls>>($"/api/contracts/GetContractBin/{DatoPrueba}");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var listaBin = responseHttp.Response;
            ExistBind = listaBin.Count > 0 ? true : false;
            BinLst?.Clear();
            if (listaBin != null || listaBin!.Count > 0)
            {
                foreach (var item in listaBin!)
                {
                    BinLst!.Add(item);
                }
            }

            return;
        }
    }
}
