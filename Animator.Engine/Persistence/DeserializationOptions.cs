﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Persistence
{
    public class DeserializationOptions
    {
        public NamespaceDefinition DefaultNamespace { get; init; }
        public CustomActivator CustomActivator { get; init; }
    }
}
