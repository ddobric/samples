using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace RefSemantic.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            MyStruct s1 = new MyStruct() { X = 1.1, Y = 2.2 };

            PassByRef(in s1);
            ReturnRefSample1();

            SpanSample1();
            SpanSample2();
            SpanSample3();
            SpanSample4();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }


        /// <summary>
        /// Size of struct is 16 bytes.
        /// sizeof(MyStruct) = 2 * sizeof(double) = 16.
        /// </summary>
        public struct MyStruct
        {
            public double X;

            public double Y;
        }

        //public readonly ref struct Span<T>
        //{
        //    private readonly ref T _ptr;
        //    private readonly int _length;

        //}

        public static double PassByRef(in MyStruct s)
        {
            return s.X + s.Y;
        }

        #region SPAN<T> Samples
        public static void SpanSample1()
        {
            byte[] buff = new byte[100000];

            Span<byte> bytes = new Span<byte>(buff);
            Span<byte> slicedBytes = bytes.Slice(start: 5, length: 2);
            slicedBytes[0] = 42;
            slicedBytes[1] = 43;

            Console.WriteLine($"slicedBytes[0] {slicedBytes[0]}");
            Console.WriteLine($"slicedBytes[1] {slicedBytes[1]}");
            Console.WriteLine($"buff[5] {slicedBytes[0]}");
            Console.WriteLine($"buff[6] {slicedBytes[1]}");

            //slicedBytes[2] = 44; // Throws IndexOutOfRangeException

            bytes[2] = 45; // OK

            // Implicite cast supported
            Span<byte> newSpan = buff;

            Console.WriteLine($"bytes[2] {bytes[2]}");

        }


        /// <summary>
        /// Heap allocation with span<>
        /// </summary>
        public static void SpanSample2()
        {
            IntPtr ptr = Marshal.AllocHGlobal(1);
            try
            {
                Span<byte> bytes;
                unsafe { bytes = new Span<byte>((byte*)ptr, 3 ); }
                bytes[0] = 0xFF;
                bytes[1] = 0xAA;
                bytes[2] = 0xBB;
                Console.WriteLine($"bytes[0] {bytes[0]} == {Marshal.ReadByte(ptr)}");

                //bytes[1] = 43; // Throws IndexOutOfRangeException
            }
            finally { Marshal.FreeHGlobal(ptr); }
        }

        public static void SpanSample3()
        {
            
            #region Element access in the List<>

            var listOfStructs = new List<MyStruct> { new MyStruct() };

            // Does not compile.
            // You need first to access element, which will do the copy of structure=>slow operation
            //listOfStructs[0].X = 42; // Error CS1612: the return value is not a variable
            MyStruct s = listOfStructs[0];
            s.X = 42;

            #endregion

            #region Solution with Span

            Span<MyStruct> spanOfStructs = new MyStruct[10];

            spanOfStructs[0].X = 42;

            Console.WriteLine($"spanOfStructs[0].X: {spanOfStructs[0].X}");

            #endregion

            #region interiors

            var arr = new byte[100];

            // Create Span<T> from Array from fixed position.
            Span<byte> interiorRef1 = arr.AsSpan().Slice(start: 20);

            // Create Span<T> from Array from fixed position and length.
            Span<byte> interiorRef2 = new Span<byte>(arr, 20, arr.Length - 20);

            //Span<byte> interiorRef3 =
            //  Span<byte>.DangerousCreate(arr, ref arr[20], arr.Length – 20);
            #endregion
        }


        /// <summary>
        /// Strings implementsd Span<string> now.
        /// </summary>
        public static void SpanSample4()
        {
            string str = "hello, world";
            string worldString = str.Substring(startIndex: 7, length: 5); // Allocates ReadOnlySpan<char> 

            var worldSpan = str.AsSpan().Slice(start: 7, length: 5); // No allocation

            Console.WriteLine(worldSpan[0]);
            
            //worldSpan[0] = 'a'; // Error CS0200: indexer cannot be assigned to
        }

        private static MyStruct[] m_List = new MyStruct[] { new MyStruct { X = 1 },
                                                     new MyStruct { X = 2 },
                                                     new MyStruct { X = 3 } };



        public static void ReturnRefSample1()
        {
            ref var res = ref getElement(1);
            res.X = 77;
        }

        private static ref MyStruct getElement(int k)
        {
            return ref m_List[k];
        }
        #endregion

        #region Compatibility Pack
        private static string GetLoggingPath()
        {
            // Verify the code is running on Windows.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Fabrikam\AssetManagement"))
                {
                    if (key?.GetValue("LoggingDirectoryPath") is string configuredPath)
                        return configuredPath;
                }
            }

            // This is either not running on Windows or no logging path was configured,
            // so just use the path for non-roaming user-specific data files.
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataPath, "Fabrikam", "AssetManagement", "Logging");
        }
        #endregion
    }
}
