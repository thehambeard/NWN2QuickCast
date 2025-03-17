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
    class SpellElementPCView : ViewBase<SpellElementVM>
    {
        [SerializeField]
        private Image _iconImage;

        [SerializeField]
        private OwlcatMultiButton _button;

        [SerializeField]
        private TextMeshProUGUI _resourceText;

        [ConditionalShow("m_UseTooltipCustomPlace")]
        [SerializeField]
        [CanBeNull]
        private RectTransform m_TooltipCustomPlace;

        [SerializeField]
        private bool m_UseTooltipCustomPlace;

        private IDisposable _hoverCoolDown;
        private IDisposable _toolTip;

        public override void BindViewImplementation()
        {
            gameObject.FixTMPMaterialShader();
            base.AddDisposable(ViewModel.Icon.Subscribe(x => _iconImage.sprite = x));
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
            base.AddDisposable(_button.OnHoverAsObservable().Subscribe(x => ViewModel.OnHover(x)));
            base.AddDisposable(_toolTip = _button.SetTooltip(
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
                    null)));
            base.AddDisposable(ViewModel.ResourceValue.Subscribe(x => _resourceText.text = x.ToString()));
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
        private RectTransform TooltipPlace
        {
            get
            {
                if (!(this.m_TooltipCustomPlace != null))
                {
                    return base.transform as RectTransform;
                }
                return this.m_TooltipCustomPlace;
            }
        }


    }
}

