using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.Models.AddNamespace
{
    public record class AddNamespaceResultModel(string Prefix, Assembly Assembly, string Namespace)
    {
        
    }
}
