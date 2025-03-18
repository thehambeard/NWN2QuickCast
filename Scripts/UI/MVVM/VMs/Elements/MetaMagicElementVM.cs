using Kingmaker.PubSubSystem;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UnitLogic;
using NWN2QuickCast.UI.MVVM.Events;
using NWN2QuickCast.UI.MVVM.VMs.Panels;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.Tooltips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.VMs.Elements
{
    public class MetaMagicElementVM : BaseDisposable, IViewModel
    {
        public readonly ReactiveProperty<Feature> MetaMagic = new ReactiveProperty<Feature>();
        public readonly IntReactiveProperty HeightenLevel = new IntReactiveProperty(-1);
        public readonly BoolReactiveProperty IsActive = new BoolReactiveProperty(false);
        public readonly ReactiveProperty<HeightenedSelectPanelVM> HeightenedSelectPanelVM = new ReactiveProperty<HeightenedSelectPanelVM>(null);
        public readonly ReactiveCommand<TooltipBaseTemplate> NewToolTipCommand = new ReactiveCommand<TooltipBaseTemplate>();
        public readonly ReactiveCommand DisposeToolTipCommand = new ReactiveCommand();
        public IReadOnlyReactiveProperty<bool> HasMeta => MetaMagic.Select(meta => meta != null).ToReactiveProperty();
        public TooltipBaseTemplate Tooltip;

        private IDisposable _heightenedHandle;

        public MetaMagicElementVM()
        {
        }

        public override void DisposeImplementation()
        {
            _heightenedHandle?.Dispose();
        }

        public void ToggleActive(Vector2 pos)
        {
            if (IsActive.Value)
                SetInactive();
            else if (HeightenedSelectPanelVM.Value == null)
                SetActive();
            else
                HeightenedSelectPanelVM.Value.ShowWindow(pos);
        }

        public void SetActive(int heightenLevel = -1)
        {
            if (HeightenedSelectPanelVM.Value != null && heightenLevel == -1)
                return;

            IsActive.Value = true;
            HeightenLevel.Value = heightenLevel;

            HeightenedSelectPanelVM.Value?.HideWindow();

            EventBus.RaiseEvent<IMetaMagicHandler>(h => h.OnMetaMagicAdd(MetaMagic.Value, HeightenLevel.Value));
        }

        public void SetInactive()
        {
            IsActive.Value = false;
            HeightenLevel.Value = -1;

            if (HeightenedSelectPanelVM.Value != null)
                HeightenedSelectPanelVM.Value.LevelSelected.Value = -1;

            EventBus.RaiseEvent<IMetaMagicHandler>(h => h.OnMetaMagicRemove(MetaMagic.Value));
        }

        public void SetMetaMagic(Feature meta, HeightenedSelectPanelVM heightenedSelectPanelVM = null)
        {
            MetaMagic.Value = meta;
            Tooltip = new TooltipTemplateFeature(meta);
            HeightenedSelectPanelVM.Value = heightenedSelectPanelVM;

            if (NewToolTipCommand.CanExecute.Value)
                NewToolTipCommand.Execute(Tooltip);

            if (heightenedSelectPanelVM != null)
                _heightenedHandle = HeightenedSelectPanelVM.Value.LevelSelected.Subscribe(x => SetActive(x));
        }

        public void ClearMetaMagic()
        {
            SetInactive();
            MetaMagic.Value = null;
            HeightenLevel.Value = -1;
            HeightenedSelectPanelVM.Value = null;
            _heightenedHandle?.Dispose();
            _heightenedHandle = null;

            if (DisposeToolTipCommand.CanExecute.Value)
                DisposeToolTipCommand.Execute();
        }
    }
}
