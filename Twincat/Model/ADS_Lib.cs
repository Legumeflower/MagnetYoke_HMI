using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using TwinCAT.Ads.SumCommand;
using TwinCAT.ValueAccess;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using TwinCAT.PlcOpen;
using System.Collections.ObjectModel;
using System.Reflection;
using MVVM_Core;
using Logger;

namespace Communication
{
    public class ADS_lib : NotificationBinding
    {
        private AdsClient ADS_COM = new AdsClient();
        private CancellationToken cancel = CancellationToken.None;
        private ResultSymbols AllSymbols;

        #region Connect

        private string _NetID;
        public string NetID
        {
            get { return _NetID; }
            set { _NetID = value; OnPropertyChanged(); }
        }

        private int _Port;
        public int Port
        {
            get { return _Port; }
            set { _Port = value; OnPropertyChanged(); }
        }

        public bool IsConnect
        {
            get { return ADS_COM.IsConnected; }
        }

        public async Task<bool> Connect(string netid, int port)
        {
            NetID = netid;
            Port = port;

            try
            {
                ADS_COM.Connect(NetID, Port);

                if (ADS_COM.IsConnected)
                {
                    bool rst = await Relaod_Symbols();

                    if (!rst) ADS_COM.Disconnect();

                    OnPropertyChanged(nameof(IsConnect));

                    return rst;
                }
                else
                {
                    OnPropertyChanged(nameof(IsConnect));
                    return false;
                }

            }
            catch(Exception e) 
            {
                Console.WriteLine( e.Message );
                OnPropertyChanged(nameof(IsConnect));
                return false;
            }
        }

        public void Disconnect()
        {
            ADS_COM.Disconnect();
            OnPropertyChanged(nameof(IsConnect));
        }

        #endregion

        #region Symbols
        public async Task<bool> Relaod_Symbols()
        {
            if (!ADS_COM.IsConnected) return false;

            try
            {
                ISymbolLoader loader = SymbolLoaderFactory.Create(ADS_COM, SymbolLoaderSettings.Default);
                AllSymbols = await loader.GetSymbolsAsync(cancel);
                if (AllSymbols.Succeeded)
                {
                    Dic_Symbol = new Dictionary<string, Symbol>();
                    Logger.Log_App.WriteLine("ADS Load All Symbols Successful");
                    return true;
                }
                else
                {
                    Logger.Log_App.WriteLine("ADS Load All Symbols Fail");
                    return false; 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.Log_App.WriteLine("ADS_Lib,Line:116," + e.Message);
                return false;
            }
        }

        //快速查表
        private Dictionary<string, Symbol> Dic_Symbol;
        
        private bool CheckSymbol(string VarPath,out Symbol? smb)
        {
            VarPath = VarPath.Replace(" ","");

            if(Dic_Symbol.ContainsKey(VarPath) == false)
            {
                if(AllSymbols.Symbols.Contains(VarPath))
                {
                    Dic_Symbol.Add(VarPath, (Symbol)AllSymbols.Symbols[VarPath]);
                }
                else
                {
                    smb = null;
                    return false;
                }
            }

            smb = Dic_Symbol[VarPath];
            return true;
        }

        #endregion

        #region Singal Read & Write
        //Read Single Value
        public async Task<object?> Read(string VarPath)
        {
            if (!ADS_COM.IsConnected) return null;

            //取得Symbol
            Symbol smb ;
            if (!CheckSymbol(VarPath,out smb)) return "Invalid Variable Path";

            //取得Value
            ResultReadValueAccess resultRead = await smb.ReadValueAsync(cancel);

            //輸出
            if (!resultRead.Succeeded) return "Read Fail";
            else
            {
                return resultRead.Value;
            }

        }


        //Write Single Value
        public async Task<bool> Write(string VarPath, object value)
        {

            if (!ADS_COM.IsConnected) return false;

            //取得Symbol
            Symbol mysmb;
            if (!CheckSymbol(VarPath, out mysmb)) return false;

            //Write Data
            ResultWriteAccess resultWrite = null;
            {
                switch (mysmb.Category)
                {
                    case DataTypeCategory.Enum:
                        switch (mysmb.ByteSize)
                        {
                            case 1:
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToSByte(value), cancel);
                                break;
                            case 2:
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt16(value), cancel);
                                break;
                            case 4:
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt32(value), cancel);
                                break;
                            case 8:
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt64(value), cancel);
                                break;
                        }

