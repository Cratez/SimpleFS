/* 
 * John Harrison
 * CST352 Assignment 3
 */

namespace FileSystemCSharp
{
    class SimpleFile : SimpleEntry, File, FSEntry
    {
        public int Length
        {
            get
            {
                return mNode.FileLength;
            }
        }

        public SimpleFile(VirtualNode node) : base(node)
        {
            //pass it on to base.
        }

        public FileStream Open()
        {
            return new SimpleStream(mNode);
        }
    }
}
