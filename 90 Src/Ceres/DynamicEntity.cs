using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ceres
{
    public class DynamicEntity : DynamicObject
    {
        protected IDictionary<string, object> container;

        public DynamicEntity()
        {
            this.container = new Dictionary<string, object>();
        }

        public override bool TrySetMember(SetMemberBinder binder,
            object value)
        {
            this.container[binder.Name] = value;

            return true;
        }

        public void SetMember(string key,
            object value)
        {
            this.container[key] = value;
        }

        public override bool TryGetMember(GetMemberBinder binder,
            out object value)
        {
            this.container.TryGetValue(binder.Name,
                out value);

            return true;
        }

        public object GetMember(string key)
        {
            var value = null as object;

            this.container.TryGetValue(key,
                out value);

            return value;
        }

        public string GetMemberAsString(string key)
        {
            return this.GetMember(key) as string;
        }

        public string[] GetKeys()
        {
            return this.container.Keys.ToArray();
        }
    }
}
