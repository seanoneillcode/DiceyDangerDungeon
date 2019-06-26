using UnityEngine;
using UnityEditor;
namespace Lovely
{
    public class Split
    {
        public Point point;
        public int points;

        public Split(Point point, int points)
        {
            this.point = point;
            this.points = points;
        }
    }
}