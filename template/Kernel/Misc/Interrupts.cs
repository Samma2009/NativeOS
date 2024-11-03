using NativeOS.Driver;
using System.Collections.Generic;

namespace NativeOS.Misc
{
    public static class Interrupts
    {
        public unsafe class INT 
        {
            public int IRQ;
            public delegate*<void> Handler;
        }

        public static List<INT> INTs;

        public static void Initialize() 
        {
            INTs = new List<INT>();
        }

        public static void EndOfInterrupt(byte irq) 
        {
            PIC.EndOfInterrupt(irq);
        }

        public static void EnableInterrupt(byte irq) 
        {
            PIC.ClearMask(irq);
        }

        public static unsafe void EnableInterrupt(byte irq,delegate* <void> handler)
        {
            PIC.ClearMask(irq);
            INTs.Add(new INT() { IRQ = irq, Handler = handler });
        }

        public static unsafe void HandleInterrupt(int irq) 
        {
            for(int i = 0; i < INTs.Count; i++) 
            {
                if (INTs[i].IRQ == irq) INTs[i].Handler();
            }
        }
    }
}