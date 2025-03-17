using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using NWN2QuickCast.MVVM.VMs.Elements;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NWN2QuickCast.UI.MVVM.VMs
{
    class NWN2ConversionWindowVM : BaseDisposable, IViewModel
    {
        public readonly ReactiveCollection<SpellConversionElementVM> Elements = new ReactiveCollection<SpellConversionElementVM>();
        public readonly ReactiveCommand ShowWindowCommand = new ReactiveCommand();
        public readonly ReactiveCommand HideWindowCommand = new ReactiveCommand();
        public RectTransform ButtonRect;

        public void ShowWindow(RectTransform buttonRect, SlotConversion slotConversion, UnitEntityData Unit)
        {
            Elements.Clear();

            ButtonRect = buttonRect;

            foreach (var conversion in slotConversion.GetMechanicSlots(Unit))
                Elements.Add(new SpellConversionElementVM(conversion));

            if (ShowWindowCommand.CanExecute.Value) 
                ShowWindowCommand.Execute();
        }
        
        public void HideWindow()
        {
            if (HideWindowCommand.CanExecute.Value)
                HideWindowCommand.Execute();
        }



        public override void DisposeImplementation()
        {
        }
    }
}
