using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
#pragma warning disable 0618

//public class MainConfig : ManualConfig
//{
//    public MainConfig()
//    {
//        Add(Job.Default.With(Platform.X64).With(CsProjCoreToolchain.NetCoreApp20));
//        Add(Job.Default.With(Platform.X64).With(CsProjCoreToolchain.NetCoreApp21));

//        Add(MemoryDiagnoser.Default);
//        Add(new MinimalColumnProvider());
//        Add(MemoryDiagnoser.Default.GetColumnProvider());
//        Set(new DefaultOrderProvider(SummaryOrderPolicy.SlowestToFastest));
//        Add(MarkdownExporter.GitHub);
//        Add(new ConsoleLogger());
//    }

//    private sealed class MinimalColumnProvider : IColumnProvider
//    {
//        public IEnumerable<IColumn> GetColumns(Summary summary)
//        {
//            yield return TargetMethodColumn.Method;
//            yield return new JobCharacteristicColumn(InfrastructureMode.ToolchainCharacteristic);
//            yield return StatisticColumn.Mean;
//        }
//    }
//}


[ClrJob(isBaseline: true), CoreJob, MonoJob, DryCoreJob]
[MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
public class BenchmarkTests
{
  
    private static int[] s_intArray = Enumerable.Range(0, 100_000).ToArray();

    private static MyStruct[] m_Elements;

    [GlobalSetup]
    public void BenchmarkTestsSetup()
    {
        int num = 1000;

        m_Elements = new MyStruct[num];

        for (int i = 0; i < num; i++)
        {
            m_Elements[i] = new MyStruct() { X = i, Y = i * i };
        }
    }


    #region Time

    [Benchmark]
    public void Time50() => Thread.Sleep(50);

    [Benchmark(Baseline = true)]
    public void Time100() => Thread.Sleep(100);

    [Benchmark]
    public void Time150() => Thread.Sleep(150);
    #endregion

    [Benchmark]
    public int EqualityComparerInt32New()
    {
        int[] items = s_intArray;

        for (int i = 0; i < items.Length; i++)
            if (EqualityComparer<int>.Default.Equals(items[i], -1))
                return i;

        return -1;
    }



    #region RefTypes
    /// <summary>
    /// Size of struct is 16 bytes.
    /// sizeof(MyStruct) = 2 * sizeof(double) = 16.
    /// </summary>
    public struct MyStruct
    {
        public double X;

        public double Y;

        public double Z;

    }

   
    MyStruct sStruct = new MyStruct() { X = 1.1, Y = 2.2 };

    int m_Cycles = 10000;

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void AddByType()
    {
        for (int i = 0; i < m_Cycles; i++)
        {
            add_by_type(sStruct);
        }
    }


    [Benchmark(OperationsPerInvoke = 10_000)]
    public void AddByInType()
    {
        for (int i = 0; i < m_Cycles; i++)
        {
            add_by_in_type(in sStruct);
        }
    }


    [Benchmark(OperationsPerInvoke = 10_000)]
    public void AddByRefType()
    {
        for (int i = 0; i < m_Cycles; i++)
        {
            add_by_ref_type(ref sStruct);
        }
    }


    public double add_by_type(MyStruct s)
    {
        return s.X + s.Y;
    }

    public double add_by_in_type(in MyStruct s)
    {
        return s.X + s.Y;
    }

    public double add_by_ref_type(ref MyStruct s)
    {
        return s.X + s.Y;
    }
    #endregion

    #region Return_Ref_By_Type



    //[Benchmark(OperationsPerInvoke = 10_000)]
   [Benchmark()]
    public void FindElementByRef()
    {
        for (int i = 0; i < m_Cycles; i++)
        {
            ref var result = ref findElement_by_ref(10);
        }
    }

    private ref MyStruct findElement_by_ref(double x)
    {
        ref MyStruct localRet = ref m_Elements[0];

        var indx = m_Elements.Length - 1;

        while ((indx > 0) && m_Elements[indx].X == x)
        {
            localRet = ref m_Elements[indx];
            indx--;
        }

        return ref localRet;
    }


    //[Benchmark(OperationsPerInvoke = 10_000)]
    [Benchmark()]
    public void FindElementByValue()
    {
        for (int i = 0; i < m_Cycles; i++)
        {
            var result = findElement_by_value(10);
        }
    }

    private MyStruct findElement_by_value(double x)
    {
        MyStruct localRet = m_Elements[0];

        var indx = m_Elements.Length - 1;

        while ((indx > 0) && m_Elements[indx].X == x)
        {
            localRet = m_Elements[indx];
            indx--;
        }

        return localRet;
    }
 
    #endregion
    
}