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
    class SpellElementVM : BaseDisposable, IViewModel
    {
        public readonly ReactiveProperty<Sprite> Icon = new ReactiveProperty<Sprite>();
        public readonly ReactiveProperty<TooltipBaseTemplate> Tooltip = new ReactiveProperty<TooltipBaseTemplate>();
        public readonly BoolReactiveProperty HasConversions = new BoolReactiveProperty();
        public readonly IntReactiveProperty ResourceValue = new IntReactiveProperty();
        public readonly ReactiveCommand OpenConversionWindowCommand = new ReactiveCommand();
        public readonly ReactiveCommand CloseConversionWindowCommand = new ReactiveCommand();

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
            base.AddDisposable(Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ => CloseConversionWindowCommand.Execute()));
        }

        private void OnUpdateHandler()
        {
            if (Spell.GetResource() != ResourceValue.Value)
                ResourceValue.Value = Spell.GetResource();
        }

        public void OnClick()
        {
            if (!Spell.IsPossibleActive() && Spell.GetConvertedAbilityData().Count > 0 && OpenConversionWindowCommand.CanExecute.Value)
                OpenConversionWindowCommand.Execute();
            else
                CastSpell();
        }

        public void OnRightClick()
        {
            if (Spell.GetConvertedAbilityData().Count > 0 && OpenConversionWindowCommand.CanExecute.Value)
                OpenConversionWindowCommand.Execute();
        }

        public void OnHover(bool state)
        {
            if (Spell == null)
                return;

            Spell.OnHover(state);
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
