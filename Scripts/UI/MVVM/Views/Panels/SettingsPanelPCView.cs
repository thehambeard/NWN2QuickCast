using NWN2QuickCast.UI.MVVM.Views.Settings;
using NWN2QuickCast.UI.MVVM.VMs.Panels;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.Views.Panels
{
    public class SettingsPanelPCView : ViewBase<SettingsPanelVM>
    {
        [SerializeField]
        private HotKeySettingPCView _hotKeySettingPCView;

        public override void BindViewImplementation()
        {
            _hotKeySettingPCView.Bind(ViewModel.HotKeySettingVM);
            gameObject.SetActive(false);
        }

        public override void DestroyViewImplementation()
        {
        }
    }
}
