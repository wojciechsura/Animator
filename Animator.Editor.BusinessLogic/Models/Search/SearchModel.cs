using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Search
{
    public class SearchModel
    {
        public SearchModel(Regex regex, bool searchBackwards)
        {
            Regex = regex;
            SearchBackwards = searchBackwards;
        }

        public Regex Regex { get; }
        public bool SearchBackwards { get; }
    }
}
