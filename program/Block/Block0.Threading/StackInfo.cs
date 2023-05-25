using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block0.Threading
{
    public class StackInfoSection
    {
        public string Name { get; set; }
        public List<StackFrame> stackFrameList = new List<StackFrame>();

        public const int StartFrameIndex = 5;

        public static StackInfoSection New()
        {
            var stackInfo = new StackInfoSection();

            StackTrace stackTrace = new StackTrace(true);


            for (int i = StartFrameIndex; i < stackTrace.FrameCount; i++)
            {
                StackFrame stackFrame = stackTrace.GetFrame(i);
                if(stackFrame.GetFileName()!=null)
                    stackInfo.stackFrameList.Add(stackFrame);
            }
            return stackInfo;
        }
    }

    //出现异常后可以在 Watch窗口 查看WorkerJob.CurretMsg.StackInfo信息
    public class StackInfo
    {
        public List<StackInfoSection> sectionList = new List<StackInfoSection>();

        private void AddStackTrace(string sectName)
        {
            var sect = StackInfoSection.New();
            sect.Name = sectName;
            sectionList.Add(sect);
        }

        public static StackInfo CreateNew(StackInfo old, string sectName)
        {
            StackInfo stackInfo = new StackInfo();
            if(old != null) 
                stackInfo.sectionList.AddRange(old.sectionList);

            stackInfo.AddStackTrace(sectName);
            return stackInfo;
        }

    }

}
