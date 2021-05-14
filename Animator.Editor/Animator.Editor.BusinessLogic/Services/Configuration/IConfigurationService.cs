using Animator.Editor.BusinessLogic.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Services.Config
{
    public interface IConfigurationService
    {
        ConfigModel Configuration { get; }

        bool Save();
    }
}
