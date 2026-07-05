using System.Windows;
using System.Windows.Controls;
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

        public static BitmapImage? GetBarrierItemImage(BarrierItems item) =>
            item switch
            {
                BarrierItems.GOLDEN_BANANA => MakeImage("gb.png"),
                BarrierItems.BLUEPRINT => MakeImage("bp.png"),
                BarrierItems.PEARL => MakeImage("pearl.png"),
                BarrierItems.CROWN => MakeImage("crown.png"),
                BarrierItems.MEDAL => MakeImage("bananamedal.png"),
                BarrierItems.RAINBOW_COIN => MakeImage("rainbowcoin.png"),
                BarrierItems.FAIRY => MakeImage("fairy.png"),
                BarrierItems.COMPANY_COIN => MakeImage("ninrarecoin.png"),
                BarrierItems.BEAN => MakeImage("bean.png"),
                _ => null
            };

        private static BitmapImage MakeImage(string filename) =>
            new(new Uri("Images/dk64/" + filename, UriKind.Relative));

        public BLockerHint()
        {
            InitializeComponent();
            DataContext = this;
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
            Picture.Source = new BitmapImage(new Uri("../Images/dk64/" + RegionName.ToString().ToLower() + ".png", UriKind.Relative));
        }

    }
}
