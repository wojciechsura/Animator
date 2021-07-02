using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Animator.Editor.BusinessLogic.Models.Navigation
{
    public class BaseNavigationModel
    {
        public BaseNavigationModel(string title, ImageSource icon, bool enabled)
        {
            Title = title;
            Icon = icon;
            Enabled = enabled;
        }

        public string Title { get; }
        public ImageSource Icon { get; }
        public bool Enabled { get; }
    }
}
