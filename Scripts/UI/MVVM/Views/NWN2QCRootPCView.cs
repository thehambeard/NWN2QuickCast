using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.UI.MVVM._PCView.ActionBar;
using Kingmaker.UI.MVVM._VM.Utility;
using NWN2QuickCast.UI.Extensions;
using NWN2QuickCast.UI.MVVM.VMs;
using NWN2QuickCast.Utility.Helpers;
using Owlcat.Runtime.UI.MVVM;
using TMPro;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.Views
{
    public class NWN2QCRootPCView : ViewBase<NWN2QCRootVM>, IInitializable
    {
        [SerializeField]
        private NWNQCWindowPCView _windowPCView;

        public static NWN2QCRootPCView Root { get; private set; }



        public override void BindViewImplementation()
        {
            _windowPCView.Bind(new NWNQCWindowVM());
            _windowPCView.gameObject.FixTMPMaterialShader();
            Root = this;
        }

        public override void DestroyViewImplementation()
        {
        }

        public void Initialize()
        {
            _windowPCView.Initialize();
        }

        [HarmonyPatch]
        internal static class BindPatch
        {
            static NWN2QCRootPCView _rootPCView;
            static GameObject _rootPCViewPrefab;

            [HarmonyPatch(typeof(ActionBarBaseView), nameof(ActionBarBaseView.BindViewImplementation))]
            [HarmonyPostfix]
            private static void Bind()
            {
                if (_rootPCViewPrefab == null)
                    _rootPCViewPrefab = ResourcesLibrary.TryGetResource<GameObject>("a27b35ffed55e50408a77b751471e0b6");

                if (_rootPCView != null)
                    _rootPCView.DestroyView();

                var go = GameObject.Instantiate(_rootPCViewPrefab, WrathHelpers.GetStaticCanvas().transform, false);
                var rootPCView = go.GetComponent<NWN2QCRootPCView>();
                rootPCView.Initialize();
                rootPCView.Bind(new NWN2QCRootVM());
            }
        }
    }
}
