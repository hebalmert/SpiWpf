using SpiWpf.Wpf.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SpiWpf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        private MainViewModel _viewModel { get; set; }

        public MainPage()
        {
            InitializeComponent();
            //Igualamos el MainWindows a MainPage, para poderlo usar desde
            //cualquier UserControler y Cerrar o Llamar otros UserControles.
            Application.Current.MainWindow = this;

            //para maximizado de la pantalla en cualquier monitor
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        //estos metodos son para maximizar la pantalla en cualquier ventana
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr bWnd, int wMsg, int wParam, int lParam);

        private void pnlControlBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void pnlControlBar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMiximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            { 
                this.WindowState = WindowState.Normal;
            }
        }
    }
}
