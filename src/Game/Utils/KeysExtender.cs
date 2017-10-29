using Microsoft.Xna.Framework.Input;

namespace Land.Utils
{
    public static class KeysExtender
    {
        public static bool IsKeyPressed(this KeyboardState state, KeyboardState oldState, params Keys[] keys)
        {
            bool result = false;
            foreach (Keys key in keys)
            {
                result = state.IsKeyDown(key) && oldState.IsKeyUp(key);
                if (result)
                    break;
            }
            return result;
        }


        public static bool IsKeyDown(this KeyboardState state, params Keys[] keys)
        {
            bool result = false;
            foreach (Keys key in keys)
            {
                result = state.IsKeyDown(key);
                if (result)
                    break;
            }
            return result;
        }
    }
}