using Animator.Engine.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Engine.Persistence
{
    public class AnimationSerializer
    {
        public Animation Deserialize(string filename)
        {
            var serializer = new ManagedObjectSerializer();
            return (Animation)serializer.Deserialize(filename);
        }

        public Animation Deserialize(XmlDocument document)
        {
            var serializer = new ManagedObjectSerializer();
            return (Animation)serializer.Deserialize(document);
        }
    }
}
