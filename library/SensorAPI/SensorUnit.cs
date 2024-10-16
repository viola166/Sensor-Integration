namespace SensorAPI{
    public interface SensorUnit {

        List<SignalsWithData> LastStreamedData { get; set; }
        
        // stream the specified data by a specified sensor (the object which calls GetData)
        // probably change return type to void
        void StreamSignalData(string[] dataChannels);

        // collect the specified data from the logging and extend already collectedData
        // SignalsWithData[] CollectSignalData(string[] dataChannels, DataInfo[] collectedData);


    }
}
