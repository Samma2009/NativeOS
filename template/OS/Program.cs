
using NativeOS;
using NativeOS.Driver;
using NativeOS.FS;
using NativeOS.Misc;
using System.Runtime.InteropServices;
using static NativeOS.NETv4;
using System.Runtime;

unsafe class Program : ProgramClass
{
    [RuntimeExport("KMain")] // do not change this, it is the entrypoint
    static void KMain()
    {

    }
}
