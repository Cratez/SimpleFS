/* 
 * John Harrison
 * CST352 Assignment 3
 */

using System.Collections.Generic;
using System.Linq;

namespace FileSystemCSharp
{
    class SimpleDirectory : SimpleEntry, Directory, FSEntry
    {

        public SimpleDirectory(VirtualNode node) : base(node)
        {
        }

        public IEnumerable<Directory> GetSubDirectories()
        {
            //didn't feel like skimming over and over for where you showed this
            //this works well enough
            var ChildNodes = mNode.GetChildren().Where(node => node.IsDirectory);
            List<Directory> directories = new List<Directory>();
            foreach(var node in ChildNodes)
            {
                directories.Add(new SimpleDirectory(node));
            }
            return directories;
        }

        public IEnumerable<File> GetFiles()
        {
            //didn't feel like skimming over and over for where you showed this
            //this works well enough
            var ChildNodes = mNode.GetChildren().Where(node => node.IsFile);
            List<File> files = new List<File>();
            foreach (var node in ChildNodes)
            {
                files.Add(new SimpleFile(node));
            }
            return files;
        }

        public Directory CreateDirectory(string name)
        {
            return new SimpleDirectory(mNode.CreateDirectoryNode(name));
        }

        public File CreateFile(string name)
        {
            return new SimpleFile(mNode.CreateFileNode(name));
        }
    }
}
