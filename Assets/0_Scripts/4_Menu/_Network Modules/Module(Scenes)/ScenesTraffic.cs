using ICENet.Traffic;

namespace Badbarbos.Network.Modules.Scenes
{
    public class NewScenePacketRequest : Packet //FROM CLIENT
    {
        public override int Id => 12121298;

        public override bool IsReliable => true;

        public int SceneId;

        protected override void Write(ref Buffer data)
        {
            data.Write(SceneId);
        }

        protected override void Read(ref Buffer data)
        {
            SceneId = data.ReadInt32();
        }
    }

    public class NewScenePacketResponse : Packet //FROM SERVER
    {
        public override int Id => 12121299;

        public override bool IsReliable => true;

        public int SceneId;

        protected override void Write(ref Buffer data)
        {
            data.Write(SceneId);
        }

        protected override void Read(ref Buffer data)
        {
            SceneId = data.ReadInt32();
        }
    }
}