using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using NWN2QuickCast.UI.MVVM.Events;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    public class MetaMagicElementVM : BaseDisposable, IViewModel
    {
        public readonly ReactiveProperty<Feature> MetaMagic = new ReactiveProperty<Feature>();
        public readonly IntReactiveProperty HeightenLevel = new IntReactiveProperty(-1);
        public readonly BoolReactiveProperty IsActive = new BoolReactiveProperty(false);
        public IReadOnlyReactiveProperty<bool> HasMeta => MetaMagic.Select(meta => meta != null).ToReactiveProperty();

        public override void DisposeImplementation()
        {
        }

        public void ToggleActive()
        {
            if (IsActive.Value)
                SetInactive();
            else
                SetActive();
        }

        public void SetActive()
        {
            IsActive.Value = true;
            EventBus.RaiseEvent<IMetaMagicHandler>(h => h.OnMetaMagicAdd(MetaMagic.Value, HeightenLevel.Value));
        }
        
        public void SetInactive()
        {
            IsActive.Value = false;
            EventBus.RaiseEvent<IMetaMagicHandler>(h => h.OnMetaMagicRemove(MetaMagic.Value));
        }

        public void SetMetaMagic(Feature meta, int heightenLevel = -1)
        {
            MetaMagic.Value = meta;
            HeightenLevel.Value = heightenLevel;
        }

        public void ClearMetaMagic()
        {
            SetInactive();
            MetaMagic.Value = null;
            HeightenLevel.Value = -1;
        }
    }
}
