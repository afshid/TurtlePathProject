using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace JanaltaPathProject.Models
{
    public class PointInfo
    {
        public Point LocPoint { get; set; }
        public int Count { get; set; }
    }
    public enum Direction
    {
        Up = 1,
        Left = 2,
        Down = 3,
        Right = 4,
    }

    public enum Movement
    {
        Left,
        Right,
        Forward
    }
}