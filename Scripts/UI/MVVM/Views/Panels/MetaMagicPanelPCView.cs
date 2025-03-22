using Kingmaker.EntitySystem.Entities;
using NWN2QuickCast.UI.MVVM.Views.Elements;
using NWN2QuickCast.UI.MVVM.VMs.Elements;
using NWN2QuickCast.UI.MVVM.VMs.Panels;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace NWN2QuickCast.UI.MVVM.Views.Panels
{
    class MetaMagicPanelPCView : ViewBase<MetaMagicPanelVM>
    {
        [SerializeField]
        private MetaMagicElementPCView _mmElementPrefab;

        [SerializeField]
        private RectTransform _content;

        [SerializeField]
        private HeightenedSelectPanelPCView _heightenedSelectPrefab;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        public override void BindViewImplementation()
        {
            foreach(var element in ViewModel.MMElements)
            {
                var go = GameObject.Instantiate(_mmElementPrefab, _content, false);
                go.Bind(element);
            }

            _heightenedSelectPrefab.Bind(ViewModel.HeightenedSelectPanelVM);
            _heightenedSelectPrefab.transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        public override void DestroyViewImplementation()
        {
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
        }

        public void Show()
        {
            _canvasGroup.alpha = 1f;
        }

        public void ToggleHideShow()
        {
            if (_canvasGroup.alpha == 0f)
                Show();
            else
                Hide();
        }

        public void SetVisible(bool state)
        {
            if (state)
                Show();
            else
                Hide();
        }
    }
}
