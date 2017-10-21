using System;
using System.Collections.Generic;

namespace Services.Core.Databinding
{
    public class Node
    {
        // considered the last part of the branch
        public string Id;
        // the depth of this node in the tree
        public int treeDepth;
        // full branch
        public string branch;

        public Node parent;
        public List<Node> subNodes = new List<Node>();

        public void AddSubNode(Node node)
        {
            foreach (var n in subNodes)
                if (n.branch.Equals(node.branch))
                {
                    LogWrapper.DebugLog("[{0}] node: {1} is already a sub node of branch: {2}", GetType(), Id, branch);
                    return;
                }

            node.parent = this;
            subNodes.Add(node);

            LogWrapper.DebugLog("[{0}] Adding node {1} to {2}", GetType(), node.branch, branch);
        }

        public void RemoveSubNode(Node node)
        {
            for (int n = 0; n < subNodes.Count; n++)
                if (subNodes[n].branch.Equals(node.branch))
                {
                    subNodes[n].parent = null;
                    subNodes.RemoveAt(n);
                    return;
                }

            LogWrapper.DebugLog("[{0}] node: {1} is not part of branch: {2}", GetType(), Id, branch);
        }

        public bool Contains(Node node)
        {
            if (branch.Equals(node.branch))
                return true;

            foreach (var n in subNodes)
                if (n.Contains(node))
                    return true;

            return false;
        }
    }

    public class Data<T> : Node
    {
        private T _value;

        public T value
        {
            set {
                _value = value; 
                NotifyComponents(); 
            }
            get { return _value; }
        }

        public List<BindingComponent<T>> bindedComponents = new List<BindingComponent<T>>();

        public void NotifyComponents()
        {
            foreach (var component in bindedComponents)
                component.OnValueChanged(branch, value);
        }

        public override string ToString()
        {
            return base.ToString() + " branch: " + branch + ", value: " + value;
        }
    }
}