using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// structure referenced from my discord bot
// modified for unity

// i know [,] arrays aren't as efficient, but i honestly don't care lol
public class Grid<T> : IEnumerable<T>
{
    public Grid(int wh, T defaultValue = default) : this(wh, wh, defaultValue) { }

    public Grid(int width, int height, T defaultValue = default)
    {
        Values = new T[height, width];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Values[y, x] = defaultValue;
            }
        }
    }

    public Grid(T[,] values)
    {
        Values = values;
    }

    public Grid(T[][] values, T defaultValue = default)
    {
        int height = values.GetUpperBound(0) + 1;
        int width = values.OrderByDescending(v => v.GetUpperBound(0)).First().GetUpperBound(0) + 1;

        Values = new T[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y >= 0 && y <= values.GetUpperBound(0))
                {
                    if (x >= 0 && x <= values[y].GetUpperBound(0))
                    {
                        Values[y, x] = values[y][x];
                        continue;
                    }
                }

                Values[y, x] = defaultValue;
            }
        }
    }

    public T[,] Values { get; }

    public int Width => Values.GetLength(1);
    public int Height => Values.GetLength(0);

    public int Count => Values.Length;

    public IEnumerator<T> GetEnumerator()
        => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator()
        => new Enumerator(this);

    public Grid<T> Clone()
        => new Grid<T>(Values);

    public bool IsInRange(int x, int y)
        => (x >= 0 && x < Width && y >= 0 && y < Height);

    public void SetValue(T value, int x, int y)
    {
        Values[y, x] = value;
    }

    public T GetValue(int x, int y)
    {
        return Values[y, x];
    }

    public bool TryGetValue(int x, int y, out T value)
    {
        value = default;

        if (!IsInRange(x, y))
        {
            return false;
        }

        value = Values[y, x];
        return true;
    }

    public T ValueAt(int i)
    {
        (int x, int y) = GetPosition(i);
        return Values[y, x];
    }

    private (int x, int y) GetPosition(int i)
    {
        int x = i;
        int y = 0;

        while (x >= Width)
        {
            x -= Width;
            y++;
        }

        return (x, y);
    }

    public T this[int x, int y]
    {
        get => Values[y, x];
        set => SetValue(value, x, y);
    }

    public T this[int i]
    {
        get => ValueAt(i);
        set
        {
            (int x, int y) = GetPosition(i);
            Values[y, x] = value;
        }
    }

    /// <summary>
    /// Clears the existing <see cref="Grid{T}"/> using a specified value.
    /// </summary>
    public void Clear(T value)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Values[y, x] = value;
    }

    /// <summary>
    /// Projects each element of a <see cref="Grid{T}"/> into a new form.
    /// </summary>
    /// <param name="selector">A transform function to apply to each element.</param>
    public Grid<TValue> Select<TValue>(Func<T, TValue> selector)
    {
        var result = new Grid<TValue>(Width, Height);
        ForEachValue((value, x, y) => result.SetValue(selector.Invoke(value), x, y));

        return result;
    }

    /// <summary>
    /// Determines whether all elements of a <see cref="Grid{T}"/> satisfy a condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    public bool All(Func<T, bool> predicate)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                if (!predicate.Invoke(Values[y, x]))
                    return false;

        return true;
    }

    /// <summary>
    /// Sets all of the elements of a row to a specified value.
    /// </summary>
    /// <param name="y">The index of the row to set.</param>
    /// <param name="value">The value that will be set for each element in the row.</param>
    public void SetRow(int y, T value)
    {
        for (int x = 0; x < Width; x++)
            Values[y, x] = value;
    }

    public void SetRow(int y, IEnumerable<T> values)
    {
        if (values.Count() > Width)
            throw new ArgumentException("The row specified must be less than or equal to the width of the grid.");

        for (int x = 0; x < Width; x++)
            Values[y, x] = values.ElementAtOrDefault(x);
    }

    /// <summary>
    /// Returns the row of the existing <see cref="Grid{T}"/> by a specified row index.
    /// </summary>
    public T[] GetRow(int y)
    {
        T[] row = new T[Width];

        for (int x = 0; x < Width; x++)
            row[x] = Values[y, x];
        return row;
    }

    public void SetColumn(int x, T value)
    {
        for (int y = 0; y < Height; y++)
            Values[y, x] = value;
    }

    public void SetColumn(int x, IEnumerable<T> values)
    {
        if (values.Count() > Height)
            throw new ArgumentException("The column specified must be less than or equal to the height of the grid.");

        for (int y = 0; y < Height; y++)
            Values[y, x] = values.ElementAtOrDefault(y);
    }

    /// <summary>
    /// Returns the column of the existing <see cref="Grid{T}"/> by a specified column index.
    /// </summary>
    public T[] GetColumn(int x)
    {
        T[] column = new T[Height];

        for (int y = 0; y < Height; y++)
            column[y] = Values[y, x];

        return column;
    }

    public Grid<T> GetRegion(int x, int y, int width, int height)
    {
        if (!IsInRange(x + width, y + height))
            throw new ArgumentException("The region specified is out of bounds.");

        var region = new Grid<T>(width, height);

        for (int py = 0; py < height; py++)
            for (int px = 0; px < width; px++)
                region.SetValue(GetValue(x + px, y + py), px, py);

        return region;
    }

    // NOTE:
    // Attempts to get the values of a partial region
    // Anything out of the current bounds of the grid is left alone
    public Grid<T> GetPartialRegion(int x, int y, int width, int height)
    {
        var region = new Grid<T>(width, height);

        for (int py = 0; py < height; py++)
            for (int px = 0; px < width; px++)
                if (IsInRange(px + x, py + y))
                    region.SetValue(Values[y + py, x + px], px, py);

        return region;
    }

    public void SetRegion(T value, int x, int y, int width, int height)
    {
        if (!IsInRange(x + width, y + height))
            throw new ArgumentException("The region specified is out of bounds.");

        for (int py = 0; py < height; py++)
            for (int px = 0; px < width; px++)
                SetValue(value, px + x, py + y);
    }

    public void SetRegion(T[,] region, int x, int y)
    {
        int width = region.GetLength(1);
        int height = region.GetLength(0);

        if (!IsInRange(x + width, y + height))
            throw new ArgumentException("The region specified is out of bounds.");

        for (int py = 0; py < height; py++)
            for (int px = 0; px < width; px++)
                SetValue(region[py, px], px + x, py + y);
    }

    public void SetRegion(Grid<T> region, int x, int y)
        => SetRegion(region.Values, x, y);

    public void SetEachValue(Func<T> action)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Values[y, x] = action.Invoke();
    }

    public void SetEachValue(Func<int, int, T> action)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Values[y, x] = action.Invoke(x, y);
    }

    public void SetEachValue(Func<T, T> action)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Values[y, x] = action.Invoke(Values[y, x]);
    }

    public void SetEachValue(Func<T, int, int, T> action)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Values[y, x] = action.Invoke(Values[y, x], x, y);
    }

    public void ForEachValue(Action<int, int> action)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                action.Invoke(x, y);
    }

    public void ForEachValue(Action<T> action)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                action.Invoke(Values[y, x]);
    }

    public void ForEachValue(Action<T, int, int> action)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                action.Invoke(Values[y, x], x, y);
    }

    public void ForEachRow(Action<T[]> action)
    {
        for (int y = 0; y < Height; y++)
            action.Invoke(GetRow(y));
    }

    public void ForEachRow(Action<T[], int> action)
    {
        for (int y = 0; y < Height; y++)
            action.Invoke(GetRow(y), y);
    }

    public void ForEachColumn(Action<T[]> action)
    {
        for (int x = 0; x < Width; x++)
            action.Invoke(GetColumn(x));
    }

    public void ForEachColumn(Action<T[], int> action)
    {
        for (int x = 0; x < Width; x++)
            action.Invoke(GetColumn(x), x);
    }

    public override string ToString()
    {
        return $"Grid<{typeof(T).Name}> => [{Width}, {Height}]";
    }

    public class Enumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private readonly T[,] _values;
        private int _position = -1;
        private T _current;

        public Enumerator(Grid<T> grid)
        {
            _values = grid.Values;
            _current = default;
        }

        object IEnumerator.Current
        {
            get => Current;
        }

        public T Current
        {
            get => _current;
        }

        public bool MoveNext()
        {
            if (++_position >= _values.Length)
            {
                return false;
            }
            else
            {
                _current = GetCurrent();
            }

            return true;
        }

        public void Reset()
        {
            _position = -1;
        }

        private T GetCurrent()
        {
            int x = _position;
            int width = _values.GetLength(1);
            int y = 0;

            while (x >= width)
            {
                x -= width;
                y++;
            }

            return _values[y, x];
        }

        void IDisposable.Dispose() { }
    }
}
