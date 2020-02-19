using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My.Logging
{
    public abstract class HogeLogEntryState
    { }

    public interface IHogeLogEntryStateUiddHolder
    {
        int Uid { get; }
    }

    public class HogeLogEntryStateConnect : HogeLogEntryState, IHogeLogEntryStateUiddHolder
    {
        public int Uid { get; set; }

        public HogeLogEntryStateConnect() { }

        public HogeLogEntryStateConnect(int uid)
        {
            Uid = uid;
        }
    }

    public class HogeLogEntryStateUnhandledException : HogeLogEntryState
    {
        public int? Uid { get; set; }
        public string? AccessPath { get; set; }
        public int? ConnectId { get; set; }

        public HogeLogEntryStateUnhandledException() { }

        public HogeLogEntryStateUnhandledException(int? uid, string? accessPath, int? connectId)
        {
            Uid = uid;
            AccessPath = accessPath;
            ConnectId = connectId;
        }
    }

    public class HogeLogEntryStateDebug : HogeLogEntryState
    {
        public string Message { get; set; } = default!;

        public HogeLogEntryStateDebug() { }

        public HogeLogEntryStateDebug(string message)
        {
            Message = message;
        }
    }
}
