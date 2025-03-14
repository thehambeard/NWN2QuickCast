using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    class ClassHeaderElementVM : ElementBaseVM
    {
        public readonly StringReactiveProperty ClassNameText = new StringReactiveProperty("");
        public readonly StringReactiveProperty ExpandedText = new StringReactiveProperty("");

        public ClassHeaderElementVM(string classNameText, List<ElementBaseVM> children = null) : base(children)
        {
            ClassNameText.Value = classNameText;
            base.AddDisposable(base.IsExpanded.Subscribe(isExpanded => ExpandedText.Value = isExpanded ? "-" : "+"));
        }

        public override void DisposeImplementation()
        {
        }
    }
}
