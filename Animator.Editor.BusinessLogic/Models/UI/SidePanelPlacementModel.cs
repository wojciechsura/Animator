using Animator.Editor.BusinessLogic.Types.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.UI
{
    public class SidePanelPlacementModel
    {
        public SidePanelPlacementModel(string display, SidePanelPlacement value)
        {
            Display = display;
            Value = value;
        }

        public string Display { get; }
        public SidePanelPlacement Value { get; }
    }
}
