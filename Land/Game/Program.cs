namespace Land
{
#if WINDOWS || XBOX
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            using (var game = new TheGame())
            {
                game.Run();
            }
        }
    }
#endif
}