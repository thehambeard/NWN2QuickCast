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
using Owlcat.Runtime.UI.Controls.Other;

namespace NWN2QuickCast.UI.MVVM.Views.Elements
{
    class SpellConversionElementPCView : SpellElementBasePCView<SpellConversionElementVM>
    {
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            base.AddDisposable(_button.OnLeftClickAsObservable().Subscribe(_ => ViewModel.OnClick()));
        }

        public override void DestroyViewImplementation()
        {
        }
    }
}