                        break;

                    case DataTypeCategory.Primitive:
                        switch (mysmb.TypeName)
                        {

                            case "BOOL":
                            case "BIT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToBoolean(value), cancel);
                                break;
                            case "BYTE":
                            case "USINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToByte(value), cancel);
                                break;
                            case "WORD":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt16(value), cancel);
                                break;
                            case "DWORD":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt32(value), cancel);
                                break;
                            case "SINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToSByte(value), cancel);
                                break;
                            case "INT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt16(value), cancel);
                                break;
                            case "UINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt16(value), cancel);
                                break;
                            case "DINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt32(value), cancel);
                                break;
                            case "UDINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt32(value), cancel);
                                break;
                            case "LINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt64(value), cancel);
                                break;
                            case "ULINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt64(value), cancel);
                                break;
                            case "REAL":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToSingle(value), cancel);
                                break;
                            case "LREAL":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToDouble(value), cancel);
                                break;
                            case "TIME":
                                TimeSpan t;
                                if (TimeSpan.TryParse(value.ToString(), out t))
                                    resultWrite = await mysmb.WriteValueAsync(t, cancel);

                                break;

                            case "Date":
                            case "DateTime":
                                DateTime dt;
                                if(DateTime.TryParse(value.ToString(),out dt) == false)
                                    resultWrite = await mysmb.WriteValueAsync(dt, cancel);

                                break;
                        }
                        break;

                    case DataTypeCategory.String:
                        resultWrite = await mysmb.WriteValueAsync(Convert.ToString(value), cancel);

                        break;

                }
                
            }

            if(resultWrite != null)
            {
                return resultWrite.Succeeded;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> Write(ISymbol symbol, object value)
        {
            if (!ADS_COM.IsConnected) return false;

            //取得Symbol
            Symbol mysmb = (Symbol)symbol;

            //Write Data
            ResultWriteAccess resultWrite = null;
            {
                switch (mysmb.Category)
                {
                    case DataTypeCategory.Enum:
                        switch (mysmb.ByteSize)
                        {
                            case 1:
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToSByte(value), cancel);
                                break;
                            case 2:
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt16(value), cancel);
                                break;
                            case 4:
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt32(value), cancel);
                                break;
                            case 8:
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt64(value), cancel);
                                break;
                        }

                        break;

                    case DataTypeCategory.Primitive:
                        switch (mysmb.TypeName)
                        {

                            case "BOOL":
                            case "BIT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToBoolean(value), cancel);
                                break;
                            case "BYTE":
                            case "USINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToByte(value), cancel);
                                break;
                            case "WORD":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt16(value), cancel);
                                break;
                            case "DWORD":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt32(value), cancel);
                                break;
                            case "SINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToSByte(value), cancel);
                                break;
                            case "INT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt16(value), cancel);
                                break;
                            case "UINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt16(value), cancel);
                                break;
                            case "DINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt32(value), cancel);
                                break;
                            case "UDINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt32(value), cancel);
                                break;
                            case "LINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToInt64(value), cancel);
                                break;
                            case "ULINT":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToUInt64(value), cancel);
                                break;
                            case "REAL":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToSingle(value), cancel);
                                break;
                            case "LREAL":
                                resultWrite = await mysmb.WriteValueAsync(Convert.ToDouble(value), cancel);
                                break;
                            case "TIME":
                                TimeSpan t;
                                if (TimeSpan.TryParse(value.ToString(), out t))
                                    resultWrite = await mysmb.WriteValueAsync(t, cancel);

                                break;

                            case "Date":
                            case "DateTime":
                                DateTime dt;
                                if (DateTime.TryParse(value.ToString(), out dt) == false)
                                    resultWrite = await mysmb.WriteValueAsync(dt, cancel);

                                break;
                        }
                        break;

                    case DataTypeCategory.String:
                        resultWrite = await mysmb.WriteValueAsync(Convert.ToString(value), cancel);

                        break;

                }

            }

            if (resultWrite != null)
            {
                return resultWrite.Succeeded;
            }
            else
            {
                return false;
            }


        }

        #endregion

        #region Block Read 
        //區塊讀取

        private string Read_Block_Str = "";
        private IList<ISymbol> IList_Symbols;
        public IList<ISymbol> String_to_Symbols(string input)
        {
            if (AllSymbols == null) return null;

            string clear_input = input.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
            List<string> path_arr = clear_input.Split(";").ToList();
            path_arr.Remove("");

            IList<ISymbol> newList = new List<ISymbol>();
            foreach (string path in path_arr)
            {
                if (AllSymbols.Symbols.Contains(path))
                {
                    //ISymbol smb = null;
                    //ISymbol smb = (ISymbol)AllSymbols.Symbols[path];
                    newList.Add(AllSymbols.Symbols[path]);
                }
            }

            return newList;
        }

        /// <summary>
        /// 回傳 List<object>
        /// </summary>
        /// <param name="input">"var1;va2;var3"</param>
        /// <returns></returns>
        public async Task<List<object>> BlockRead(string input)
        {
            if(input != Read_Block_Str)
            {
                IList_Symbols = String_to_Symbols(input);
                input = Read_Block_Str;
            }

            if (IList_Symbols == null) return null;

            SumSymbolRead readCommand;
            try
            {
                readCommand = new SumSymbolRead(ADS_COM, IList_Symbols);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            ResultSumValues resultSumRead = await readCommand.ReadAsync(cancel);

            List<object> result = new List<object>();
            
            int Index = 0;
            
            foreach( Symbol smb in IList_Symbols )
            {
                if (smb != null)
                {
                    result.Add(resultSumRead.Values[Index]);
                    Index++;
                }
                else
                {
                    result.Add(null);
                }
            }

            return result;
        }

        /// <summary>
        /// 回傳 Dic<string, object>
        /// </summary>
        /// <param name="input"> "var1;va2;var3" </param>
        /// <returns></returns>
        public async Task<Dictionary<string,object>> BlockRead2(string input)
        {
            if (input != Read_Block_Str)
            {
                IList_Symbols = String_to_Symbols(input);
                input = Read_Block_Str;
            }

            if (IList_Symbols == null) return null;

            SumSymbolRead readCommand;
            try
            {
                readCommand = new SumSymbolRead(ADS_COM, IList_Symbols);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            ResultSumValues resultSumRead = await readCommand.ReadAsync(cancel);

            Dictionary<string, object> dic_rst = new Dictionary<string, object>();


            //List<object> result = new List<object>();

            int Index = 0;

            foreach (Symbol smb in IList_Symbols)
            {
                if (smb != null)
                {
                    dic_rst.Add(smb.InstancePath, resultSumRead.Values[Index]);
                    Index++;
                }
                else
                {
                    dic_rst.Add(smb.InstancePath, null);
                }
            }

            return dic_rst;
        }

        public async Task<List<object>> BlockRead3(IList<ISymbol> symbols)
        {
            if (IList_Symbols == null) return null;

            SumSymbolRead readCommand;
            try
            {
                readCommand = new SumSymbolRead(ADS_COM, symbols);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            ResultSumValues resultSumRead = await readCommand.ReadAsync(cancel);

            List<object> result = new List<object>();

            int Index = 0;

            foreach (Symbol smb in IList_Symbols)
            {
                if (smb != null)
                {
                    result.Add(resultSumRead.Values[Index]);
                    Index++;
                }
                else
                {
                    result.Add(null);
                }
            }

            return result;
        }

        #endregion



    }

    public class NotificationBinding: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
