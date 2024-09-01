using SpiWpf.Entities.DTOs;
using SpiWpf.Wpf.ViewModels;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CutControlDetailPage.xaml
    /// </summary>
    public partial class CutControlDetailPage : UserControl
    {
        private CutControlDetailViewModel _viewModel { get; set; } 

        public CutControlDetailPage()
        {
            InitializeComponent();
            _viewModel = new CutControlDetailViewModel();
            DataContext = _viewModel;
        }


    }
}
