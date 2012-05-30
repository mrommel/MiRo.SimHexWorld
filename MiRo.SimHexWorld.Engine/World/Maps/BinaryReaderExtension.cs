using System.Globalization;
using System.IO;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public static class BinaryReaderExtension
    {
        public static string ReadStringZ(this BinaryReader source)
        {
            return ReadStringZeroTerminated(source);
        }

        /// <summary>
        /// Read an AsciiZ string in from the binary reader
        /// </summary>
        /// <param name="reader">Binary reader instance</param>
        /// <returns>String, null terminator is truncted,
        /// stream reader positioned at byte after null</returns>
        private static string ReadStringZeroTerminated(BinaryReader reader)
        {
            string result = "";
            for (int i = 0; i < reader.BaseStream.Length; i++)
            {
                char c;
                if ((c = (char)reader.ReadByte()) == 0)
                {
                    break;
                }
                result += c.ToString(CultureInfo.InvariantCulture);
            }
            return result;
        }
    }
}