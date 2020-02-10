namespace AForge.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public static class MemoryManager
    {
        private static int busyBlocks = 0;
        private static int cachedMemory = 0;
        private static int currentCacheSize = 0;
        private static int maximumCacheSize = 3;
        private static int maxSizeToCache = 0x1400000;
        private static List<CacheBlock> memoryBlocks = new List<CacheBlock>();
        private static int minSizeToCache = 0x2800;

        public static IntPtr Alloc(int size)
        {
            lock (memoryBlocks)
            {
                if (((busyBlocks >= maximumCacheSize) || (size > maxSizeToCache)) || (size < minSizeToCache))
                {
                    return Marshal.AllocHGlobal(size);
                }
                if (currentCacheSize == busyBlocks)
                {
                    IntPtr memoryBlock = Marshal.AllocHGlobal(size);
                    memoryBlocks.Add(new CacheBlock(memoryBlock, size));
                    busyBlocks++;
                    currentCacheSize++;
                    cachedMemory += size;
                    return memoryBlock;
                }
                for (int i = 0; i < currentCacheSize; i++)
                {
                    CacheBlock block = memoryBlocks[i];
                    if (block.Free && (block.Size >= size))
                    {
                        block.Free = false;
                        busyBlocks++;
                        return block.MemoryBlock;
                    }
                }
                for (int j = 0; j < currentCacheSize; j++)
                {
                    CacheBlock block2 = memoryBlocks[j];
                    if (block2.Free)
                    {
                        Marshal.FreeHGlobal(block2.MemoryBlock);
                        memoryBlocks.RemoveAt(j);
                        currentCacheSize--;
                        cachedMemory -= block2.Size;
                        IntPtr ptr2 = Marshal.AllocHGlobal(size);
                        memoryBlocks.Add(new CacheBlock(ptr2, size));
                        busyBlocks++;
                        currentCacheSize++;
                        cachedMemory += size;
                        return ptr2;
                    }
                }
                return IntPtr.Zero;
            }
        }

        public static void Free(IntPtr pointer)
        {
            lock (memoryBlocks)
            {
                for (int i = 0; i < currentCacheSize; i++)
                {
                    if (memoryBlocks[i].MemoryBlock == pointer)
                    {
                        memoryBlocks[i].Free = true;
                        busyBlocks--;
                        goto Label_0062;
                    }
                }
                Marshal.FreeHGlobal(pointer);
            Label_0062:;
            }
        }

        public static int FreeUnusedMemory()
        {
            lock (memoryBlocks)
            {
                int num = 0;
                for (int i = currentCacheSize - 1; i >= 0; i--)
                {
                    if (memoryBlocks[i].Free)
                    {
                        Marshal.FreeHGlobal(memoryBlocks[i].MemoryBlock);
                        cachedMemory -= memoryBlocks[i].Size;
                        memoryBlocks.RemoveAt(i);
                        num++;
                    }
                }
                currentCacheSize -= num;
                return num;
            }
        }

        public static int BusyMemoryBlocks
        {
            get
            {
                lock (memoryBlocks)
                {
                    return busyBlocks;
                }
            }
        }

        public static int CachedMemory
        {
            get
            {
                lock (memoryBlocks)
                {
                    return cachedMemory;
                }
            }
        }

        public static int CurrentCacheSize
        {
            get
            {
                lock (memoryBlocks)
                {
                    return currentCacheSize;
                }
            }
        }

        public static int FreeMemoryBlocks
        {
            get
            {
                lock (memoryBlocks)
                {
                    return (currentCacheSize - busyBlocks);
                }
            }
        }

        public static int MaximumCacheSize
        {
            get
            {
                lock (memoryBlocks)
                {
                    return maximumCacheSize;
                }
            }
            set
            {
                lock (memoryBlocks)
                {
                    maximumCacheSize = Math.Max(0, Math.Min(10, value));
                }
            }
        }

        public static int MaxSizeToCache
        {
            get
            {
                return maxSizeToCache;
            }
            set
            {
                maxSizeToCache = value;
            }
        }

        public static int MinSizeToCache
        {
            get
            {
                return minSizeToCache;
            }
            set
            {
                minSizeToCache = value;
            }
        }

        private class CacheBlock
        {
            public bool Free;
            public IntPtr MemoryBlock;
            public int Size;

            public CacheBlock(IntPtr memoryBlock, int size)
            {
                this.MemoryBlock = memoryBlock;
                this.Size = size;
                this.Free = false;
            }
        }
    }
}

