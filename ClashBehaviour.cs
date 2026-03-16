namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Defines the behavior when adding a column to a DataFrame and a column with the same name already exists
/// </summary>
public enum ClashBehaviour
{
    /// <summary>
    /// Keep the existing column and do not add the new column
    /// </summary>
    KeepOriginal,

    /// <summary>
    /// Remove the existing column and add the new column in its place
    /// </summary>
    ReplaceOriginal,

    /// <summary>
    /// Throw an InvalidOperationException when a name clash occurs (default behavior)
    /// </summary>
    Exception
}