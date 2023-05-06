using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Options
{
    [Verb("render-parallel", HelpText = "Renders the animation, using multiple threads (experimental)")]
    public class RenderParallelOptions : BaseRenderOptions
    {
        [Option('t', "threads", Default = 4, HelpText = "How many threads to use", Required = false, SetName = "Threads")]
        public int Threads { get; set; }
    }
}
