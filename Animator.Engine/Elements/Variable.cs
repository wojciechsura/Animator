using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class Variable : Element
    {
        #region Name managed property

        /// <summary>Name of this variable</summary>
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly ManagedProperty NameProperty = ManagedProperty.Register(typeof(Variable),
            nameof(Name),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = "" });

        #endregion
    }
}
