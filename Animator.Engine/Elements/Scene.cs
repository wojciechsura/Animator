using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class Scene : ManagedObject
    {
        #region Name managed property

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly ManagedProperty NameProperty = ManagedProperty.Register(typeof(Scene),
            nameof(Name),
            typeof(string));

        #endregion
    }
}
