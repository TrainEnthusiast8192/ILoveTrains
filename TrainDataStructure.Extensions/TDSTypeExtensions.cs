namespace TrainDataStructure.Internal.Extensions;
internal static class TDSTypeExtensions
{
    /// <summary>
    /// Converts the given boolean to an integer branchlessly
    /// </summary>
    /// <param name="b">Boolean to convert</param>
    /// <returns>0 if false, 1 if true</returns>
    public unsafe static int ToInt(this bool b) => *(byte*)&b;
    /// <summary>
    /// Converts the given integer to a boolean branchlessly
    /// </summary>
    /// <param name="n">Integer to convert</param>
    /// <returns>false if 0, else true</returns>
    public static bool ToBool(this int n) => n != 0;
}
