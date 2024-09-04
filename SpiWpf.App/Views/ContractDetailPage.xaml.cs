using SpiWpf.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ContractDetailPage.xaml
    /// </summary>
    public partial class ContractDetailPage : UserControl
    {
        private ContractDetailsViewModel _viewmodel {  get; set; }

        public ContractDetailPage()
        {
            InitializeComponent();
            _viewmodel = new ContractDetailsViewModel();
            DataContext = _viewmodel;
            this.Loaded += LoadContractView;
        }

        private void LoadContractView(object sender, RoutedEventArgs e)
        {
            _viewmodel.LoadContractDetails();
        }
    }
}
