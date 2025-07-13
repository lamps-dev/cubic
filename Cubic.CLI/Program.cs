using System.IO;
using System;

namespace CubicCLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // See https://aka.ms/new-console-template for more information
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            String line;
            StreamReader sr = new StreamReader("ascii-art.txt");
            //Read the first line of text
            line = sr.ReadLine();
            //Continue to read until you reach end of file
/* while (line != null)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            Console.WriteLine(line);
        }
        line = sr.ReadLine();
    } */
            while (line != null)
            {
                Console.WriteLine(line.TrimEnd());
                line = sr.ReadLine();
            }
            //close the file
            sr.Close();
            using (var spinner = new Spinner(10, 10))
            {
                spinner.Start();
                if (!System.IO.Directory.Exists("assets"))
                {
                    Console.WriteLine("Cannot continue. Reason: assets folder missing, please recreate it and run the program again.");
                }
                else
                {
                    Thread.Sleep(3000);
                    spinner.Stop();
                }
            }
        }
        public class Spinner : IDisposable
        {
            private const string Sequence = @"/-\|";
            private int counter = 0;
            private readonly int left;
            private readonly int top;
            private readonly int delay;
            private bool active;
            private readonly Thread thread;

            public Spinner(int left, int top, int delay = 100)
            {
                this.left = left;
                this.top = top;
                this.delay = delay;
                thread = new Thread(Spin);
            }

            public void Start()
            {
                active = true;
                if (!thread.IsAlive)
                    thread.Start();
            }

            public void Stop()
            {
                active = false;
                Draw(' ');
            }

            private void Spin()
            {
                while (active)
                {
                    Turn();
                    Thread.Sleep(delay);
                }
            }

            private void Draw(char c)
            {
                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(c);
            }

            private void Turn()
            {
                Draw(Sequence[++counter % Sequence.Length]);
            }

            public void Dispose()
            {
                Stop();
            }
        }
    }
}