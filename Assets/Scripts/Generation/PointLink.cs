using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Lovely
{
    public class PointLink
    {
        public Point from;
        public Point to;

        public PointLink(Point from, Point to)
        {
            this.from = from;
            this.to = to;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                PointLink p = (PointLink)obj;
                return (from == p.from && to == p.to) || (from == p.to && to == p.from);
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -1951484959;
            hashCode = hashCode * -1521134295 + (EqualityComparer<Point>.Default.GetHashCode(from) * EqualityComparer<Point>.Default.GetHashCode(to));
            return hashCode;
        }
    }
}