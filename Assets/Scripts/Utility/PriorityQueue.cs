using FibonacciHeap;
using System;

public class PriorityQueue<TElement, TPriority> : IPriorityQueue<TElement, TPriority>
    where TPriority : IComparable<TPriority>
{
    private readonly FibonacciHeap<TElement, TPriority> heap;

    public int Count { get; private set; }

    public PriorityQueue(TPriority minPriority)
    {
        heap = new FibonacciHeap<TElement, TPriority>(minPriority);
    }

    public void Insert(TElement item, TPriority priority)
    {
        heap.Insert(new FibonacciHeapNode<TElement, TPriority>(item, priority));
        Count++;
    }

    public TElement Peek()
    {
        FibonacciHeapNode<TElement, TPriority> minElement = heap.Min();
        if (minElement != null)
            return minElement.Data;
        else
            return default;
    }

    public FibonacciHeapNode<TElement, TPriority> PeekNode()
    {
        FibonacciHeapNode<TElement, TPriority> minElement = heap.Min();
        if (minElement != null)
            return minElement;
        else
            return default;
    }

    public TElement Pop()
    {
        FibonacciHeapNode<TElement, TPriority> minElement = heap.RemoveMin();
        if (minElement != null)
        {
            Count--;
            return minElement.Data;
        }
        else
            return default;
    }

    public FibonacciHeapNode<TElement, TPriority> PopNode()
    {
        FibonacciHeapNode<TElement, TPriority> minElement = heap.Min();
        if (minElement != null)
        {
            Count--;
            return minElement;
        }
        else
            return default;
    }
}

public interface IPriorityQueue<T, P>
{
    /// <summary>
    /// Inserts and item with a priority
    /// </summary>
    /// <param name="item"></param>
    /// <param name="priority"></param>
    void Insert(T item, P priority);

    /// <summary>
    /// Returns the element with the highest priority
    /// </summary>
    /// <returns></returns>
    T Peek();

    /// <summary>
    /// Deletes and returns the element with the highest priority
    /// </summary>
    /// <returns></returns>
    T Pop();
}