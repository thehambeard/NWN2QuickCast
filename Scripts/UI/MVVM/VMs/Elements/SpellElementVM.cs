using Kingmaker;
using Kingmaker.UI.UnitSettings;
using Kingmaker.UnitLogic.Abilities;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.Tooltips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    class SpellElementVM : BaseDisposable, IViewModel
    {
        public readonly ReactiveProperty<Sprite> Icon = new ReactiveProperty<Sprite>();
        public readonly ReactiveProperty<TooltipBaseTemplate> Tooltip = new ReactiveProperty<TooltipBaseTemplate>();
        public readonly BoolReactiveProperty HasConversions = new BoolReactiveProperty();
        public readonly IntReactiveProperty ResourceValue = new IntReactiveProperty();

        public readonly MechanicActionBarSlotSpell Spell;
        
        public SpellElementVM(MechanicActionBarSlotSpell spellSlot)
        {
            Spell = spellSlot;

            Icon.Value = spellSlot.GetIcon();

            var convertdata = Spell.GetConvertedAbilityData();
            HasConversions.Value = convertdata.Count > 0 && (Spell.IsPossibleActive(null));
            if (HasConversions.Value)
                Main.Logger.Log("Converts!");

            Tooltip.Value = Spell.GetTooltipTemplate();
            ResourceValue.Value = Spell.GetResource();

            base.AddDisposable(MainThreadDispatcher.UpdateAsObservable().Subscribe(_ => OnUpdateHandler()));
        }

        private void OnUpdateHandler()
        {
            if (Spell.GetResource() != ResourceValue.Value)
                ResourceValue.Value = Spell.GetResource();
        }

        public void OnClick()
        {
            CastSpell();
        }

        public void CastSpell()
        {
            Spell.OnClick();
        }

        public override void DisposeImplementation()
        {
        }
    }
}
