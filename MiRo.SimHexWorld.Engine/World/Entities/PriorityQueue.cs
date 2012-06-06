using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public void Update(T contained)
        {
            T t = mA.FirstOrDefault(a => a.Equals(contained));
            int idx = mA.IndexOf(t);
            mA[idx] = contained;

            mA.Sort();
        }

        public void Enqueue(T value)
        {
            Add(value);
        }

        public T Find(T value)
        {
            return mA.FirstOrDefault( a => a.CompareTo(value ) == 0 );
        }

        public T Dequeue()
        {
            T first = mA.First();
            mA.RemoveAt(0);
            return first;
        }

        /// <summary>Clear all the elements from the priority queue</summary>
        public void Clear()
        {
            mA.Clear();
        }

        /// <summary>Add an element to the priority queue - O(log(n)) time operation.</summary>
        /// <param name="item">The item to be added to the queue</param>
        public void Add(T item)
        {
            // We add the item to the end of the list (at the bottom of the
            // tree). Then, the heap-property could be violated between this element
            // and it's parent. If this is the case, we swap this element with the 
            // parent (a safe operation to do since the element is known to be less
            // than it's parent). Now the element move one level up the tree. We repeat
            // this test with the element and it's new parent. The element, if lesser
            // than everybody else in the tree will eventually bubble all the way up
            // to the root of the tree (or the head of the list). It is easy to see 
            // this will take log(N) time, since we are working with a balanced binary
            // tree.
            int n = mA.Count; 
            mA.Add(item);
            while (n != 0)
            {
                int p = n / 2;    // This is the 'parent' of this item
                if (mA[n].CompareTo(mA[p]) >= 0) 
                    break;  // Item >= parent

                T tmp = mA[n]; 
                mA[n] = mA[p]; 
                mA[p] = tmp; // Swap item and parent

                n = p;            // And continue
            }
        }

        /// <summary>Returns the number of elements in the queue.</summary>
        public int Count
        {
            get { return mA.Count; }
        }

        /// <summary>Returns true if the queue is empty.</summary>
        /// Trying to call Peek() or Next() on an empty queue will throw an exception.
        /// Check using Empty first before calling these methods.
        public bool Empty
        {
            get { return mA.Count == 0; }
        }

        /// <summary>Allows you to look at the first element waiting in the queue, without removing it.</summary>
        /// This element will be the one that will be returned if you subsequently call Next().
        public T Peek()
        {
            if (mA.Count == 0)
                return default(T);

            return mA[0];
        }

        /// <summary>Removes and returns the first element from the queue (least element)</summary>
        /// <returns>The first element in the queue, in ascending order.</returns>
        public T Next()
        {
            // The element to return is of course the first element in the array, 
            // or the root of the tree. However, this will leave a 'hole' there. We
            // fill up this hole with the last element from the array. This will 
            // break the heap property. So we bubble the element downwards by swapping
            // it with it's lower child until it reaches it's correct level. The lower
            // child (one of the orignal elements with index 1 or 2) will now be at the
            // head of the queue (root of the tree).
            T val = mA[0];
            int nMax = mA.Count - 1;
            mA[0] = mA[nMax]; mA.RemoveAt(nMax);  // Move the last element to the top

            int p = 0;
            while (true)
            {
                // c is the child we want to swap with. If there
                // is no child at all, then the heap is balanced
                int c = p * 2; 
                if (c >= nMax) 
                    break;

                // If the second child is smaller than the first, that's the one
                // we want to swap with this parent.
                if (c + 1 < nMax && mA[c + 1].CompareTo(mA[c]) < 0) 
                    c++;

                // If the parent is already smaller than this smaller child, then
                // we are done
                if (mA[p].CompareTo(mA[c]) <= 0) 
                    break;

                // Othewise, swap parent and child, and follow down the parent
                T tmp = mA[p]; mA[p] = mA[c]; mA[c] = tmp;
                p = c;
            }
            return val;
        }

        /// <summary>The List we use for implementation.</summary>
        List<T> mA = new List<T>();
    }
}
