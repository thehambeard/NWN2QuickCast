using Kingmaker.UI.MVVM._VM.Utility;
using NWN2QuickCast.UI.MVVM.Views.Panels;
using NWN2QuickCast.UI.MVVM.VMs.Panels;
using NWN2QuickCast.UI.MVVM.VMs;
using Owlcat.Runtime.UI.MVVM;
using UnityEngine;
using UnityEngine.UI;
using NWN2QuickCast.Settings;
using UniRx;
using System;
using Kingmaker.PubSubSystem;
using NWN2QuickCast.UI.MVVM.Events;
using Kingmaker;

namespace NWN2QuickCast.UI.MVVM.Views
{
    class NWNQCWindowPCView : ViewBase<NWNQCWindowVM>, IInitializable
    {
        [SerializeField]
        private SpellPanelPCView _spellPanelPCView;

        [SerializeField]
        private MetaMagicPanelPCView _metaMagicPanelPCView;

        [SerializeField]
        private NWN2ConversionWindowPCView _conversionWindowPCView;

        [SerializeField]
        private SettingsPanelPCView _settingsPanelPCView;

        [SerializeField]
        private Button _settingsButton;

        private IDisposable _hideShowBinding;

        public override void BindViewImplementation()
        {
            _metaMagicPanelPCView.Bind(ViewModel.MetaMagicPanelVM);
            _conversionWindowPCView.Bind(ViewModel.NWN2ConversionWindowVM);
            _spellPanelPCView.Bind(ViewModel.SpellPanelVM);
            _settingsPanelPCView.Bind(ViewModel.SettingsPanelVM);
           
            LoadRectProperties();
            LoadKeyBindings();

            base.AddDisposable(_settingsButton.OnClickAsObservable().Subscribe(_ =>
            {
                _settingsPanelPCView.gameObject.SetActive(!_settingsPanelPCView.gameObject.activeSelf);
                _spellPanelPCView.SetVisible(!_settingsPanelPCView.gameObject.activeSelf);
                _metaMagicPanelPCView.SetVisible(!_settingsPanelPCView.gameObject.activeSelf);
            }));
            base.AddDisposable(_hideShowBinding);
            base.AddDisposable(EventBus.Subscribe(this));
        }

        private void LoadKeyBindings()
        {
            var setting = Main.Settings.GetSetting<HotKeySetting>(SettingKeys.HotKeyShowHide);
            _hideShowBinding = Game.Instance.Keyboard.Bind(SettingKeys.HotKeyShowHide, ToggleShowHide);
            setting.RegisterHotkey();
        }

        public override void DestroyViewImplementation()
        {
        }

        public void Initialize()
        {
            _spellPanelPCView.Initialize();
            _conversionWindowPCView.Initialize();
        }

        public void SaveRectProperties(Vector2 position, Vector2 size, Vector3 scale)
        {
            var setting = new WindowSetting(
                gameObject.activeSelf,
                position.x,
                position.y,
                size.x,
                size.y,
                scale.x,
                scale.y,
                scale.z);

            Main.Settings.SetSetting<WindowSetting>(SettingKeys.MainWindowSetting, setting);
        }

        public void LoadRectProperties()
        {
            var rect = (RectTransform)transform;
            var setting = Main.Settings.GetSetting<WindowSetting>(SettingKeys.MainWindowSetting);

            gameObject.SetActive(setting.WindowIsShown);

            rect.anchoredPosition = new Vector2(
                setting.WindowPosX,
                setting.WindowPosY);

            rect.sizeDelta = new Vector2(
                setting.WindowSizeX,
                setting.WindowSizeY);

            rect.localScale = new Vector3(
                setting.WindowScaleX,
                setting.WindowScaleY,
                setting.WindowScaleZ);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ToggleShowHide()
        {
            if (gameObject.activeSelf)
                Hide();
            else
                Show();
        }
    }
}
