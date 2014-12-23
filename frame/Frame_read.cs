using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace framespace
{
    partial class Frame
    {
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

                int ret = ReadExternHeader();
                if (ret > 0) { return ret; }

                VerifyFieldLengths();               

                return 0;
            }
            finally
            {
                if (instream != null && instream != Console.In)
                {
                    try { instream.Close(); }
                    catch { }
                    try { instream.Dispose(); }
                    catch { }
                }
            }
        }

        private int ReadExternHeader()
        {
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

            return 0;
        }

        private void VerifyFieldLengths()
        {
            // make sure if any of the fields are less than the field # + offset,
            // then they are defined as that length
            for (int i = 0; i < MaxFieldLengths.Count; ++i)
            {
                int min = (FieldOffset + i).ToString().Length;
                if (MaxFieldLengths[i] < min) { MaxFieldLengths[i] = min; }
            }
        }

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


    }
}
