using System.Reflection;

namespace BuildingBlocks.Core
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
