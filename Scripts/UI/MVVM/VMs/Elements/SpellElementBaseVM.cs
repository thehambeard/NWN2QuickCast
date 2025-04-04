﻿using Kingmaker.UI.UnitSettings;
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
    public abstract class SpellElementBaseVM : VirtualListElementVMBase
    {
        public readonly ReactiveProperty<Sprite> Icon = new ReactiveProperty<Sprite>();
        public readonly ReactiveProperty<TooltipBaseTemplate> Tooltip = new ReactiveProperty<TooltipBaseTemplate>();
        public readonly IntReactiveProperty ResourceValue = new IntReactiveProperty();

        public readonly MechanicActionBarSlot Spell;

        public SpellElementBaseVM(MechanicActionBarSlot spellSlot)
        {
            Spell = spellSlot;

            Tooltip.Value = Spell.GetTooltipTemplate();
            ResourceValue.Value = Spell.GetResource();

            base.AddDisposable(MainThreadDispatcher
                .UpdateAsObservable()
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(_ => OnUpdateHandler()));
        }

        protected virtual void OnUpdateHandler()
        {
            try
            {
                var resource = Spell.GetResource();

                if (resource != ResourceValue.Value)
                    ResourceValue.Value = resource;
            }
            catch
            {
                Dispose();
            }
        }

        public virtual void OnClick() => CastSpell();


        public virtual void OnRightClick() { }

        public virtual void OnHover(bool state)
        {
            if (Spell == null)
                return;

            Spell.OnHover(state);
        }

        public void CastSpell() => Spell.OnClick();

        public override void DisposeImplementation()
        {
        }
    }
}
