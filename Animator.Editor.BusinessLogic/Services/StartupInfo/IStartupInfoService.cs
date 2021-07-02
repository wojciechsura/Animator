using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Services.StartupInfo
{
    public interface IStartupInfoService
    {
        string[] Parameters { get; set; }
    }
}
