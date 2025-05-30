namespace TrainDataStructure.Nodes.Interfacing;
public interface ITrainSignalReceiver
{
    public static abstract string GetSignalTemplate();
    public static abstract string? FillTemplate(params object[] parameters);
}