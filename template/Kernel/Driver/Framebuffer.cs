using NativeOS.Driver;
using NativeOS.Graph;
using NativeOS.Misc;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NativeOS
{
    public static unsafe class Framebuffer
    {
        public static ushort Width;
        public static ushort Height;
        public static uint* VideoMemory { get; private set; }

        public static uint* FirstBuffer;

        public static Graphics Graphics;

        static bool _DoubleBuffered = false;
        public static bool DoubleBuffered 
        {
            get 
            {
                return _DoubleBuffered;
            }
            set 
            {
                if (Graphics == null) return;
                if (_DoubleBuffered == value) return;

                Graphics.Clear(0x0);
                Graphics.VideoMemory = value ? FirstBuffer : VideoMemory;
                _DoubleBuffered = value;
                if (!_DoubleBuffered)
                {
                    Console.Clear();
                }
            }
        }

        public static void Update()
        {
            if (DoubleBuffered)
            {
                Native.Movsd(VideoMemory, FirstBuffer, (ulong)(Width * Height));
            }
            if(Graphics != null) Graphics.Update();
        }

        public static void Initialize(ushort XRes, ushort YRes,uint* FB)
        {
            Width = XRes;
            Height = YRes;
            FirstBuffer = (uint*)Allocator.Allocate((ulong)(XRes * YRes * 4));
            Native.Stosd(FirstBuffer, 0, (ulong)(XRes * YRes));
            Control.MousePosition.X = XRes / 2;
            Control.MousePosition.Y = YRes / 2;
            Graphics = new Graphics(Width, Height, FB);
            VideoMemory = FB;

            Console.Clear();
        }
    }
}