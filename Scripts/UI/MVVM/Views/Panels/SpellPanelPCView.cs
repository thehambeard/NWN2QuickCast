using Kingmaker.UI.MVVM._VM.Utility;
using NWN2QuickCast.UI.Extensions;
using NWN2QuickCast.UI.MVVM.Views.Elements;
using NWN2QuickCast.UI.MVVM.VMs.Elements;
using NWN2QuickCast.UI.MVVM.VMs.Panels;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.VirtualListSystem;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.Views.Panels
{ 
    internal class SpellPanelPCView : ViewBase<SpellPanelVM>, IInitializable
    {
        [SerializeField]
        private VirtualListVertical _spellVirtualList;

        [SerializeField]
        private ClassHeaderElementPCView _classHeaderPrefab;

        [SerializeField]
        private SpellLevelCollectionElementPCView _spellLevelCollectionPrefab;

        public override void BindViewImplementation()
        {
            base.AddDisposable(_spellVirtualList.Subscribe(ViewModel.Elements));

            _classHeaderPrefab.gameObject.FixTMPMaterialShader();
            _spellLevelCollectionPrefab.gameObject.FixTMPMaterialShader();
        }

        public override void DestroyViewImplementation()
        {
        }

        public void Initialize()
        {
            InitializeVirtualList(_spellVirtualList);
        }

        private void InitializeVirtualList(VirtualListComponent virtualListComponent)
        {
            virtualListComponent.Initialize(new IVirtualListElementTemplate[]
            {
                    new VirtualListElementTemplate<ClassHeaderElementVM>(_classHeaderPrefab),
                    new VirtualListElementTemplate<SpellLevelCollectionElementVM>(_spellLevelCollectionPrefab),
            });
        }
    }
}
