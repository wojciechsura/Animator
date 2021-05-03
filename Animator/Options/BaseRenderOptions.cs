using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Options
{
    public class BaseRenderOptions
    {
        [Option('s', "source", Default = null, HelpText = "Source file containing the animation.", Required = true)]
        public string Source { get; set; }

        [Option('o', "outfile", Default = null, Required = true)]
        public string OutFile { get; set; }
    }
}
