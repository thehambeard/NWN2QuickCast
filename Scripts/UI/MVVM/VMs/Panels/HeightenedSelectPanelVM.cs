using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.VMs.Panels
{
    public class HeightenedSelectPanelVM : BaseDisposable, IViewModel
    {
        public readonly IntReactiveProperty LevelSelected = new IntReactiveProperty(-1);
        public readonly ReactiveCommand<Vector2> ShowWindowCommand = new ReactiveCommand<Vector2>();
        public readonly ReactiveCommand HideWindowCommand = new ReactiveCommand();

        public override void DisposeImplementation()
        {
        }

        public void ShowWindow(Vector2 pos)
        {
            if (ShowWindowCommand.CanExecute.Value)
                ShowWindowCommand.Execute(pos);
        }

        public void HideWindow() 
        {
            if (HideWindowCommand.CanExecute.Value)
                HideWindowCommand.Execute();
        }
    }
}
