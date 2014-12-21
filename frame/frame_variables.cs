using System;
using System.Collections.Generic;
using System.IO;

namespace framespace
{
    partial class Frame
    {
        PrintAlignmentType PrintAlignment { get; set; }
        string FilenameIn { get; set; }
        string FilenameOut { get; set; } // -F
        bool ReadStdin { get; set; }
        bool WriteStdout { get; set; }
        TextWriter OutStream { get; set; }

        bool PrintLineNumber { get; set; } // -n
        bool PrintFieldCount { get; set; } // -f
        bool PrintFieldMaxWidth { get; set; } // -m

        PrintHeaderType Header { get; set; } // -h
        string FilenameHeader { get; set; }
        string HeaderLine { get; set; }

        bool TrimFields { get; set; } // -t

        bool PrintFieldGutters { get; set; } // -q

        int LineCountToPrint { get; set; }

        int SkipRows { get; set; } // -s -- NOT IMPLEMENTED YET
        int StartSkipRows { get; set; }

        bool UseTwoPassMethod { get; set; } // -2 -- NOT IMPLEMENTED YET

        char Delimiter { get; set; } // -d

        char EmptyFieldDelimiter { get; set; } // -e

        int FieldOffset { get; set; } // -o

        bool PreserveQuotes { get; set; } // -Q

        bool DynamicFields { get; set; } // -y --not implemented

        char WhitespaceLetter { get; set; }

        public bool Verbose { get; set; }

        /// <summary>
        /// Populated unless "two-pass" option (-2) is given
        /// </summary>
        List<string> RawFile = new List<string>();
        int MaxReadLine { get; set; }

        /// <summary>
        /// Key is the fieldnumber, 0 based
        /// Value is the max length of that field
        /// </summary>
        Dictionary<int, int> MaxFieldLengths = new Dictionary<int, int>();
        int MaxFieldCount { get; set; }

        int LengthOfMaxField
        {
            get
            {
                int n = (MaxFieldCount + FieldOffset).ToString().Length;
                return (n < 2 ? 2 : n);
            }
        }

        int LengthOfMaxLineNumber
        {
            get
            {
                int n = MaxReadLine.ToString().Length;
                return (n < 2 ? 2 : n);
            }
        }

        int LengthOfMaxFieldCount
        {
            get
            {
                int n = MaxFieldCount.ToString().Length;
                return (n < 2 ? 2 : n);
            }
        }
    }
}
