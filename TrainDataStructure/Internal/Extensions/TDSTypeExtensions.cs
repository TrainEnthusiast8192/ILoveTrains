namespace TrainDataStructure.Internal.Extensions;
internal static class TDSTypeExtensions
{
    public unsafe static int ToInt(this bool b) => *(byte*)&b;
    public static bool ToBool(this int n) => n != 0;
}