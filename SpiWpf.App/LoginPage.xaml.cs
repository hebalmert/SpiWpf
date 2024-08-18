using SpiWpf.Wpf.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SpiWpf.Wpf
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private LoginViewModel _ViewModel { get; set; }

        public LoginPage()
        {
            InitializeComponent();
            _ViewModel = new LoginViewModel();
            DataContext = _ViewModel;
        }


        //Esto es para poder hacer click sostenido en la ventana y poderla mover a cualquier lugar
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
