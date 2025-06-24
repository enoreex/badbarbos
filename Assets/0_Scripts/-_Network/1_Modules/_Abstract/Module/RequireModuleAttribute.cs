using System;

namespace Badbarbos.Network
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireModuleAttribute : Attribute
    {
        public Type ModuleType { get; }

        public RequireModuleAttribute(Type moduleType) => ModuleType = moduleType;
    }
}