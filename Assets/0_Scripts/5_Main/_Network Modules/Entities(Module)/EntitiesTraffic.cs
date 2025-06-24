using ICENet.Traffic;

namespace Badbarbos.Network.Modules.Entities
{
    #region Prop Enter

    public class PropEnterRequest : Packet //FROM CLIENT
    {
        public override int Id => 7575510;

        public override bool IsReliable => true;

        public int EntityId;

        protected override void Write(ref ICENet.Traffic.Buffer data)
        {
            data.Write(EntityId!);
        }

        protected override void Read(ref ICENet.Traffic.Buffer data)
        {
            EntityId = data.ReadInt32();
        }
    }

    public class PropEnterResponse : Packet //FROM SERVER
    {
        public override int Id => 7575511;

        public override bool IsReliable => true;

        public int EntityId;

        protected override void Write(ref ICENet.Traffic.Buffer data)
        {
            data.Write(EntityId!);
        }

        protected override void Read(ref ICENet.Traffic.Buffer data)
        {
            EntityId = data.ReadInt32();
        }
    }

    public class PropAnotherEntered : Packet
    {
        public override int Id => 7575512;

        public override bool IsReliable => true;

        public int MemberId;

        public int EntityId;

        protected override void Write(ref ICENet.Traffic.Buffer data)
        {
            data.Write(MemberId);
            data.Write(EntityId);
        }

        protected override void Read(ref ICENet.Traffic.Buffer data)
        {
            MemberId = data.ReadInt32();
            EntityId = data.ReadInt32();
        }
    }

    #endregion

    #region Prop Data

    public class ClientToServerPropDataPacket : Packet //FROM CLIENT
    {
        public override int Id => 7575513;

        public override bool IsReliable => false;

        public byte[] Buffer;

        protected override void Write(ref ICENet.Traffic.Buffer data)
        {
            data.Write(Buffer.Length);
            data.Write(Buffer);
        }

        protected override void Read(ref ICENet.Traffic.Buffer data)
        {
            var bytes = data.ReadInt32();
            Buffer = data.ReadBytes(bytes);
        }
    }

    public class ServerToClientPropDataPacket : Packet //FROM SERVER
    {
        public override int Id => 7575514;

        public override bool IsReliable => false;

        public int MemberId;

        public int EntityId;

        public byte[] Buffer;

        protected override void Write(ref ICENet.Traffic.Buffer data)
        {
            data.Write(MemberId);
            data.Write(EntityId);

            data.Write(Buffer.Length);
            data.Write(Buffer);
        }

        protected override void Read(ref ICENet.Traffic.Buffer data)
        {
            MemberId = data.ReadInt32();
            EntityId = data.ReadInt32();

            var bytes = data.ReadInt32();
            Buffer = data.ReadBytes(bytes);
        }
    }

    #endregion

    #region Geg Activation

    public class GegActivationRequest : Packet //FROM CLIENT
    {
        public override int Id => 7575610;

        public override bool IsReliable => false;

        public int EntityId;

        protected override void Write(ref ICENet.Traffic.Buffer data)
        {
            data.Write(EntityId);
        }

        protected override void Read(ref ICENet.Traffic.Buffer data)
        {
            EntityId = data.ReadInt32();
        }
    }

    public class GegActivationResponse : Packet //FROM SERVER
    {
        public override int Id => 7575611;

        public override bool IsReliable => false;

        public int MemberId;

        public int EntityId;

        protected override void Write(ref ICENet.Traffic.Buffer data)
        {
            data.Write(MemberId);
            data.Write(EntityId);
        }

        protected override void Read(ref ICENet.Traffic.Buffer data)
        {
            MemberId = data.ReadInt32();
            EntityId = data.ReadInt32();
        }
    }

    #endregion
}
