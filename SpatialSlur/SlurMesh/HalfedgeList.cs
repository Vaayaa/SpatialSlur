﻿using System;
using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public class HalfedgeList<E> : HeElementList<E>
        where E : Halfedge<E>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public HalfedgeList(int capacity)
            : base(capacity)
        {
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountUnused()
        {
            int result = 0;

            for (int i = 0; i < Count; i += 2)
                if (Items[i].IsUnused) result += 2;

            return result;
        }


        /// <summary>
        /// Removes all unused elements in the list and re-indexes the remaining.
        /// Does not change the capacity of the list.
        /// If the list has any associated attributes, be sure to compact those first.
        /// </summary>
        public void Compact()
        {
            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                var he = Items[i];
                if (he.IsUnused) continue; // skip unused halfedge pairs

                he.Index = marker;
                Items[marker++] = he;

                he = he.Twin;

                he.Index = marker;
                Items[marker++] = he;
            }

            AfterCompact(marker);
        }


        /// <summary>
        /// Removes all attributes corresponding with unused elements.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public void CompactAttributes<U>(List<U> attributes)
        {
            int marker = SwimAttributes(attributes);
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// Moves attributes corresponding with used elements to the front of the given list.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public int SwimAttributes<U>(IList<U> attributes)
        {
            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                if (Items[i].IsUnused) continue; // skip unused halfedge pairs
                attributes[marker++] = attributes[i];
                attributes[marker++] = attributes[i + 1];
            }

            return marker;
        }
    }
}
