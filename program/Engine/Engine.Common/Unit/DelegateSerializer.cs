using AutoSerializer;
using Block0.Net.Serialize;
using System;

namespace Engine.Common.Unit
{
    internal class DelegateSeriazlier : ISerializerStub
    {

        public object Deserialize(Type type, ArraySegment<byte> bytes)
        {
            return SerializerProxy.Deserialize(type, bytes);
        }


        public ArraySegment<byte> Serialize(object t)
        {
            var bytes = SerializerProxy.Serialize(t.GetType() , t);
            return new ArraySegment<byte>(bytes);
        }
    }
}