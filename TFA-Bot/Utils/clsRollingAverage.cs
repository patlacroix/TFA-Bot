using System;
using System.Collections.Generic;

namespace TFABot
{

public sealed class clsRollingAverage
{
    public clsRollingAverage(int maxRememberedNumbers)
    {
        if (maxRememberedNumbers <= 0)
        {
            throw new ArgumentException("maxRememberedNumbersmust be greater than 0.", "maxRememberedNumbers");
        }

        this.counts = new Queue<int>(maxRememberedNumbers);
        this.maxSize = maxRememberedNumbers;
        this.currentTotal = 0L;
        this.padLock = new Object();
    }

    private const int defaultMaxRememberedNumbers = 10;

    private readonly Queue<int> counts;
    private readonly int maxSize;

    private long currentTotal;

    private object padLock;

    public void Add(int value)
    {
        lock (this.padLock)
        {
        
            if (value < LowestEver) LowestEver = value;
            if (value > HighestEver) HighestEver = value;
        
            if (this.counts.Count == this.maxSize)
            {
                this.currentTotal -= (long)this.counts.Dequeue();
            }

            this.counts.Enqueue(value);

            this.currentTotal += (long)value;
        }
    }


    public int LowestEver {get; private set;}
    public int HighestEver {get; private set;}

    public int CurrentAverage
    {
        get
        {
            long lenCounts;
            long observedTotal;

            lock (this.padLock)
            {
                lenCounts = (long)this.counts.Count;
                observedTotal = this.currentTotal;
            }

            if (lenCounts == 0)
            {
                return LowestEver = 0;
            }
            else if (lenCounts == 1)
            {
                return (int)observedTotal;
            }
            else
            {
                return (int)(observedTotal / lenCounts);
            }
        }
    }

    public void Clear()
    {
        lock (this.padLock)
        {
            this.currentTotal = 0L;
            this.counts.Clear();
        }
    }
    
    public int Count
    {
        get
        {
            return counts.Count;
        }
    }
    
    public int[] GetValues()
    {
        return counts.ToArray();
    }
    
}
}