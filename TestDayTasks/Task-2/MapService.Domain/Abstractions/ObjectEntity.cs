using MapService.Domain.ValueObjects;

namespace MapService.Domain.Abstractions;

public abstract class ObjectEntity<T> : Entity
{
    public Coordinate TopLeft { get; set; }
    public Coordinate BottomRight => new(TopLeft.X + Width, TopLeft.Y + Height);
    public int Width { get; set; }
    public int Height { get; set; }
    public T Types { get; set; }

    public bool Intersects(Area area)
    {
        var objLeft = TopLeft.X;
        var objRight = BottomRight.X;
        var objTop = TopLeft.Y;
        var objBottom = BottomRight.Y;

        var areaLeft = area.TopLeft.X;
        var areaRight = area.BottomRight.X;
        var areaTop = area.TopLeft.Y;
        var areaBottom = area.BottomRight.Y;

        var isLeftOf = objRight < areaLeft;   
        var isRightOf = objLeft > areaRight;  
        var isAbove = objBottom < areaTop;    
        var isBelow = objTop > areaBottom;    

        return !(isLeftOf || isRightOf || isAbove || isBelow);
    }


    public bool Covers(Coordinate coord)
    {
        return coord.X >= TopLeft.X &&
               coord.X <= BottomRight.X &&
               coord.Y >= TopLeft.Y &&
               coord.Y <= BottomRight.Y;
    }
}