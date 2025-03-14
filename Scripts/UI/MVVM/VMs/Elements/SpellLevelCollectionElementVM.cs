using Kingmaker.EntitySystem.Entities;
using Kingmaker.GameModes;
using Kingmaker;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using Kingmaker.EntitySystem.Persistence;
using Kingmaker.UI.MVVM._VM.ActionBar;
using Kingmaker.UI.UnitSettings;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    class SpellLevelCollectionElementVM : ElementBaseVM
    {
        
        public readonly IntReactiveProperty Level = new IntReactiveProperty();
        public readonly ReactiveCollection<SpellElementVM> SpellElements = new ReactiveCollection<SpellElementVM>();
        
        public SpellLevelCollectionElementVM(int level, List<SpellElementVM> spellElementVMs = null)
        {
            Level.Value = level;

            if (spellElementVMs != null)
                SpellElements = new ReactiveCollection<SpellElementVM>(spellElementVMs);
        }

        public override void DisposeImplementation()
        {
        }

        
    }
}
