﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heart_Module.Data.Scripts.HeartModule.Utility
{
    // Adapted from Servy's implementation (https://stackoverflow.com/questions/13981406/simple-priority-queue-in-c-sharp-what-will-be-better-than-list-with-custom-sor)

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TElement">The type of the actual elements that are stored</typeparam>
    /// <typeparam name="TKey">The type of the priority.  It probably makes sense to be an int or long, \
    /// but any type that can be the key of a SortedDictionary will do.</typeparam>
    public class PriorityQueue<TElement, TKey>
    {
        private SortedDictionary<TKey, Queue<TElement>> dictionary = new SortedDictionary<TKey, Queue<TElement>>();
        private Func<TElement, TKey> selector;

        public PriorityQueue(Func<TElement, TKey> selector)
        {
            this.selector = selector;
        }

        public bool IsEmpty => dictionary.Count == 0;
        public int Count => dictionary.Count;

        public void Enqueue(TElement item)
        {
            TKey key = selector(item);
            Queue<TElement> queue;
            if (!dictionary.TryGetValue(key, out queue))
            {
                queue = new Queue<TElement>();
                dictionary.Add(key, queue);
            }

            queue.Enqueue(item);
        }

        public TElement Dequeue()
        {
            if (dictionary.Count == 0)
                throw new Exception("No items to Dequeue:");
            var key = dictionary.Keys.First();

            var queue = dictionary[key];
            var output = queue.Dequeue();
            if (queue.Count == 0)
                dictionary.Remove(key);

            return output;
        }
    }
}
