using Kingmaker.UI.MVVM._VM.Utility;
using NWN2QuickCast.UI.MVVM.VMs.Elements;
using NWN2QuickCast.UI.MVVM.Views.Elements;
using NWN2QuickCast.UI.MVVM.VMs;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.VirtualListSystem;
using Owlcat.Runtime.UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using Kingmaker.UI.Constructor;
using NWN2QuickCast.UI.Extensions;
using UnityEngine.EventSystems;
using NWN2QuickCast.Settings;
using Kingmaker;

namespace NWN2QuickCast.UI.MVVM.Views
{
    class NWN2ConversionWindowPCView : ViewBase<NWN2ConversionWindowVM>, IInitializable, IPointerClickHandler
    {
        [SerializeField]
        private VirtualListGridVertical _virtGrid;

        [SerializeField]
        private SpellConversionElementPCView _coversionPrefab;

        [SerializeField]
        private int _maxElementsInRow = 5;

        [SerializeField]
        private Vector2 _padding = new Vector2(30f, 25f);

        [SerializeField]
        private RectTransform _parentRect;
        
        [SerializeField]
        private RectTransform _scrollBarTransform;

        [SerializeField]
        private CanvasGroup _menuCanvasGroup;

        [SerializeField]
        private RectTransform _conversionBoxRect;

        [SerializeField]
        private float _fadeDuration = 0.25f;

        public void Initialize()
        {
            _virtGrid.Initialize(new IVirtualListElementTemplate[]
            {
                new VirtualListElementTemplate<SpellConversionElementVM>(_coversionPrefab)
            });
            gameObject.FixTMPMaterialShader();
        }

        public override void BindViewImplementation()
        {
            base.AddDisposable(_virtGrid.Subscribe(ViewModel.Elements));
            base.AddDisposable(ViewModel.Elements.ObserveCountChanged().Subscribe(UpdateGridLayout));
            base.AddDisposable(ViewModel.ShowWindowCommand.Subscribe(_ => ShowMenu()));
            base.AddDisposable(ViewModel.HideWindowCommand.Subscribe(_ => HideMenu()));
            UpdateGridLayout(ViewModel.Elements.Count);
            gameObject.SetActive(false);
        }

        private void UpdateGridLayout(int elementCount)
        {
            if (elementCount == 0)
                return;

            int idealRowCount = Mathf.FloorToInt(Mathf.Sqrt(elementCount));

            if (idealRowCount == 0)
                idealRowCount++;

            var elementsInRow = Mathf.Min(idealRowCount, _maxElementsInRow);
            float cellSize = _virtGrid.m_LayoutSettings.Width
                + _virtGrid.m_LayoutSettings.Padding.Right
                + _virtGrid.m_LayoutSettings.Padding.Left;

            _virtGrid.m_LayoutSettings.ElementsInRow = elementsInRow;

            float rawWidth = elementsInRow * cellSize + _padding.x;

            float maxHeight = _maxElementsInRow * cellSize + _padding.y;
            float rawHeight = Mathf.Min(Mathf.Ceil((float) elementCount / idealRowCount) * cellSize + _padding.y, maxHeight);

            _conversionBoxRect.sizeDelta = new Vector2(rawWidth, rawHeight);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_conversionBoxRect);
            _virtGrid.LateUpdate();
        }

        public void ShowMenu()
        {
            GetScaleSettings();
            gameObject.SetActive(true);
            _menuCanvasGroup.alpha = 0f;
            StartCoroutine(PlaceAndFadeMenuNextFrame());
        }

        public void HideMenu()
        {
            _menuCanvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.OutQuad);
            gameObject.SetActive(false);
        }

        private void GetScaleSettings()
        {
            var rect = (RectTransform)transform;
            var setting = Main.Settings.GetSetting<WindowSetting>(SettingKeys.MainWindowSetting);

            rect.localScale = new Vector3(
                setting.WindowScaleX,
                setting.WindowScaleY,
                setting.WindowScaleZ);
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

                    if (result.gameObject.transform.IsChildOf(_conversionBoxRect))
                    {
                        clickedInside = true;
                        break;
                    }
                }

                if (!clickedInside)
                    HideMenu();
            }
        }

        private IEnumerator PlaceAndFadeMenuNextFrame()
        {
            yield return new WaitForEndOfFrame();

            Vector2[] directions = new Vector2[]
            {
                Vector2.right,
                Vector2.left,
                Vector2.up,
                Vector2.down,
            };

            foreach (var dir in directions)
            {
                Vector2 candidatePos = UIUtility.LimitPositionRectInRect(GetCandidatePosition(dir), _parentRect, _conversionBoxRect);
                _conversionBoxRect.anchoredPosition = candidatePos;
                
                    
                    if (UIUtility.AreRectTransformsEdgeToEdge(_conversionBoxRect, ViewModel.ButtonRect))
                    break;
            }
            _menuCanvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.OutQuad);
        }

        private Vector2 GetCandidatePosition(Vector2 direction)
        {
            Vector3[] buttonWorldCorners = new Vector3[4];
            ViewModel.ButtonRect.GetWorldCorners(buttonWorldCorners);
            Vector3 buttonCenterWorld = (buttonWorldCorners[0] + buttonWorldCorners[2]) / 2f;
            Vector2 buttonCenterLocal = _parentRect.InverseTransformPoint(buttonCenterWorld);
            Vector2 buttonSize = ViewModel.ButtonRect.sizeDelta;

            Vector2 offset = Vector2.zero;
            Vector2 pivot = Vector2.one * 0.5f;

            if (direction == Vector2.right)
            {
                pivot = new Vector2(0f, 0.5f);
                offset = new Vector2(buttonSize.x / 2f, 0f);
            }
            else if (direction == Vector2.left)
            {
                pivot = new Vector2(1f, 0.5f);
                offset = new Vector2(-buttonSize.x / 2f, 0f);
            }
            else if (direction == Vector2.down)
            {
                pivot = new Vector2(0.5f, 1f);
                offset = new Vector2(0f, -buttonSize.y / 2f);
            }
            else if (direction == Vector2.up)
            {
                pivot = new Vector2(0.5f, 0f);
                offset = new Vector2(0f, buttonSize.y / 2f);
            }

            _conversionBoxRect.pivot = pivot;

            return buttonCenterLocal + offset;
        }

        public override void DestroyViewImplementation()
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }
    }
}
