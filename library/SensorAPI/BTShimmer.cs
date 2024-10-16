// using InTheHand.Net.Bluetooth for Bluetooth RFCOMM

using InTheHand.Net.Bluetooth;
using InTheHand.Net;
using ShimmerAPI;
using InTheHand.Net.Bluetooth.AttributeIds;
using InTheHand.Net.Sockets;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace SensorAPI{
    public class BTShimmer : ShimmerLogAndStream, SensorUnit
    {

        protected String ShimmerBluetoothAddress;
        Guid g = new Guid("00001101-0000-1000-8000-00805F9B34FB");
        readonly BluetoothClient client = new BluetoothClient();
        BluetoothEndPoint sensor = default!;
        BluetoothAddress addr = default!;
        Stream btStream = Stream.Null;
        int receivedData;
        String devID;
        protected Thread StreamThread;
        string[] dataChannelsToStream;
        public readonly object lockObj = new object();


        private List<SignalsWithData> _lastStreamedData = new List<SignalsWithData>();
        public List<SignalsWithData> LastStreamedData 
        { 
            get 
            { 
                lock(lockObj) {
                    return _lastStreamedData;
                }
            }
            set {
                lock(lockObj) {
                    _lastStreamedData = value;
                }
            }
        }

        // called in constructors
        public void Initializing(String devID, String bluetoothAddress) {
            ShimmerBluetoothAddress = bluetoothAddress;
            this.addr = BluetoothAddress.Parse(bluetoothAddress);
            this.devID = devID;
        }

        // *** CONSTRUCTORS *** 
        public BTShimmer(String devID, String bluetoothAddress)
            : base(devID)
        {
            Initializing(devID, bluetoothAddress);

        }

        public BTShimmer(String devID, String bluetoothAddress, double samplingRate, int accelRange, int gsrRange, int setEnabledSensors, bool enableLowPowerAccel, bool enableLowPowerGyro, bool enableLowPowerMag, int gyroRange, int magRange, byte[] exg1configuration, byte[] exg2configuration, bool internalexppower)
            : base(devID, samplingRate, accelRange, gsrRange, setEnabledSensors, enableLowPowerAccel, enableLowPowerGyro, enableLowPowerMag, gyroRange, magRange, exg1configuration, exg2configuration, internalexppower) {
            
            Initializing(devID, bluetoothAddress);

            }

        // *** PROTECTED INHERITED METHODS (ShimmerLogAndStream) ***
        protected override bool IsConnectionOpen()
        {
            Console.WriteLine("IsConnectionOpen()" + client.Connected);
            return client.Connected;
        }

        protected override void CloseConnection()
        {
            btStream.Close();
        }

        protected override void OpenConnection()
        {
            Console.WriteLine("in OpenConnection()");
            sensor = new BluetoothEndPoint(addr, g);
            SetState(SHIMMER_STATE_CONNECTING);
            client.Connect(addr, BluetoothProtocol.RFCommProtocol);
            btStream = client.GetStream();
            Console.WriteLine("Client connected?: " + client.Connected);
        }

        protected override void FlushConnection()
        {
            btStream.Flush();
        }

        protected override void FlushInputConnection()
        {
            btStream.Flush();
        }

        protected override void WriteBytes(byte[] b, int index, int length)
        {
            // btStream.Write(b, index, length);

            // identifing the integers/bytes to trace the code
            btStream.Write(b, index, length);
            Console.WriteLine("start, length: " + length);

            for (int i = 0; i < length; i++) {
                Console.WriteLine(b[i]);
            }
            Console.WriteLine("stop");
        }

        protected override int ReadByte()
        {
            int byteRead = btStream.ReadByte();

            // if no data is received and the state is "streaming" --> error
            if (byteRead == -1 && GetState() == ShimmerBluetooth.SHIMMER_STATE_STREAMING)
            {
                CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, "Connection lost");
                OnNewEvent(newEventArgs);
                Disconnect();

            // if no data is received and the state is "connected" --> error
            } else if (byteRead == -1 && GetState() == ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
            {
                CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, "Connection lost");
                OnNewEvent(newEventArgs);
                Disconnect();
            }
            return byteRead;
        }

        // *** PUBLIC INHERITED METHODS ***
        public override string GetShimmerAddress()
        {
            return ShimmerBluetoothAddress;
        }

        public override void SetShimmerAddress(string address)
        {
            ShimmerBluetoothAddress = address;
            this.addr = BluetoothAddress.Parse(address);
        }

        // *** INTERFACE METHODS ***
        public void StreamSignalData(string[] dataChannels) {
            dataChannelsToStream = dataChannels;
            StreamThread = new Thread(new ThreadStart(Streaming));
            StreamThread.Name = "Stream Thread for Device: " + DeviceName;
            StreamThread.Start();

            System.Threading.Thread.Sleep(500);
        }

        public void Streaming() {
            List<SignalsWithData> fillStreamedData = new List<SignalsWithData>();
            // to store all sensor information in ShimmerBluetooth attributes call ReadData()
            // ReadData() will read bytes and interpret them accordingly if state == connected
            // ReadData() will read bytes and build/store object cluster (KeepObjectCluster) if state == streaming
            while (!StopReading) {
                fillStreamedData.Clear();
                for (int i = 0; i < dataChannelsToStream.Length; i++) {
                    // List<string> allStoredSignalNames = KeepObjectCluster.GetNames();
                    List<double> signalDataTypes = KeepObjectCluster.GetData();
                    
                    if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.GYROSCOPE_X) {
                        if (KeepObjectCluster != null) {

                            // get Index of specified signal in the list of signal names from the last object cluster
                            int indexGyroscope_X = getSignalIndex(Shimmer3Configuration.SignalNames.GYROSCOPE_X);
                            SignalsWithData gyro_x_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexGyroscope_X]);
                            fillStreamedData.Add(gyro_x_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.GYROSCOPE_Y) {
                        if (KeepObjectCluster != null) {

                            int indexGyroscope_Y = getSignalIndex(Shimmer3Configuration.SignalNames.GYROSCOPE_Y);
                            SignalsWithData gyro_y_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexGyroscope_Y]);
                            fillStreamedData.Add(gyro_y_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.GYROSCOPE_Z) {
                        if (KeepObjectCluster != null) {

                            int indexGyroscope_Z = getSignalIndex(Shimmer3Configuration.SignalNames.GYROSCOPE_Z);
                            SignalsWithData gyro_z_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexGyroscope_Z]);
                            fillStreamedData.Add(gyro_z_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_X) {
                        if (KeepObjectCluster != null) {

                            int indexLNAccel_X = getSignalIndex(Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_X);
                            SignalsWithData ln_accel_x_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexLNAccel_X]);
                            fillStreamedData.Add(ln_accel_x_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Y) {
                        if (KeepObjectCluster != null) {

                            int indexLNAccel_Y = getSignalIndex(Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Y);
                            SignalsWithData ln_accel_y_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexLNAccel_Y]);
                            fillStreamedData.Add(ln_accel_y_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Z) {
                        if (KeepObjectCluster != null) {

                            int indexLNAccel_Z = getSignalIndex(Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Z);
                            SignalsWithData ln_accel_z_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexLNAccel_Z]);
                            fillStreamedData.Add(ln_accel_z_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.WIDE_RANGE_ACCELEROMETER_X) {
                        if (KeepObjectCluster != null) {

                            int indexWDAccel_X = getSignalIndex(Shimmer3Configuration.SignalNames.WIDE_RANGE_ACCELEROMETER_X);
                            SignalsWithData wr_accel_x_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexWDAccel_X]);
                            fillStreamedData.Add(wr_accel_x_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.WIDE_RANGE_ACCELEROMETER_Y) {
                        if (KeepObjectCluster != null) {

                            int indexWDAccel_Y = getSignalIndex(Shimmer3Configuration.SignalNames.WIDE_RANGE_ACCELEROMETER_Y);
                            SignalsWithData wr_accel_y_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexWDAccel_Y]);
                            fillStreamedData.Add(wr_accel_y_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.WIDE_RANGE_ACCELEROMETER_Z) {
                        if (KeepObjectCluster != null) {

                            int indexWDAccel_Z = getSignalIndex(Shimmer3Configuration.SignalNames.WIDE_RANGE_ACCELEROMETER_Z);
                            SignalsWithData wr_accel_z_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexWDAccel_Z]);
                            fillStreamedData.Add(wr_accel_z_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.MAGNETOMETER_X) {
                        if (KeepObjectCluster != null) {

                            int indexMag_X = getSignalIndex(Shimmer3Configuration.SignalNames.MAGNETOMETER_X);
                            SignalsWithData mag_x_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexMag_X]);
                            fillStreamedData.Add(mag_x_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.MAGNETOMETER_Y) {
                        if (KeepObjectCluster != null) {

                            int indexMag_Y = getSignalIndex(Shimmer3Configuration.SignalNames.MAGNETOMETER_Y);
                            SignalsWithData mag_y_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexMag_Y]);
                            fillStreamedData.Add(mag_y_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.MAGNETOMETER_Z) {
                        if (KeepObjectCluster != null) {

                            int indexMag_Z = getSignalIndex(Shimmer3Configuration.SignalNames.MAGNETOMETER_Z);
                            SignalsWithData mag_z_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexMag_Z]);
                            fillStreamedData.Add(mag_z_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.PRESSURE) {
                        if (KeepObjectCluster != null) {

                            int indexPressure = getSignalIndex(Shimmer3Configuration.SignalNames.PRESSURE);
                            SignalsWithData pressure_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexPressure]);
                            fillStreamedData.Add(pressure_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.TEMPERATURE) {
                        if (KeepObjectCluster != null) {

                            int indexTemp = getSignalIndex(Shimmer3Configuration.SignalNames.TEMPERATURE);
                            SignalsWithData temp_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexTemp]);
                            fillStreamedData.Add(temp_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.GSR) {
                        if (KeepObjectCluster != null) {

                            int indexGSR = getSignalIndex(Shimmer3Configuration.SignalNames.GSR);
                            SignalsWithData gsr_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexGSR]);
                            fillStreamedData.Add(gsr_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.GSR_CONDUCTANCE) {
                        if (KeepObjectCluster != null) {

                            int indexGSR_C = getSignalIndex(Shimmer3Configuration.SignalNames.GSR_CONDUCTANCE);
                            SignalsWithData gsr_con_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexGSR_C]);
                            fillStreamedData.Add(gsr_con_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.EXG1_CH1) {
                        if (KeepObjectCluster != null) {

                            int indexEXG1_CH1 = getSignalIndex(Shimmer3Configuration.SignalNames.EXG1_CH1);
                            SignalsWithData exg1_ch1_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexEXG1_CH1]);
                            fillStreamedData.Add(exg1_ch1_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.EXG1_CH2) {
                        if (KeepObjectCluster != null) {

                            int indexEXG1_CH2 = getSignalIndex(Shimmer3Configuration.SignalNames.EXG1_CH2);
                            SignalsWithData exg1_ch2_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexEXG1_CH2]);
                            fillStreamedData.Add(exg1_ch2_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.EXG2_CH1) {
                        if (KeepObjectCluster != null) {

                            int indexEXG2_CH1 = getSignalIndex(Shimmer3Configuration.SignalNames.EXG2_CH1);
                            SignalsWithData exg2_ch1_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexEXG2_CH1]);
                            fillStreamedData.Add(exg2_ch1_info);
                        }
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.EXG2_CH2) {
                        if (KeepObjectCluster != null) {

                            int indexEXG2_CH2 = getSignalIndex(Shimmer3Configuration.SignalNames.EXG2_CH2);
                            SignalsWithData exg2_ch2_info = new SignalsWithData(devID, dataChannelsToStream[i], signalDataTypes[indexEXG2_CH2]);
                            fillStreamedData.Add(exg2_ch2_info);
                        }
                    }

                }
                
                lock(lockObj) {
                    LastStreamedData.Clear();
                    LastStreamedData.AddRange(fillStreamedData);
                }
            }
        }


        // method should be existing in ShimmerLogAndStream, but doesn't... 
        public void StopStreamingandLog() {
            // 151 = 0x97 : STOP_SDBT_COMMAND
            WriteBytes(new byte[1] { (byte) 151}, 0, 1);
            // WriteBytes(new byte[1] { (byte)ShimmerBluetooth.PacketTypeShimmer3SDBT.STOP_SDBT_COMMAND }, 0, 1);
            System.Threading.Thread.Sleep(200);
            isLogging = false;
            FlushInputConnection();
            ObjectClusterBuffer.Clear();
            StopReading = true;
            if (IsConnectionOpen())
            {
                SetState(SHIMMER_STATE_CONNECTED);
            }
        }
    }
}
