using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagnetYoke_HMI;
using System.Timers;
using MVVM_Core;
using Timer = System.Timers.Timer;

namespace FM2002.ViewModel
{
    public class FM2002_ViewModel : ViewModelBase
    {
        public FM2002_ViewModel()
        {
            myRs232 = new Model.RS232_Lib();

            Meter_Value = 0;

            DataQueue = new List<byte>();

            //myRs232.COM.DataReceived += COM_DataReceived;

            //Read_Timer = new DispatcherTimer();
            //Read_Timer.Interval = TimeSpan.FromMilliseconds(100);
            //Read_Timer.Tick += Read_Timer_Tick;
            //Read_Timer.Start();

            Read_Timer = new Timer(100);
            Read_Timer.Elapsed += Read_Timer_Elapsed;
            Read_Timer.Start();
        }



        private void COM_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public Model.RS232_Lib myRs232
        {
            get => GetF<Model.RS232_Lib>();
            set => SetF(value);
        }

        public float Meter_Value
        {
            get => GetF<float>();
            set
            {
                Meter_Value_str = value.ToString("000.00");
                SetF(value);
            }
        }

        public string Meter_Value_str
        {
            get => GetF<string>();
            set => SetF(value);
        }


        Timer Read_Timer;

        private List<byte> DataQueue;

        private byte Data_Seperator = 0x0D;

        private void Read_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (myRs232.IsConnect && myRs232.COM.BytesToRead > 0)
            {
                byte[] new_data = myRs232.Read();

                for (var i = 0; i < new_data.Length; i++)
                {
                    new_data[i] &= 0x7F;

                    if (new_data[i] == Data_Seperator)
                    {
                        Meter_Value = CalculateValue(DataQueue);
                        DataQueue.Clear();
                    }
                    else
                    {
                        DataQueue.Add(new_data[i]);
                    }
                }
            }
        }

        private void Read_Timer_Tick(object sender, EventArgs e)
        {
            if (myRs232.IsConnect && myRs232.COM.BytesToRead > 0)
            {
                byte[] new_data = myRs232.Read();

                for (var i = 0; i < new_data.Length; i++)
                {
                    new_data[i] &= 0x7F;

                    if (new_data[i] == Data_Seperator)
                    {
                        // 使用 Invoke 更新介面
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Meter_Value = CalculateValue(DataQueue);
                        });

                        //清除buffer
                        DataQueue.Clear();
                    }
                    else
                    {
                        DataQueue.Add(new_data[i]);
                    }
                }
            }
        }

        private void COM_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte[] new_data = myRs232.Read();
            byte tmp = 0;

            for (var i = 0; i < new_data.Length; i++)
            {
                new_data[i] &= 0x7F;

                if (new_data[i] == Data_Seperator)
                {
                    // 使用 Invoke 更新介面
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Meter_Value = CalculateValue(DataQueue);
                    });

                    DataQueue.Clear();
                }
                else
                {
                    DataQueue.Add(new_data[i]);
                }
            }
        }

        private float CalculateValue(List<byte> data)
        {
            string str_rst = System.Text.Encoding.ASCII.GetString(data.ToArray());
            float float_rst;

            if (float.TryParse(str_rst, out float_rst))
                return float_rst;
            else
                return 0;

        }

    }

}
