﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TilePoint
{
    [SerializeField] public int X;
    [SerializeField] public int Y;

    public TilePoint() {}

    public TilePoint(GameObject o, bool swapYZ = true)
        : this(o.transform.localPosition, swapYZ) {}
    
    public TilePoint(Vector3 v, bool swapYZ = true)
        : this(v.x.FlooredInt(), swapYZ ? v.z.FlooredInt() : v.y.FlooredInt()) {}
    
    public TilePoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool IsCardinal() => (X == 0 && Y != 0) || (X != 0 && Y == 0);
    public int TotalMagnitude() => Math.Abs(X) + Math.Abs(Y);
    
    public TilePoint Clone() => new TilePoint(X, Y);
    public override string ToString() => $"{X},{Y}";
    public override int GetHashCode() => ToString().GetHashCode();
    public override bool Equals(object obj) => obj is TilePoint point && Equals(point);
    private bool Equals(TilePoint other) => other.X == X && other.Y == Y;

    public int DistanceFrom(TilePoint other) => (this - other).Distance();
    public int Distance() => Math.Abs(X) + Math.Abs(Y);
    public Vector3 Plus(Vector3 v) => v + new Vector3(X, Y, 0);
    public TilePoint Plus(TilePoint t) => t + this;
    public TilePoint Plus(int x, int y) => new TilePoint(x, y) + this;
    public static TilePoint operator +(TilePoint t, TilePoint t2) => new TilePoint(t.X + t2.X, t.Y + t2.Y);
    public static TilePoint operator -(TilePoint t, TilePoint t2) => new TilePoint(t.X - t2.X, t.Y - t2.Y);
    public static TilePoint operator /(TilePoint t, int divisor) => new TilePoint(t.X / divisor, t.Y / divisor);
    public static TilePoint operator *(TilePoint t, int multiplier) => new TilePoint(t.X * multiplier, t.Y * multiplier);
    public Vector2Int ToVector2Int() => new Vector2Int(X, Y);

    public bool IsAdjacentTo(TilePoint other)
    {
        var delta = this - other;
        return delta.IsCardinal() && delta.TotalMagnitude() == 1;
    }

    public List<TilePoint> InBetween(TilePoint other)
    {
        var results = new List<TilePoint>();
        var minX = X > other.X ? other.X : X;
        var maxX = X > other.X ? X : other.X;
        var minY = Y > other.Y ? other.Y : Y;
        var maxY = Y > other.Y ? Y : other.Y;
        for (var x = minX; x <= maxX; x++)
            for (var y = minY; y <= maxY; y++)
                if ((minX != maxX || minY != maxY)
                    && (minX == maxX || (x != minX && x != maxX)) 
                    && (minY == maxY || (y != minY && y != maxY)))
                    results.Add(new TilePoint(x, y));
        return results;
    }
    
    public List<TilePoint> GetAdjacents() => GetCardinals(1);
    
    public List<TilePoint> GetCardinals(int distance = 2)
        => new List<TilePoint>
        {
            new TilePoint(X, Y + distance),
            new TilePoint(X, Y - distance),
            new TilePoint(X + distance, Y),
            new TilePoint(X - distance, Y),
        };
    
    public bool IsInBounds(int maxX, int maxY) => 
        X >= 0 && X < maxX &&
        Y >= 0 && Y < maxY;
    
    public bool IsInBounds(Vector2Int size) => 
        X >= 0 && X < size.x &&
        Y >= 0 && Y < size.y;
}
