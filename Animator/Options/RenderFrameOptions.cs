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
        [Option('f', "frame-index", Default = null, HelpText = "Frame number", Required = true, SetName = "FrameIndex")]
        public int? FrameIndex { get; set; }

        [Option('t', "time-offset", Default = null, HelpText = "Time offset", Required = true, SetName = "TimeOffset")]
        public TimeSpan? TimeOffset { get; set; }        
    }
}
