using SpiWpf.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ContractPage.xaml
    /// </summary>
    public partial class ContractPage : UserControl
    {
        private ContractViewModel _ViewModel {  get; set; }

        public ContractPage()
        {
            InitializeComponent();
            _ViewModel = new ContractViewModel();
            DataContext = _ViewModel;
            this.Loaded += LoadContractView;
        }

        private async void LoadContractView(object sender, RoutedEventArgs e)
        {
            await _ViewModel.LoadContratos();
            this.UpdateLayout();
        }

        private async void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var texto = sender as TextBox;
            string txtBuscar = texto!.Text;
            if (txtBuscar == null && txtBuscar!.Length == 0) { return; }

            await _ViewModel.SearchTxt(txtBuscar!);
        }
    }
}
