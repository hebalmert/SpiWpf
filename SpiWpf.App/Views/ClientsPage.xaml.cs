using SpiWpf.Wpf.ViewModels;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : UserControl
    {
        private ClientsViewModel _ViewModel {  get; set; }

        public ClientsPage()
        {
            InitializeComponent();
            _ViewModel = new ClientsViewModel();
            DataContext = _ViewModel;
            this.Loaded += LoadClientView;
        }

        private async void LoadClientView(object sender, RoutedEventArgs e)
        {
            await _ViewModel.LoadCLients();
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
