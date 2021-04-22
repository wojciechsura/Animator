using Animator.Engine.Animation;
using Animator.Engine.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class BaseElement : ManagedObject
    {
        public void FindNamedElements(NameRegistry names)
        {
            if (IsPropertySet(NameProperty) && Name != null)
            {
                if (!names.TryGetValue(Name, out List<BaseElement> list))
                {
                    list = new();
                    names[Name] = list;
                }

                list.Add(this);
            }

            foreach (var prop in GetProperties(true))
            {
                if (IsPropertySet(prop))
                {
                    var value = GetValue(prop);
                    if (value is BaseElement baseElement)
                    {
                        baseElement.FindNamedElements(names);
                    }
                    else if (value is IList list)
                    {
                        foreach (var obj in list)
                            if (obj is BaseElement listItemBaseElement)
                                listItemBaseElement.FindNamedElements(names);
                    }
                }
            }
        }

        #region Name managed property

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly ManagedProperty NameProperty = ManagedProperty.Register(typeof(BaseElement),
            nameof(Name),
            typeof(string),
            new ManagedSimplePropertyMetadata(null) { NotAnimatable = true });

        #endregion
    }
}
