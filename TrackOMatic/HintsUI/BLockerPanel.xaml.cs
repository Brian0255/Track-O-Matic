using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TrackOMatic
{
    public partial class BLockerPanel : UserControl
    {
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(BLockerPanel));
        public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register("LineColor", typeof(Color), typeof(BLockerPanel));
        public static readonly DependencyProperty HintTypeProperty = DependencyProperty.Register("HintType", typeof(HintType), typeof(BLockerPanel));
        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public HintType HintType
        {
            get { return (HintType)GetValue(HintTypeProperty); }
            set { SetValue(HintTypeProperty, value); }
        }

        public Dictionary<RegionName, BLockerHint> RegionToBLockerHint = new();
        public BLockerPanel()
        {
            InitializeComponent();
            foreach (var child in MainGrid.Children)
            {
                if (child is BLockerHint hint)
                {
                    RegionToBLockerHint[hint.RegionName] = hint;
                }
            }
        }

        public void Reset()
        {
            foreach (var hint in RegionToBLockerHint.Values)
            {
                hint.GBCount.Text = "?";
                hint.GB.SetIndex(0);
            }
        }

        public void LoadSavedGBCounts(Dictionary<RegionName, string> GBCounts)
        {
            foreach (var entry in GBCounts)
            {
                RegionToBLockerHint[entry.Key].GBCount.Text = entry.Value;
            }
        }

        public Dictionary<RegionName, string> GetGBCounts()
        {
            var GBCounts = new Dictionary<RegionName, string>();
            foreach (var entry in RegionToBLockerHint)
            {
                GBCounts[entry.Key] = entry.Value.GBCount.Text;
            }
            return GBCounts;
        }

        public void LoadSavedImageIndexes(Dictionary<RegionName, int> imageIndexes)
        {
            foreach (var entry in imageIndexes)
            {
                RegionToBLockerHint[entry.Key].GB.SetIndex(entry.Value);
            }
        }

        public Dictionary<RegionName, int> GetImageIndexes()
        {
            var imageIndexes = new Dictionary<RegionName, int>();
            foreach (var entry in RegionToBLockerHint)
            {
                imageIndexes[entry.Key] = entry.Value.GB.GetIndex();
            }
            return imageIndexes;
        }

        public void LoadBLockerInfo(List<BLockerInfo> blockerInfo)
        {
            if (blockerInfo == null)
            {
                return;
            }

            for (int i = 0; i < blockerInfo.Count; i++)
            {
                var blocker = blockerInfo[i];
                var regionName = Region.LOBBY_ORDER[i];
                if (!RegionToBLockerHint.TryGetValue(regionName, out var hint))
                {
                    continue;
                }

                if (!JSONKeyMappings.SPOILER_BARRIER_TO_BARRIER_ITEM.ContainsKey(blocker.item))
                {
                    continue;
                }

                var adjusted_index = (int)JSONKeyMappings.SPOILER_BARRIER_TO_BARRIER_ITEM[blocker.item];
                hint.GB.SetIndex(adjusted_index);
                hint.GBCount.Text = blocker.cost.ToString();
            }
        }
    }
}
