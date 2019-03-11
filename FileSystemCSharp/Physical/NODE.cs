/* 
 * John Harrison
 * CST352 Assignment 3
 */

using System;
using System.Text;
using System.Linq;

namespace FileSystemCSharp
{
    abstract class NODE : SECTOR
    {
        private string mName;

        public int FirstDataAt => NextSectorAt;
        public string Name
        {
            get { return mName; }
            set
            {
                if (value.Length > Constants.MAX_FILE_NAME)
                {
                    throw new Exception("Name too long!");
                }
                mName = value;
                Encoding.ASCII.GetBytes(mName).CopyTo(mRaw, Constants.NAME_AT);
            }
        }

        protected NODE(byte[] raw) : base(raw)
        {
            mName = Encoding.ASCII.GetString(raw, Constants.NAME_AT, Constants.MAX_FILE_NAME).Trim(new char[1]);
        }

        protected NODE(int bytesPerSector, SectorType type, int nextSectorAt, string name) : base(bytesPerSector, type, nextSectorAt)
        {
            if (name.Length > Constants.MAX_FILE_NAME)
            {
                throw new Exception($"Name {name} too long!");
            }
            mName = name;
            Encoding.ASCII.GetBytes(name).CopyTo(mRaw, Constants.NAME_AT);
        }
    
        public static NODE CreateFromBytes(byte[] raw)
        {
            SectorType type = GetTypeFromBytes(raw);
            NODE result = null;

            if(type == SectorType.DIR_NODE)
            {
                result = DIR_NODE.CreateFromBytes(raw);
            }
            else if( type == SectorType.FILE_NODE)
            {
                result = FILE_NODE.CreateFromBytes(raw);
            }
            else
            {
                throw new Exception("Expected a DIR_NODE or FILE_NODE!");
            }
            return result;
        }
    }
}
