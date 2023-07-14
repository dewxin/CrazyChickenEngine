using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.Assorted
{
    public static class ArraySegmentEx
    {
#if NETCOREAPP2_0_OR_GREATER
#else
        public static void CopyTo<T>(this ArraySegment<T> _, T[] destination, int destinationIndex)
        {
            if (_.Array == null)
            {
                throw new ArgumentException("Array is null");
            }
            System.Array.Copy(_.Array, _.Offset, destination, destinationIndex, _.Count);
        }

        public static ArraySegment<T> Slice<T>(this ArraySegment<T> _, int index, int count)
        {

            if ((uint)index > (uint)_.Count || (uint)count > (uint)(_.Count - index))
            {
                throw new ArgumentException("param error");
            }

            return new ArraySegment<T>(_.Array, _.Offset + index, count);
        }

        public static ArraySegment<T> Slice<T>(this ArraySegment<T> _, int index)
        {
            if ((uint)index > (uint)_.Count)
            {
                throw new ArgumentException("param error");
            }

            return new ArraySegment<T>(_.Array, _.Offset + index, _.Count - index);
        }
#endif
    }
}
