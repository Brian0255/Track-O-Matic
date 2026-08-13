using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic
{
    /// <summary>
    /// Interaction logic for RegionGrid.xaml
    /// </summary>
    public partial class RegionGrid : UniformGrid
    {
        public Region? Region;
        public List<Item> VialItems = new();

        /// <summary>
        /// Gets the list of vials for each color, plus specialty ones like Keys and Kongs for some spoiler modes.
        /// </summary>
        public Dictionary<VialColor, List<Item>> Vials { get; private set; } = new();

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
            {
                gridremainder = 1;
            }

            int gridnum = Math.Max((Children.Count / 5) + gridremainder, 1);

            Rows = gridnum;

            double height = (1 + ((Children.Count - 1) / 5)) / 2.0;
            //if (Region.RegionName == RegionName.START) height = 1;
            if (Children.Count <= 5)
            {
                height = 1;
            }

            height = 1;

            if (Parent is Grid parentGrid && parentGrid.Parent is Grid outerOuterGrid)
            {
                int row = (int)Parent.GetValue(Grid.RowProperty);
                outerOuterGrid.RowDefinitions[row].Height = new GridLength(height, GridUnitType.Star);
            }
        }

        public void AddInitialVial(VialColor color)
        {
            var resourceName = "vial_" + color.ToString().ToLowerInvariant();
            Item vialImage = new()
            {
                ItemImage = (BitmapImage)FindResource(resourceName)
            };
            vialImage.SetRegion(Region);
            vialImage.Disable();
            vialImage.MouseDown -= vialImage.Item_MouseDown;
            vialImage.MouseDown += vialImage.Item_Return;
            Vials[color].Add(vialImage);
            VialItems.Add(vialImage);
            Children.Add(vialImage);
            if (Region?.RegionName == RegionName.UNHINTABLE_MOVES)
            {
                return;
            }

            AdjustSpacing();
        }

        public void ResetVials()
        {
            foreach (var vialItem in VialItems)
            {
                if (vialItem.Parent != null)
                {
                    Children.Remove(vialItem);
                }
            }
            VialItems = new();
            Vials = new();
            InitializeVials();
        }

        private bool ValidInsertionPoint(int index, Item vial, bool userPlacing)
        {
            if (vial.Parent != null)
            {
                return true;
            }

            if (index < 0 || index > Children.Count - 1)
            {
                return false;
            }

            if (Children[index] is Item item)
            {
                //niche case where 
                //1. autotracker wants to place a clear vial move (e.g. a slam)
                //2. user already placed another clear vial move there (e.g. diving)
                //3. as a result, we need to remove the user placed move first
                if (!userPlacing && !item.AutoPlaced && item.Tag != null)
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
                foreach (var vial in vials)
                {
                    int index = VialItems.IndexOf(vial);
                    if (!ValidInsertionPoint(index, vial, userPlacing))
                    {
                        continue;
                    }

                    if (button.Star.Visibility == Visibility.Visible || vial.Star.Visibility == Visibility.Visible)
                    {
                        button.SetStarVisibility(Visibility.Visible);
                        vial.Star.Visibility = Visibility.Visible;
                    }
                    Children.RemoveAt(index);
                    Children.Insert(index, button);
                    return;
                }
            }
        }

        private void RemoveWithVialCheck(Item button)
        {
            if (button.Tag == null)
            {
                return;
            }

            if (VialItems.Count() == 0)
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
                vial.Star.Visibility = Visibility.Hidden;
                Children.RemoveAt(index);
                Children.Insert(index, vial);
                return;
            }
        }

        public void Handle_RegionGrid(Item button, bool add, bool userPlacing = true, bool brighten = true)
        {
        }
    }
}
