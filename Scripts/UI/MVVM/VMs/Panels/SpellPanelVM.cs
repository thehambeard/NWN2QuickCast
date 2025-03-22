using Kingmaker.EntitySystem.Entities;
using Kingmaker.GameModes;
using Kingmaker.UI.MVVM._VM.ActionBar;
using Kingmaker;
using Kingmaker.UI.UnitSettings;
using NWN2QuickCast.UI.MVVM.VMs.Elements;
using Owlcat.Runtime.UI.MVVM;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Kingmaker.EntitySystem.Persistence;
using Kingmaker.PubSubSystem;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using NWN2QuickCast.UI.MVVM.Events;
using Kingmaker.UnitLogic.FactLogic;
using UnityEngine;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Owlcat.Runtime.Core;
using Owlcat.Runtime.UI.Utility;

namespace NWN2QuickCast.UI.MVVM.VMs.Panels
{
    public class SpellPanelVM : BaseDisposable,
        IViewModel,
        ISelectionHandler,
        IUnitEquipmentHandler,
        ISpellBookUIHandler,
        IPlayerAbilitiesHandler,
        ISpellBookRest,
        ISpellBookCustomSpell,
        ILevelUpCompleteUIHandler,
        IFactCollectionUpdatedHandler,
        IMetaMagicHandler,
        IConversionWindowHandler
    {
        public readonly ReactiveCollection<VirtualListElementVMBase> Elements = new ReactiveCollection<VirtualListElementVMBase>();
        private ClassHeaderElementVM _root = new ClassHeaderElementVM("root");

        public readonly ReactiveProperty<UnitEntityData> SelectedUnit = new ReactiveProperty<UnitEntityData>();
        public UnitEntityData SelectedUnitValue => SelectedUnit.Value;

        private bool _needUpdateSelection;
        private bool _needsReset;
        private List<(MechanicActionBarSlotSpell spell, MetamagicBuilder metaBuilder)> _spells = new List<(MechanicActionBarSlotSpell, MetamagicBuilder)>();
        
        public readonly MetaMagicPanelVM MetaMagicPanelVM;
        public readonly NWN2ConversionWindowVM ConversionWindowVM;

        public SpellPanelVM(MetaMagicPanelVM metaVM, NWN2ConversionWindowVM conversionWindowVM)
        {
            MetaMagicPanelVM = metaVM;
            ConversionWindowVM = conversionWindowVM;

            base.AddDisposable(MainThreadDispatcher.UpdateAsObservable().Subscribe(_ => OnUpdateHandler()));
            base.AddDisposable(EventBus.Subscribe(this));
            BuildElements();
            base.AddDisposable(Game.Instance.SelectionCharacter.SelectionCharacterUpdated.Subscribe(_ =>
                this._needUpdateSelection = Game.Instance.SelectionCharacter.SelectedUnit?.Value.Value != SelectedUnitValue));
            base.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(x =>
                this._needUpdateSelection = x != SelectedUnitValue));
        }

        public override void DisposeImplementation()
        {
            Elements.Clear();
            _spells = null;
        }

