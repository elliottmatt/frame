using System;

namespace framespace
{
    class Program
    {
        static int Main(string[] args)
        {
            Frame f = null;
            try
            {
                if (args.Length > 0 && (args[0] == "--help" || args[0] == "/?"))
                {
                    Console.WriteLine(Helpfile.Trim());
                    return 1;
                }
                f = new Frame();
                int n = f.ParseArgs(args);
                if (n > 0)
                    return n;

                return f.Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Fatal error!");
                if (f == null || f.Verbose) { Console.Error.WriteLine(ex.ToString()); }
                else { Console.Error.WriteLine("Run in verbose mode for more details!"); }
                return 2;
            }
        }

        static string Helpfile = @"
frame by Matt Elliott (July 2004) rewrite in C# (December 2014)

usage: frame <file> [options]
Prints out the file in a ""pretty"" format that allows for easy viewing.

Alignment:
   -l    Left (default)
   -r    Right
   -c    Center

Details:
   -n    Print line number every line
   -f    Print number of fields per line
   -m    Print maximum width of each field for print out.
         This includes the header row so it may be slightly skewed.

Addtional Options:
   -h [?] Print a row as header (instead of field number) (no space)
          Optional: Provide a filename and it will use the first line as the header
           -h              Will use first row of current file as header row
           -h ""c:\a.a""     Will use first row of ""c:\a.a""
           -h header.txt   Will use first row of ""header.txt""

   -t     Trim leading and trailing white space from cell before determining length

   -q     Remove extra 2 spaces from print out. Converts | a | to |a|

   -N ?   Print only ? number of lines from file (no space)
           -N10    Prints 10 lines (default)
           -N100   Prints 100 lines
           -N5000  Prints 5000 lines
           -N-1    Prints all lines
                   Note: (-ALL) prints all lines also

   -s ?   Skip x number of lines (no space)
           -s1     Skips the first row (good for headers)
           -s5000  Skips 5000 lines

   -2     Make 2-passes through file
          By default it loads everything into memory during the first pass.
          (This is useful for reading LOTS of lines.)

   -d ?   Use '?' as delimiter
           -d ""|""   Uses '|' (default)
           -d ;     Uses ';'
           -d \t    Uses tab
           -d tab   Uses tab
           -d "" ""   Uses space
           -d 0x??  Uses the hex value of ?? (ie 0x20 = space)

   -e [?] Use ""?"" as empty char signifier (Optional ""?"")
           -e#     Uses '#' (default)
           -e      Uses a space (No ? written)
           -e0x??  Uses the hex value of ?? (ie 0x41 = ""A"")

   -o ?   Use '?' as field count offset
           -o0     Starts field count zero-based (default)
           -o1     Starts field count 1-based

   -Q     Flag preserves quotes while parsing (using BuildArgVB()) (Added 2005-02-03)

   -w [?] Show the added whitespace. By default shows using * but optional char.

   -y     Dynamically grow the size of the fields, but do not shrink them.

I\O:
   -F [?] Will write the output to ""input.frame"" if no filename is given.
          Optional: Place the filename of where the output should be written.
           -F
           -F out.txt
           -F ""c:\out.txt""

Known Issues:
        - Tabs will destroy the lining up of the data because
          they are only one char but print out differently.
        - When reading stdin, if ""-N-1"" is given for all lines,
          only 1,000 lines will be read. Must use -N# for more from stdin
        - There is a numbering issue with (-s) and (-h) (with no external file). TODO CHECK THIS
          I doubt I'm going to fix this.
";
    }
}
