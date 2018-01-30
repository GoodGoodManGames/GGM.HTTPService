using System;
using System.Collections.Generic;
using System.Text;
using GGM.Context;
using GGM.Context.Attribute;
using GGM.Web.MessageExample;
using GGM.Serializer;

namespace GGM.Web.MessageExample
{
    [Managed(ManagedType.Singleton)]
    public class TestObjFactory : ISerializerFactory
    {
        public ISerializer Create() => new TestObjSerializer();
    }
}