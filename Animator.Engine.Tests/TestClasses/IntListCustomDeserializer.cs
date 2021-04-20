using Animator.Engine.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class IntListCustomDeserializer : CustomCollectionSerializer
    {
        public override IList Deserialize(string data)
        {
            return data.Split(',')
                .Select(x => int.Parse(x))
                .ToList();
        }

        public override string Serialize(IList list)
        {
            List<string> data = new();
            foreach (var item in list)
                data.Add(((int)item).ToString());

            return string.Join(',', data);
        }
    }
}
