using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Search
{
    public class ReplaceAllModel : ReplaceModel
    {
        public ReplaceAllModel(Regex regex, string replace, bool searchBackwards, bool inSelection) 
            : base(regex, replace, searchBackwards)
        {
            InSelection = inSelection;
        }

        public bool InSelection { get; }
    }
}
