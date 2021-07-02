using Animator.Editor.BusinessLogic.Services.ImageResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Animator.Editor.Services.ImageResources
{
    public class ImageResources : IImageResources
    {
        private string Prefix = "pack://application:,,,/Animator.Editor;component/Resources/Images/";

        public ImageSource GetIconByName(string resourceName)
        {
            if (String.IsNullOrEmpty(resourceName))
                return null;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(Prefix + resourceName);
            image.EndInit();

            return image;            
        }
    }
}
