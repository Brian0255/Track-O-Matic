using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DK64PointsTracker
{
    /// <summary>
    /// Interaction logic for RegionGrid.xaml
    /// </summary>
    public partial class RegionGrid : UniformGrid
    {
        public Region Region;
        public Dictionary<VialColor, List<Item>> Vials;
        public List<Item> VialItems = new();

        public void InitializeVials()
        {
            Vials = new()
            {
                { VialColor.CLEAR, new List<Item>() },

                { VialColor.YELLOW, new List<Item>() },
                { VialColor.RED, new List<Item>() },
                { VialColor.BLUE, new List<Item>() },
                { VialColor.PURPLE, new List<Item>() },
                { VialColor.GREEN, new List<Item>() },

                { VialColor.KEY, new List<Item>() },
                { VialColor.KONG, new List<Item>() }
            };
        }

        //public List<Image> Vials;
        public RegionGrid()
        {
            InitializeComponent();
            InitializeVials();
        }

        private void AdjustSpacing()
        {
            int gridremainder = 0;
            if (Children.Count % 5 != 0)
                gridremainder = 1;

            int gridnum = Math.Max((Children.Count / 5) + gridremainder, 1);

            Rows = gridnum;

            double height = 1 + ((Children.Count - 1) / 5) / 2;
            var outerGrid = ((Parent as Grid).Parent as Grid);
            int row = (int)Parent.GetValue(Grid.RowProperty);
            outerGrid.RowDefinitions[row].Height = new GridLength(height, GridUnitType.Star);
        }

        public void AddInitialVial(VialColor color)
        {
            var imageName = (color.ToString() + "_vial.png").ToLower();
            Item vialImage = new()
            {
              ItemImage = new()
              {
                  Source = new BitmapImage(new Uri("Images/dk64/" + imageName, UriKind.Relative))
              }
            };
            vialImage.SetRegion(Region);
            vialImage.Interactable = false;
            vialImage.MouseDown -= vialImage.Item_MouseDown;
            vialImage.MouseDown += vialImage.Item_Return;
            Vials[color].Add(vialImage);
            VialItems.Add(vialImage);
            Children.Add(vialImage);
            AdjustSpacing();
        }

        public void ResetVials()
        {
            foreach(var vialItem in VialItems)
            {
                if (vialItem.Parent != null) Children.Remove(vialItem);
            }
            VialItems = new();
            Vials = new();
            InitializeVials();
        }

        private bool ValidInsertionPoint(int index, Item vial, bool userPlacing)
        {
            if (vial.Parent != null) return true;
            if(index < 0 || index > Children.Count - 1) return false;
            if (Children[index] is Item item)
            {
                //niche case where 
                //1. autotracker wants to place a clear vial move (e.g. a slam)
                //2. user already placed another clear vial move there (e.g. diving)
                //3. as a result, we need to remove the user placed move first
                if (!userPlacing && item.CanLeftClick && item.Tag != null)
                {
                    item.HandleItemReturn();
                    return true;
                }
            }
            return false;
        }

        private void AddWithVialCheck(Item button, bool userPlacing)
        {
            var item = (ItemName)button.Tag;
            if (VialItems.Count() == 0)
            {
                Children.Add(button);
                return;
            }
            var itemInfo = ImportantCheckList.ITEMS[item];
            if (itemInfo.VialColor != VialColor.NONE && Vials.ContainsKey(itemInfo.VialColor))
            {
                var vials = Vials[itemInfo.VialColor];
                foreach(var vial in vials)
                {
                    int index = VialItems.IndexOf(vial);
                    if (!ValidInsertionPoint(index, vial, userPlacing)) continue;
                    button.Checkmark.Visibility = vial.Checkmark.Visibility;
                    button.X.Visibility = vial.X.Visibility;
                    Children.RemoveAt(index);
                    Children.Insert(index, button);
                    return;
                }
            }
        }

        private void RemoveWithVialCheck(Item button)
        {
            if (button.Tag == null) return;
            if(VialItems.Count() == 0)
            {
                Children.Remove(button);
                return;
            }
            var item = (ItemName)button.Tag;
            var itemInfo = ImportantCheckList.ITEMS[item];
            if (itemInfo.VialColor != VialColor.NONE && Vials.ContainsKey(itemInfo.VialColor))
            {
                int index = Children.IndexOf(button);
                var vial = VialItems[index];
                Children.RemoveAt(index);
                Children.Insert(index, vial);
                return;
            }
        }

        public void Handle_RegionGrid(Item button, bool add, bool userPlacing = true)
        {
            MainWindow window = ((MainWindow)Application.Current.MainWindow);
            ImportantCheck check = null;
            if(button.Tag != null)
            {
                var item = (ItemName)button.Tag;
                check = ImportantCheckList.ITEMS[item];
            }
            if (add)
            {
                try
                {
                    button.SetRegion(Region);
                    AddWithVialCheck(button, userPlacing);
                }
                catch (Exception)
                {
                    return;
                }
                Region.AddCheck(check);
            }
            else
            {
                RemoveWithVialCheck(button);
                button.ClearRegion();
                Region.RemoveCheck(check);
            }
            AdjustSpacing();
            Region.UpdateRequiredChecksTotal();
        }

        private void Item_Drop(Object sender, DragEventArgs e)
        {
            MainWindow window = ((MainWindow)Application.Current.MainWindow);
            if (e.Data.GetDataPresent(typeof(Item)))
            {

                Item item = e.Data.GetData(typeof(Item)) as Item;
                if(item.Parent is Grid) Add_Item(item);
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (System.IO.Path.GetExtension(files[0]).ToUpper() == ".JSON")
                    window.ParseSpoiler(files[0]);
            }
        }

        public void Add_Item(Item item, bool userPlacing = true)
        {
            // move item to region
            Panel itemGrid = item.Parent as Panel;
            if(itemGrid != null) itemGrid.Children.Remove(item);
            Handle_RegionGrid(item, true, userPlacing);
            if (item.Parent == null) item.HandleItemReturn();
            else
            {
                item.MouseMove -= item.Item_MouseMove;
                item.MouseDown -= item.Item_Return;
                item.MouseDown += item.Item_Return;
            }
        }
    }
}
