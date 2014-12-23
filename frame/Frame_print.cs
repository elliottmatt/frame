using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framespace
{
    partial class Frame
    {
        private void CalculateBuffer(int max, int len, out int left, out int right)
        {
            if (PrintAlignment == PrintAlignmentType.Left)
            {
                left = 0;
                right = max - len;
            }
            else if (PrintAlignment == PrintAlignmentType.Right)
            {
                left = max - len;
                right = 0;
            }
            else
            {
                // for the extra odd # to be on the left side:                
                right = (max - len) / 2;
                left = max - len - right;
            }
        }

        private void PrintField(string s, int max, bool isHeader)
        {
            PrintField(s, max, isHeader, WhitespaceLetter, WhitespaceLetter);
        }

        private void PrintField(string s, int max, bool isHeader, char padding, char extrachar)
        {
            int leftlen;
            int rightlen;

            if (s == null)
            {
                s = "";
                padding = EmptyFieldDelimiter;
                extrachar = EmptyFieldDelimiter;
            }
            CalculateBuffer(max, s.Length, out leftlen, out rightlen);

            if (PrintFieldGutters) { OutStream.Write(extrachar); }

            OutStream.Write(new string(padding, leftlen));
            OutStream.Write(s);
            OutStream.Write(new string(padding, rightlen));

            if (PrintFieldGutters) { OutStream.Write(extrachar); }
        }

        /// <summary>
        /// Header consists of:
        /// 1) |---------------------| <-- hyphen line
        /// 2) |    |    | 5     | 1 | <-- (optional) max width of field
        /// 3) | LN | FC | 1     | 2 | <-- field number
        /// 4) |    |    | line3 | c | <-- (optional) header field
        /// 5) |-----------------+---| <-- hyphen line
        ///     (a) optional line number
        ///           (b) optional field count for each row
        /// </summary>
        private void PrintHeader()
        {
            // (1)
            // print top line
            PrintHyphenLine(false);

            // (2)
            if (PrintFieldMaxWidth)
            {
                PrintLineNumberField("FS");
                PrintFieldCountField("");

                for (int i = 0; i < MaxFieldCount; ++i)
                {
                    OutStream.Write("|");
                    PrintField(MaxFieldLengths[i].ToString(), MaxFieldLengths[i], true);
                }
                OutStream.Write("|" + System.Environment.NewLine);
            }

            // (3)
            {
                PrintLineNumberField("LN");
                PrintFieldCountField("FC");

                // print field numbers
                for (int i = 0; i < MaxFieldCount; ++i)
                {
                    OutStream.Write('|');
                    PrintField((FieldOffset + i).ToString(), MaxFieldLengths[i], true);
                }
                OutStream.Write("|" + System.Environment.NewLine);
            }

            // (4) print headers (if we need them)
            if (Header == PrintHeaderType.PrintRow1 || Header == PrintHeaderType.PrintExternFile)
            {
                string[] fields = ParseClass.ParseLine(HeaderLine, TrimFields, Delimiter, PreserveQuotes);

                PrintLineNumberField("");
                PrintFieldCountField(fields.Length.ToString());

                for (int i = 0; i < MaxFieldCount; ++i)
                {
                    OutStream.Write("|");
                    if (i >= fields.Length)
                        PrintField(null, MaxFieldLengths[i], true);
                    else
                        PrintField(fields[i], MaxFieldLengths[i], true);
                }
                OutStream.Write("|" + System.Environment.NewLine);
            }

            // (5) print plus line between header & body
            PrintHyphenLine(true);
        }

        private void PrintLineNumberField(string s)
        {
            if (PrintLineNumber)
            {
                OutStream.Write('|');
                PrintField(s, LengthOfMaxLineNumber, true);
            }
        }

        private void PrintFieldCountField(string s)
        {
            if (PrintFieldCount)
            {
                OutStream.Write('|');
                PrintField(s, LengthOfMaxFieldCount, true);
            }
        }

        private void PrintBody()
        {
            for (int rowcount = 0; rowcount < RawFile.Count; rowcount++)
            {
                string line = RawFile[rowcount];
                string[] fields = ParseClass.ParseLine(line, TrimFields, Delimiter, PreserveQuotes);

                PrintLineNumberField((StartSkipRows + rowcount + 1).ToString());
                PrintFieldCountField(fields.Length.ToString());

                for (int i = 0; i < MaxFieldCount; ++i)
                {
                    OutStream.Write("|");
                    if (i >= fields.Length)
                        PrintField(null, MaxFieldLengths[i], true);
                    else
                        PrintField(fields[i], MaxFieldLengths[i], true);
                }
                OutStream.Write("|" + System.Environment.NewLine);
            }
        }

        private void PrintHyphenLine(bool includePlus)
        {
            char hyphen = '-';
            char openingchar = '-';
            char fielddivider = (includePlus ? '+' : '-');
            if (PrintLineNumber)
            {
                OutStream.Write(openingchar);
                openingchar = fielddivider;
                PrintField("", LengthOfMaxLineNumber, true, hyphen, hyphen);
            }
            if (PrintFieldCount)
            {
                OutStream.Write(openingchar);
                openingchar = fielddivider;
                PrintField("", LengthOfMaxFieldCount, true, hyphen, hyphen);
            }
            for (int i = 0; i < MaxFieldCount; ++i)
            {
                OutStream.Write((i == 0) ? openingchar : fielddivider);
                PrintField("", MaxFieldLengths[i], true, hyphen, hyphen);
            }
            OutStream.Write("|" + System.Environment.NewLine);
        }

        private int PrintOutFrame()
        {
            PrintHeader();
            PrintBody();
            PrintHyphenLine(false);
            return 0;
        }
    }
}
