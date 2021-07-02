using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Animator.Editor.BusinessLogic.Services.ImageResources
{
    public interface IImageResources
    {
        ImageSource GetIconByName(string resourceName);
    }
}
