using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace GamepadMapper.Configuration.Parsing
{
    public interface ILineReader
    {
        string Peek();

        void Move();

        string Path { get; }

        int Line { get; }
    }

    public interface IFileReader
    {
        ILineReader ReadFile(string path);
    }

    public interface ITokenStream
    {
        Token[] Peek();

        void Move();

        string Path { get; }

        int Line { get; }
    }

    public interface ILogger
    {
        void WriteLine(string line);
    }

    public class LineReader : ILineReader, IDisposable
    {
        private readonly StreamReader stream;
        private string currentLine;

        public LineReader(string path)
        {
            stream = new StreamReader(path);
            Path = path;
            Move();
        }

        public string Peek() => currentLine;

        public void Move()
        {
            if (stream.EndOfStream)
            {
                currentLine = null;
                return;
            }

            currentLine = stream.ReadLine();
            Line++;
        }

        public string Path { get; }

        public int Line { get; private set; }

        public void Dispose()
        {
            stream.Dispose();
        }
    }

    public class FileReader : IFileReader
    {
        public ILineReader ReadFile(string path)
        {
            return new LineReader(path);
        }
    }

    public class FileLogger : ILogger, IDisposable
    {
        private StreamWriter streamWriter;
        private int lineNumber;

        public FileLogger()
        {
            Path = DateTime.Now.ToString("yyyyddMMHHmmss") + ".log";
        }

        public string Path { get; }

        public void WriteLine(string line)
        {
            if (streamWriter == null)
            {
                streamWriter = new StreamWriter(Path);
            }

            if (lineNumber++ > 10_000)
            {
                // Sometimes the parser can get in an infinite loop.
                // Until that is fixed this will prevent gigantic log files.
                Application.Current?.Shutdown();
                return;
            }

            streamWriter.WriteLine(line);
            streamWriter.Flush();
        }

        public void Dispose() => streamWriter?.Dispose();
    }

    public class ConsoleLogger : ILogger
    {
        public void WriteLine(string line) => Console.WriteLine(line);
    }


    public static class TokenStreamExtensions
    {
        public static void UnexpectedLine(this ILogger logger, ITokenStream stream)
        {
            logger.WriteLine($"Error: Unexpected line at {stream.Path}:{stream.Line}.");
        }

        public static Token[] PeekNonEmpty(this ITokenStream stream)
        {
            while (true)
            {
                var tokens = stream.Peek();
                if (tokens == null)
                {
                    return null;
                }

                if (tokens.Length > 0)
                {
                    return tokens;
                }

                stream.Move();
            }
        }

        public static bool Starts(this ITokenStream stream, string type)
        {
            var row = stream.PeekNonEmpty();
            if (row == null)
            {
                return false;
            }

            if (string.Equals(row[0].Value, type, StringComparison.OrdinalIgnoreCase))
            {
                stream.Move();
                return true;
            }

            return false;
        }

        public static bool Starts(this ITokenStream stream, string type, ILogger logger, out string name)
        {
            var row = stream.PeekNonEmpty();
            if (row == null)
            {
                name = null;
                return false;
            }

            if (string.Equals(row[0].Value, type, StringComparison.OrdinalIgnoreCase))
            {
                if (row.Length > 1)
                {
                    name = row[1].Value;
                }
                else
                {
                    name = null;
                    logger?.WriteLine($"Warning: No item name at {stream.Path}:{stream.Line}. Items without a name may get ignored.");
                }

                stream.Move();
                return true;
            }

            name = null;
            return false;
        }

        public static bool Ends(this ITokenStream stream, string type, ILogger logger, out Token[] row, out string token0)
        {
            row = stream.PeekNonEmpty();
            if (row == null)
            {
                token0 = null;
                return true;
            }

            token0 = row[0].Value.ToLower();
            if (token0 == "end")
            {
                if (row.Length > 1)
                {
                    if (!string.Equals(row[1].Value, type, StringComparison.OrdinalIgnoreCase))
                    {
                        logger?.WriteLine($"Warning: Expected 'end {type}' at {stream.Path}:{stream.Line}.");
                    }
                }

                stream.Move();
                return true;
            }

            return false;
        }
    }
}
