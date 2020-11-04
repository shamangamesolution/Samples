using Shaman.Serialization;
using Shaman.Serialization.Messages;

namespace Contracts
{
    public class Position : EntityBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Position()
        {
            
        }
        
        public Position(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        protected override void SerializeBody(ITypeWriter typeWriter)
        {
            typeWriter.Write(X);
            typeWriter.Write(Y);
            typeWriter.Write(Z);
        }

        protected override void DeserializeBody(ITypeReader typeReader)
        {
            X = typeReader.ReadFloat();
            Y = typeReader.ReadFloat();
            Z = typeReader.ReadFloat();
        }
    }
}