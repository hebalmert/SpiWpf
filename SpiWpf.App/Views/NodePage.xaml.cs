using SpiWpf.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for NodePage.xaml
    /// </summary>
    public partial class NodePage : UserControl
    {
        private NodeViewModel _viewModel { get; set; }

        public NodePage()
        {
            InitializeComponent();
            _viewModel = new NodeViewModel();
            DataContext = _viewModel;
            this.Loaded += LoadNodesView;
        }

        private async void LoadNodesView(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadNodes();
            this.UpdateLayout();

        }
    }
}
