using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Search
{
    public class ReplaceModel : SearchModel
    {
        public ReplaceModel(Regex regex, string replace, bool searchBackwards)
            : base(regex, searchBackwards)
        {
            Replace = replace;
        }

        public string Replace { get; set; }
    }
}
