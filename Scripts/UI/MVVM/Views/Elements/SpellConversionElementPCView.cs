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

namespace NWN2QuickCast.UI.MVVM.Views.Elements
{
    class SpellConversionElementPCView : VirtualListElementViewBase<SpellConversionElementVM>
    {
        [SerializeField]
        private Image _icon;

        public override void BindViewImplementation()
        {
            base.AddDisposable(ViewModel.Icon.Subscribe(x => _icon.sprite = x));
        }

        public override void DestroyViewImplementation()
        {
        }
    }
}
