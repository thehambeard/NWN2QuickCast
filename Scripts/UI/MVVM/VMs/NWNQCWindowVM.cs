using NWN2QuickCast.UI.MVVM.VMs.Panels;
using Owlcat.Runtime.UI.MVVM;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace NWN2QuickCast.UI.MVVM.VMs
{
    public class NWNQCWindowVM : BaseDisposable, IViewModel
    {
        public MetaMagicPanelVM MetaMagicPanelVM;
        public NWN2ConversionWindowVM NWN2ConversionWindowVM;
        public SpellPanelVM SpellPanelVM;
        public SettingsPanelVM SettingsPanelVM;

        public NWNQCWindowVM()
        {
            base.AddDisposable(MetaMagicPanelVM = new MetaMagicPanelVM());
            base.AddDisposable(NWN2ConversionWindowVM = new NWN2ConversionWindowVM());
            base.AddDisposable(SpellPanelVM = new SpellPanelVM(MetaMagicPanelVM, NWN2ConversionWindowVM));
            base.AddDisposable(SettingsPanelVM = new SettingsPanelVM());
        }

        public override void DisposeImplementation()
        {
        }
    }
}
