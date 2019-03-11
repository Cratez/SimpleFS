/* 
 * John Harrison
 * CST352 Assignment 3
 */

namespace FileSystemCSharp
{
    static class Constants
    {
        public static readonly int MAX_FILE_NAME = 12;
        public static readonly char PATH_SEPARATOR = '/';

        public static readonly int DRIVE_INFO_SECTOR = 0;
        public static readonly int ROOT_DIR_SECTOR = 1;
        public static readonly int ROOT_DATA_SECTOR = 2;

        public static readonly int FILE_SIZE_AT = 17;

        //sector
        public static readonly int TYPE_AT = 0;
        public static readonly int NEXT_SECTOR_AT = 1;
        public static readonly int SECTOR_DATA_LEN = 5;

        //node
        public static readonly int NODE_DATA_LEN = 17;
        public static readonly int NAME_AT = 5;

        public static readonly int ENTRY_COUNT_AT = 17;
    }
}
