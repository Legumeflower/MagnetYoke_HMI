using MagnetYoke_HMI;
using MVVM_Core;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using Logger;

namespace FM2002.Model
{
    public class COM_Parameter : ViewModelBase
    {
        public string PortName
        {
            get => GetF<string>();
            set => SetF(value);
        }
        public int BaudRate
        {
            get => GetF<int>();
            set => SetF(value);
        }
        public System.IO.Ports.Handshake Handshake
        {
            get => GetF<Handshake>();
            set => SetF(value);
        }
        public Parity Parity
        {
            get => GetF<Parity>();
            set => SetF(value);
        }
        public int DataBits
        {
            get => GetF<int>();
            set => SetF(value);
        }
        public StopBits StopBits
        {
            get => GetF<StopBits>();
            set => SetF(value);
        }
        public int ReadTimeout
        {
            get => GetF<int>();
            set => SetF(value);
        }
        public int WriteTimeout
        {
            get => GetF<int>();
            set => SetF(value);
        }

        public COM_Parameter(string _portName = "COM14",
                             int _baudrate = 9600,
                             Handshake _handshake = Handshake.None,
                             Parity _parity = Parity.None,
                             int _databits = 8,
                             StopBits _stopbits = StopBits.One,
                             int _ReadTimeout = 100,
                             int _WriteTimeout = 100)
        {
            PortName = _portName;
            BaudRate = _baudrate;
            Handshake = _handshake;
            Parity = _parity;
            DataBits = _databits;
            StopBits = _stopbits;
            ReadTimeout = _ReadTimeout;
            WriteTimeout = _WriteTimeout;
        }
    }

    public class RS232_Lib : ViewModelBase
    {

        //private Debug_Tool myDebug = App.Current.Resources["myDebug"] as Debug_Tool;

        public RS232_Lib()
        {
            COM_Param = new COM_Parameter();

            COM = new SerialPort()
            {
                PortName = COM_Param.PortName,
                BaudRate = COM_Param.BaudRate,
                DataBits = COM_Param.DataBits,
                StopBits = COM_Param.StopBits,
                ParityReplace = 0,
            };

            Set_RelayCommand();
        }

        public SerialPort COM
        {
            get => GetF<SerialPort>();
            set => SetF(value);
        }

        //COM Parameter
        public COM_Parameter COM_Param
        {
            get => GetF<COM_Parameter>();
            set => SetF(value);
        }

        //IsConnect
        public bool IsConnect
        {
            get { return (COM != null && COM.IsOpen); }
        }

        //RelayCommand
        public void Set_RelayCommand()
        {
            Relay_Open = new RelayCommand(OpenByRelay);
            Relay_Close = new RelayCommand(Close);
            Relay_OpenClose = new RelayCommand(OpenClose);
        }

        //Open COM
        public RelayCommand Relay_Open { get; set; }
        private void OpenByRelay(object obj)
        {
            Open();
        }
        public bool Open()
        {
            try
            {
                COM = new SerialPort()
                {
                    //PortName = COM_Param.PortName,
                    //BaudRate = COM_Param.BaudRate,
                    //Handshake = COM_Param.Handshake,
                    //Parity = COM_Param.Parity,
                    //DataBits = COM_Param.DataBits,
                    //StopBits = COM_Param.StopBits,
                    //ReadTimeout = COM_Param.ReadTimeout,
                    //WriteTimeout = COM_Param.WriteTimeout,
                    PortName = COM_Param.PortName,
                    BaudRate = COM_Param.BaudRate,
                    DataBits = COM_Param.DataBits,
                    StopBits = COM_Param.StopBits,
                    ParityReplace = 0,
                    RtsEnable = true,
                    DtrEnable = true,
                };
                COM.Open();

                COM.ErrorReceived += COM_ErrorReceived;
                Log_App.WriteLine($"Open {COM.PortName} OK");
            }
            catch
            {
                Log_App.WriteLine($"Open {COM.PortName} Fail");
            }

            OnPropertyChanged("IsConnect");
            return COM.IsOpen;
        }

        private void COM_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Log_App.WriteLine($"{e.ToString()}");
        }

        //Close COM
        public RelayCommand Relay_Close { get; set; }
        public void Close(object obj)
        {
            try
            {
                COM.ErrorReceived -= COM_ErrorReceived;
                COM.Close();
                COM.Dispose();
                Log_App.WriteLine($"Close {COM.PortName} OK");
            }
            catch (Exception e)
            {
                Log_App.WriteLine($"Close {COM.PortName} Fail, {e.Message}");
            }

            OnPropertyChanged("IsConnect");
                
        }

        //Open or Close COM
        public RelayCommand Relay_OpenClose { get; set; }
        public void OpenClose(object obj)
        {
            if (COM is null || !COM.IsOpen)
            {
                Open();
            }
            else
            {
                Close(obj);
            }
        }
        //Write
        object Sendlocker = new object();

        public RelayCommand Relay_Write { get; set; }

        public void Write(object obj)
        {
            if (COM == null || !COM.IsOpen || obj is null) return;

            byte[] write_stream = null;
            if (obj is string)
            {

                write_stream = Encoding.UTF8.GetBytes((string)obj);
            }
            else
            {
                write_stream = ObjectToBytes(obj);
            }

            lock (Sendlocker)
            {
                COM.Write(write_stream, 0, write_stream.Length);
            }
        }

        //Write_Read
        public byte[] Write_Read(byte[] write_stream)
        {
            if (COM == null || !COM.IsOpen) return null;

            //清掉之前沒有讀取的資料
            if (COM.BytesToRead > 0)
            {
                byte[] Discard_buf = new byte[COM.BytesToRead];
                COM.Read(Discard_buf, 0, Discard_buf.Length);
            }


            lock (Sendlocker)
            {
                COM.Write(write_stream, 0, write_stream.Length);
            }

            Thread.Sleep(100);

            int count = COM.BytesToRead;
            byte[] read_buf = new byte[count];
            COM.Read(read_buf, 0, read_buf.Length);

            return read_buf;

        }

        //Read
        public byte[] Read()
        {
            if (COM == null || !COM.IsOpen) return null;

            byte[] read_buf = null;
            if (COM.BytesToRead > 0)
            {
                read_buf = new byte[COM.BytesToRead];
                COM.Read(read_buf, 0, read_buf.Length);
            }

            return read_buf;
        }

        //Object Serialize
        public static byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }
        public static object BytesToObject(byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }
    }

}
