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
        public CompletionData(string text)
        {
            Text = text;
            Content = text;
        }

        public ImageSource Image { get; set; }

        public string Text { get; set; }

        public object Content { get; set; }

        public object Description { get; set; }

        public double Priority { get; set; }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
