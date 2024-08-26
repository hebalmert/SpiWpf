using SpiWpf.Wpf.ViewModels;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for SuspendedNewPage.xaml
    /// </summary>
    public partial class SuspendedNewPage : UserControl
    {
        private SuspendedNewViewModel _viewModel {  get; set; }

        public SuspendedNewPage()
        {
            InitializeComponent();
            _viewModel = new SuspendedNewViewModel();
            DataContext = _viewModel;
        }

    }
}
