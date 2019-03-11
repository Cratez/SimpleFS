/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System.Collections.Generic;

namespace FileSystemCSharp
{
    abstract class SimpleEntry : FSEntry
    {
        protected VirtualNode mNode;

        public string Name => mNode.Name;
        public Directory Parent => (mNode.Parent == null) ? null : new SimpleDirectory(mNode.Parent);
        public virtual bool IsDirectory => mNode.IsDirectory;
        public virtual bool IsFile => mNode.IsFile; 
        public string FullPathName
        {
            /*
             * again, couldnt find if you did this, my way seems hacky.
             * 
             */
            get
            {
                string fullName = string.Empty;
                Stack<VirtualNode> stack = new Stack<VirtualNode>();

                //get all the virtualnodes
                VirtualNode currentNode = mNode;
                do
                {
                    //push current node
                    stack.Push(currentNode);

                    //get next node in chain
                    currentNode = currentNode.Parent;
                } while (currentNode != null);

                //get root node name
                fullName = stack.Pop().Name;
                if (stack.Count > 0) // this is for stopping stupid extra slash
                {
                    fullName += stack.Pop().Name;
                    foreach (VirtualNode node in stack)
                    {
                        fullName += Constants.PATH_SEPARATOR + node.Name;
                    }
                }

                return fullName;
            }
        }

        protected SimpleEntry(VirtualNode node)
        {
            this.mNode = node;
        }

        public void Rename(string name)
        {
            mNode.Rename(name);
        }

        public void Move(Directory destination)
        {
            mNode.Move((destination as SimpleEntry).mNode);
        }

        public void Delete()
        {
            mNode.Delete();
        }
    }
}
