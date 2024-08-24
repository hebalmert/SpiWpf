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
    /// Interaction logic for ServerPage.xaml
    /// </summary>
    public partial class ServerPage : UserControl
    {
        private ServerViewModel _ServerView {  get; set; }

        public ServerPage()
        {
            InitializeComponent();
            _ServerView = new ServerViewModel();
            DataContext = _ServerView;
            this.Loaded += LoadClientView;
        }

        private async void LoadClientView(object sender, RoutedEventArgs e)
        {
            await _ServerView.LoadServers();
            this.UpdateLayout();

        }
    }
}
