using Land.Classes;

namespace Land
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var game = new TheGame())
            {
                game.Run();
            }
        }
    }
#endif
}

