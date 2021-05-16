using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Animator.Editor.Controls
{
    public class CompletionData : ICompletionData
    {
        private readonly int offset;

        public CompletionData(string text, string display, int offset)
        {
            Text = text;
            Content = display;
            this.offset = offset;
        }

        public ImageSource Image { get; set; }

        public string Text { get; set; }

        public object Content { get; set; }

        public object Description { get; set; }

        public double Priority { get; set; }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            int start = completionSegment.Offset;
            textArea.Document.Replace(completionSegment, this.Text);
            textArea.Caret.Offset = start + offset;
        }
    }
}
