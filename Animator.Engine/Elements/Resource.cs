using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract partial class Resource : Element
    {
        public abstract object GetValue();

        #region Key managed property

        /// <summary>Key of this resource</summary>
        public string Key
        {
            get => (string)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        public static readonly ManagedProperty KeyProperty = ManagedProperty.Register(typeof(Resource),
            nameof(Key),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = "" });

        #endregion
    }
}
