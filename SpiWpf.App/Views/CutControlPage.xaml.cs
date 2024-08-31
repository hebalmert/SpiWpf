using SpiWpf.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CutControlPage.xaml
    /// </summary>
    public partial class CutControlPage : UserControl
    {
        private CutControlViewModel _viewModel { get; set; }


        public CutControlPage()
        {
            InitializeComponent();
            _viewModel = new CutControlViewModel();
            DataContext = _viewModel;
            this.Loaded += LoadCutControlView;
        }

        private async void LoadCutControlView(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadCutControl();
            this.UpdateLayout();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
