using NWN2QuickCast.UI.MVVM.VMs.Elements;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using NWN2QuickCast.UI.Extensions;
using NWN2QuickCast.UI.MVVM.VMs;
using Kingmaker.PubSubSystem;
using NWN2QuickCast.UI.MVVM.Events;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.Other;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Tooltips;
using JetBrains.Annotations;
using Kingmaker.Utility;

namespace NWN2QuickCast.UI.MVVM.Views.Elements
{
    class SpellElementPCView : SpellElementBasePCView<SpellElementVM>
    {
        private IDisposable _hoverCoolDown;

        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            base.AddDisposable(_button.OnLeftClickAsObservable().Subscribe(_ =>
            {
                ViewModel.OnClick();
                _hoverCoolDown?.Dispose();
                _toolTip?.Dispose();
                _hoverCoolDown = Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
                {
                    _toolTip = _button.SetTooltip(
                        ViewModel.Tooltip,
                        new TooltipConfig(
                            InfoCallPCMethod.ShiftRightMouseButton,
                            InfoCallConsoleMethod.LongShortRightStickButton,
                            false,
                            false,
                            this.TooltipPlace,
                            0,
                            0,
                            0,
                            null));
                });
            }));
            base.AddDisposable(_button.OnRightClickAsObservable().Subscribe(_ =>
            { 
                ViewModel.OnRightClick();
                _hoverCoolDown?.Dispose();
                _toolTip?.Dispose();
                _hoverCoolDown = Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
                {
                    _toolTip = _button.SetTooltip(
                        ViewModel.Tooltip,
                        new TooltipConfig(
                            InfoCallPCMethod.ShiftRightMouseButton,
                            InfoCallConsoleMethod.LongShortRightStickButton,
                            false,
                            false,
                            this.TooltipPlace,
                            0,
                            0,
                            0,
                            null));
                });
            }));
            
            base.AddDisposable(ViewModel.OpenConversionWindowCommand.Subscribe(_ => OpenConversionWindow()));
            base.AddDisposable(ViewModel.CloseConversionWindowCommand.Subscribe(_ => CloseConversionWindow()));
        }

        public void OpenConversionWindow()
        {
            EventBus.RaiseEvent<IConversionWindowHandler>(h => h.OpenConversionWindow(
                (RectTransform)_button.transform,
                ViewModel.Spell.GetConvertedAbilityData(),
                ViewModel.Spell.Unit));
        }

        public void CloseConversionWindow()
        {
            EventBus.RaiseEvent<IConversionWindowHandler>(h => h.CloseConversionWindow());
        }

        public override void DestroyViewImplementation()
        {
        }
    }
}

