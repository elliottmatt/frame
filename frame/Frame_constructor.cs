using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framespace
{
    public partial class Frame
    {
        public Frame()
        {
            FilenameIn = null;
            WriteStdout = true;
            OutStream = null;

            PrintLineNumber = false;
            PrintFieldCount = false;
            PrintFieldMaxWidth = false;

            Header = PrintHeaderType.PrintFieldNum;
            TrimFields = false;
            PrintFieldGutters = true;
            LineCountToPrint = 10;

            SkipRows = 0;
            StartSkipRows = 0;
            UseTwoPassMethod = false;
            Delimiter = '|';
            EmptyFieldDelimiter = '#';

            FieldOffset = 1;
            PreserveQuotes = false;

            MaxFieldCount = -1;
            MaxReadLine = 0;
            WhitespaceLetter = ' ';

            Verbose = false;
        }
    }
}
