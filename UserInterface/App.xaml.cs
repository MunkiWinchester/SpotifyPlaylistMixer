using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Runtime.InteropServices;

namespace UserInterface
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        #region User32.dll import
        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string cls, string win);
        [DllImport("user32")]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32")]
        private static extern bool OpenIcon(IntPtr hWnd);
        #endregion

        private static readonly string Name = Assembly.GetExecutingAssembly().GetName().Name;
        private static Mutex _appMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            _appMutex = new Mutex(true, Name, out bool isNew);
            if (!isNew)
            {
                ActivateOtherWindow();
                _appMutex = null;
                Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _appMutex?.ReleaseMutex();
            base.OnExit(e);
        }

        private static void ActivateOtherWindow()
        {
            var other = FindWindow(null, Views.MainWindow.WindowTitle);
            if (other != IntPtr.Zero)
            {
                SetForegroundWindow(other);
                if (IsIconic(other))
                    OpenIcon(other);
            }
        }
    }
}