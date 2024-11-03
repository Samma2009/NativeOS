using NativeOS.Driver;
using NativeOS.FS;
using NativeOS.Misc;
using System;

namespace NativeOS.Graph
{
    [Obsolete("This graphics does not support unbuffered mode",true)]
    internal unsafe class VMWareSVGAIIGraphics : Graphics
    {
        VMWareSVGAII svga;

        public VMWareSVGAIIGraphics(ushort Width = 1280,ushort Height = 768) : base(Width,Height,null)
        {
            svga = new VMWareSVGAII();
            svga.SetMode(Width,Height);
            Framebuffer.Initialize(Width, Height, svga.Video_Memory);
            base.VideoMemory = Framebuffer.FirstBuffer;

        }

        public override void Update()
        {
            svga.Update();
        }
    }
}