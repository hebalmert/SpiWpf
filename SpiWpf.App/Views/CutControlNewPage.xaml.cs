using SpiWpf.Wpf.ViewModels;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CutControlNewPage.xaml
    /// </summary>
    public partial class CutControlNewPage : UserControl
    {
        private CutControlNewViewModel _viewModel { get; set; }

        public CutControlNewPage()
        {
            InitializeComponent();
            _viewModel = new CutControlNewViewModel();
            DataContext = _viewModel;
        }


    }
}
