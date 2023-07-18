using Block0.Threading.Pipe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block0.Threading.Worker
{
    public enum WorkerJobID : byte
    {
        None = 0,
        Anonymous = 1,

        Named = 50,
        Log,
        OutputNetwork,
        InputNetwork,
        UserNamed = 100,
    }

    public class ThreadLocal
    {
        public static ThreadLocal<MsgWorkerJob> WorkerJob { get; set; } = new ThreadLocal<MsgWorkerJob>();
    }

    public abstract class WorkerJob
    {
        //0表示 没有初始ID
        public byte JobID { get; internal set; }
        
        public abstract int ExecutePriority { get; }
        public abstract float EstimatedTimeCost { get; } //ms

        public const float ExpectedTimeCostPerWorker = 10f;//10ms

        public volatile Worker CurrentWorker;

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {

        }
        public abstract void Execute();

    }

    public interface IUniqueJobID
    {
        byte UniqueID { get;}
    }


    //未受管理的任务 需要自己执行init execute函数
    public interface IUnManagedJob
    {

    }
}
