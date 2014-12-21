using System;

namespace framespace
{
    partial class Frame
    {
        public int ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; )
            {
                if (!args[i].StartsWith("-"))
                {
                    if (ReadStdin)
                    {
                        FilenameIn = args[i];
                        i++;
                        continue;
                    }
                    else
                    {
                        Console.Error.WriteLine("Expected input of |{0}|. (Did you give two files?)", args[i]);
                        return 1;
                    }
                }

                switch (args[i])
                {
                    case "-l": PrintAlignment = PrintAlignmentType.Left; i++; break;
                    case "-c": PrintAlignment = PrintAlignmentType.Center; i++; break;
                    case "-r": PrintAlignment = PrintAlignmentType.Right; i++; break;
                    case "-n": PrintLineNumber = true; i++; break;
                    case "-f": PrintFieldCount = true; i++; break;
                    case "-m": PrintFieldMaxWidth = true; i++; break;
                    case "-h":
                        {
                            i++;
                            if (i < args.Length && !args[i].StartsWith("-"))
                            {
                                Header = PrintHeaderType.PrintExternFile;
                                FilenameHeader = args[i];
                                i++;
                            }
                            else
                            {
                                Header = PrintHeaderType.PrintRow1;
                            }
                            break;
                        }
                    case "-t": TrimFields = true; i++; break;
                    case "-q": PrintFieldGutters = false; i++; break;
                    case "-N":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected a number after -N.");
                                return 1;
                            }
                            int count;
                            if (!Int32.TryParse(args[i], out count))
                            {
                                Console.Error.WriteLine("Expected a number after -N. Unable to parse to a number.");
                                return 1;
                            }
                            LineCountToPrint = count;
                            i++;
                            break;
                        }
                    case "-s":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected a number after -s.");
                                return 1;
                            }
                            int count;
                            if (!Int32.TryParse(args[i], out count) || count < 0)
                            {
                                Console.Error.WriteLine("Expected a positive number after -s. Unable to parse to a number.");
                                return 1;
                            }
                            StartSkipRows = SkipRows = count;
                            i++;
                            break;
                        }
                    case "-2": UseTwoPassMethod = true; i++; break;
                    case "-d":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected a delimiter after -d.");
                                return 1;
                            }
                            char parsed;
                            int n = ParseClass.ParseStringArg(args[i], out parsed);
                            if (n > 0) return n;
                            Delimiter = parsed;
                            i++;
                            break;
                        }
                    case "-w":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected a delimiter after -w.");
                                return 1;
                            }
                            char parsed;
                            int n = ParseClass.ParseStringArg(args[i], out parsed);
                            if (n > 0) return n;
                            WhitespaceLetter = parsed;
                            i++;
                            break;
                        }
                    case "-e":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected a delimiter after -e.");
                                return 1;
                            }
                            char parsed;
                            int n = ParseClass.ParseStringArg(args[i], out parsed);
                            if (n > 0) return n;
                            EmptyFieldDelimiter = parsed;
                            i++;
                            break;
                        }
                    case "-o":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected a delimiter after -d.");
                                return 1;
                            }
                            int n;
                            int ret = ParseClass.ParseIntArg(args[i], out n);
                            if (ret > 0) return ret;
                            FieldOffset = n;
                            i++;
                            break;
                        }
                    case "-Q": PreserveQuotes = true; i++; break;
                    case "-y": DynamicFields = true; i++; break;
                    case "-F":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected another input after -F.");
                                return 1;
                            }
                            FilenameOut = args[i];
                            WriteStdout = false;
                            i++;
                            break;
                        }
                    case "-v": Verbose = true; i++; break;

                    default:
                        {
                            Console.WriteLine("Expected input of |{0}|.", args[i]);
                            return 1;
                        }
                }
            }

            return 0;
        }
    }
}
