using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    [ContentProperty(nameof(Keyframes))]
    public partial class For : StoryboardEntry
    {
        internal override void AddKeyframesRecursive(List<Keyframe> keyframes)
        {
            foreach (var keyframe in Keyframes)
                keyframe.AddKeyframesRecursive(keyframes);
        }

        // Public properties --------------------------------------------------

        #region Keyframes managed collection

        public ManagedCollection<StoryboardEntry> Keyframes
        {
            get => (ManagedCollection<StoryboardEntry>)GetValue(KeyframesProperty);
        }

        public static readonly ManagedProperty KeyframesProperty = ManagedProperty.RegisterCollection(typeof(For),
            nameof(Keyframes),
            typeof(ManagedCollection<StoryboardEntry>));

        #endregion
    }
}
