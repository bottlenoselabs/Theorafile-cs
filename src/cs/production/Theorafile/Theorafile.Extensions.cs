using System;
using System.Runtime.InteropServices;
using System.Text;

namespace bottlenoselabs
{
    public static partial class Theorafile
    {
        public static unsafe int Open(string name, out IntPtr file)
        {
            file = AllocTheoraFile();
            var nameC = new Runtime.CString(Marshal.StringToHGlobalAnsi(name));

            int result;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                /* Windows fopen doesn't like UTF8, use LPCSTR and pray */
                result = tf_fopen(nameC, (OggTheora_File*)file);
            }
            else
            {
                var nameUtf8 = Utf8Encode(name);
                result = tf_fopen(nameUtf8, (OggTheora_File*)file);
                Marshal.FreeHGlobal((IntPtr) nameUtf8);
            }

            return result;
        }
        
        public static unsafe int OpenCallbacks(IntPtr datasource, out IntPtr file, tf_callbacks io)
        {
            file = AllocTheoraFile();
            return tf_open_callbacks((void*)datasource, (OggTheora_File*)file, io);
        }
        
        public static unsafe void Close(ref IntPtr file)
        {
            tf_close((OggTheora_File*)file);
            Marshal.FreeHGlobal(file);
            file = IntPtr.Zero;
        }
        
        /* Notice that we did not implement an OggTheora_File struct, but are
         * instead using a pointer natively malloc'd.
         *
         * C# Interop for Xiph structs is basically impossible to do, so
         * we just alloc what _should_ be the full size of the structure for
         * the OS and architecture, then pass that around as if that's a real
         * struct. The size is just what you get from sizeof(OggTheora_File).
         *
         * Don't get mad at me, get mad at C#.
         *
         * -flibit
         */

        private static IntPtr AllocTheoraFile()
        {
            // Do not attempt to understand these numbers at all costs!
            const int size32 = 1160;
            const int size64Unix = 1472;
            const int size64Windows = 1328;

            PlatformID platform = Environment.OSVersion.Platform;
            if (IntPtr.Size == 4)
            {
                /* Technically this could be a little bit smaller, but
                 * some 32-bit architectures may be higher even on Unix
                 * targets (like ARMv7).
                 * -flibit
                 */
                return Marshal.AllocHGlobal(size32);
            }

            if (IntPtr.Size == 8)
            {
                if (platform == PlatformID.Unix)
                {
                    return Marshal.AllocHGlobal(size64Unix);
                }
                else if (platform == PlatformID.Win32NT)
                {
                    return Marshal.AllocHGlobal(size64Windows);
                }

                throw new NotSupportedException("Unhandled platform!");
            }

            throw new NotSupportedException("Unhandled architecture!");
        }
        
        private static unsafe byte* Utf8Encode(string str)
        {
            var bufferSize = str.Length * 4 + 1;
            var buffer = (byte*) Marshal.AllocHGlobal(bufferSize);
            fixed (char* stringPointer = str)
            {
                Encoding.UTF8.GetBytes(
                    stringPointer,
                    str.Length + 1,
                    buffer,
                    bufferSize
                );
            }
            return buffer;
        }
    }
}
