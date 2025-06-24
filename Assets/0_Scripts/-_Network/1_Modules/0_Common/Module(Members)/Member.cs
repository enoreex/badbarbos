namespace Badbarbos.Network.Modules.Members
{
    public abstract class Member
    {
        public readonly string Name;

        public readonly int Id;

        public Member(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }

    public sealed class MemberClient : Member
    {
        public MemberClient(string name, int id) : base(name, id)
        {
        }
    }
}