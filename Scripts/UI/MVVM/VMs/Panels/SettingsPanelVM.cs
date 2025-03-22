using NWN2QuickCast.UI.MVVM.VMs.Settings;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWN2QuickCast.UI.MVVM.VMs.Panels
{
    public class SettingsPanelVM : BaseDisposable, IViewModel
    {
        public HotKeySettingVM HotKeySettingVM;

        public SettingsPanelVM()
        {
            base.AddDisposable(HotKeySettingVM = new HotKeySettingVM());
        }

        public override void DisposeImplementation()
        {
            base.AddDisposable(HotKeySettingVM = new HotKeySettingVM());
        }
    }
}
