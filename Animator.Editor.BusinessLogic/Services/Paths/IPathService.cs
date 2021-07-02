using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Services.Paths
{
    public interface IPathService
    {
        string ConfigPath { get; }
        string StoredFilesPath { get; }
    }
}
