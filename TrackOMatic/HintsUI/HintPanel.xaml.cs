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
    public partial class HintPanel : UserControl
    {
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(HintPanel));
        public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register("LineColor", typeof(Color), typeof(HintPanel));
        public static readonly DependencyProperty HintTypeProperty = DependencyProperty.Register("HintType", typeof(HintType), typeof(HintPanel));
        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public HintType HintType
        {
            get { return (HintType)GetValue(HintTypeProperty); }
            set { SetValue(HintTypeProperty, value); }
        }
        public HintPanel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Reset()
        {
            foreach (var child in HintList.Children.OfType<HintInfo>().ToList()) 
            {
                HintList.Children.Remove(child);
            }
        }

        private void RemoveHint(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                HintInfo hint = (HintInfo)sender;
                HintList.Children.Remove(hint);
            }
        }

        private HintInfo CreateNewHint(bool isSavedHint = false)
        {
            var newHint = new HintInfo(HintType, Name, isSavedHint);
            var insertionPoint = HintList.Children.Count - 1;
            HintList.Children.Insert(insertionPoint, newHint);
            newHint.MouseDown += RemoveHint;
            return newHint;
        }

        public void OnAddHint(object sender, MouseEventArgs e)
        {
            CreateNewHint();
        }

        public void AddSavedHint(SavedHint savedHint)
        {
            var newHint = CreateNewHint(true);
            newHint.SetUpFromSavedHint(savedHint);
        }

        public List<SavedHint> GetSavedHints()
        {
            List<SavedHint> hints = new();
            foreach (var hint in HintList.Children)
            {
                if (hint is HintInfo hintInfo) hints.Add(hintInfo.SavedHint);
            }
            return hints;
        }
    }
}
