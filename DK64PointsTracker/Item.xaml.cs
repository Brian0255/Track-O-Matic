using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DK64PointsTracker
{
    /// <summary>
    /// Interaction logic for Draggable.xaml
    /// </summary>
    public partial class Item : ContentControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ItemImageProperty =
        DependencyProperty.Register("ItemImage", typeof(Image), typeof(Item));

        private Region region;

        public bool CanLeftClick { get; set; } = true;

        public Image ItemImage
        {
            get { return (Image)GetValue(ItemImageProperty); }
            set { SetValue(ItemImageProperty, value); }
        }
        public bool Checked { get; }
        private bool pressed = false;

        public Item()
        {
            InitializeComponent();
            Checked = false;
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetRegion(Region newRegion)
        {
            region = newRegion;
        }

        public void ClearRegion()
        {
            region = null;
        }

        //Adorner subclass specific to this control
        private class ItemAdorner : Adorner
        {
            Rect renderRect;
            ImageSource imageSource;
            public Point CenterOffset;
            public ItemAdorner(Item adornedElement, double opacity = 1.0) : base(adornedElement)
            {
                renderRect = new Rect(adornedElement.DesiredSize);
                Opacity = opacity;
                this.IsHitTestVisible = false;
                if (adornedElement.Content is StackPanel)
                {
                    var blah = (StackPanel)adornedElement.Content;
                    imageSource = (blah.Children[0] as Image).Source;
                    var brush = new ImageBrush();
                    brush.ImageSource = imageSource;
                    brush.Stretch = Stretch.UniformToFill;
                }
                else
                {
                    imageSource = ((adornedElement).ItemImage as Image).Source;
                }
                CenterOffset = new Point(-renderRect.Width / 2, -renderRect.Height / 2);
            }
            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawImage(imageSource, renderRect);
            }
        }

        //Struct to use in the GetCursorPos function
        private struct PInPoint
        {
            public int X;
            public int Y;
            public PInPoint(int x, int y)
            {
                X = x; Y = y;
            }
            public PInPoint(double x, double y)
            {
                X = (int)x; Y = (int)y;
            }
            public Point GetPoint(double xOffset = 0, double yOffet = 0)
            {
                return new Point(X + xOffset, Y + yOffet);
            }
            public Point GetPoint(Point offset)
            {
                return new Point(X + offset.X, Y + offset.Y);
            }
        }

        [DllImport("user32.dll")]
        static extern void GetCursorPos(ref PInPoint p);

        private ItemAdorner myAdornment;
        private PInPoint pointRef = new PInPoint();

        public void Item_MouseMove(object sender, MouseEventArgs e)
        {
            if (pressed)
            {
                var opacity = (e.RightButton == MouseButtonState.Pressed) ? 0.375 : 1.0;
                Opacity = 1.0;
                var adLayer = AdornerLayer.GetAdornerLayer(this);
                myAdornment = new ItemAdorner(this, opacity);
                adLayer.Add(myAdornment);
                var parent = Parent;
                DragDrop.DoDragDrop(this, this, DragDropEffects.Copy);
                pressed = false;
                ItemImage.Opacity = (Parent == parent) ? 1.0 : opacity;
                adLayer.Remove(myAdornment);
            }
        }

        public void Item_MouseDown(object sender, MouseEventArgs e)
        {
            pressed = true;
        }

        public void Item_Return(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                X.Visibility = Visibility.Hidden;
                Checkmark.Visibility = (Checkmark.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
                if (region != null) region.UpdateRequiredChecksTotal();
            }
            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                Checkmark.Visibility = Visibility.Hidden;
                X.Visibility = (X.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
            }
            if (e.LeftButton == MouseButtonState.Pressed && CanLeftClick)
            {
                HandleItemReturn();
            }
        }

        public void HandleItemReturn()
        {
            Checkmark.Visibility = Visibility.Hidden;
            ImageItem.Opacity = 1.0;
            var itemGrid = MainWindow.Items;
            RegionGrid parent = this.Parent as RegionGrid;

            ((RegionGrid)Parent).Handle_RegionGrid(this, false);
            itemGrid.Children.Add(this);

            //((MainWindow)Application.Current.MainWindow).DecrementCollected(MainWindow.itemValues[this.Tag.ToString()]);

            MouseDown -= Item_Return;

            MouseMove -= Item_MouseMove;
            MouseMove += Item_MouseMove;
        }

        private void Item_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            GetCursorPos(ref pointRef);
            Point relPos = this.PointFromScreen(pointRef.GetPoint(myAdornment.CenterOffset));
            myAdornment.Arrange(new Rect(relPos, myAdornment.DesiredSize));

            Mouse.SetCursor(Cursors.None);
            e.Handled = true;
        }
    }
}