using System;
using CopyBud.Win32;

namespace CopyBud.Mutex
{
    //Taken from https://www.codeproject.com/Articles/32908/C-Single-Instance-App-With-the-Ability-To-Restore
    public static class SingleInstance
    {
        public static readonly int WM_SHOWFIRSTINSTANCE =
            User32Wrapper.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", ProgramInfo.AssemblyGuid);
        static System.Threading.Mutex _mutex;
        public static bool Start()
        {
            bool onlyInstance;
            var mutexName = $"Local\\{ProgramInfo.AssemblyGuid}";

            // if you want your app to be limited to a single instance
            // across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
            // string mutexName = String.Format("Global\\{0}", ProgramInfo.AssemblyGuid);

            _mutex = new System.Threading.Mutex(true, mutexName, out onlyInstance);
            return onlyInstance;
        }
        public static void ShowFirstInstance()
        {
            User32Wrapper.PostMessage(
                (IntPtr)User32Wrapper.HWND_BROADCAST,
                WM_SHOWFIRSTINSTANCE,
                IntPtr.Zero,
                IntPtr.Zero);
        }
        public static void Stop()
        {
            _mutex.ReleaseMutex();
        }
    }
}
