using SpiWpf.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for SuspendedPage.xaml
    /// </summary>
    public partial class SuspendedPage : UserControl
    {
        private SuspendedViewModel _viewModel {  get; set; }

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
