using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Data;
using SpiWpf.Entities.Models;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    class ContractDetailsViewModel : ObservableObject
    {
        private static int? DatoPrueba;

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

        public ContractDetailsViewModel()
        {

        }

        public void PaseParametro(int value)
        {
            DatoPrueba = value;
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

            return;
        }
    }
}
