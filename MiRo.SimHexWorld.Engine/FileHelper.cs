using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace MiRo.SimHexWorld.Engine
{
    public static class FileHelper
    {
        #region CreateGameContentFile
        /// <summary>
        /// Create game content file, will create file if it does not exist.
        /// Else the existing file should be loaded.
        /// </summary>
        /// <param name="relativeFilename">Relative filename.</param>
        /// <param name="createNew">Create new file.</param>
        /// <returns>FileStream</returns>
        //public static FileStream CreateGameContentFile(string relativeFilename, bool createNew)
        //{
        //    string fullPath = Path.Combine( StorageContainer.TitleLocation, relativeFilename);
        //    return File.Open(fullPath,
        //        createNew ? FileMode.Create : FileMode.OpenOrCreate,
        //        FileAccess.Write, FileShare.ReadWrite);
        //}
        #endregion

        #region LoadGameContentFile
        /// <summary>
        /// Load game content file, returns null if file was not found.
        /// </summary>
        /// <param name="relativeFilename">Relative filename.</param>
        /// <returns>FileStream</returns>
        public static FileStream LoadGameContentFile(string relativeFilename)
        {
            string fullPath = Path.Combine(Application.StartupPath, relativeFilename);
            if (File.Exists(fullPath) == false)
                return null;
            else
                return File.Open(fullPath,
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        #endregion

        #region SaveGameContentFile
        /// <summary>
        /// Save game content file, returns the open file.
        /// </summary>
        /// <param name="relativeFilename">Relative filename.</param>
        /// <returns>FileStream</returns>
        public static FileStream SaveGameContentFile(string relativeFilename)
        {
            string fullPath = Path.Combine(Application.StartupPath, relativeFilename);
            return File.Open(fullPath,
                FileMode.Create, FileAccess.Write);
        }
        #endregion

        #region OpenOrCreateFileForCurrentPlayer

        /// <summary>
        /// Open or create file for current player.  Basically just creates a
        /// FileStream using the specified FileMode flag, but on the Xbox360
        /// we have to ask the user first where he wants to.
        /// Basically used for the GameSettings and the Log class.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="mode">Mode</param>
        /// <param name="access">Access</param>
        /// <returns>FileStream</returns>
        //public static FileStream OpenFileForCurrentPlayer(string filename, FileMode mode, FileAccess access)
        //{
        //    // Open a storage container.
        //    StorageContainer container = null;

        //    // Add the container path to our filename.
        //    string fullFileName = Path.Combine(container.Path, filename);

        //    // Opens or creates the requested file.
        //    return new FileStream(
        //        fullFileName, mode, access, FileShare.ReadWrite);
        //}
        #endregion

        #region Get text lines
        /// <summary>
        /// Returns the number of text lines we have in the file.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <returns>Array of strings.</returns>
        static public string[] GetLines(string filename)
        {
            try
            {
                StreamReader reader = new StreamReader(
                    new FileStream(filename, FileMode.Open, FileAccess.Read),
                    System.Text.Encoding.UTF8);
                // Generic version
                List<string> lines = new List<string>();
                do
                {
                    lines.Add(reader.ReadLine());
                } while (reader.Peek() > -1);
                reader.Close();
                return lines.ToArray();
            }
            catch (FileNotFoundException)
            {
                // Failed to find a file.
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                // Failed to find a directory.
                return null;
            }
            catch (IOException)
            {
                // Something else must have happened.
                return null;
            }
        }
        #endregion

        #region Write Helpers
        /// <summary>
        /// Write Vector3 to stream.
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="vector">Vector3</param>
        public static void WriteVector3(BinaryWriter writer, Vector3 vector)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }

        /// <summary>
        /// Write Vector4 to stream.
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="vec">Vector4</param>
        public static void WriteVector4(BinaryWriter writer, Vector4 vector)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
            writer.Write(vector.W);
        }

        /// <summary>
        /// Write Matrix to stream.
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="matrix">Matrix</param>
        public static void WriteMatrix(BinaryWriter writer, Matrix matrix)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(matrix.M11);
            writer.Write(matrix.M12);
            writer.Write(matrix.M13);
            writer.Write(matrix.M14);
            writer.Write(matrix.M21);
            writer.Write(matrix.M22);
            writer.Write(matrix.M23);
            writer.Write(matrix.M24);
            writer.Write(matrix.M31);
            writer.Write(matrix.M32);
            writer.Write(matrix.M33);
            writer.Write(matrix.M34);
            writer.Write(matrix.M41);
            writer.Write(matrix.M42);
            writer.Write(matrix.M43);
            writer.Write(matrix.M44);
        }
        #endregion

        #region Read Helpers
        /// <summary>
        /// Read Vector3 from stream.
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Vector3</returns>
        public static Vector3 ReadVector3(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }

        /// <summary>
        /// Read Vector4 from stream.
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Vector4</returns>
        public static Vector4 ReadVector4(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Vector4(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }

        /// <summary>
        /// Read Matrix from stream.
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Matrix</returns>
        public static Matrix ReadMatrix(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Matrix(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }
        #endregion
    }
}
