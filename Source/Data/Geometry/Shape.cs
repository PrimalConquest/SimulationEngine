using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Geometry
{
    public struct Shape
    {

        ///<summary>
        ///Ancher point must be the leftmost point on the bottom row of the shape
        ///</summary>
        public Point Anchor { get; set; }
        public HashSet<Point>? Offsets { get; set; }

        public Shape()
        {
            Offsets = null;
        }

        public Shape(HashSet<Point> offsets)
        {
            Offsets = offsets;
        }

        /// <summary>
        /// <para>
        /// Parse a shape from a string grid pattern.<br/>
        /// 'X' = occupied cell, '_' = empty. <br/>
        /// Rows are defined top-to-bottom in the string array, but Y increases upward internally (row 0 of array = top = highest Y).<br/>
        /// </summary>
        public static Shape Parse(string[] rows)
        {
            if (rows == null || rows.Length == 0)
                throw new ArgumentException("Shape pattern must have at least one row.");

            if(rows.Length == 1 && rows[0].Length == 1) return new Shape();

            int height = rows.Length;
            HashSet<Point> extras = new();

            int anchorY = height-1, anchorX = rows[anchorY].IndexOf('X');

            if (anchorX == -1)
                throw new ArgumentException("Shape pattern must contain at least one 'X'.");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < rows[y].Length; x++)
                {
                    if (rows[y][x] != 'X')
                    {
                        continue;
                    }
                    
                    int foundX = x - anchorX, foundY = anchorY - y;

                    if (foundX == 0 && foundY == 0) continue;

                    extras.Add(new Point { X = foundX, Y = foundY });
                }
            }

            return extras.Count > 0 ? new Shape(extras) : new Shape();
        }
    }
}
