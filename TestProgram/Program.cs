// See https://aka.ms/new-console-template for more information

using InTheHand.Net.Sockets;
using SensorAPI;
using ShimmerAPI;
using System;

namespace TestProgram
{
    public class Program
    {
        static void Main(string[] args)
        {
            /***
            BluetoothDeviceInfo deviceInfo = FindSensorUnit().GetAwaiter().GetResult();
            BluetoothAddress sensorUnitAdress = (BluetoothAddress) deviceInfo.DeviceAddress;
            BTShimmer deviceBt = new BTShimmer("test", deviceInfo.DeviceAddress.ToString());
            ***/


            // MANUALLY CONNECT TO SHIMMER DEVICE VIA BLUETOOTH
            
            // TEST
            
            int enabledSensors = ((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG1_24BIT | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG2_24BIT); // | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_LSM303DLHC_MAG | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_MPU9150_GYRO);
            double samplingRate = 51.2;

            byte[] defaultECGReg1 = ShimmerBluetooth.SHIMMER3_DEFAULT_TEST_REG1;
            byte[] defaultECGReg2 = ShimmerBluetooth.SHIMMER3_DEFAULT_TEST_REG2;
            
            Console.WriteLine("Please enter Address of the Shimmer Device you want to pair");
            var shimmerAddress = Console.ReadLine(); // "fc:0f:e7:b5:6a:66"
            // BTShimmer deviceBt = new BTShimmer("test", shimmerAddress);

            BTShimmer deviceBt = new BTShimmer("Shimmer_6A66", shimmerAddress, samplingRate, 0, ShimmerBluetooth.GSR_RANGE_AUTO, enabledSensors, false, false, false, 1, 0, defaultECGReg1, defaultECGReg2, false);
            
            // BUG: Doesn't write on sdlog.cfg --> accessability problems?
            // specify trial
            Console.WriteLine("Please enter the trial ID");
            var trialID = Console.ReadLine();
            deviceBt.SetExperimentID(trialID);
            deviceBt.WriteExpID();  

            // write updated sd configuration settings in sdlog.cfg
            deviceBt.SdConfigWrite();

            bool wait = true;

            while (wait) {
                if (Console.ReadLine() == "x") {
                    wait = false;
                }
            }

            deviceBt.Connect();

            wait = true;

            while (wait) {
                if (Console.ReadLine() == "x") {
                    wait = false;
                }
            }


            deviceBt.StartStreamingandLog();
            
            wait = true;

            while (wait) {
                if (Console.ReadLine() == "x") {
                    wait = false;
                }
            }

            string[] dataChannels = ["Gyroscope Z"];
            deviceBt.StreamSignalData(dataChannels);
            

            bool streaming = true;

            while (streaming) {
                if (Console.ReadLine() == "x") {
                    streaming = false;
                }

            }

            // Check State: should be 3
            Console.WriteLine(deviceBt.GetState());
            
            deviceBt.StopStreamingandLog();

            // Check State: should be 2
            Console.WriteLine(deviceBt.GetState());

            wait = true;

            while (wait) {
                if (Console.ReadLine() == "x") {
                    wait = false;
                }
            }

            deviceBt.Disconnect();

            // Check State: should be 0
            Console.WriteLine(deviceBt.GetState());

        }

        /***
        protected static async Task<BluetoothDeviceInfo> FindSensorUnit() {
            BluetoothDevicePicker devicePicker = new BluetoothDevicePicker();
            var deviceInfo = await devicePicker.PickSingleDeviceAsync();

            return deviceInfo;
        }
        ***/
    }
}