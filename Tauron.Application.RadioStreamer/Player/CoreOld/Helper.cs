using System;
using System.Linq;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Player.CoreOld
{
    /// <summary>
    ///     Summary description for Helper.
    /// </summary>
    public static class Helper
    {
        #region Functions

        public static int MakeLong(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xffff);
        }

        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr) ((HiWord << 16) | (LoWord & 0xffff));
        }

        public static int HiWord(int Number)
        {
            return (Number >> 16) & 0xffff;
        }

        public static int LoWord(int Number)
        {
            return Number & 0xffff;
        }

        public static int Bool2Int(bool input)
        {
            int output = 0;
            if (input) output++;
            return output;
        }

        public static bool Int2Bool(int input)
        {
            bool output = input != 0;
            ;
            return output;
        }

        #endregion

        [NotNull]
        public static string PrintFlags([NotNull] object enumvalue)
        {
            Type t = enumvalue.GetType();
            return
                Enum.GetNames(t)
                    .Where(enumName => (((int) enumvalue) & (int) Enum.Parse(t, enumName)) != 0)
                    .Aggregate("", (current, enumName) => current + ("[" + enumName + "]"));
        }
    }
}