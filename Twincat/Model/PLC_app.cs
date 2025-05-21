using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Tc.Communication;
using static Tc.Model.PLC_Parameter;
using TwinCAT.Ads.TypeSystem;
using System.Timers;
using Timer = System.Timers.Timer;
using Microsoft.VisualBasic;
using static Tc.Model.PLC_Parameter.Valve_Ctrl_Struct;
using static Tc.Model.PLC_Parameter.Axis_Param_Struct.Home_Param_Struct;
using static Tc.Model.PLC_Parameter.Axis_Ctrl_Struct;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MVVM_Core;
using System.Configuration;
using static Tc.Model.PLC_Parameter.Feeder_Struct;

namespace Tc.Model
{
    public class PLC_Parameter
    {
        //public PLC_Parameter(GVL_Struct _gvl)
        //{
        //    GVL = _gvl;
        //    Initial();
        //}

        public PLC_Parameter()
        {
            GVL = new GVL_Struct() { X_SIZE = 128, Y_SIZE = 128, Valve_SIZE = 16, AXIS_FIRST = 1, AXIS_LAST = 1};

            Update_GVL_text = string.Join(";", GVL.Get_ReadBlock_text());

            Initial();
        }
        public void Initial()
        {
            List<string> block_str_list = new List<string>();

            //SYSTEM Data
            SYSTEM_VAR = new SYSTEM_VAR_Struct();

            //IO Data
            IO_Param = new IO_Param_Struct() { X = new ObservableCollection<bool>() , Y = new ObservableCollection<bool>() };
            block_str_list.AddRange(IO_Param.Get_ReadBlock_text());

            //Valve Setting
            Valve_Setting_Array = new ObservableCollection<Valve_Setting_Struct>();
            for (int i = 1; i <= GVL.Valve_SIZE; i++)
            {
                var setting = new Valve_Setting_Struct() { ID = i };
                Valve_Setting_Array.Add(setting);
                block_str_list.AddRange(setting.Get_ReadBlock_text());
            }

            //Valve Control
            Valve_Ctrl_Array = new ObservableCollection<Valve_Ctrl_Struct>();
            for (int i = 1; i <= GVL.Valve_SIZE; i++)
            {
                var ctrl = new Valve_Ctrl_Struct() { ID = i };
                Valve_Ctrl_Array.Add(ctrl);
                block_str_list.AddRange(ctrl.Get_ReadBlock_text());
            }

            //Axis Status
            Axis_Status_Array = new ObservableCollection<Axis_Status>();
            for (int i = GVL.AXIS_FIRST; i <= GVL.AXIS_LAST; i++)
            {
                var status = new Axis_Status() { ID = i };
                Axis_Status_Array.Add(status);
                block_str_list.AddRange(status.Get_ReadBlock_text());
            }

            //Axis PERSISTENT Parameter
            Axis_Param_Array = new ObservableCollection<Axis_Param_Struct>();
            for (int i = GVL.AXIS_FIRST; i <= GVL.AXIS_LAST; i++)
            {
                var param = new Axis_Param_Struct() { ID = i };
                Axis_Param_Array.Add(param);
                block_str_list.AddRange(param.Get_ReadBlock_text());
            }

            //Axis Control
            Axis_Ctrl_Array = new ObservableCollection<Axis_Ctrl_Struct>();
            for (int i = GVL.AXIS_FIRST;  i <= GVL.AXIS_LAST; i++)
            {
                var ctrl = new Axis_Ctrl_Struct() { ID = i };
                Axis_Ctrl_Array.Add(ctrl);
                block_str_list.AddRange(ctrl.Get_ReadBlock_text());
            }

            Feeder_Param = new Feeder_Struct();
            block_str_list.AddRange(Feeder_Param.Get_ReadBlock_text());

            Update_ReadBlock_text = string.Join(";", block_str_list);
        }


        public class GVL_Struct : ViewModelBase
        {
            public int X_SIZE{ get => GetF<int>(); set => SetF(value); }
            public int Y_SIZE{ get => GetF<int>(); set => SetF(value); }
            public int Valve_SIZE{ get => GetF<int>(); set => SetF(value); }
            public int AXIS_FIRST{ get => GetF<int>(); set => SetF(value); }
            public int AXIS_LAST{ get => GetF<int>(); set => SetF(value); }

