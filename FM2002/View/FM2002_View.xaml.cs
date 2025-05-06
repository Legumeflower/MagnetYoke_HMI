using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
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
using UserControl = System.Windows.Controls.UserControl;

namespace FM2002.View
{
    /// <summary>
    /// FM2002_View.xaml 的互動邏輯
    /// </summary>
    public partial class FM2002_View : UserControl
    {
        public ObservableCollection<int> BaudRateOptions { get; set; }
        public ObservableCollection<Handshake> HandshakeOptions { get; set; }
        public ObservableCollection<Parity> ParityOptions { get; set; }
        public ObservableCollection<int> DataBitsOptions { get; set; }
        public ObservableCollection<StopBits> StopBitsOptions { get; set; }


        public FM2002_View()
        {
            BaudRateOptions = new ObservableCollection<int>() { 921600, 460800, 230400, 115200, 57600, 38400, 19200, 9600, 4800, 2400, 1200 };

            HandshakeOptions = new ObservableCollection<Handshake>(Enum.GetValues(typeof(Handshake)) as Handshake[]);

            ParityOptions = new ObservableCollection<Parity>(Enum.GetValues(typeof(Parity)) as Parity[]);

            DataBitsOptions = new ObservableCollection<int>() { 8, 7, 6, 5 };

            StopBitsOptions = new ObservableCollection<StopBits>(Enum.GetValues(typeof(StopBits)) as StopBits[]);


            InitializeComponent();

        }
    }
}
