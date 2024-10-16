namespace SensorAPI{
    public class SignalsWithData {
        // which sensor captured the data
        public string sensorName;
        // which kind of data is provided
        public string dataChannel;
        // value of the data
        public double data;
        public SignalsWithData(string sensorName, string dataChannel, double data) {
            this.sensorName = sensorName;
            this.dataChannel = dataChannel;
            this.data = data;
        }
    }
}