        private void OnUnitChanged(UnitEntityData unit)
        {
            MetaMagicPanelVM.OnUnitChanged(unit);

            if (unit != null)
                CollectSpells(unit);
        }
        private void BuildElements()
        {
            _root = new ClassHeaderElementVM("root");
            Elements.Clear();

            foreach (var slotGroup in _spells
                .Where(x => !MetaMagicPanelVM.HasActiveMetas || x.spell.Spell.Spellbook.Blueprint.Spontaneous)
                .GroupBy(x => x.spell.Spell.Spellbook.Blueprint.Name))
            {
                var spellCollection = new List<ElementBaseVM>();

                foreach (var slotByLevel in slotGroup
                    .GroupBy(x => x.metaBuilder.ResultSpellLevel)
                    .OrderBy(x => x.Key))
                {
                    var spellList = new List<SpellElementVM>();

                    foreach (var spell in slotByLevel)
                    {
                        if (spell.spell.IsBad())
                        {
                            Main.Logger.Warning($"Mech slot bad: {spell}");
                            continue;
                        }

                        if (MetaMagicPanelVM.HasActiveMetas && spell.metaBuilder.ResultSpellLevel > spell.spell.Spell.Spellbook.MaxSpellLevel)
                            continue;

                        spellList.Add(new SpellElementVM(CreateSpell(spell)));
                    }

                    if (spellList.Count > 0)
                        spellCollection.Add(new SpellLevelCollectionElementVM((int)slotByLevel.Key, spellList));
                }
                _root.AddChild(new ClassHeaderElementVM(slotGroup.Key, spellCollection));
            }
            _root.Flatten(false).ForEach(x => Elements.Add(x));
        }

        private MechanicActionBarSlotSpell CreateSpell((MechanicActionBarSlotSpell spell, MetamagicBuilder metamagicBuilder) spell)
        {
            var result = spell.spell;

            if (spell.metamagicBuilder.AppliedMetamagicFeatures.Count == 0 
                || result is MechanicActionBarSlotMemorizedSpell
                || result.Spell.MetamagicData != null)
                return result;

            return new MechanicActionBarSlotSpontaneousSpell(spell.metamagicBuilder.ResultAbilityData)
            {
                Unit = SelectedUnitValue
            };
        }

        private void OnUpdateHandler()
        {
            if (!LoadingProcess.Instance.IsLoadingInProcess)
            {
                if (_needUpdateSelection)
                {
                    UpdateSelection();
                    _needUpdateSelection = false;
                    _needsReset = true;
                }
                if ((_needsReset || SelectedUnitValue != null && SelectedUnitValue.UISettings.Dirty)
                    && (Game.Instance.CurrentMode == GameModeType.Default || Game.Instance.CurrentMode == GameModeType.Pause)
                    && SelectedUnitValue != null
                    && SelectedUnitValue.IsInGame)
                {
                    OnUnitChanged(SelectedUnitValue);
                    BuildElements();
                    _needsReset = false;
                }
            }
        }

        private void CollectSpells(UnitEntityData unit)
        {
            _spells = ActionBarSpellbookHelper.Fetch(unit)
                .Where(slot => !MetaMagicPanelVM.HasActiveMetas || (slot.Spell.MetamagicData == null && PossibleToApplyAll(slot.Spell, MetaMagicPanelVM.GetActiveMetas())))
                .Select(slot =>
                {
                    var builder = new MetamagicBuilder(slot.Spell.Spellbook, slot.Spell);
                    ApplyAllMeta(MetaMagicPanelVM.GetActiveMetas(), builder);
                    return (slot, builder);
                })
                .ToList();
        }

        private void ApplyMeta((Feature feature, int heightenlevel) tuple, MetamagicBuilder metamagicBuilder)
        {
            if (tuple.heightenlevel == -1)
                metamagicBuilder.AddMetamagic(tuple.feature);
            else
                metamagicBuilder.AddHeightenMetamagic(tuple.feature, tuple.heightenlevel);
        }

        private void ApplyAllMeta(List<(Feature feature, int heightenlevel)> tuples, MetamagicBuilder metamagicBuilder)
        {
            foreach (var tuple in tuples)
                ApplyMeta(tuple, metamagicBuilder);
        }

        private bool PossibleToApplyAll(AbilityData spell, List<(Feature feature, int heightenLevel)> tuples)
        {
            bool result = true;

            foreach (var tuple in tuples)
                result &= PossibleToApply(spell, tuple);

            return result;
        }

        private bool PossibleToApply(AbilityData spell, (Feature feature, int heightenLevel) tuple)
        {
            if (spell == null)
                return false;

            Metamagic metamagic = tuple.feature.GetComponent<AddMetamagicFeat>().Metamagic;

            return (spell.Blueprint.AvailableMetamagic & metamagic) == metamagic
                && spell.MetamagicData == null || (spell.MetamagicData != null && (spell.MetamagicData.MetamagicMask & metamagic) != metamagic);
        }

