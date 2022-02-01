using System;

namespace DataMigration
{
    public class ConsoleProgress : IProgress
    {
        private int _totalPolicyQuotes;
        private int _passed;
        private int _failed;
        private int _totalBatches;
        private int _currentBatch;
        private string _status;

        public ConsoleProgress()
        {
            Update();
        }

        public int TotalPolicyQuotes { get { return _totalPolicyQuotes; } set { _totalPolicyQuotes = value; ClearConsole(); Update(); } }
        public int Passed { get { return _passed; } set { _passed = value; ClearConsole(); Update(); } }
        public int Failed { get { return _failed; } set { _failed = value; ClearConsole(); Update(); } }
        public int TotalBatches { get { return _totalBatches; } set { _totalBatches = value; ClearConsole(); Update(); } }
        public int CurrentBatch { get { return _currentBatch; } set { _currentBatch = value; ClearConsole(); Update(); } }
        public string Status { get { return _status; } set { _status = value; UpdateStatus(); } }

        public string ProcessStart { get; set; }
        public string ProcessEnd { get; set; }

        private void Update()
        {
            Console.WriteLine($"Total Policy/Quote Count : {TotalPolicyQuotes}");
            Console.WriteLine($"Passed                   : {Passed}");
            Console.WriteLine($"Failed                   : {Failed}");
            Console.WriteLine($"Processing Batch         : {CurrentBatch} of {TotalBatches}");
            Console.WriteLine($"Status                   : {Status}");
        }

        private void UpdateStatus()
        {
            if (!Console.IsOutputRedirected)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearCurrentConsoleLine();
            }
            Console.WriteLine($"Status                   : {Status}");
        }

        private static void ClearCurrentConsoleLine()
        {
            if (!Console.IsOutputRedirected)
            {
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, currentLineCursor);
            }
        }

        private static void ClearConsole()
        {
            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
            }
        }

    }
}
