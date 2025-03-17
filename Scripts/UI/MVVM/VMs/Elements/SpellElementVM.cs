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
using TurnBased.Controllers;
using UniRx;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    class SpellElementVM : SpellElementBase
    {
        
        public readonly BoolReactiveProperty HasConversions = new BoolReactiveProperty();
        public readonly ReactiveCommand OpenConversionWindowCommand = new ReactiveCommand();
        public readonly ReactiveCommand CloseConversionWindowCommand = new ReactiveCommand();


        public SpellElementVM(MechanicActionBarSlotSpell spellSlot) : base(spellSlot)
        {
            Icon.Value = spellSlot.GetIcon();

            var convertdata = Spell.GetConvertedAbilityData();
            HasConversions.Value = convertdata.Count > 0 && (Spell.IsPossibleActive(null));
        }

        public override void OnClick()
        {
            if (!Spell.IsPossibleActive() && Spell.GetConvertedAbilityData().Count > 0 && OpenConversionWindowCommand.CanExecute.Value)
            {
                OpenConversionWindowCommand.Execute();
            }
            else
                base.OnClick();
        }

        public override void OnRightClick()
        {
            if (Spell.GetConvertedAbilityData().Count > 0 && OpenConversionWindowCommand.CanExecute.Value)
            {
                OpenConversionWindowCommand.Execute();
            }
        }

        public override void OnHover(bool state) =>
            base.OnHover(state);

        public override void DisposeImplementation()
        {
        }
    }
}
