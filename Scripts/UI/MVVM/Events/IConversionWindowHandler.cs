using Kingmaker.EntitySystem.Entities;
using Kingmaker;
using Kingmaker.PubSubSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NWN2QuickCast.UI.MVVM.Events
{
    public interface IConversionWindowHandler : IGlobalSubscriber
    {
        public void OpenConversionWindow(RectTransform buttonRect, SlotConversion slotConversion, UnitEntityData unit);
        public void CloseConversionWindow();
    }
}