        private void UpdateSelection() => SelectedUnit.Value =
            Game.Instance.SelectionCharacter.IsSingleSelected
            && Game.Instance.SelectionCharacter.CurrentSelectedCharacter.IsDirectlyControllable
                ? Game.Instance.SelectionCharacter.CurrentSelectedCharacter
                : null;

        public void OnUnitSelectionAdd(UnitEntityData selected) => _needUpdateSelection = selected != SelectedUnitValue;
        public void OnUnitSelectionRemove(UnitEntityData selected) => _needUpdateSelection = true;

        void IFactCollectionUpdatedHandler.HandleFactCollectionUpdated(EntityFactsProcessor collection)
        {
            EntityDataBase owner = collection.Manager.Owner;
            if (this._needsReset || (object)this.SelectedUnitValue != (object)owner)
                return;
            switch (collection)
            {
                case AbilityCollection _:
                case ActivatableAbilityCollection _:
                    this._needsReset = true;
                    break;
            }
        }

        void ILevelUpCompleteUIHandler.HandleLevelUpComplete(UnitEntityData unit, bool isChargen)
        {
            this._needsReset = this._needsReset || (UnitEntityData)this.SelectedUnitValue?.Descriptor == (UnitDescriptor)unit;
        }

        void IPlayerAbilitiesHandler.HandleAbilityRemoved(Ability ability)
        {
            if ((ability != null ? ability.Blueprint.GetComponent<AbilityDeliverTouch>() : (AbilityDeliverTouch)null) != null)
                return;
            this._needsReset = true;
        }

        void ISpellBookRest.OnSpellBookRestHandler(UnitEntityData unit)
        {
            this._needsReset = this._needsReset || this.SelectedUnitValue == (UnitDescriptor)unit;
        }

        void ISpellBookCustomSpell.AddSpellHandler(AbilityData ability)
        {
            this._needsReset = this._needsReset || this.SelectedUnitValue?.Descriptor == ability.Caster;
        }

        void ISpellBookCustomSpell.RemoveSpellHandler(AbilityData ability)
        {
            this._needsReset = this._needsReset || this.SelectedUnitValue?.Descriptor == ability.Caster;
        }

        void ISpellBookUIHandler.HandleMemorizedSpell(AbilityData data, UnitDescriptor owner)
        {
            this._needsReset = this._needsReset || this.SelectedUnitValue?.Descriptor == owner;
        }

        void ISpellBookUIHandler.HandleForgetSpell(AbilityData data, UnitDescriptor owner)
        {
            this._needsReset = this._needsReset || this.SelectedUnitValue?.Descriptor == owner;
        }

        void IPlayerAbilitiesHandler.HandleAbilityAdded(Ability ability)
        {
            if ((ability != null ? ability.Blueprint.GetComponent<AbilityDeliverTouch>() : (AbilityDeliverTouch)null) != null)
                return;
            this._needsReset = true;
        }

        public void HandleEquipmentSlotUpdated(ItemSlot slot, ItemEntity previousItem)
        {
            this._needsReset = this._needsReset || this.SelectedUnitValue?.Descriptor == slot.Owner;
        }

        public void OnMetaMagicAdd(Feature metamagic, int heightenLevel = -1)
        {
            _needsReset = true;
        }

        public void OnMetaMagicRemove(Feature metamagic)
        {
            _needsReset = true;
        }

        public void OpenConversionWindow(RectTransform buttonRect, SlotConversion slotConversion, UnitEntityData unit) =>
            ConversionWindowVM.ShowWindow(buttonRect, slotConversion, unit);


        public void CloseConversionWindow() 
            => ConversionWindowVM.HideWindow();
    }
}
