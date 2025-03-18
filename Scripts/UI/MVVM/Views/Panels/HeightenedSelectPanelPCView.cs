using NWN2QuickCast.UI.MVVM.VMs.Panels;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NWN2QuickCast.UI.MVVM.Views.Panels
{
    public class HeightenedSelectPanelPCView : ViewBase<HeightenedSelectPanelVM>
    {
        [SerializeField]
        private List<Button> _buttons;

        private RectTransform _rectTransform;

        public override void BindViewImplementation()
        {
            _rectTransform = (RectTransform)transform;
            for (int i = 0; i < _buttons.Count; i++)
            {
                int level = i + 1;
                base.AddDisposable(_buttons[i].onClick.AsObservable()
                    .Subscribe(_ => ViewModel.LevelSelected.Value = level));
            }

            base.AddDisposable(ViewModel.ShowWindowCommand.Subscribe(x =>
            {
                _rectTransform.anchoredPosition = x;
                gameObject.SetActive(true);
            }));
            base.AddDisposable(ViewModel.HideWindowCommand.Subscribe(_ => gameObject.SetActive(false)));
            gameObject.SetActive(false);
        }

        public override void DestroyViewImplementation()
        {
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                bool clickedInside = false;

                foreach (var result in results)
                {
                    if (result.gameObject == null) continue;

                    if (result.gameObject.transform.IsChildOf(_rectTransform))
                    {
                        clickedInside = true;
                        break;
                    }
                }

                if (!clickedInside)
                {
                    _rectTransform.gameObject.SetActive(false);
                }

            }
        }
    }
}
