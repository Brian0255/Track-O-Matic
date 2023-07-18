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
        public RegionGrid()
        {
            InitializeComponent();
        }

        public void Handle_RegionGrid(Item button, bool add)
        {
            MainWindow window = ((MainWindow)Application.Current.MainWindow);
            ItemName item = (ItemName)button.Tag;
            var check = ImportantCheckList.ITEMS[item];
            if (add)
            {
                try
                {
                    Children.Add(button);
                    button.SetRegion(Region);
                }
                catch (Exception)
                {
                    return;
                }
                Region.AddCheck(check);
            }
            else
            {
                button.ClearRegion();
                Region.RemoveCheck(check);
                Children.Remove(button);
            }

            int gridremainder = 0;
            if (Children.Count % 5 != 0)
                gridremainder = 1;

            int gridnum = Math.Max((Children.Count / 5) + gridremainder, 1);

            Rows = gridnum;

            // default 1, add .5 for every row
            double length = 1 + ((Children.Count - 1) / 5) / 2;
            var outerGrid = ((Parent as Grid).Parent as Grid);
            int row = (int)Parent.GetValue(Grid.RowProperty);
            outerGrid.RowDefinitions[row].Height = new GridLength(length, GridUnitType.Star);
            Region.UpdateRequiredChecksTotal();
            /*
            if (MainWindow.data.mode == Mode.AltHints || MainWindow.data.mode == Mode.OpenKHAltHints)
            {
                RegionComplete();

                string regionName = Name.Substring(0, Name.Length - 4);
                if (MainWindow.data.RegionsData[regionName].hint != null)
                {
                    TextBlock hint = MainWindow.data.RegionsData[regionName].hint;
                    int value = Int32.Parse(hint.Text);
                    ((MainWindow)App.Current.MainWindow).SetReportValue(hint, Children.Count + 1);
                }
            }
            */
        }

        private void Item_Drop(Object sender, DragEventArgs e)
        {
            MainWindow window = ((MainWindow)Application.Current.MainWindow);
            if (e.Data.GetDataPresent(typeof(Item)))
            {

                Item item = e.Data.GetData(typeof(Item)) as Item;
                if(item.Parent is Grid) Add_Item(item, window);
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (System.IO.Path.GetExtension(files[0]).ToUpper() == ".JSON")
                    window.ParseSpoiler(files[0]);
            }
        }

        public void Add_Item(Item item, MainWindow window=null)
        {
            // move item to region
            Panel itemGrid = item.Parent as Panel;
            itemGrid.Children.Remove(item);
            Handle_RegionGrid(item, true);

            // update collection count
            //window.IncrementCollected(itemValue);
            
            // update mouse actions
            //item.MouseDoubleClick -= item.Item_Click;
            item.MouseMove -= item.Item_MouseMove;
            item.MouseDown -= item.Item_Return;
            item.MouseDown += item.Item_Return;
           

            //item.DragDropEventFire(item.Name, Name.Remove(Name.Length - 4, 4), true);
        }

        public bool Handle_Report()
        {
            /*
            bool isreport = false;

            // item is a report
            if (data.hintsLoaded && (int)item.GetValue(Grid.RowProperty) == 0)
            {
                int index = (int)item.GetValue(Grid.ColumnProperty);

                // out of report attempts
                if (data.reportAttempts[index] == 0)
                    return false;

                // check for correct report location
                if (data.reportLocations[index] == Name.Substring(0, Name.Length - 4))
                {
                    // hint text and resetting fail icons
                    window.SetHintText(Codes.GetHintTextName(data.reportInformation[index].Item1) + " has " + data.reportInformation[index].Item2 + " important checks");
                    data.ReportAttemptVisual[index].SetResourceReference(ContentControl.ContentProperty, "Fail0");
                    data.reportAttempts[index] = 3;
                    isreport = true;
                    item.DragDropEventFire(data.reportInformation[index].Item1, data.reportInformation[index].Item2);

                    // set region report hints to as hinted then checks if the report location was hinted to set if its a hinted hint
                    data.RegionsData[data.reportInformation[index].Item1].hinted = true;
                    if (data.RegionsData[data.reportLocations[index]].hinted == true)
                    {
                        data.RegionsData[data.reportInformation[index].Item1].hintedHint = true;
                    }

                    // loop through hinted region for reports to set their info as hinted hints
                    for (int i = 0; i < data.RegionsData[data.reportInformation[index].Item1].RegionGrid.Children.Count; ++i)
                    {
                        Item gridItem = data.RegionsData[data.reportInformation[index].Item1].RegionGrid.Children[i] as Item;
                        if (gridItem.Name.Contains("Report"))
                        {
                            int reportIndex = int.Parse(gridItem.Name.Substring(6)) - 1;
                            data.RegionsData[data.reportInformation[reportIndex].Item1].hintedHint = true;
                            window.SetReportValue(data.RegionsData[data.reportInformation[reportIndex].Item1].hint, data.reportInformation[reportIndex].Item2 + 1);
                        }
                    }

                    // auto update region important check number
                    window.SetReportValue(data.RegionsData[data.reportInformation[index].Item1].hint, data.reportInformation[index].Item2 + 1);
                }
                else
                {
                    // update fail icons when location is report location is wrong
                    AddFailIcon(index);
                    return false;
                }
            }

            if (isreport)
            {
                item.MouseEnter -= item.Report_Hover;
                item.MouseEnter += item.Report_Hover;
            }
            */
            return true;
        }

        public void RegionComplete()
        {
            /*
            string regionName = Name.Substring(0, Name.Length - 4);
            if (regionName == "FlowerFields" || MainWindow.data.RegionsData[regionName].complete == true)
                return;

            List<string> items = new List<string>();
            items.AddRange(MainWindow.data.RegionsData[Name.Substring(0, Name.Length - 4)].checkCount);

            foreach (var child in Children)
            {
                Item item = child as Item;
                char[] numbers = { '1', '2', '3', '4', '5' };
                if (items.Contains(item.Name.TrimEnd(numbers)))
                {
                    items.Remove(item.Name.TrimEnd(numbers));
                }
            }

            if (items.Count == 0)
            {
                MainWindow.data.RegionsData[Name.Substring(0, Name.Length - 4)].complete = true;
            }
            */
        }
    }
}
