using NeonFtool.Libraries;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace NeonFtool.Classes
{
    internal class Spam
    {
        private Process process;
        private int fKey;
        private int skill;
        private int interval;
        private Thread thread;

        public Spam(Process process, int interval, int fKey, int skill)
        {
            this.process = process;
            this.interval = interval < 100 ? 100 : interval;
            this.fKey = fKey;
            this.skill = skill;
        }

        public void Start(Button button)
        {
            thread = new Thread(Execute);
            thread.Start();

            process.EnableRaisingEvents = true;
            process.Exited += (object sender, System.EventArgs e) =>
            {
                if (thread.IsAlive && button.Text == "Stop")
                {
                    button.Invoke((MethodInvoker)delegate ()
                    {
                        button.PerformClick();
                    });
                }
            };
        }

        public void Stop()
        {
            thread.Interrupt();
        }

        private void Execute()
        {
            try
            {
                while (true)
                {
                    if (fKey != -1)
                    {
                        Function.PostMessage(process.MainWindowHandle, Constants.WM_KEYDOWN, fKey, 0);
                    }
                    if (skill != -1)
                    {
                        Function.PostMessage(process.MainWindowHandle, Constants.WM_KEYDOWN, skill, 0);
                    }

                    Thread.Sleep(interval);
                }
            }
            catch (ThreadInterruptedException)
            { }
        }
    }
}
