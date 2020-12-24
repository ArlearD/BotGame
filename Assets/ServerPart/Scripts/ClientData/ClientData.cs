using LiteNetLib.Utils;
using UnityEngine;

namespace ServerPart.Scripts
{
    public class ClientData : INetSerializable
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public string Name { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Position.x);
            writer.Put(Position.y);
            writer.Put(Position.z);

            writer.Put(Rotation.x);
            writer.Put(Rotation.y);
            writer.Put(Rotation.z);
            writer.Put(Rotation.w);
            
            writer.Put(Name);
        }

        public void Deserialize(NetDataReader reader)
        {
            Position = new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            Rotation = new Quaternion(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            Name = reader.GetString();
        }
    }
}