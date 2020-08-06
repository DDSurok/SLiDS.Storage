using System.Collections.Generic;
using System.Dynamic;

namespace SLiDS.Storage.Api
{
    public sealed class RepositoryDynamicObject : DynamicObject
    {
        private readonly Dictionary<string, object> _properties;
        public RepositoryDynamicObject(Dictionary<string, object> properties) => _properties = properties;
        public override IEnumerable<string> GetDynamicMemberNames() => _properties.Keys;
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_properties.ContainsKey(binder.Name))
            {
                result = _properties[binder.Name];
                return true;
            }
            result = null;
            return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!_properties.ContainsKey(binder.Name))
                return false;
            _properties[binder.Name] = value;
            return true;
        }
    }
}
