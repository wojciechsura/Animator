﻿using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers
{
    public class NamespaceViewModel
    {
        private readonly Dictionary<Type, HashSet<Type>> availableTypes = new();

        public NamespaceViewModel(string prefix, Assembly assembly, string @namespace)
        {
            Prefix = prefix;
            Assembly = assembly;
            Namespace = @namespace;

            foreach (var type in Assembly.GetTypes()
                .Where(t => !t.IsAbstract))
            {
                if (type.Namespace == @namespace)
                {
                    var current = type;

                    while (current != typeof(object))
                    {
                        if (!availableTypes.TryGetValue(current, out var types))
                        {
                            types = new();
                            availableTypes.Add(current, types);
                        }

                        types.Add(type);

                        current = current.BaseType;
                    }
                }
            }
        }

        public string Prefix { get; }
        public string Namespace { get; }
        public Assembly Assembly { get; }

        public IEnumerable<Type> GetAvailableTypesFor(Type type)
        {
            if (availableTypes.TryGetValue(type, out var types))
                return types;

            return Enumerable.Empty<Type>();
        }
    }
}
