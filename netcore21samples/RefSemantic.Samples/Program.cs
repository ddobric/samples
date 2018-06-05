using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RefSemantic.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            MyStruct s1 = new MyStruct() { X = 1.1, Y = 2.2 };

            PassByRef(in s1);

            SpanSample1();
            SpanSample2();
            SpanSample3();
            SpanSample4();

            Console.WriteLine("Hello World!");
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

            Console.WriteLine($"bytes[2] {bytes[2]}");

        }

        public static void SpanSample2()
        {
            IntPtr ptr = Marshal.AllocHGlobal(1);
            try
            {
                Span<byte> bytes;
                unsafe { bytes = new Span<byte>((byte*)ptr, 1); }
                bytes[0] = 42;
                Console.WriteLine($"bytes[0] {bytes[0]} == {Marshal.ReadByte(ptr)}");

                //bytes[1] = 43; // Throws IndexOutOfRangeException
            }
            finally { Marshal.FreeHGlobal(ptr); }
        }

        public static void SpanSample3()
        {
            Span<MyStruct> spanOfStructs = new MyStruct[1];
            spanOfStructs[0].X = 42;
            Console.WriteLine($"spanOfStructs[0].X: {spanOfStructs[0].X}");

            var listOfStructs = new List<MyStruct> { new MyStruct() };

            //listOfStructs[0].X = 42; // Error CS1612: the return value is not a variable
        }

        public static void SpanSample4()
        {
            string str = "hello, world";
            string worldString = str.Substring(startIndex: 7, length: 5); // Allocates ReadOnlySpan<char> 
            var worldSpan = str.AsSpan().Slice(start: 7, length: 5); // No allocation
            Console.WriteLine(worldSpan[0]);
            //worldSpan[0] = 'a'; // Error CS0200: indexer cannot be assigned to
        }
        #endregion
    }
}
