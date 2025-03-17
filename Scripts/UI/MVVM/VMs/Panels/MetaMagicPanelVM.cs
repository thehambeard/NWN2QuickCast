using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using NWN2QuickCast.UI.MVVM.VMs.Elements;
using Owlcat.Runtime.Core;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace NWN2QuickCast.UI.MVVM.VMs.Panels
{
    public class MetaMagicPanelVM : BaseDisposable, IViewModel
    {
        public readonly ReactiveCollection<MetaMagicElementVM> MMElements = new ReactiveCollection<MetaMagicElementVM>();
        public readonly ReactiveProperty<UnitEntityData> Unit = new ReactiveProperty<UnitEntityData>();
        public bool HasActiveMetas => GetActiveMetas().Count > 0;
        public bool HasMetas => GetMetas().Count > 0;

        public MetaMagicPanelVM()
        {
            for (int i = 0; i < 12; i++)
                MMElements.Add(new MetaMagicElementVM());
        }

        public void OnUnitChanged(UnitEntityData unit)
        {
            var spellbook = unit.Descriptor.Spellbooks?.FirstOrDefault();

            if (unit == Unit.Value)
                return;

            Unit.Value = unit;

            ClearMetas();

            if (unit == null || spellbook == null || !unit.Descriptor.Spellbooks.Any(x => x.Blueprint.Spontaneous))
                return;

            var ruleCollectMetamagic = Rulebook.Trigger(new RuleCollectMetamagic(spellbook, null, null));

            int index;

            for (index = 0; index < ruleCollectMetamagic.KnownMetamagics.Count; index++)
                MMElements[index].SetMetaMagic(ruleCollectMetamagic.KnownMetamagics[index]);

            ClearMetas(index);
        }

        public void ClearMetas(int startIndex = 0)
        {
            for (int i = startIndex; i < MMElements.Count; i++)
                MMElements[i].ClearMetaMagic();
        }

        public List<(Feature feature, int heightenLevel)> GetMetas()
        {
            List<(Feature feature, int heightenLevel)> result = new List<(Feature feature, int heightenLevel)>();

            foreach (var element in MMElements.Where(x => x.HasMeta.Value))
                result.Add((element.MetaMagic.Value, element.HeightenLevel.Value));

            return result;
        }

        public List<(Feature feature, int heightenLevel)> GetActiveMetas()
        {
            List<(Feature feature, int heightenLevel)> result = new List<(Feature feature, int heightenLevel)>();

            foreach (var element in MMElements.Where(x => x.HasMeta.Value && x.IsActive.Value))
                result.Add((element.MetaMagic.Value, element.HeightenLevel.Value));

            return result;
        }


        public override void DisposeImplementation()
        {
            MMElements.ForEach(x => x.Dispose());
            MMElements.Clear();
        }
    }
}
