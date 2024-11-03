using Internal.Runtime.CompilerHelpers;
using NativeOS;
using NativeOS.Driver;
using NativeOS.FS;
using NativeOS.Graph;
using NativeOS.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = NativeOS.Driver.Timer;

namespace NativeOS.Misc
{
    internal static unsafe class EntryPoint
    {
        [RuntimeExport("Entry")]
        public static void Entry(MultibootInfo* Info, IntPtr Modules, IntPtr Trampoline)
        {
            Allocator.Initialize((IntPtr)0x20000000);

            StartupCodeHelpers.InitializeModules(Modules);

            PageTable.Initialise();

            ASC16.Initialise();

            VBEInfo* info = (VBEInfo*)Info->VBEInfo;
            if (info->PhysBase != 0)
            {
                Framebuffer.Initialize(info->ScreenWidth, info->ScreenHeight, (uint*)info->PhysBase);
                Framebuffer.Graphics.Clear(0x0);
            }
            else
            {
                for (; ; ) Native.Hlt();
            }
            
            Console.Setup();
            Console.WriteLine($"ConsoleInit");
            IDT.Disable();
            Console.WriteLine($"IDT");
            GDT.Initialise();
            Console.WriteLine($"GDT");
            IDT.Initialize();
            Console.WriteLine($"IDTI");
            Interrupts.Initialize();
            Console.WriteLine($"II");
            IDT.Enable();
            Console.WriteLine($"IDTE");

            SSE.enable_sse();
            Console.WriteLine($"sse");
            //AVX.init_avx();

            ACPI.Initialize();
        PIC.Enable();
            Timer.Initialize();
            Console.WriteLine($"acpi");

            Keyboard.Initialize();
            Console.WriteLine($"keyboard");

            Serial.Initialise();
            Console.WriteLine($"serial");

            PS2Controller.Initialize();
            Console.WriteLine($"ps2");
            VMwareTools.Initialize();
            Console.WriteLine($"vmware tools");

            SMBIOS.Initialise();
            Console.WriteLine($"smbios");

            PCI.Initialise();
            Console.WriteLine($"pci");

            IDE.Initialize();
            Console.WriteLine($"ide");
            SATA.Initialize();
            Console.WriteLine($"sata");

            ThreadPool.Initialize();
            Console.WriteLine($"tp");

            Console.WriteLine($"[SMP] Trampoline: 0x{((ulong)Trampoline).ToString("x2")}");
            Native.Movsb((byte*)SMP.Trampoline, (byte*)Trampoline, 512);

            SMP.Initialize((uint)SMP.Trampoline);

            //Only fixed size vhds are supported!
            Console.Write("[Initrd] Initrd: 0x");
            Console.WriteLine((Info->Mods[0]).ToString("x2"));
            Console.WriteLine("[Initrd] Initializing Ramdisk");
            new Ramdisk((IntPtr)(Info->Mods[0]));
            //new FATFS();
            new TarFS();

            KMain();
        }

        [DllImport("*")]
        public static extern void KMain();
    }
}