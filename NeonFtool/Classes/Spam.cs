using NeonFtool.Libraries;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeonFtool.Classes
{
    internal class Spam
    {
        private readonly Process _process;
        private readonly int _fKey;
        private readonly int _skill;
        private readonly int _interval;

        private CancellationTokenSource _cts;

        public Spam(Process process, int interval, int fKey, int skill)
        {
            _process  = process;
            _interval = Math.Max(interval, 100); // enforce minimum 100 ms
            _fKey     = fKey;
            _skill    = skill;
        }

        public void Start(Button button)
        {
            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            Task.Run(() => Execute(token), token);

            _process.EnableRaisingEvents = true;
            _process.Exited += (_, _) =>
            {
                if (!token.IsCancellationRequested && button.Text == "Stop")
                {
                    button.Invoke((MethodInvoker)button.PerformClick);
                }
            };
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private void Execute(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_fKey != -1)
                        Function.PostMessage(_process.MainWindowHandle, Constants.WM_KEYDOWN, _fKey, 0);

                    if (_skill != -1)
                        Function.PostMessage(_process.MainWindowHandle, Constants.WM_KEYDOWN, _skill, 0);

                    Task.Delay(_interval, token).GetAwaiter().GetResult();
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation path — nothing to do.
            }
        }
    }
}
