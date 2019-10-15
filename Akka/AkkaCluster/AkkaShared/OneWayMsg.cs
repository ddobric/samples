namespace AkkaShared.Shared
{
    public class StartCalcMsg
    {
        public ulong SumFrom { get; set; }

        public ulong SumTo { get; set; }
    }

    public class LongRunningMsg
    {
        public long Time { get; set; }
    }

    public class SingleLongRunningMsg
    {
        public long Time { get; set; }
    }
}
