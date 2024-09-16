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
    public partial class HelmDoorPanel : UserControl
    {
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(HelmDoorPanel));
        public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register("LineColor", typeof(Color), typeof(HelmDoorPanel));
        public static readonly DependencyProperty HintTypeProperty = DependencyProperty.Register("HintType", typeof(HintType), typeof(HelmDoorPanel));
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

        public List<int> ItemImageIndexes { get; private set; }
        private List<HelmDoorHint> hints;
        public HelmDoorPanel()
        {
            InitializeComponent();
            DataContext = this;
            hints = new();
            foreach(var child in MainGrid.Children)
            {
                if(child is HelmDoorHint hint)
                {
                    hints.Add(hint);
                }
            }
        }

        public void Reset()
        {
            foreach (var element in MainGrid.Children)
            {
                if(element is HelmDoorHint hint)
                {
                    hint.ItemCount.Text = "?";
                    hint.DoorItem.SetImage(new BitmapImage(new Uri("../images/dk64/gb.png", UriKind.Relative)));
                }
            }
        }

        public void LoadSavedHelmDoorCounts(List<string> ItemCounts)
        {
            for(int i = 0; i < ItemCounts.Count; ++i)
            {
                hints[i].ItemCount.Text = ItemCounts[i];
            }
        }

        public List<string> GetItemCounts()
        {
            var list = new List<string>();
            foreach(var entry in hints)
            {
                list.Add(entry.ItemCount.Text);
            }
            return list;
        }

        public void LoadSavedImageIndexes(List<int> imageIndexes)
        {
            for (int i = 0; i < imageIndexes.Count; ++i)
            {
                hints[i].DoorItem.SetIndex(imageIndexes[i]);
            }
        }

        public List<int> GetImageIndexes()
        {
            var list = new List<int>();
            foreach (var hint in hints)
            {
                list.Add(hint.DoorItem.GetIndex());
            }
            return list;
        }
    }
}
