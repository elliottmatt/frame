using System;
using System.IO;

namespace framespace
{
    public enum PrintAlignmentType
    {
        Left,
        Right,
        Center
    };

    public enum PrintHeaderType
    {
        PrintFieldNum,
        PrintRow1,
        PrintExternFile
    };

    public partial class Frame
    {
		public int Run()
		{
            if (!DynamicFields)
            {
                // read through and get the stats
                int n = GetFileStats();
                if (n > 0) return n;
            }

            try
            {
                OutStream = (WriteStdout ? Console.Out : new StreamWriter(FilenameOut));

                if (UseTwoPassMethod || DynamicFields)
                {
                    throw new Exception("TODO 2 pass");
                }
                else
                {
                    return PrintOutFrame();
                }
            }
            finally
            {
                if (OutStream != null && OutStream != Console.Out)
                {
                    try { OutStream.Close(); } catch { }
                    try { OutStream.Dispose(); } catch { }
                }
            }
		}
    }
}
