using Shaman.Serialization;
using Shaman.Serialization.Messages;

namespace Contracts
{
    public struct PlayerProperties
    {
        public const byte NickName = 1;
    }
    
    public class PlayerData : EntityBase
    {
        public byte Index { get; set; }
        public string Name { get; set; }

        public PlayerData()
        {
            
        }
        
        public PlayerData(byte index, string name)
        {
            Index = index;
            Name = name;
        }

        
        protected override void SerializeBody(ITypeWriter typeWriter)
        {
            typeWriter.Write(Index);
            typeWriter.Write(Name);
        }

        protected override void DeserializeBody(ITypeReader typeReader)
        {
            Index = typeReader.ReadByte();
            Name = typeReader.ReadString();
        }
    }
}