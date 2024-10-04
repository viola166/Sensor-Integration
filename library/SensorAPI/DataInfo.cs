namespace SensorAPI{
    public class DataInfo {
        // which sensor captured the data
        public string sensorName;
        // which kind of data is provided
        public string dataChannel;
        // value of the data
        public double data;
        public DataInfo(string sensorName, string dataChannel, double data) {
            this.sensorName = sensorName;
            this.dataChannel = dataChannel;
            this.data = data;
        }
    }
}