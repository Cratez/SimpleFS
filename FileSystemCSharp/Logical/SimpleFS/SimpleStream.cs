/* 
 * John Harrison
 * CST352 Assignment 3
 *
 */

namespace FileSystemCSharp
{
    class SimpleStream : FileStream
    {
        private VirtualNode mNode;

        public SimpleStream(VirtualNode node)
        {
            mNode = node;
        }

        public void Close()
        {
            //todo?
        }

        public byte[] Read(int index, int length)
        {
            return mNode.Read(index, length);
        }

        public void Write(int index, byte[] data)
        {
            mNode.Write(index, data);
        }
    }
}
