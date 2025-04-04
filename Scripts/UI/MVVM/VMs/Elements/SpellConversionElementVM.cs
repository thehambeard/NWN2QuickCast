﻿using Kingmaker.UI.UnitSettings;
using UniRx;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    public class SpellConversionElementVM : SpellElementBaseVM
    {
        public SpellConversionElementVM(MechanicActionBarSlot slot) : base(slot)
        {
            Icon.Value = slot.GetForeIcon();
        }

        public override void DisposeImplementation()
        {
            base.DisposeImplementation();
        }
    }
}
