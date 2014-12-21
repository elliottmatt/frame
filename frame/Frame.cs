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
        private int SaveFieldCount(string line)
        {
            string[] fields = ParseClass.ParseLine(line, TrimFields, Delimiter, PreserveQuotes);
            if (fields.Length > MaxFieldCount)
                MaxFieldCount = fields.Length;

            SaveFieldLengths(fields);            

            return 0;
        }

        private void SaveFieldLengths(string[] fields)
        {
            for (int i = 0; i < fields.Length; ++i)
            {
                if (!MaxFieldLengths.ContainsKey(i))
                {
                    MaxFieldLengths[i] = fields[i].Length;
                }
                else
                {
                    if (fields[i].Length > MaxFieldLengths[i])
                        MaxFieldLengths[i] = fields[i].Length;
                }
            }
        }

        private int GetFileStats()
        {
            TextReader instream = null;
            try
            {
                instream = (ReadStdin ? Console.In : new StreamReader(FilenameIn));

                bool firstZero = true;
                for (int row = 0; ; row++)
                {
                    bool saveLine = true;
                    bool processLine = true;
                    string line = instream.ReadLine();
                    if (line == null)
                        break;

                    if (firstZero && row == 0 && Header == PrintHeaderType.PrintRow1)
                    {
                        row = -1;
                        HeaderLine = line;
                        saveLine = false;
                        // still process it
                        firstZero = false;
                    }

                    // if we still want to save it, it's not a header
                    if (saveLine && SkipRows > 0)
                    {
                        SkipRows--;
                        saveLine = false;
                        processLine = false;
                    }

                    if (processLine)
                    {
                        SaveFieldCount(line);
                    }
                        
                    if (saveLine)
                    {
                        MaxReadLine++;
                        if (!UseTwoPassMethod)
                        {
                            RawFile.Add(line);
                        }
                        if (LineCountToPrint > 0)
                        {
                            LineCountToPrint--;
                            if (LineCountToPrint <= 0)
                            {
                                break;
                            }
                        }
                    }
                }

                if (Header == PrintHeaderType.PrintExternFile)
                {
                    try
                    {
                        using (StreamReader externHeader = new StreamReader(FilenameHeader))
                        {
                            HeaderLine = externHeader.ReadLine();
                            if (HeaderLine == null)
                            {
                                Console.Error.WriteLine("Error! Unable to read a line from {0}", FilenameHeader);
                                return 1;
                            }
                            SaveFieldCount(HeaderLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Verbose) { Console.Error.WriteLine(ex.ToString()); }
                        Console.Error.WriteLine("Error! Unable to read a line from {0}", FilenameHeader);
                        return 1;
                    }
                }

                // make sure if any of the fields are less than the field # + offset,
                // then they are defined as that length
                for (int i = 0; i < MaxFieldLengths.Count; ++i)
                {
                    int min = (FieldOffset + i + 1).ToString().Length;
                    if (MaxFieldLengths[i] < min)
                        MaxFieldLengths[i] = min;
                }

                return 0;
            }
            finally
            {
                if (instream != null && instream != Console.In)
                {
                    try { instream.Close(); } catch { }
                    try { instream.Dispose(); } catch { }
                }
            }
        }

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
                OutStream.Write("|");
                OutStream.Write(System.Environment.NewLine);
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
                OutStream.Write("|");
                OutStream.Write(System.Environment.NewLine);
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
                OutStream.Write("|");
                OutStream.Write(System.Environment.NewLine);
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
                OutStream.Write("|");
                OutStream.Write(System.Environment.NewLine);
            }
        }

        private void PrintHyphenLine(bool includePlus)
        {
            char openingchar = '-';
            char fielddivider = (includePlus ? '+' : '-');
            if (PrintLineNumber)
            {
                OutStream.Write(openingchar);
                openingchar = fielddivider;
                PrintField("", LengthOfMaxLineNumber, true, '-', '-');
            }
            if (PrintFieldCount)
            {
                OutStream.Write(openingchar);
                openingchar = fielddivider;
                PrintField("", LengthOfMaxFieldCount, true, '-', '-');
            }
            for (int i = 0; i < MaxFieldCount; ++i)
            {
                if (i == 0) { OutStream.Write(openingchar); }
                if (i > 0) { OutStream.Write(fielddivider); }
                PrintField("", MaxFieldLengths[i], true, '-', '-');
            }
            OutStream.Write("-");
            OutStream.Write(System.Environment.NewLine);
        }

        private int PrintOutFrame()
        {
            PrintHeader();

            PrintBody();

            PrintHyphenLine(false);

            return 0;
        }

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
