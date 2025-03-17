using Kingmaker.UI.UnitSettings;
using UniRx;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    public class SpellConversionElementVM : SpellElementBase
    {
        public readonly MechanicActionBarSlotSpell Slot;

        public SpellConversionElementVM(MechanicActionBarSlotSpell slot) : base(slot)
        {
            Slot = slot;
            Icon.Value = slot.GetForeIcon();
        }

        public override void DisposeImplementation()
        {
        }
    }
}
