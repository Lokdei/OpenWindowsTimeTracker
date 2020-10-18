using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace OpenWindowsTimeTracker
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Tracking active window!");
            var tracker = new Tracker();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public class Tracker
        {
            Timer _timer { get; }
            AutoResetEvent _autoEvent = null;

            public Tracker()
            {
                _autoEvent = new AutoResetEvent(initialState: false);
                _timer = new Timer(Callback, _autoEvent, 1000, 1000);
                Console.Read();
            }

            private void Callback(object stateInfo)
            {
                var caption = GetCaptionOfActiveWindow();
                Console.WriteLine($"{caption.Retrieved.ToShortTimeString()} {caption.Id} - {caption.Title}");
            }

            public ActiveWindow GetCaptionOfActiveWindow()
            {
                var result = new ActiveWindow();

                var handle = GetForegroundWindow();
                result.Id = handle.ToString();
                // Obtain the length of the text   
                var intLength = GetWindowTextLength(handle) + 1;
                var stringBuilder = new StringBuilder(intLength);
                if (GetWindowText(handle, stringBuilder, intLength) > 0)
                {
                    result.Title = stringBuilder.ToString();
                }
                return result;
            }
        }

        public class ActiveWindow
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public DateTime Retrieved { get; } = DateTime.Now;
        }
    }
}
