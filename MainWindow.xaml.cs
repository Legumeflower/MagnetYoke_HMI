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
using RCAPINet;
using Tc.Model;

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

            //Epson_Robot_Initial();
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
                var sts = sc_spel.ErrorOn;
                
                sc_spel.RunDialog(SpelDialogs.RobotManager);
            }
            catch(Exception ex) 
            {
                Console.Write(ex.Message);
            }

        }

        private PLC_app PLC_temp;
        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            PLC_temp = new PLC_app();

            PLC_temp.ADS_COM.NetID = "192.168.0.4.1.1";

            PLC_temp.ADS_COM.Port = 851;

            PLC_temp.Initial();
            //PLC_temp = await PLC_app.Create_PLC_app("192.168.0.4.1.1",851);
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(333);
            await PLC_temp.ADS_COM.Write("Valve_Param.Valve_Array[1].Open_Time", t);
        }
    }
}