using SpiWpf.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : UserControl
    {
        private ClientsViewModel _ClientsViewModel {  get; set; }

        public ClientsPage()
        {
            InitializeComponent();
            _ClientsViewModel = new ClientsViewModel();
            DataContext = _ClientsViewModel;
            this.Loaded += LoadClientView;
        }

        private async void LoadClientView(object sender, RoutedEventArgs e)
        {
            await _ClientsViewModel.LoadCLients();
            this.UpdateLayout();
           
        }
    }
}
