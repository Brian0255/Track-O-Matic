using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Windows.Media.Imaging;

namespace TrackOMatic
{
    public partial class Item : ContentControl, INotifyPropertyChanged
    {
        public Region Region { get; set; }
        public bool Interactible { get; set; } = true;

        public bool CanLeftClick { get; set; } = true;
        public bool AutoPlaced { get; set; } = false;
        private ItemName ItemName { get; set; }
        private ItemBrightnessChanger ItemBrightnessChanger { get; set; }

        public static readonly DependencyProperty ItemImageProperty =
        DependencyProperty.Register("ItemImage", typeof(Image), typeof(Item));

        public static readonly DependencyProperty HoverTextProperty =
     DependencyProperty.Register(
         "HoverText",
         typeof(string),
         typeof(Item));

        public string HoverText
        {
            get { return (string)GetValue(HoverTextProperty); }
            set { SetValue(HoverTextProperty, value); }
        }

        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public Image ItemImage
        {
            get { return (Image)GetValue(ItemImageProperty); }
            set { 
                SetValue(ItemImageProperty, value);
                if (mainWindow.ITEM_TO_BACKGROUND_IMAGE.ContainsKey(this))
                {
                    mainWindow.ITEM_TO_BACKGROUND_IMAGE[this].BackgroundItemImage.Source = ItemImage.Source;
                }
            }
        }

        public void InitHoverPoints()
        {
            var matchingBackground = mainWindow.ITEM_TO_BACKGROUND_IMAGE[this];
            if (!mainWindow.SpoilerSettings.PointsEnabled)
            {
                ToolTip.Visibility = Visibility.Collapsed;
                matchingBackground.ToolTip.Visibility = Visibility.Collapsed;
                return;
            }
            var itemName = (ItemName)Tag;
            var matchingCheck = ImportantCheckList.ITEMS[itemName];
            HoverText = matchingCheck.PointValue.ToString() + " points";
            ToolTip.Visibility = Visibility.Visible;
            matchingBackground.ToolTip.Visibility = Visibility.Visible;
            matchingBackground.ToolTip.Content = HoverText;
        }

        private bool pressed = false;

        public Item()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void Item_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Tag == null) return;
            ItemName = (ItemName)Tag;
            ItemBrightnessChanger = new ItemBrightnessChanger(ItemImage, ItemName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetStarVisibility(Visibility newVisibility)
        {
            Star.Visibility = newVisibility;
            MainWindow MainWindow = (MainWindow)Application.Current.MainWindow;
            if (MainWindow.ITEM_TO_BACKGROUND_IMAGE.ContainsKey(this))
                MainWindow.ITEM_TO_BACKGROUND_IMAGE[this].SetStarVisibility(newVisibility);
        }

        public void SetRegion(Region newRegion)
        {
            Region = newRegion;
        }

        public void ClearRegion()
        {
            Region = null;
        }

        public void SetBackgroundImageVisibility(Visibility newVisibility)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ITEM_TO_BACKGROUND_IMAGE[this].Visibility = newVisibility;
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
                adornedElement.ChangeOpacity(opacity);
                Opacity = opacity;
                IsHitTestVisible = false;
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
                    imageSource = ((adornedElement).ItemImage).Source;
                }
                CenterOffset = new Point(-renderRect.Width / 2, -renderRect.Height / 2);
            }
            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawImage(imageSource, renderRect);
            }
        }

        private void PerformSave(bool ignoreAutotrackField)
        {
            if (Tag == null) return;
            var itemName = (ItemName)Tag;
            var regionName = (Region == null) ? RegionName.UNKNOWN : Region.RegionName;
            mainWindow.DataSaver.AddSavedItem(new SavedItem(itemName, regionName, Star.Visibility, false, ItemImage.Opacity), ignoreAutotrackField);
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

        private void ResetImage()
        {
            ItemName itemName = (ItemName)Tag;
            var resourceName = itemName.ToString().ToLower();
            ItemImage = (Image)FindResource(resourceName);
        }

        public void ChangeOpacity(double newOpacity)
        {
            ItemImage.Opacity = newOpacity;
            mainWindow.ITEM_TO_BACKGROUND_IMAGE[this].ChangeOpacity(newOpacity);
        }

        public void Brighten()
        {
            ItemBrightnessChanger.Brighten();
            ItemImage = ItemBrightnessChanger.ItemImage;
        }

        public void Darken()
        {
            ItemBrightnessChanger.Darken();
            ItemImage = ItemBrightnessChanger.ItemImage;
        }

        public void Item_MouseMove(object sender, MouseEventArgs e)
        {
            if (pressed && Interactible)
            {
                ItemName itemName = (ItemName)Tag;
                var resourceName = itemName.ToString().ToLower();
                ItemImage = (Image)FindResource(resourceName);
                var opacity = (e.RightButton == MouseButtonState.Pressed) ? 0.375 : 1.0;
                Opacity = 1.0;
                var adLayer = AdornerLayer.GetAdornerLayer(this);
                myAdornment = new ItemAdorner(this, opacity);
                adLayer.Add(myAdornment);
                var parent = Parent;

                DragDrop.DoDragDrop(this, this, DragDropEffects.Copy);

                pressed = false;

                if (Parent == parent) Darken();
                else Brighten();

                if (Parent == parent) ChangeOpacity(1.0);
                else ChangeOpacity(opacity);

                PerformSave(false);
                adLayer.Remove(myAdornment);
            }
        }

        public void ToggleStar()
        {
            var newVisibility = (Star.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
            SetStarVisibility(newVisibility);
            if (mainWindow.ITEM_TO_BACKGROUND_IMAGE.ContainsKey(this))
            {
                mainWindow.ITEM_TO_BACKGROUND_IMAGE[this].SetStarVisibility(newVisibility);
            }
            if (Region != null) Region.UpdateRequiredChecksTotal();
            PerformSave(true);
        }

        public void Item_MouseDown(object sender, MouseEventArgs e)
        {
            CheckMiddleClick(sender, e);
            pressed = (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed && Interactible);
        }

        private void CheckMiddleClick(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ToggleStar();
            }
        }

        public void Item_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0) ToggleStar();
        }

        public void Item_Return(object sender, MouseEventArgs e)
        {
            CheckMiddleClick(sender, e);
            if (e.LeftButton == MouseButtonState.Pressed && CanLeftClick)
            {
                HandleItemReturn();
                PerformSave(false);
            }
        }

        public void HandleItemReturn()
        {
            if (!Interactible) return;
            Image.Opacity = 1.0;
            var itemGrid = MainWindow.Items;
            if (Parent != null)
            {
                RegionGrid parent = Parent as RegionGrid;
                ((RegionGrid)Parent).Handle_RegionGrid(this, false);
            }
            itemGrid.Children.Add(this);
            var itemName = (ItemName)Tag;
            var BWResourceName = itemName.ToString().ToLower() + "_bw";
            ItemImage = (Image)FindResource(BWResourceName);
            SetBackgroundImageVisibility(Visibility.Hidden);
            MouseDown -= Item_Return;
            MouseDown += Item_MouseDown;
            MouseMove -= Item_MouseMove;
            MouseMove += Item_MouseMove;
        }

        private void Item_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (!Interactible) return;
            GetCursorPos(ref pointRef);
            Point relPos = PointFromScreen(pointRef.GetPoint(myAdornment.CenterOffset));
            myAdornment.Arrange(new Rect(relPos, myAdornment.DesiredSize));
            Mouse.SetCursor(Cursors.None);
            e.Handled = true;
        }
    }
}