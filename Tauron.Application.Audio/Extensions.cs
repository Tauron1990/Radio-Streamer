namespace Tauron.Application.BassLib
{
    internal static class Extensions
    {
        public static void CheckBass(this bool value)
        {
            if (value) return;

            throw new BassException();
        }

        public static void CheckBass(this int value)
        {
            if(value <= 0) 
                throw new BassException();
        }
    }
}