using System;

namespace DwFramework.Core
{
    public sealed class TwoWayNode<T>
    {
        public TwoWayNode<T> PreviousNode { get; private set; }
        public TwoWayNode<T> NextNode { get; private set; }
        public T Value { get; private set; }

        public TwoWayNode(TwoWayNode<T> previousNode = null, TwoWayNode<T> nextNode = null, T value = default)
        {
            PreviousNode = previousNode;
            NextNode = nextNode;
            Value = value;
        }

        public void SetParentNode(TwoWayNode<T> node)
        {
            PreviousNode = node;
        }

        public void SetNextNode(TwoWayNode<T> node)
        {
            NextNode = node;
        }

        public void SetValue(T value)
        {
            Value = value;
        }
    }
}
