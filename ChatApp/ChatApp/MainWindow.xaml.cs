using System.Windows;
using System.Windows.Input;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void closeWindows()
        {
            
        }
        public void Init()
        {
            InitializeComponent();
        }
        private void Move(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}