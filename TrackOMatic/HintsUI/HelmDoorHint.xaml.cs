using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TrackOMatic
{
    public partial class HelmDoorHint : UserControl
    {
        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register("LabelText", typeof(string), typeof(HelmDoorHint));

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }
        public HelmDoorHint()
        {
            InitializeComponent();
            List<List<BitmapImage>> BLockerSources = new()
            {
                new()
                {
                    (BitmapImage)FindResource("golden_banana"),
                    (BitmapImage)FindResource("blueprint"),
                    (BitmapImage)FindResource("pearl"),
                    (BitmapImage)FindResource("crown"),
                    (BitmapImage)FindResource("medal"),
                    (BitmapImage)FindResource("rainbow_coin"),
                    (BitmapImage)FindResource("fairy"),
                },
                new()
                {
                    (BitmapImage)FindResource("company_coin"),
                    (BitmapImage)FindResource("bean"),
                }
            };
            DoorItem.ImageSources = BLockerSources;
        }

    }
}
