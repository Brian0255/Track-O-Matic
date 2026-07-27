using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TrackOMatic.Data;

namespace TrackOMatic
{
    public partial class BLockerHint : UserControl
    {
        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.Register("RegionName", typeof(RegionName), typeof(BLockerHint));
        public RegionName RegionName
        {
            get { return (RegionName)GetValue(RegionNameProperty); }
            set { SetValue(RegionNameProperty, value); }
        }

        public BitmapImage GetBarrierItemImage(BarrierItems item) =>
            item switch
            {
                BarrierItems.GOLDEN_BANANA => (BitmapImage)FindResource("golden_banana"),
                BarrierItems.BLUEPRINT => (BitmapImage)FindResource("blueprint"),
                BarrierItems.PEARL => (BitmapImage)FindResource("pearl"),
                BarrierItems.CROWN => (BitmapImage)FindResource("crown"),
                BarrierItems.MEDAL => (BitmapImage)FindResource("medal"),
                BarrierItems.RAINBOW_COIN => (BitmapImage)FindResource("rainbow_coin"),
                BarrierItems.FAIRY => (BitmapImage)FindResource("fairy"),
                BarrierItems.COMPANY_COIN => (BitmapImage)FindResource("company_coin"),
                BarrierItems.BEAN => (BitmapImage)FindResource("bean"),
                _ => throw new ArgumentException($"Unknown barrier item: {item}")
            };

        public BLockerHint()
        {
            InitializeComponent();
            List<List<BitmapImage>> BLockerSources = new()
                {
                    new() { GetBarrierItemImage(BarrierItems.GOLDEN_BANANA),
                            GetBarrierItemImage(BarrierItems.BLUEPRINT),
                            GetBarrierItemImage(BarrierItems.PEARL),
                            GetBarrierItemImage(BarrierItems.CROWN),
                            GetBarrierItemImage(BarrierItems.MEDAL),
                            GetBarrierItemImage(BarrierItems.RAINBOW_COIN),
                            GetBarrierItemImage(BarrierItems.FAIRY) },
                    new() { GetBarrierItemImage(BarrierItems.COMPANY_COIN),
                            GetBarrierItemImage(BarrierItems.BEAN) }
                };
            GB.ImageSources = BLockerSources;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Picture.Source = (ImageSource)FindResource(RegionName.ToString().ToLowerInvariant());
        }

    }
}
