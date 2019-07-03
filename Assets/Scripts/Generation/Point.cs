using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Lovely
{
    public class Point
    {
        public PointType type;
        public Vector3 pos;
        public int risk;
        public Node node;
        internal int amount;

        public Point(PointType type, Vector3 pos)
        {
            this.type = type;
            this.pos = pos;
            risk = 0;
        }

        public static bool operator ==(Point a, Point b)
        {
            // an item is always equal to itself
            if (object.ReferenceEquals(a, b))
                return true;

            // if both a and b were null, we would have already escaped so check if either is null
            if (object.ReferenceEquals(a, null))
                return false;
            if (object.ReferenceEquals(b, null))
                return false;

            // Now that we've made sure we are working with real objects:
            return a.pos == b.pos;
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        public override bool Equals(System.Object obj)
        {
            System.Object asObj = (Point)obj;
            return Equals(obj as Point);
        }

        public bool Equals(Point other)
        {
            return other == this;
        }

        public override int GetHashCode()
        {
            // Life is easy when there is only one property!
            // Then again.. what was the point of all this?
            return pos.GetHashCode();
        }
    }

    public enum PointType
    {

        START,
        RISK,
        NONE,
        HEALTH,
        POTION,
        END,
        POISON,
        GHOST,
        FRIEND,
        SWORD,
        GOLD,
        PRISONER,
        TELEPORT,
        TRAP,
        ARMOUR,
        PERM_HEALTH_INC,
        PERM_ROLL_INC,
        PERM_START_ARMOUR,
        INFO
    }
}
