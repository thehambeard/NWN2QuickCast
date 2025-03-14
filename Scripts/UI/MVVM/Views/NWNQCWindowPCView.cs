using Kingmaker.UI.MVVM._VM.Utility;
using NWN2QuickCast.UI.MVVM.Views.Panels;
using NWN2QuickCast.UI.MVVM.VMs.Panels;
using NWN2QuickCast.UI.MVVM.VMs;
using Owlcat.Runtime.UI.MVVM;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.Views
{
    class NWNQCWindowPCView : ViewBase<NWNQCWindowVM>, IInitializable
    {
        [SerializeField]
        private SpellPanelPCView _spellPanelPCView;

        [SerializeField]
        private MetaMagicPanelPCView _metaMagicPanelPCView;


        public override void BindViewImplementation()
        {
            var metaVM = new MetaMagicPanelVM();
            _metaMagicPanelPCView.Bind(metaVM);
            _spellPanelPCView.Bind(new SpellPanelVM(metaVM));
        }

        public override void DestroyViewImplementation()
        {
        }

        public void Initialize()
        {
            _spellPanelPCView.Initialize();
        }
    }
}
