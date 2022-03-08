using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OrganizationBankingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Value { get; set; }

        private readonly string _mapper;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(int value, string mapper)
        {
            Value = value;
            _mapper = mapper;
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
