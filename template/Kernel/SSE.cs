using System.Runtime.InteropServices;

namespace NativeOS
{
    internal static class SSE
    {
        [DllImport("*")]
        public static extern void enable_sse();
    }
}