            public List<string> Get_ReadBlock_text()
            {
                List<string> list = new List<string>()
                {
                    "GVL.X_SIZE",
                    "GVL.Y_SIZE",
                    "GVL.Valve_SIZE",
                    "GVL.AXIS_FIRST",
                    "GVL.AXIS_LAST",
                };

                return list;
            }
        }
        public GVL_Struct GVL { get; set; }

        public class SYSTEM_VAR_Struct : ViewModelBase
        {
            public enum ControlMode
            {
                Initial =   0,
                Manual  =   1,
            }

            public ControlMode ModeNow { get => GetF<ControlMode>(); set => SetF(value); }
            public bool SoftReset { get => GetF<bool>(); set => SetF(value); }
            public bool EMG_Bypass { get => GetF<bool>(); set => SetF(value); }

            public List<string> Get_ReadBlock_text()
            {
                List<string> list = new List<string>()
                {
                    "SYSTEM_VAR.ModeNow",
                    "SYSTEM_VAR.SoftReset",
                    "SYSTEM_VAR.EMG_Bypass",
                };
                return list;
            }

        }
        public SYSTEM_VAR_Struct SYSTEM_VAR { get; set; }

        public class IO_Param_Struct : ViewModelBase
        {
            public enum X_Function
            {
                btn_EMG,//X00
                btn_START,//X01
                btn_Reset,//X02
                Air_Pressure,//X03
                MagPush_FSW,//X04
                MagPush_HSW,//X05
                MagPush_BSW,//X06
                MagPush_ComIn,//X07

                CY1_ON,//X08
                CY1_OFF,//X09
                CY2_ON,//X0A
                CY2_OFF,//X0B
                CY3_ON,//X0C
                CY3_OFF,//X0D
                Presss_ComIn,//X0E
                CyPush_ComIn,//X0F

                Hall_NS,//X10
                X11,//X11
                X12,//X12
                X13,//X13
                X14,//X14
                X15,//X15
                X16,//X16
                X17,//X17

                X18,//X18
                X19,//X19
                X1A,//X1A
                X1B,//X1B
                X1C,//X1C
                X1D,//X1D
                X1E,//X1E
                X1F,//X1F
            }

            public enum Y_Function
            {
                CY1_ON, //	Y00
                CY1_OFF,    //	Y01
                CY2_ON, //	Y02
                CY2_OFF,    //	Y03
                CY3_ON, //	Y04
                CY3_OFF,    //	Y05
                Y06,    //	Y06
                Y07,    //	Y07

                Y08,    //	Y08
                Y09,    //	Y09
                Y0A,    //	Y0A
                Y0B,    //	Y0B
                Y0C,    //	Y0C
                Y0D,    //	Y0D
                Y0E,    //	Y0E
                Y0F,    //	Y0F

                Y10,    //	Y10
                Y11,    //	Y11
                Y12,    //	Y12
                Y13,    //	Y13
                Y14,    //	Y14
                Y15,    //	Y15
                Y16,    //	Y16
                Y17,    //	Y17

                Y18,    //	Y18
                Y19,    //	Y19
                Y1A,    //	Y1A
                Y1B,    //	Y1B
                Y1C,    //	Y1C
                Y1D,    //	Y1D
                Y1E,    //	Y1E
                Y1F,    //	Y1F

                Y20,    //	Y20
                Y21,    //	Y21
                Y22,    //	Y22
                Y23,    //	Y23
                Y24,    //	Y24
                Y25,    //	Y25
                Y26,    //	Y26
                Y27,    //	Y27

                Y28,    //	Y28
                Y29,    //	Y29
                Y2A,    //	Y2A
                Y2B,    //	Y2B
                Y2C,    //	Y2C
                Y2D,    //	Y2D
                Y2E,    //	Y2E
                Y2F,	//	Y2F
            }

            public void Set_X(UInt16[] Datas)
            {
                if (X is null || X.Count != (Datas.Length * 16))
                {
                    X = new ObservableCollection<bool>(Enumerable.Repeat(false, 128));
                }

                for (int i = 0; i < Datas.Length; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        X[i * 16 + j] = ((Datas[i] >> j) & 0x01) == 1;
                    }
                }
            }
            public void Set_Y(UInt16[] Datas)
            {
                if (Y is null || Y.Count != (Datas.Length * 16))
                {
                    Y = new ObservableCollection<bool>(Enumerable.Repeat(false, 128));
                }

                for (int i = 0; i < Datas.Length; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        Y[i * 16 + j] = ((Datas[i] >> j) & 0x01) == 1;
                    }
                }
            }

