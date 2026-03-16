using System;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// A RollingWindow is a subset of cells from a SourceColumn.
/// </summary>
/// <typeparam name="T"></typeparam>
public class RollingWindow<T>
    where T : unmanaged
{
    public PrimitiveDataFrameColumn<T> SourceColumn { get; }
    public int                         WindowSize   { get; }

    public RollingWindow(PrimitiveDataFrameColumn<T> column, int windowSize)
    {
        this.SourceColumn = column ?? throw new ArgumentNullException(nameof(column));
        this.WindowSize   = windowSize > 0 ? windowSize : throw new ArgumentOutOfRangeException(nameof(windowSize), "WindowSize must be greater than 0");
    }
}