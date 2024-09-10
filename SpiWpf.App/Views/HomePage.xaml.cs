using SpiWpf.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        private HomeViewModel _viewModel {  get; set; }

        public HomePage()
        {
            InitializeComponent();
            _viewModel = new HomeViewModel();
            DataContext = _viewModel;
            this.Loaded += LoadDatosView;
        }

        private async void LoadDatosView(object sender, RoutedEventArgs e)
        {
           await _viewModel.LoadDatos();
        }
    }
}
