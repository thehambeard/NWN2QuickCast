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

namespace NWN2QuickCast.UI.MVVM.Views.Elements
{
    class SpellElementPCView : ViewBase<SpellElementVM>
    {
        [SerializeField]
        private Image _iconImage;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private TextMeshProUGUI _resourceText;

        public override void BindViewImplementation()
        {
            gameObject.FixTMPMaterialShader();
            base.AddDisposable(ViewModel.Icon.Subscribe(x => _iconImage.sprite = x));
            base.AddDisposable(_button.onClick.AsObservable().Subscribe(_ => ViewModel.OnClick()));
            base.AddDisposable(ViewModel.ResourceValue.Subscribe(x => _resourceText.text = x.ToString()));
        }

        public override void DestroyViewImplementation()
        {
        }
    }
}
