using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Options
{
    [Verb("render-frame", HelpText = "Renders single frame of the animation.")]
    public class RenderFrameOptions : BaseRenderOptions
    {
        [Option(Default = null, HelpText = "Frame number", Required = true, SetName = "Offset")]
        public int? FrameIndex { get; set; }

        [Option(Default = null, HelpText = "Time offset", Required = true, SetName = "Offset")]
        public TimeSpan? TimeOffset { get; set; }        
    }
}
