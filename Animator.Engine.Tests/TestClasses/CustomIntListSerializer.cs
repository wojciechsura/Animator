using Animator.Engine.Base;
using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class CustomIntListSerializer : TypeSerializer
    {
        public override bool CanDeserialize(string value) => true;

        public override bool CanSerialize(object obj) => obj is List<int>;

        public override object Deserialize(string data)
        {
            return data.Split(',')
                .Select(x => int.Parse(x))
                .ToList();
        }

        public override string Serialize(object obj)
        {
            List<string> data = new();
            foreach (var item in (IList)obj)
                data.Add(((int)item).ToString());

            return string.Join(',', data);
        }
    }
}
