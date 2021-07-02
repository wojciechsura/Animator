using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Services.FileIcons
{
    public interface IFileIconProvider
    {
        System.Windows.Media.ImageSource GetImageForFile(string filename);
        System.Windows.Media.ImageSource GetImageForFolder(string folderName);
    }
}
