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
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Picture.Source = new BitmapImage(new Uri("../Images/dk64/" + RegionName.ToString().ToLower() + ".png", UriKind.Relative));
        }

        private void GBCount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && GBCount.Text == "?")
            {
                GBCount.Text = "";
            }
        }

        private void CheckSave()
        {
            if (GBCount.Text != "?")
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.DataSaver.Save();
            }
        }

        private void GBCount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                CheckSave();
            }
        }

        private void GBCount_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckSave();
        }
    }
}
