using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using RCAPINet;

namespace MagnetYoke_HMI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Epson_Robot_Initial();
        }

        private RCAPINet.Spel sc_spel;

        private void Epson_Robot_Initial()
        {
            sc_spel = new RCAPINet.Spel();
            sc_spel.Initialize();
            sc_spel.EventReceived += Sc_spel_EventReceived;
            sc_spel.Project = @"E:\Work_APPLY\Magnet Yoke\Robot\APITest\APITest.sprj";
            sc_spel.Connect("LSB-Virtual");

            
        }

        private void Sc_spel_EventReceived(object sender, SpelEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var Pos = sc_spel.GetRobotPos(SpelRobotPosType.World, 0, 0, 0);
                var Ja = sc_spel.GetRobotPos(SpelRobotPosType.Joint,0,0,0);
                sc_spel.RunDialog(SpelDialogs.RobotManager);
            }
            catch(Exception ex) 
            {
                Console.Write(ex.Message);
            }

        }
    }
}