// using InTheHand.Net.Bluetooth for Bluetooth RFCOMM

using InTheHand.Net.Bluetooth;
using InTheHand.Net;
using ShimmerAPI;
using InTheHand.Net.Bluetooth.AttributeIds;
using InTheHand.Net.Sockets;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace SensorAPI{
    public class BTShimmer : ShimmerLogAndStream, SensorInfos
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
            List<DataInfo> dataInfos = new List<DataInfo>();
            // to store all sensor information in ShimmerBluetooth attributes call ReadData()
            // ReadData() will read bytes and interpret them accordingly if state == connected
            // ReadData() will read bytes and build/store object cluster (KeepObjectCluster) if state == streaming
            while (!StopReading) {
                for (int i = 0; i < dataChannelsToStream.Length; i++) {
                    // List<string> allStoredSignalNames = KeepObjectCluster.GetNames();
                    List<double> signalDataTypes = KeepObjectCluster.GetData();
                    if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.ECG_LA_RA) {
                        Console.WriteLine("in ECG_LA_RA");
                        if (KeepObjectCluster != null) {
                            
                            // get Index of specified signal in the list of signal names from the last object cluster
                            int indexECG_LA_RA = getSignalIndex(Shimmer3Configuration.SignalNames.ECG_LA_RA);
                            DataInfo ecg_la_ra_info = new DataInfo(devID, dataChannelsToStream[i], signalDataTypes[indexECG_LA_RA]);
                            dataInfos.Add(ecg_la_ra_info);
                        }
                        break;
                    } else if (dataChannelsToStream[i] == Shimmer3Configuration.SignalNames.GYROSCOPE_Z) {
                        Console.WriteLine("in GYROSCOPE_Z");
                        if (KeepObjectCluster != null) {

                            int indexGyroscope_Z = getSignalIndex(Shimmer3Configuration.SignalNames.GYROSCOPE_Z);
                            DataInfo gyro_z_info = new DataInfo(devID, dataChannelsToStream[i], signalDataTypes[indexGyroscope_Z]);
                            dataInfos.Add(gyro_z_info);
                        }
                        break;
                    } // else if...

                }
                System.Threading.Thread.Sleep(1000);

                DataInfo lastDataInfo = dataInfos.Last();
                Console.WriteLine(lastDataInfo.dataChannel + ": " + lastDataInfo.data);
            }
            DataInfo[] dataInfosArray = dataInfos.ToArray();
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