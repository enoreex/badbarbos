using ICENet.Traffic;

namespace Badbarbos.Network.Modules.Members
{
    public class MemberRegisterPacket : Packet //FROM CLIENT
    {
        public override int Id => 959595;

        public override bool IsReliable => true;

        public string MemberName;

        protected override void Write(ref Buffer data)
        {
            data.Write(MemberName);
        }

        protected override void Read(ref Buffer data)
        {
            MemberName = data.ReadString();
        }
    }

    public class MemberAddedPacket : Packet //FROM SERVER
    {
        public override int Id => 969696;

        public override bool IsReliable => true;

        public string MemberName;

        public int MemberId;

        protected override void Write(ref Buffer data)
        {
            data.Write(MemberName);
            data.Write(MemberId);
        }

        protected override void Read(ref Buffer data)
        {
            MemberName = data.ReadString();
            MemberId = data.ReadInt32();
        }
    }

    public class MemberRemovedPacket : Packet //FROM SERVER
    {
        public override int Id => 979797;

        public override bool IsReliable => true;

        public int MemberId;

        protected override void Write(ref Buffer data)
        {
            data.Write(MemberId);
        }

        protected override void Read(ref Buffer data)
        {
            MemberId = data.ReadInt32();
        }
    }

    public class MemberAcceptedPacket : Packet //FROM SERVER
    {
        public override int Id => 559696;

        public override bool IsReliable => true;

        public int MemberId;

        protected override void Write(ref Buffer data)
        {
            data.Write(MemberId);
        }

        protected override void Read(ref Buffer data)
        {
            MemberId = data.ReadInt32();
        }
    }
}