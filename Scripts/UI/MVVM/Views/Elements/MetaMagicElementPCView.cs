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
using Kingmaker.PubSubSystem;

namespace NWN2QuickCast.UI.MVVM.Views.Elements
{
    class MetaMagicElementPCView : ViewBase<MetaMagicElementVM>
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private Image _icon;

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
            base.AddDisposable(_button.onClick.AsObservable().Subscribe(_ => ViewModel.ToggleActive()));
        }

        public override void DestroyViewImplementation()
        {
        }
    }
}
