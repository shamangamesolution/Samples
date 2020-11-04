using System.Collections.Generic;
using Contracts.Extensions;
using Shaman.Serialization;
using Shaman.Serialization.Extensions;
using Shaman.Serialization.Messages.Extensions;
using Shaman.Serialization.Messages.Udp;

namespace Contracts.DTO
{
    public class UpdatePositionEvent : EventBase
    {
        public Position Position { get; set; }
        
        public UpdatePositionEvent() : base(EventOperationCodes.UpdatePositionEvent)
        {
        }

        public UpdatePositionEvent(Position position) : this()
        {
            Position = position;
        }

        protected override void SerializeBody(ITypeWriter typeWriter)
        {
            typeWriter.WriteEntity(Position);
        }

        protected override void DeserializeBody(ITypeReader typeReader)
        {
            Position = typeReader.ReadEntity<Position>();
        }
    }
}