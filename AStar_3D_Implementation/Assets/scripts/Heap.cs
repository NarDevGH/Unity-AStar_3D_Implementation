using System.Collections;
using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int count;
    public int Count { get { return count; } }

    public Heap(int maxSize)
    {
        items = new T[maxSize];
    }

    public void Add(T item) 
    {
        item.heapIndex = count;
        items[count] = item;
        SortUp(item);
        count++;
    }
    public T RemoveFirst() 
    {
        T firstItem = items[0];
        count--;
        items[0] = items[count];
        items[0].heapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }
    public bool Contains(T item) 
    {
        return Equals(item, items[item.heapIndex]);
    }

    private void SortUp(T item) 
    {
        int parenIndex = (item.heapIndex - 1) / 2;

        while (true)
        {
            T itemParent = items[parenIndex];
            if (item.CompareTo(itemParent) > 0)
            {
                Swap(item, itemParent);
            }
            else 
            {
                break;
            }

            parenIndex = (item.heapIndex - 1) / 2;
        }
    }

    private void SortDown(T item) 
    {
        while (true)
        {
            int childIndexLeft = item.heapIndex * 2 +1;
            int childIndexRight = item.heapIndex * 2 +2;
            int swapIndex = 0;

            if (childIndexLeft < count)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < count)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else 
            {
                return;
            }
        }
    }

    private void Swap(T itemA, T itemB) 
    {
        items[itemA.heapIndex] = itemB;
        items[itemB.heapIndex] = itemA;

        int tempIndex = itemA.heapIndex;
        itemA.heapIndex = itemB.heapIndex;
        itemB.heapIndex = tempIndex;
    }

} 

public interface IHeapItem<T> :IComparable<T>
{
    int heapIndex { get; set; }
}