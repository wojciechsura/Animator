using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.ViewModels.Document
{
    public class CompletionInfo
    {
        public CompletionInfo(string display, string content, int offset)
        {
            Display = display;
            Content = content;
            Offset = offset;
        }

        public string Display { get; }
        public string Content { get; }
        public int Offset { get; }
    }
}
