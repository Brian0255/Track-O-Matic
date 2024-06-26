﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Forms;

namespace TrackOMatic
{
    public partial class PathOrFoundItem : ContentControl
    {
        private ItemBrightnessChanger ItemBrightnessChanger;
        public PathOrFoundItem(ItemName itemName)
        {
            InitializeComponent();
            ItemBrightnessChanger = new ItemBrightnessChanger(Image, itemName);
            ItemBrightnessChanger.Brighten();
            DataContext = this;
        }
    }
}