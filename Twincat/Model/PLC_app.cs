using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Communication;
using static Model.PLC_Parameter;

namespace Model
{
    public class PLC_Parameter
    {
        public PLC_Parameter(GVL_Struct _gvl)
        {
            GVL = _gvl;

            SYSTEM_VAR = new SYSTEM_VAR_Struct();

            IO_Param = new IO_Param_Struct() { X = new bool[GVL.X_SIZE], Y = new bool[GVL.Y_SIZE] };

            Valve_Ctrl_Array = new ObservableCollection<Valve_Ctrl_Struct>();
            Valve_Setting_Array = new ObservableCollection<Valve_Setting_Struct>();
            for (int i = 1; i < GVL.Valve_SIZE; i++)
            {
                Valve_Ctrl_Array.Add(new Valve_Ctrl_Struct());
                Valve_Setting_Array.Add(new Valve_Setting_Struct());
            }


        }

        public class GVL_Struct
        {
            public int X_SIZE;
            public int Y_SIZE;
            public int Valve_SIZE;
            public int AXIS_FIRST;
            public int AXIS_LAST;

        }
        public GVL_Struct GVL;

        public class SYSTEM_VAR_Struct
        {
            public enum ControlMode
            {
                Initial =   0,
                Manual  =   1,
            }

            public ControlMode ModeNow;
            public bool SoftReset;
            public bool EMG_Bypass;

        }
        public SYSTEM_VAR_Struct SYSTEM_VAR;

        public class IO_Param_Struct
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
                X = new bool[Datas.Length * 16];

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
                Y = new bool[Datas.Length * 16];

                for (int i = 0; i < Datas.Length; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        Y[i * 16 + j] = ((Datas[i] >> j) & 0x01) == 1;
                    }
                }
            }

            public bool[] X;
            public bool[] Y;
        }
        public IO_Param_Struct IO_Param;

        public class Valve_Ctrl_Struct
        {
            public enum Valve_Cmd
            {
                None = 0,
                Open = 1,
                Close = 2,
            }
            public enum Valve_Sts
            {
                UnKown = 0,
                ON,
                OFF,
                ON_Error,
                OFF_Error,
                ON_ing,
                OFF_ing
            }

            public bool bEnable;
            public Valve_Cmd Command;
            public Valve_Sts Status;
        }
        public ObservableCollection<Valve_Ctrl_Struct> Valve_Ctrl_Array;
        public class Valve_Setting_Struct
        {
            public string Name;
            public int ON_OUT_BitNum;
            public int OFF_OUT_BitNum;
            public int ON_SW_BitNum;
            public int OFF_SW_BitNum;
            public TimeSpan Open_Time;
            public TimeSpan Close_Time;
            public bool Keep;
            public string ValveType;
        }
        public ObservableCollection<Valve_Setting_Struct> Valve_Setting_Array;

        public class Axis_Status
        {
            public bool Ready;
            public bool Error;
            public UInt16 ErrorCode;
            public int OpMode;

            public double PosFdb;
            public double PosTgt;
            public double PosDiff;

            public double VelFdb;

            public int CurTgt;
            public int CurFdb;
            public double Loading;

            public bool Homed;
            public bool FSW;
            public bool BSW;
            public bool HSW;
        }
    }

    public class PLC_app
    {
        public ADS_lib ADS_COM { get; set; }

        public PLC_app()
        {

        }
        public static async Task<PLC_app> Create_PLC_app(string _netid, int _port)
        {
            var instance = new PLC_app();

            instance.ADS_COM = new ADS_lib() { NetID = _netid, Port = _port };

            bool rst = await instance.ADS_COM.Connect();

            if (instance.ADS_COM.IsConnect)
            {
                Logger.Log_App.WriteLine($"{_netid} is connected.");

                object? X_SIZE = await instance.ADS_COM.Read("GVL.X_SIZE");
                object? Y_SIZE = await instance.ADS_COM.Read("GVL.Y_SIZE");
                object? Valve_Size = await instance.ADS_COM.Read("GVL.Valve_Size");
                object? AXIS_FIRST = await instance.ADS_COM.Read("GVL.AXIS_FIRST");
                object? AXIS_LAST = await instance.ADS_COM.Read("GVL.AXIS_LAST");

                GVL_Struct gvl = new GVL_Struct() 
                { 
                    X_SIZE = (int)X_SIZE, 
                    Y_SIZE = (int)Y_SIZE, 
                    Valve_SIZE = (int)Valve_Size, 
                    AXIS_FIRST = (int)AXIS_FIRST, 
                    AXIS_LAST =(int)AXIS_LAST 
                };

                instance.PLC_Param = new PLC_Parameter(gvl);

            }


            return instance;
        }
        public PLC_Parameter PLC_Param{ get; set; }



    }
}
