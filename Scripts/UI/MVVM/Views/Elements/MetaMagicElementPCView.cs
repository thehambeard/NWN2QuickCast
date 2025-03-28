using NWN2QuickCast.UI.MVVM.VMs.Elements;
using Owlcat.Runtime.UI.MVVM;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.Other;

namespace NWN2QuickCast.UI.MVVM.Views.Elements
{
    class MetaMagicElementPCView : ViewBase<MetaMagicElementVM>
    {
        [SerializeField]
        private OwlcatMultiButton _button;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private GameObject _activeLayer;

        private TooltipHandler _tooltipHandler;

        public override void BindViewImplementation()
        {
            base.AddDisposable(ViewModel.MetaMagic.Subscribe(x =>
            {
                if (x != null)
                {
                    _icon.sprite = x.Icon;
                    _icon.color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    _icon.sprite = null;
                    _icon.color = new Color(0f, 0f, 0f, 0f);
                }
            }));
            base.AddDisposable(_button.OnLeftClickAsObservable().Subscribe(_ =>
            {
                ViewModel.ToggleActive(((RectTransform)transform).anchoredPosition);
            }));
            base.AddDisposable(ViewModel.NewToolTipCommand.Subscribe(x =>
            {
                _tooltipHandler?.Dispose();
                _tooltipHandler = this.SetTooltip(ViewModel.Tooltip, default);
            }));
            base.AddDisposable(ViewModel.DisposeToolTipCommand.Subscribe(x =>
            {
                _tooltipHandler?.Dispose();
            }));
            base.AddDisposable(ViewModel.IsActive.Subscribe(x => _activeLayer.SetActive(x)));
        }

        public override void DestroyViewImplementation()
        {
            _tooltipHandler?.Dispose();
        }
    }
}
