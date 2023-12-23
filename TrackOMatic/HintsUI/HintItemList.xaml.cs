﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TrackOMatic
{
    public partial class HintItemList : UserControl
    {
        private List<ItemName> sortedItemList = new();

        public static readonly DependencyProperty CustomHorizontalAlignmentProperty =
            DependencyProperty.Register(
                "CustomHorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(HintItemList),
                new PropertyMetadata(HorizontalAlignment.Left)
            );

        public HorizontalAlignment CustomHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(CustomHorizontalAlignmentProperty); }
            set { SetValue(CustomHorizontalAlignmentProperty, value); }
        }

        public double[] SelectionDialogPosition { get; set; } = { 0, 0 };
        public HintItemList()
        {
            InitializeComponent();
            DataContext = this;
            ProcessSelectedItems(new());

        }

        public void OpenItemSelectionDialog()
        {
            var dialog = new HintItemSelectionDialog(sortedItemList);

            dialog.Left = SelectionDialogPosition[0];
            dialog.Top = SelectionDialogPosition[1];

            dialog.ShowDialog();
            ProcessSelectedItems(dialog.SelectedItems);
        }

        public void Image_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed) OpenItemSelectionDialog();
        }

        public void AddNewImageToPanel(ItemName itemName)
        {
            var resourceName = itemName.ToString().ToLower();
            var image = ((Image)FindResource(resourceName));
            var newItem = new PathOrFoundItem(image);
            newItem.MouseDown += Image_MouseDown;
            ItemPanel.Children.Add(newItem);
        }

        public void ProcessSelectedItems(List<ItemName> itemSet)
        {
            ItemPanel.Children.Clear();
            ItemPanel.BeginInit();
            sortedItemList = new();
            foreach (var itemName in itemSet)
            {
                int index = (-1 * sortedItemList.BinarySearch(itemName)) - 1;
                sortedItemList.Insert(index, itemName);
            }
            if (sortedItemList.Count == 0)
            {
                sortedItemList.Add(ItemName.NONE);
            }
            foreach (var item in sortedItemList)
            {
                AddNewImageToPanel(item);
            }
            ItemPanel.EndInit();
        }
    }
}
