using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            DataContext = this;
            foreach(var child in MainGrid.Children)
            {
                if(child is BLockerHint hint)
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
                hint.GB.SetImage(new BitmapImage(new Uri("../images/dk64/gb.png", UriKind.Relative)));
            }
        }

        public void LoadSavedGBCounts(Dictionary<RegionName, string> GBCounts)
        {
            foreach(var entry in GBCounts)
            {
                RegionToBLockerHint[entry.Key].GBCount.Text = entry.Value;
            }
        }

        public Dictionary<RegionName,string> GetGBCounts()
        {
            var GBCounts = new Dictionary<RegionName,string>();
            foreach(var entry in RegionToBLockerHint)
            {
                GBCounts[entry.Key] = entry.Value.GBCount.Text;
            }
            return GBCounts;
        }
    }
}
