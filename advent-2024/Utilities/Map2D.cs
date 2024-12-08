using System.Collections;
using System.Text;

namespace AdventOfCode.Y2K24.Utilities;


public class Map2D<T> : IEnumerable<T>
{

    private readonly List<T?[]> _mapData = [];
    private int _width = 0;
    public int Width => _width;
    public int Height => _mapData.Count;
    public T? this[int x, int y]
    {
        get => _mapData[y][x];
        set => _mapData[y][x] = value;
    }
    public Map2D (int width, int height = 0)
    {
        _width = width;
        for (var y = 0; y < height; y++)
        {
            AppendRow();
        }
    }


    public T?[] AppendRow(T?[]? data = null)
    {
        var newRow = new T?[_width];
        _mapData.Add(newRow);

        if (data is null)
            return newRow;

        Array.Copy(data, 0,
            newRow, 0, _width);
        
        return newRow;
    }

    /// <summary>
    /// Get deep clone of current map
    /// </summary>
    public Map2D<T> Clone()
    {
        var newMap = new Map2D<T>(_width);
        CopyTo(newMap);
        return newMap;
    }

    /// <summary>
    /// Copies the current map to the specified other map.
    /// Clears the other map before copying
    /// </summary>
    /// <param name="other"></param>
    public void CopyTo(Map2D<T> other)
    {
        other._mapData.Clear();
        for (var y = 0; y < Height ; y++)
        {
            other.AppendRow(_mapData[y]);
        }
    }

    /// <summary>
    /// Merges values of current map onto another map.
    /// </summary>
    /// <param name="other">The other map, if not specified, merge will be performed with a blank map</param>
    /// <param name="mask">Optionally specify specific values to include, does not copy other values.</param>
    /// <param name="expand">If true, and the current map is smaller than the other map,
    ///     expands the resulting map to the size of the current map.
    ///     Always true if 'other' map is null</param>
    /// <param name="includeNulls">If true, include null values during the merge (opaque merge).</param>
    /// <returns>A new map representing the product</returns>
    public Map2D<T> Merge(Map2D<T>? other = null, T[]? mask = null, bool expand = false, bool includeNulls = false)
    {
        var width = expand || other is null ? Width : other.Width;
        var height = expand || other is null ? Height : other.Height;
        Map2D<T> newMap = new Map2D<T>(width);
        
        if (other is not null)
            other.CopyTo(newMap);

        while (newMap.Height < Height)
        {
            newMap.AppendRow();
        }
        
        for (var y = 0; y < height ; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var value =  this[x, y];
                if (mask is not null && mask.Contains(value) ||
                    !includeNulls && value is null)
                    continue;
                
                newMap[x,y] = value;
            }
        }

        return newMap;
    }

    public IEnumerable<string> ToStringLines(Func<T?,char>? characterMapper = null)
    {
        characterMapper ??= value => value switch
        {
            not null => 'x',
            _ => ' '
        };

        foreach(var row in _mapData)
        {
            var lineBuffer = new StringBuilder();
            foreach (var col in row)
            {
                var c = characterMapper.Invoke(col);
                lineBuffer.Append(c);
            }
            yield return lineBuffer.ToString();
        }
    }


    public IEnumerator<T> GetEnumerator()
    {
        for (var y = 0; y < Height ; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var value = this[x, y];
                /*
                if (value is null)
                    continue;
                */
                yield return value;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class Map2D
{
    public static Map2D<TValue>? FromReader<TValue>(TextReader reader, Func<char, TValue?>? characterMapper = null)
    {
        characterMapper ??= value => value switch
        {
            _ => default(TValue?)
        };
        var line = reader.ReadLine();
        Map2D<TValue>? map = null;
        while (!string.IsNullOrEmpty(line))
        {
            if (map is null)
            {
                map = new Map2D<TValue>(line.Length);
            }

            var row = map.AppendRow();
            var charValues = line.ToCharArray();
            for (var col = 0; col < row.Length; col++)
            {
                var c = charValues[col];
                var value = characterMapper.Invoke(c);
                row[col] = value;
            }
            line = reader.ReadLine();
        }
        return map;
    }    
}