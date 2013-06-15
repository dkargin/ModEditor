using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ModEditor
{
    public class Tools
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        /// 
        public static object Clone(object source, System.Type type)
        {
            /*
            if (!type.IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }*/

            IFormatter formatter = new BinaryFormatter();
            XmlSerializer serializer = new XmlSerializer(type);
            Stream stream = new MemoryStream();
            using (stream)
            {
                serializer.Serialize(stream, source);
                //formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                //return formatter.Deserialize(stream);
                return serializer.Deserialize(stream);
            }
        }

        public static T Clone<T>(T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            return (T)Clone(source, typeof(T));
        }
        // Writes data to temporary file
        public class SafeWriter
        {
            FileStream stream;

            public FileStream Stream
            {
                get
                {
                    return stream;
                }
            }

            //public FI.OpenRead();
            FileInfo tmpFileInfo;
            FileInfo fileInfo;

            public SafeWriter(FileInfo info)
            {
                this.fileInfo = info;
                tmpFileInfo = new FileInfo(info.FullName + ".tmp");
                try
                {
                    tmpFileInfo.Delete();
                }
                catch (Exception)
                {
                }
                stream = tmpFileInfo.OpenWrite();
            }
            // close stream and copy data from temporary file, deleting temporary file
            public void Finish()
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    tmpFileInfo.CopyTo(fileInfo.FullName, true);
                    tmpFileInfo.Delete();
                }
            }
        }

        public static FileInfo[] GetFilesFromDirectory(string DirPath)
        {
            DirectoryInfo Dir = new DirectoryInfo(DirPath);
            FileInfo[] result;
            try
            {
                FileInfo[] FileList = Dir.GetFiles("*.*", SearchOption.AllDirectories);

                result = FileList;
            }
            catch
            {
                result = new FileInfo[0];
            }
            return result;
        }

        public static DirectoryInfo[] GetDirectoriesFromDirectory(string DirPath)
        {
            DirectoryInfo Dir = new DirectoryInfo(DirPath);
            DirectoryInfo[] result;
            try
            {
                DirectoryInfo[] DirList = Dir.GetDirectories();//("*.*", SearchOption.AllDirectories);

                result = DirList;
            }
            catch
            {
                result = new DirectoryInfo[0];
            }
            return result;
        }

        private const int TVIF_STATE = 0x8;
        private const int TVIS_STATEIMAGEMASK = 0xF000;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETITEM = TV_FIRST + 63;

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
        private struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam,
                                                 ref TVITEM lParam);

        /// <summary>
        /// Hides the checkbox for the specified node on a TreeView control.
        /// </summary>
        public static void HideCheckBox(TreeNode node)
        {
            TreeView tvw = node.TreeView;
            if (tvw == null)
                return;
            TVITEM tvi = new TVITEM();
            tvi.hItem = node.Handle;
            tvi.mask = TVIF_STATE;
            tvi.stateMask = TVIS_STATEIMAGEMASK;
            tvi.state = 0;
            SendMessage(tvw.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);
        }

        static public string EmbedToString<Type>(string source, Type obj)
        {
            Regex rx = new Regex("\\$\\{(\\w+)\\.(\\w+)\\}");

            var TypeInfo = typeof(Type);

            /// for each match calls lambda to evaluate replacement string
            string result = rx.Replace(source, (Match match) =>
            {
                string className = match.Groups[1].Value;
                string fieldName = match.Groups[2].Value;
                if (className.Equals(TypeInfo.Name))
                {
                    try
                    {
                        /// Obtain string value from specified field
                        return TypeInfo.GetField(fieldName).GetValue(obj).ToString();
                    }
                    catch (Exception)
                    {
                        /// Cannot access <fieldName>
                    }
                }
                else
                {
                    /// Do not do anything with data from other class
                }
                return match.ToString();
            });

            return result;
        }
    }
}
