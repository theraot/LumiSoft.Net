using System;

namespace LumiSoft.Net.FTP
{
    /// <summary>
    /// This class holds single file or directory in the FTP server.
    /// </summary>
    public class FTP_ListItem
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Directory or file name.</param>
        /// <param name="size">File size in bytes, zero for directory.</param>
        /// <param name="modified">Directory or file last modification time.</param>
        /// <param name="isDir">Specifies if list item is directory or file.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>name</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public FTP_ListItem(string name,long size,DateTime modified,bool isDir)
        {
            if(name == null){
                throw new ArgumentNullException("name");
            }
            if(name == ""){
                throw new ArgumentException("Argument 'name' value must be specified.");
            }

            Name     = name;
            Size     = size;
            Modified = modified;
            IsDir    = isDir;
        }

        /// <summary>
        /// Gets if current item is directory.
        /// </summary>
        public bool IsDir { get; }

        /// <summary>
        /// Gets if current item is file.
        /// </summary>
        public bool IsFile
        {
            get{ return !IsDir; }
        }

        /// <summary>
        /// Gets the name of the file or directory.
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
        /// Gets file size in bytes.
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// Gets last time file or direcory was modified.
        /// </summary>
        public DateTime Modified { get; }
    }
}
