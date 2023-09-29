using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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

namespace Animator.Designer.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy PropertyPopup.xaml
    /// </summary>
    public partial class PropertyPopup : Grid
    {
        public PropertyPopup()
        {
            InitializeComponent();
        }

        #region ShowSetToDefault

        public bool ShowSetToDefault
        {
            get { return (bool)GetValue(ShowSetToDefaultProperty); }
            set { SetValue(ShowSetToDefaultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSetToDefault.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSetToDefaultProperty =
            DependencyProperty.Register("ShowSetToDefault", typeof(bool), typeof(PropertyPopup), new PropertyMetadata(true));

        #endregion

        #region EnableSetToDefault

        public bool EnableSetToDefault
        {
            get { return (bool)GetValue(EnableSetToDefaultProperty); }
            set { SetValue(EnableSetToDefaultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableSetToDefault.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableSetToDefaultProperty =
            DependencyProperty.Register("EnableSetToDefault", typeof(bool), typeof(PropertyPopup), new PropertyMetadata(true));

        #endregion

        #region ShowSetToString

        public bool ShowSetToString
        {
            get { return (bool)GetValue(ShowSetToStringProperty); }
            set { SetValue(ShowSetToStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSetToString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSetToStringProperty =
            DependencyProperty.Register("ShowSetToString", typeof(bool), typeof(PropertyPopup), new PropertyMetadata(true));

        #endregion

        #region EnableSetToString

        public bool EnableSetToString
        {
            get { return (bool)GetValue(EnableSetToStringProperty); }
            set { SetValue(EnableSetToStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableSetToString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableSetToStringProperty =
            DependencyProperty.Register("EnableSetToString", typeof(bool), typeof(PropertyPopup), new PropertyMetadata(true));

        #endregion

        #region ShowSetToCollection

        public bool ShowSetToCollection
        {
            get { return (bool)GetValue(ShowSetToCollectionProperty); }
            set { SetValue(ShowSetToCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSetToCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSetToCollectionProperty =
            DependencyProperty.Register("ShowSetToCollection", typeof(bool), typeof(PropertyPopup), new PropertyMetadata(true));

        #endregion

        #region EnableSetToCollection

        public bool EnableSetToCollection
        {
            get { return (bool)GetValue(EnableSetToCollectionProperty); }
            set { SetValue(EnableSetToCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableSetToCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableSetToCollectionProperty =
            DependencyProperty.Register("EnableSetToCollection", typeof(bool), typeof(PropertyPopup), new PropertyMetadata(true));

        #endregion
    }
}
