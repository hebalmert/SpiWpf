using System.Windows;
using SpiWpf.Wpf;
using SpiWpf.Wpf.Views;

namespace SpiWpf.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Application.Current.Properties["urlBase"] = "https://spi.nexxtplanet.net";
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var LoginPage = new LoginPage();
            LoginPage.Show();
            LoginPage.IsVisibleChanged += (s, ev) => 
            {
                if (LoginPage.IsVisible == false && LoginPage.IsLoaded)
                { 
                    var mainview = new MainPage();
                    mainview.Show();    
                    LoginPage.Close();
                }
            };
        }
    }

}
