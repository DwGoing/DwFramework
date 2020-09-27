using System;

namespace DwFramework.Core
{
    public sealed class Tree<T>
    {
        public TwoWayNode<T> Root { get; private set; }

        public Tree()
        {

        }

        public void Add(T value)
        {
            var node = new TwoWayNode<T>(value: value);
        }

        private void Plant(TwoWayNode<T> node)
        {

        }
    }
}
