using System.Windows;
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


        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            //GradientStopCollection gsc = new GradientStopCollection();
            //gsc.Add(new GradientStop()
            //{
            //    Color = Colors.Red,
            //    Offset = 0.0
            //});
            //gsc.Add(new GradientStop()
            //{
            //    Color = Colors.Black,
            //    Offset = 0.5
            //});
            //myButton.Background = new LinearGradientBrush(gsc, new Point(0.5, 0), new Point(0.5, 1));
        }
    }
}
