#region Using Statements

using System;

#endregion

namespace Land
{
#if WINDOWS || LINUX || XBOX
    /// <summary>
    ///     The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        #if WINDOWS || LINUX
        [STAThread]
        #endif
        private static void Main()
        {
            using (var game = new TheGame())
                game.Run();
        }
    }
#endif
}