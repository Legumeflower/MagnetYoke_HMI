using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Model;
using Axis_Ctrl_Struct = Tc.Model.PLC_Parameter.Axis_Ctrl_Struct;

namespace Tc.ViewModel
{
    public class PLC_ViewModel : ViewModelBase
    {

        public PLC_Parameter PLC_Param;


        public PLC_ViewModel()
        {
            PLC_Param = new PLC_Parameter();
        }


    }
}
