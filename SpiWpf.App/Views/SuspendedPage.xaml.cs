using SpiWpf.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for SuspendedPage.xaml
    /// </summary>
    public partial class SuspendedPage : UserControl
    {
        private SuspendedViewModel _viewModel { get; set; }

        public SuspendedPage()
        {
            InitializeComponent();
            _viewModel = new SuspendedViewModel();
            DataContext = _viewModel;
            this.Loaded += LoadSuspendedView;
        }

        private async void LoadSuspendedView(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadSuspended();
            this.UpdateLayout();
        }

        private async void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var texto = sender as TextBox;
            string txtBuscar = texto!.Text;
            if (txtBuscar == null && txtBuscar!.Length == 0) { return; }

            await _viewModel.SearchTxt(txtBuscar!);
        }
    }
}
