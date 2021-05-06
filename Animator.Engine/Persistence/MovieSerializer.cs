using Animator.Engine.Base.Persistence;
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
    public class MovieSerializer
    {
        private readonly DeserializationOptions deserializationOptions;

        public MovieSerializer()
        {
            deserializationOptions = new DeserializationOptions
            {
                DefaultNamespace = new NamespaceDefinition(Assembly.GetExecutingAssembly().FullName,
                    typeof(Movie).Namespace),
                CustomSerializers = new Dictionary<Type, TypeSerializer>
                {
                    { typeof(Brush), new BrushSerializer() }
                }
            };
        }

        public Movie Deserialize(string filename)
        {
            var serializer = new ManagedObjectSerializer();
            return (Movie)serializer.Deserialize(filename, deserializationOptions);
        }

        public Movie Deserialize(XmlDocument document)
        {
            var serializer = new ManagedObjectSerializer();
            return (Movie)serializer.Deserialize(document, deserializationOptions);
        }
    }
}
