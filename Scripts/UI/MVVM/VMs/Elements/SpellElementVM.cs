using Kingmaker.UI.UnitSettings;
using UniRx;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    class SpellElementVM : SpellElementBaseVM
    {
        
        public readonly BoolReactiveProperty HasConversions = new BoolReactiveProperty();
        public readonly ReactiveCommand OpenConversionWindowCommand = new ReactiveCommand();
        public readonly ReactiveCommand CloseConversionWindowCommand = new ReactiveCommand();


        public SpellElementVM(MechanicActionBarSlot spellSlot) : base(spellSlot)
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
            base.DisposeImplementation();
        }
    }
}