            public ObservableCollection<bool> X
            {
                get => GetF<ObservableCollection<bool>>();
                set => SetF(value);
            }
            public ObservableCollection<bool> Y
            {
                get => GetF<ObservableCollection<bool>>();
                set => SetF(value);
            }

            public List<string> Get_ReadBlock_text()
            {
                int X_SIZE = X.Count;
                int Y_SIZE = Y.Count;
                List<string> list = new List<string>();

                for (int i = 1; i <= (X_SIZE/16); i++)
                {
                    list.Add($"IO_Param.X_Arr_Link[{i}].words");
                }

                for (int i = 1; i <= (Y_SIZE/16); i++)
                {
                    list.Add($"IO_Param.Y_Arr_Link[{i}].words");
                }

                return list;
            }
        }
        public IO_Param_Struct IO_Param { get; set; }

        public class Valve_Setting_Struct : ViewModelBase
        {
            public int ID { get => GetF<int>(); set => SetF(value); }

            public string Name { get => GetF<string>(); set => SetF(value); }
            public int ON_OUT_BitNum { get => GetF<int>(); set => SetF(value); }
            public int OFF_OUT_BitNum { get => GetF<int>(); set => SetF(value); }
            public int ON_SW_BitNum { get => GetF<int>(); set => SetF(value); }
            public int OFF_SW_BitNum { get => GetF<int>(); set => SetF(value); }
            public TimeSpan Open_Time { get => GetF<TimeSpan>(); set => SetF(value); }
            public TimeSpan Close_Time { get => GetF<TimeSpan>(); set => SetF(value); }
            public bool Keep { get => GetF<bool>(); set => SetF(value); }
            public string ValveType { get => GetF<string>(); set => SetF(value); }

            public List<string> Get_ReadBlock_text()
            {
                int idx = this.ID;

                string Header = $"Valve_Param.Valve_Array[{idx}]";
                List<string> list = new List<string>()
                {
                    $"{Header}.Name",
                    $"{Header}.ON_OUT_BitNum",
                    $"{Header}.OFF_OUT_BitNum",
                    $"{Header}.ON_SW_BitNum",
                    $"{Header}.OFF_SW_BitNum",
                    $"{Header}.Open_Time",
                    $"{Header}.Close_Time",
                    $"{Header}.Keep",
                    $"{Header}.ValveType",
                };
                return list;
            }
        }
        public ObservableCollection<Valve_Setting_Struct> Valve_Setting_Array { get; set; }

        public class Valve_Ctrl_Struct : ViewModelBase
        {
            public int ID { get => GetF<int>(); set => SetF(value); }
            public enum Valve_Cmd: short
            {
                None = 0,
                Open = 1,
                Close = 2,
            }
            public enum Valve_Sts : short
            {
                UnKown = 0,
                ON,
                OFF,
                ON_Error,
                OFF_Error,
                ON_ing,
                OFF_ing
            }

            public bool bEnable { get => GetF<bool>(); set => SetF(value); }
            public Valve_Cmd Command { get => GetF<Valve_Cmd>(); set => SetF(value); }
            public Valve_Sts Status { get => GetF<Valve_Sts>(); set => SetF(value); }

            public List<string> Get_ReadBlock_text()
            {
                int idx = this.ID;

                string Header = $"Valve_Param.Valve_Ctrl_Array[{idx}]";
                List<string> list = new List<string>()
                {
                    $"{Header}.bEnable",
                    $"{Header}.Command",
                    $"{Header}.Status"
                };
                return list;
            }
        }
        public ObservableCollection<Valve_Ctrl_Struct> Valve_Ctrl_Array { get; set; }

        public class Axis_Status : ViewModelBase
        {
            public int ID { get => GetF<int>(); set => SetF(value); }

            public bool Ready { get => GetF<bool>(); set => SetF(value); }
            public bool Error { get => GetF<bool>(); set => SetF(value); }
            public UInt16 ErrorCode { get => GetF<UInt16>(); set => SetF(value); }
            public sbyte OpMode { get => GetF<sbyte>(); set => SetF(value); }

            public double PosFdb { get => GetF<double>(); set => SetF(value); }
            public double PosTgt { get => GetF<double>(); set => SetF(value); }
            public double PosDiff { get => GetF<double>(); set => SetF(value); }

            public double VelFdb { get => GetF<double>(); set => SetF(value); }

            public int CurTgt { get => GetF<int>(); set => SetF(value); }
            public int CurFdb { get => GetF<int>(); set => SetF(value); }
            public double Loading { get => GetF<double>(); set => SetF(value); }

            public bool Homed { get => GetF<bool>(); set => SetF(value); }
            public bool FSW { get => GetF<bool>(); set => SetF(value); }
            public bool BSW { get => GetF<bool>(); set => SetF(value); }
            public bool HSW { get => GetF<bool>(); set => SetF(value); }

            public List<string> Get_ReadBlock_text()
            {
                int idx = this.ID;
                string Header = $"Axis_Param.MD[{idx}]";
                List<string> list = new List<string>()
                {
                    $"{Header}.Ready",
                    $"{Header}.Error",
                    $"{Header}.ErrorCode",
                    $"{Header}.OpMode",

                    $"{Header}.PosFdb",
                    $"{Header}.PosTgt",
                    $"{Header}.PosDiff",

                    $"{Header}.VelFdb",

                    $"{Header}.TargetTorque",
                    $"{Header}.IFdb",
                    $"{Header}.Loading",

                    $"{Header}.Homed",
                    $"{Header}.FSW",
                    $"{Header}.BSW",
                    $"{Header}.HSW",

                };
                return list;
            }
        }
        public ObservableCollection<Axis_Status> Axis_Status_Array { get; set; }

        public class Axis_Param_Struct : ViewModelBase
        {
            public Axis_Param_Struct()
            {
                JOG_Speed = new ObservableCollection<double>() { 0.0, 0.0 };
            }

            public int ID { get => GetF<int>(); set => SetF(value); }
            public enum Driver_Type:short
            {
                None = 0,
                cpc = 1,
                Delta = 2
            }
            public Driver_Type DriverType { get => GetF<Driver_Type>(); set => SetF(value); }

            //Soft Limit
            public bool SoftLimit_Enable { get => GetF<bool>(); set => SetF(value); }
            public double Max_Position { get => GetF<double>(); set => SetF(value); }
            public double Min_Position { get => GetF<double>(); set => SetF(value); }

            //Velocity Limit
            public double Max_Velocity { get => GetF<double>(); set => SetF(value); }
            public double Max_Acc { get => GetF<double>(); set => SetF(value); }
            public double Max_Dec { get => GetF<double>(); set => SetF(value); }
            public double Def_Acc { get => GetF<double>(); set => SetF(value); }
            public double Def_Dec { get => GetF<double>(); set => SetF(value); }
            public double Def_Jerk { get => GetF<double>(); set => SetF(value); }

            //Jog Speed
            public ObservableCollection<double> JOG_Speed
            { 
                get => GetF<ObservableCollection<double>>(); 
                set => SetF(value); 
            } 

            public double AccRate { get => GetF<double>(); set => SetF(value); }
            public double JerkRate { get => GetF<double>(); set => SetF(value); }

            //Motion Speed
            public double Move_Velocity { get => GetF<double>(); set => SetF(value); }
            public double Move_AccDec { get => GetF<double>(); set => SetF(value); }
            public double Move_Jerk { get => GetF<double>(); set => SetF(value); }


            public class Home_Param_Struct : ViewModelBase
            {
                public enum Encoder_Type:short
                {
                    SingleLoop = 1,
                    DualLoop=2
                }
                public Encoder_Type EncoderType { get => GetF<Encoder_Type>(); set => SetF(value); }
                public enum HomeDirection:short
                {
                    Backward = 0,
                    Forward  = 1
                }
                public HomeDirection Dir { get => GetF<HomeDirection>(); set => SetF(value); }

                public double Offset { get => GetF<double>(); set => SetF(value); }
	            public bool GoZero { get => GetF<bool>(); set => SetF(value); }
	            public double Vel_Limit { get => GetF<double>(); set => SetF(value); }
	            public double Vel_HSW { get => GetF<double>(); set => SetF(value); }
	            public double Vel_GoZero { get => GetF<double>(); set => SetF(value); }
                public int Modulo_Direction { get => GetF<int>(); set => SetF(value); }
	            public bool HSW_Inverse { get => GetF<bool>(); set => SetF(value); }
	            public bool FSW_Inverse { get => GetF<bool>(); set => SetF(value); }
                public bool BSW_Inverse { get => GetF<bool>(); set => SetF(value); }
            }
            public Home_Param_Struct HP = new Home_Param_Struct();

            public List<string> Get_ReadBlock_text()
            {
                int idx = this.ID;
                string Header = $"Axis_Param.MP[{idx}]";
                List<string> list = new List<string>()
                {
                    $"{Header}.DriverType",

                    $"{Header}.SoftLimit_Enable",
                    $"{Header}.Max_Position",
                    $"{Header}.Min_Position",

                    $"{Header}.Max_Velocity",
                    $"{Header}.Max_Acc",
                    $"{Header}.Max_Dec",
                    $"{Header}.Def_Acc",
                    $"{Header}.Def_Dec",
                    $"{Header}.Def_Jerk",
                    $"{Header}.JOG_Speed[1]",
                    $"{Header}.JOG_Speed[2]",
                    $"{Header}.AccRate",
                    $"{Header}.JerkRate",
                    $"{Header}.Move_Velocity",
                    $"{Header}.Move_AccDec",
                    $"{Header}.Move_Jerk",

                    $"{Header}.HP.Encoder",
                    $"{Header}.HP.Dir",
                    $"{Header}.HP.Offset",
                    $"{Header}.HP.GoZero",
                    $"{Header}.HP.Vel_Limit",
                    $"{Header}.HP.Vel_HSW",
                    $"{Header}.HP.Vel_GoZero",
                    $"{Header}.HP.Modulo_Direction",
                    $"{Header}.HP.HSW_Inverse",
                    $"{Header}.HP.FSW_Inverse",
                    $"{Header}.HP.BSW_Inverse",
                };
                return list;
            }
        }
        public ObservableCollection<Axis_Param_Struct> Axis_Param_Array { get; set; }

        public class Axis_Ctrl_Struct : ViewModelBase
        {
            public int ID { get => GetF<int>(); set => SetF(value); }

            public bool Power { get => GetF<bool>(); set => SetF(value); }
            public double Override { get => GetF<double>(); set => SetF(value); }

            public bool Reset { get => GetF<bool>(); set => SetF(value); }
            public bool Halt { get => GetF<bool>(); set => SetF(value); }
            public bool Home { get => GetF<bool>(); set => SetF(value); }

            public bool JogF { get => GetF<bool>(); set => SetF(value); }
            public bool JogB { get => GetF<bool>(); set => SetF(value); }
            public enum JogVel_select : short
            {
                LOW =1,
                HIGH =2,
            }
            public JogVel_select JogVel_Switch { get => GetF<JogVel_select>(); set => SetF(value); }//0:Low,1:High

            public bool Move { get => GetF<bool>(); set => SetF(value); }
            public enum Move_Select : short
            {
                ABS = 0,
                REL = 1,
            }
            public Move_Select Move_Abs_Rel { get => GetF<Move_Select>(); set => SetF(value); }//0:Abs,1:Rel
            public double Move_Target { get => GetF<double>(); set => SetF(value); }

            public bool ForceMove { get => GetF<bool>(); set => SetF(value); }
            public int Force_Target { get => GetF<int>(); set => SetF(value); } // 0~1000;

            public List<string> Get_ReadBlock_text()
            {
                int idx = this.ID;

                string Header = $"Axis_Param.Axis_Ctrl_Arr[{idx}]";
                List<string> list = new List<string>()
                {
                    $"{Header}.Power",
                    $"{Header}.Override",

                    $"{Header}.bReset",
                    $"{Header}.bHalt",
                    $"{Header}.bHome",

                    $"{Header}.bJogF",
                    $"{Header}.bJogB",
                    $"{Header}.JogVel_Switch",

                    $"{Header}.bMove",
                    $"{Header}.ABS_REL",
                    $"{Header}.Pos_Target",

                    $"{Header}.ForceMove",
                    $"{Header}.Force_Target",
                };
                return list;
            }
        }
        public ObservableCollection<Axis_Ctrl_Struct> Axis_Ctrl_Array { get; set; }

        public class Feeder_Struct : ViewModelBase
        {
            public bool Feeder_Auto { get => GetF<bool>(); set => SetF(value); }
            public bool Start { get => GetF<bool>(); set => SetF(value); }
            public bool Ax_Power { get => GetF<bool>(); set => SetF(value); }
            public bool Ax_Push { get => GetF<bool>(); set => SetF(value); }
            public bool AX_Move { get => GetF<bool>(); set => SetF(value); }
            public bool CY2_Ctrl { get => GetF<bool>(); set => SetF(value); }
            public bool CY3_Ctrl { get => GetF<bool>(); set => SetF(value); }

            public enum Feeder_Step : short
            {
                WaitStart = 0,
                First = 1,
                Slider_PushMag = 1,
                CY3_Mag_ON = 2,
                CY2_Mag_ON = 3,
                CY3_Spacer_ON = 4,
                CY3_Spacer_Delay = 5,
                CY3_Spacer_OFF = 6,
                CY2_Mag_OFF = 7,
                Idle = 8,
                Last = Idle,
            }
            public Feeder_Step step { get => GetF<Feeder_Step>(); set => SetF(value); }

            public int Ax_PushForce { get => GetF<int>(); set => SetF(value); }
            public double Ax_MovePos { get => GetF<double>(); set => SetF(value); }

            public List<string> Get_ReadBlock_text()
            {
                string Header = $"Feeder_ControlLoop";
                List<string> list = new List<string>()
                {
                    $"{Header}.Feeder_Auto",
                    $"{Header}.Start",
                    $"{Header}.Ax_Power",
                    $"{Header}.Ax_Push",
                    $"{Header}.AX_Move",
                    $"{Header}.CY2_Ctrl",
                    $"{Header}.CY3_Ctrl",
                    $"{Header}.step",
                    $"{Header}.Ax_PushForce",
                    $"{Header}.Ax_MovePos",
                };
                return list;
            }

        }
        public Feeder_Struct Feeder_Param { get; set; }


        public string Update_GVL_text;
        public string Update_ReadBlock_text;
    }

    public class PLC_app
    {
        public PLC_app()
        {
            ADS_COM = new ADS_lib() { NetID = "192.168.0.4.1.1", Port = 851};

            PLC_Param = new PLC_Parameter();

            Timer_Initial();
        }

        public ADS_lib ADS_COM { get; set; }

        public PLC_Parameter PLC_Param{ get; set; }

        public async void Initial()
        {
            Update_Timer.Stop();

            while (Update_Timer.Enabled) ;

            bool rst = await this.ADS_COM.Connect();

            if (this.ADS_COM.IsConnect)
            {
                Logger.Log_App.WriteLine($"{this.ADS_COM.NetID} is connected.");

                object? X_SIZE = await this.ADS_COM.Read("GVL.X_SIZE");
                object? Y_SIZE = await this.ADS_COM.Read("GVL.Y_SIZE");
                object? Valve_Size = await this.ADS_COM.Read("GVL.Valve_Size");
                object? AXIS_FIRST = await this.ADS_COM.Read("GVL.AXIS_FIRST");
                object? AXIS_LAST = await this.ADS_COM.Read("GVL.AXIS_LAST");


                this.PLC_Param.GVL.X_SIZE = (Int16)X_SIZE;
                this.PLC_Param.GVL.Y_SIZE = (Int16)Y_SIZE;
                this.PLC_Param.GVL.Valve_SIZE = (Int16)Valve_Size;
                this.PLC_Param.GVL.AXIS_FIRST = (Int16)AXIS_FIRST;
                this.PLC_Param.GVL.AXIS_LAST = (Int16)AXIS_LAST;
                this.PLC_Param.Initial();

                Update_Timer.Start();
            }
            
        }

        // Update Timer
        private Timer Update_Timer;
        private void Timer_Initial()
        {
            Update_Timer = new Timer();
            Update_Timer.Interval = 250;
            Update_Timer.Elapsed += Update_Timer_Elapsed;

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            while (Update_Flag) ;

            this.ADS_COM.Disconnect();

            Update_Timer.Stop();
        }

        bool Update_Flag = false;
        private async void Update_Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (this.ADS_COM.IsConnect)
            {
                if (!Update_Flag)
                {
                    Update_Flag = true;

                    //Dictionary<string, object> fdb_Data = await this.ADS_COM.BlockRead2(this.PLC_Param.Update_ReadBlock_text);
                    List<object> blk_Data = await this.ADS_COM.BlockRead(this.PLC_Param.Update_ReadBlock_text);

                    int idx = 0;
                    int seletRange = 0;

                    seletRange = this.PLC_Param.GVL.X_SIZE / 16 ;
                    var X_Data = blk_Data.Skip(idx).Take(seletRange).Cast<UInt16>().ToArray(); idx += seletRange;
                    this.PLC_Param.IO_Param.Set_X(X_Data);

                    seletRange = this.PLC_Param.GVL.Y_SIZE / 16;
                    var Y_Data = blk_Data.Skip(idx).Take(seletRange).Cast<UInt16>().ToArray(); idx += seletRange;
                    this.PLC_Param.IO_Param.Set_Y(Y_Data);

                    for (int i = 0; i < this.PLC_Param.GVL.Valve_SIZE; i++)
                    {
                        var v = this.PLC_Param.Valve_Setting_Array[i];

                        v.Name = (string)blk_Data[idx]; idx++;
                        v.ON_OUT_BitNum = (Int16)blk_Data[idx]; idx++;
                        v.OFF_OUT_BitNum = (Int16)blk_Data[idx]; idx++;
                        v.ON_SW_BitNum = (Int16)blk_Data[idx]; idx++;
                        v.OFF_SW_BitNum = (Int16)blk_Data[idx]; idx++;
                        v.Open_Time = (TimeSpan)blk_Data[idx]; idx++;
                        v.Close_Time = (TimeSpan)blk_Data[idx]; idx++;
                        v.Keep = (bool)blk_Data[idx]; idx++;
                        v.ValveType = (string)blk_Data[idx]; idx++;
                    }

                    for (int i = 0; i < this.PLC_Param.GVL.Valve_SIZE; i++)
                    {
                        var v = this.PLC_Param.Valve_Ctrl_Array[i];

                        v.bEnable = (bool)blk_Data[idx]; idx++;
                        v.Command = (Valve_Cmd)blk_Data[idx]; idx++;
                        v.Status = (Valve_Sts)blk_Data[idx]; idx++;
                    }

                    for (int i = 0; i < (this.PLC_Param.GVL.AXIS_LAST- this.PLC_Param.GVL.AXIS_FIRST) + 1; i++)
                    {
                        var ax = this.PLC_Param.Axis_Status_Array[i];
                        ax.Ready = (bool)blk_Data[idx]; idx++;
                        ax.Error = (bool)blk_Data[idx]; idx++;
                        ax.ErrorCode = (UInt16)blk_Data[idx]; idx++;
                        ax.OpMode = (sbyte)blk_Data[idx]; idx++;

                        ax.PosFdb = (double)blk_Data[idx]; idx++;
                        ax.PosTgt = (double)blk_Data[idx]; idx++;
                        ax.PosDiff = (double)blk_Data[idx]; idx++;

                        ax.VelFdb = (double)blk_Data[idx]; idx++;

                        ax.CurTgt = (Int16)blk_Data[idx]; idx++;
                        ax.CurFdb = (Int16)blk_Data[idx]; idx++;
                        ax.Loading = (double)blk_Data[idx]; idx++;

                        ax.Homed = (bool)blk_Data[idx]; idx++;
                        ax.FSW = (bool)blk_Data[idx]; idx++;
                        ax.BSW = (bool)blk_Data[idx]; idx++;
                        ax.HSW = (bool)blk_Data[idx]; idx++;
                    }

                    for (int i = 0; i < (this.PLC_Param.GVL.AXIS_LAST - this.PLC_Param.GVL.AXIS_FIRST) + 1; i++)
                    {
                        var ax = this.PLC_Param.Axis_Param_Array[i];

                        ax.DriverType = (Axis_Param_Struct.Driver_Type)blk_Data[idx]; idx++;
                        //Soft Limit
                        ax.SoftLimit_Enable = (bool)blk_Data[idx]; idx++;
                        ax.Max_Position = (double)blk_Data[idx]; idx++;
                        ax.Min_Position = (double)blk_Data[idx]; idx++;

                        //Velocity Limit
                        ax.Max_Velocity = (double)blk_Data[idx]; idx++;
                        ax.Max_Acc = (double)blk_Data[idx]; idx++;
                        ax.Max_Dec = (double)blk_Data[idx]; idx++;
                        ax.Def_Acc = (double)blk_Data[idx]; idx++;
                        ax.Def_Dec = (double)blk_Data[idx]; idx++;
                        ax.Def_Jerk = (double)blk_Data[idx]; idx++;

                        //Jog Speed
                        i = 0;
                        foreach (var j_spd in ax.JOG_Speed)
                        {
                            ax.JOG_Speed[i] = (double)blk_Data[idx]; idx++;
                            i++;
                        }
                        ax.AccRate = (UInt16) blk_Data[idx]; idx++;
                        ax.JerkRate = (UInt16) blk_Data[idx]; idx++;

                        //Motion Speed
                        ax.Move_Velocity = (double) blk_Data[idx]; idx++;
	                    ax.Move_AccDec = (double) blk_Data[idx]; idx++;
	                    ax.Move_Jerk = (double) blk_Data[idx]; idx++;

                        //Home Parameter
                        var hp = ax.HP;
                        hp.EncoderType = (Encoder_Type)blk_Data[idx]; idx++;
                        hp.Dir = (HomeDirection)blk_Data[idx]; idx++;
                        hp.Offset = (double)blk_Data[idx]; idx++;
                        hp.GoZero = (bool)blk_Data[idx]; idx++;
                        hp.Vel_Limit = (double)blk_Data[idx]; idx++;
                        hp.Vel_HSW = (double)blk_Data[idx]; idx++;
                        hp.Vel_GoZero = (double)blk_Data[idx]; idx++;
                        hp.Modulo_Direction = (Int16)blk_Data[idx]; idx++;
                        hp.HSW_Inverse = (bool)blk_Data[idx]; idx++;
                        hp.FSW_Inverse = (bool)blk_Data[idx]; idx++;
                        hp.BSW_Inverse = (bool)blk_Data[idx]; idx++;
                    }

                    for (int i = 0; i < (this.PLC_Param.GVL.AXIS_LAST - this.PLC_Param.GVL.AXIS_FIRST) + 1; i++)
                    {
                        var ax = this.PLC_Param.Axis_Ctrl_Array[i];

                        ax.Power = (bool)blk_Data[idx]; idx++;
                        ax.Override = (double)blk_Data[idx]; idx++;

                        ax.Reset = (bool)blk_Data[idx]; idx++;
                        ax.Halt = (bool)blk_Data[idx]; idx++;
                        ax.Home = (bool)blk_Data[idx]; idx++;

                        ax.JogF = (bool)blk_Data[idx]; idx++;
                        ax.JogB = (bool)blk_Data[idx]; idx++;
                        ax.JogVel_Switch = (JogVel_select)blk_Data[idx]; idx++;

                        ax.Move = (bool)blk_Data[idx]; idx++;
                        ax.Move_Abs_Rel = (Move_Select)blk_Data[idx]; idx++;
                        ax.Move_Target = (double)blk_Data[idx]; idx++;

                        ax.ForceMove = (bool)blk_Data[idx]; idx++;
                        ax.Force_Target = (Int16)blk_Data[idx]; idx++;
                    }

                    this.PLC_Param.Feeder_Param.Feeder_Auto = (bool)blk_Data[idx]; idx++;
                    this.PLC_Param.Feeder_Param.Start = (bool)blk_Data[idx]; idx++;
                    this.PLC_Param.Feeder_Param.Ax_Power = (bool)blk_Data[idx]; idx++;
                    this.PLC_Param.Feeder_Param.Ax_Push = (bool)blk_Data[idx]; idx++;
                    this.PLC_Param.Feeder_Param.AX_Move = (bool)blk_Data[idx]; idx++;
                    this.PLC_Param.Feeder_Param.CY2_Ctrl = (bool)blk_Data[idx]; idx++;
                    this.PLC_Param.Feeder_Param.CY3_Ctrl = (bool)blk_Data[idx]; idx++;
                    this.PLC_Param.Feeder_Param.step = (Feeder_Step)blk_Data[idx]; idx++;
                    this.PLC_Param.Feeder_Param.Ax_PushForce = (int)blk_Data[idx]; idx++;
                    this.PLC_Param.Feeder_Param.Ax_MovePos = (double)blk_Data[idx]; idx++;

                    Update_Flag = false;
                }
            }
        }
    }
}
