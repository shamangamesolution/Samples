using System.Collections.Generic;
using Shaman.Serialization;
using Shaman.Serialization.Messages.Extensions;

namespace Contracts.Extensions
{
    public static class SerializationExtensions
    {
        public static void Write(this ITypeWriter typeWriter, Dictionary<byte, Position> dict)
        {
            if (dict == null)
                dict = new Dictionary<byte, Position>();
            
            typeWriter.Write((byte)dict.Count);
            foreach (var item in dict)
            {
                typeWriter.Write(item.Key);
                typeWriter.WriteEntity(item.Value);
            }
        }

        public static Dictionary<byte, Position> ReadByteEntityDictionary(this ITypeReader typeReader)
        {
            var result = new Dictionary<byte, Position>();
            var count = typeReader.ReadByte();
            for (var i = 0; i < count; i++)
            {
                var key = typeReader.ReadByte();
                var value = typeReader.ReadEntity<Position>();
                result.Add(key, value);
            }

            return result;
        }
    }
}