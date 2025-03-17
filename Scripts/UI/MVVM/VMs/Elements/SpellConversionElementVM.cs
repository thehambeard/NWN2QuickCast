using Kingmaker.UI.UnitSettings;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace NWN2QuickCast.MVVM.VMs.Elements
{
    class SpellConversionElementVM : VirtualListElementVMBase
    {
        public readonly ReactiveProperty<Sprite> Icon = new ReactiveProperty<Sprite>();

        public readonly MechanicActionBarSlot Slot;

        public SpellConversionElementVM(MechanicActionBarSlot slot)
        {
            Slot = slot;
            Icon.Value = slot.GetForeIcon();
        }

        public override void DisposeImplementation()
        {
        }
    }
}
