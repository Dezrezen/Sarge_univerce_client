using System;

namespace SargeUniverse.Scripts.BattleSystem.PathFinding
{
    public class PriorityQueue
    {
        private readonly Cell[] _nodes;
        public int Count { get; private set; }

        public PriorityQueue(int maxNodes) 
        {
            Count = 0;
            _nodes = new Cell[maxNodes + 1];
        }

        public void Clear()
        {
            Array.Clear(_nodes, 1, Count);
            Count = 0;
        }

        public bool Contains(Cell node)
        {
            return _nodes[node.queueIndex] == node;
        }

        public void Enqueue(Cell node, double priority) 
        {
            node.priority = priority;
            Count++;
            _nodes[Count] = node;
            node.queueIndex = Count;
            CascadeUp(node);
        }

        public Cell Dequeue() 
        {
            var returnMe = _nodes[1];
            
            if (Count == 1) 
            {
                _nodes[1] = null;
                Count = 0;

                return returnMe;
            }
            
            var formerLastNode = _nodes[Count];
            _nodes[1] = formerLastNode;
            formerLastNode.queueIndex = 1;
            _nodes[Count] = null;
            Count--;
            
            CascadeDown(formerLastNode);
            return returnMe;
        }

        public void UpdatePriority(Cell node, double priority)
        {
            node.priority = priority;
            OnNodeUpdated(node);
        }

        public void Remove(Cell node) 
        {
            if (node.queueIndex == Count) 
            {
                _nodes[Count] = null;
                Count--;

                return;
            }
            
            var formerLastNode = _nodes[Count];
            _nodes[node.queueIndex] = formerLastNode;
            formerLastNode.queueIndex = node.queueIndex;
            _nodes[Count] = null;
            Count--;
            
            OnNodeUpdated(formerLastNode);
        }

        private void CascadeUp(Cell node) 
        {
            int parent;

            if (node.queueIndex > 1) 
            {
                parent = node.queueIndex >> 1;
                var parentNode = _nodes[parent];
                if (HasHigherOrEqualPriority(parentNode, node)) { return; }
                
                _nodes[node.queueIndex] = parentNode;
                parentNode.queueIndex = node.queueIndex;

                node.queueIndex = parent;
            } 
            else
            {
                return;
            }

            while (parent > 1) 
            {
                parent >>= 1;
                var parentNode = _nodes[parent];
                if (HasHigherOrEqualPriority(parentNode, node)) { break; }
                
                _nodes[node.queueIndex] = parentNode;
                parentNode.queueIndex = node.queueIndex;

                node.queueIndex = parent;
            }
            _nodes[node.queueIndex] = node;
        }

        private void CascadeDown(Cell node) 
        {
            var finalQueueIndex = node.queueIndex;
            var childLeftIndex = 2 * finalQueueIndex;

            if (childLeftIndex > Count)
            {
                return;
            }
            
            var childRightIndex = childLeftIndex + 1;
            var childLeft = _nodes[childLeftIndex];

            if (HasHigherPriority(childLeft, node)) 
            {
                if (childRightIndex > Count) 
                {
                    node.queueIndex = childLeftIndex;
                    childLeft.queueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    _nodes[childLeftIndex] = node;
                    return;
                }
                
                var childRight = _nodes[childRightIndex];
                if (HasHigherPriority(childLeft, childRight)) 
                {
                    childLeft.queueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                } 
                else 
                {
                    childRight.queueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
            }
            else if (childRightIndex > Count) 
            {
                return;
            } 
            else 
            {
                var childRight = _nodes[childRightIndex];
                if (HasHigherPriority(childRight, node)) 
                {
                    childRight.queueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
                else 
                {
                    return;
                }
            }

            while (true) 
            {
                childLeftIndex = 2 * finalQueueIndex;
                if (childLeftIndex > Count) 
                {
                    node.queueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                }
                
                childRightIndex = childLeftIndex + 1;
                childLeft = _nodes[childLeftIndex];

                if (HasHigherPriority(childLeft, node))
                {
                    if (childRightIndex > Count) 
                    {
                        node.queueIndex = childLeftIndex;
                        childLeft.queueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        _nodes[childLeftIndex] = node;
                        break;
                    }
                    
                    var childRight = _nodes[childRightIndex];
                    if (HasHigherPriority(childLeft, childRight)) 
                    {
                        childLeft.queueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    } 
                    else 
                    {
                        childRight.queueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                else if (childRightIndex > Count) 
                {
                    node.queueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;

                    break;
                } 
                else
                {
                    var childRight = _nodes[childRightIndex];
                    if (HasHigherPriority(childRight, node)) 
                    {
                        childRight.queueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    else 
                    {
                        node.queueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = node;
                        break;
                    }
                }
            }
        }

        private bool HasHigherPriority(Cell higher, Cell lower)
        {
            return higher.priority < lower.priority;
        }

        private bool HasHigherOrEqualPriority(Cell higher, Cell lower)
        {
            return higher.priority <= lower.priority;
        }

        private void OnNodeUpdated(Cell node) 
        {
            var parentIndex = node.queueIndex >> 1;
            if (parentIndex > 0 && HasHigherPriority(node, _nodes[parentIndex]))
            { 
                CascadeUp(node);
            }
            else
            { 
                CascadeDown(node); 
            }
        }
    }
}