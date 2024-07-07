using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrackOMatic
{
    public partial class BasicItemSelector : Window
    {
        public ImageSource SelectedImageSource { get; private set; }
        public BasicItemSelector(List<List<BitmapImage>> toAdd)
        {
            InitializeComponent();
            DataContext = this;
            for(int i = 0; i < toAdd.Count; ++i)
            {
                var row = toAdd[i];
                for(int j = 0; j < row.Count; ++j)
                {
                    var border = new Border
                    {
                        BorderThickness = new Thickness(0),
                        UseLayoutRounding = true,
                        SnapsToDevicePixels = true,
                    };
                    var shadow = new DropShadowEffect
                    {
                        Color = Colors.Black,
                        Direction = 0,
                        ShadowDepth = 0,
                        BlurRadius = 14,
                        Opacity = 0.5
                    };
                    border.Effect = shadow;
                    RenderOptions.SetBitmapScalingMode(border, BitmapScalingMode.Fant);
                    RenderOptions.SetClearTypeHint(border, ClearTypeHint.Enabled);
                    
                    var imageSource = toAdd[i][j];
                    Image image = new()
                    {
                        Source = imageSource,
                        Margin = new Thickness(1)
                    };
                    image.MouseDown += ImagePressed;
                    border.Child = image;
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    ItemGrid.Children.Add(border);
                }
            }
            Height = (toAdd.Count < 2) ? 80 : 120;
            Row2.Height = (toAdd.Count < 2) ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        }

        private void ImagePressed(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var image = (Image)sender;
                SelectedImageSource = image.Source;
                Close();
            }
        }
    }
}
