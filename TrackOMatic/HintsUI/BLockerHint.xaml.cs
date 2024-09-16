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
    public partial class BLockerHint  : UserControl
    {
        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.Register("RegionName", typeof(RegionName), typeof(BLockerHint));
        public RegionName RegionName
        {
            get { return (RegionName)GetValue(RegionNameProperty); }
            set { SetValue(RegionNameProperty, value); }
        }  

        public BLockerHint()
        {
            InitializeComponent();
            DataContext = this;
            List<List<BitmapImage>> BLockerSources = new()
            {
                new()
                {
                    new BitmapImage( new Uri("images/dk64/gb.png", UriKind.Relative)),
                    new BitmapImage( new Uri("images/dk64/bp.png", UriKind.Relative)),
                    new BitmapImage( new Uri("images/dk64/pearl.png", UriKind.Relative)),
                    new BitmapImage( new Uri("images/dk64/crown.png", UriKind.Relative)),
                    new BitmapImage( new Uri("images/dk64/bananamedal.png", UriKind.Relative)),
                    new BitmapImage( new Uri("images/dk64/rainbowcoin.png", UriKind.Relative)),
                    new BitmapImage( new Uri("images/dk64/fairy.png", UriKind.Relative)),
                },
                new()
                {
                    new BitmapImage( new Uri("images/dk64/ninrarecoin.png", UriKind.Relative)),
                    new BitmapImage( new Uri("images/dk64/bean.png", UriKind.Relative)),
                }
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
