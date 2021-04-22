﻿using Animator.Engine.Base.Persistence;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Engine.Elements.Persistence
{
    public class AnimationSerializer
    {
        private readonly DeserializationOptions deserializationOptions;

        public AnimationSerializer()
        {
            deserializationOptions = new DeserializationOptions
            {
                DefaultNamespace = new NamespaceDefinition(Assembly.GetExecutingAssembly().FullName,
                    typeof(Animation).Namespace),
                CustomSerializers = new Dictionary<Type, TypeSerializer>
                {
                    { typeof(Brush), new BrushSerializer() },
                    { typeof(List<PathElement>), new PathElementsSerializer() }
                }
            };
        }

        public Animation Deserialize(string filename)
        {
            var serializer = new ManagedObjectSerializer();
            return (Animation)serializer.Deserialize(filename, deserializationOptions);
        }

        public Animation Deserialize(XmlDocument document)
        {
            var serializer = new ManagedObjectSerializer();
            return (Animation)serializer.Deserialize(document, deserializationOptions);
        }
    }
}