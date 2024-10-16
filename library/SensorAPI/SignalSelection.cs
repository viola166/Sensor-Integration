
using System.Collections.ObjectModel;
using System.ComponentModel;
using ShimmerAPI;

namespace SensorAPI;

public class SignalSelection
{

    // sensor information regarding signal accessability of all selected sensors
    public static List<ObservableCollection<object>> signalLists = new List<ObservableCollection<object>>();
    
    // names of signals from single sensor (instance indicated in "headerText")

    private ObservableCollection<string> _names_SignalList = new ObservableCollection<string>();

    public ObservableCollection<string> Names_SignalList
    {
        get { return _names_SignalList; }
        set 
        { 
            _names_SignalList = value; 
            OnPropertyChanged(nameof(Names_SignalList)); 
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public void populate_SignalSelectionList(string headerText) {

        ObservableCollection<object> singleSignalList = new ObservableCollection<object>();
        ObservableCollection<string> displayedSensorNames = new ObservableCollection<string>();

        if (headerText.Equals("Shimmer ECG") || headerText.Equals("Shimmer EMG")) {
            
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.LOW_NOISE_ACCELEROMETER);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.WIDE_RANGE_ACCELEROMETER);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.MAGNETOMETER);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.GYROSCOPE);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.PRESSURE_TEMPERATURE);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.ECG_OR_EMG_1);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.ECG_OR_EMG_2);


            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.LOW_NOISE_ACCELEROMETER.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.WIDE_RANGE_ACCELEROMETER.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.MAGNETOMETER.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.GYROSCOPE.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.PRESSURE_TEMPERATURE.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.ECG_OR_EMG_1.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.ECG_OR_EMG_2.sensorName);

        } else if (headerText.Equals("Shimmer IMU")) {
            
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.LOW_NOISE_ACCELEROMETER);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.WIDE_RANGE_ACCELEROMETER);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.MAGNETOMETER);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.GYROSCOPE);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.PRESSURE_TEMPERATURE);


            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.LOW_NOISE_ACCELEROMETER.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.WIDE_RANGE_ACCELEROMETER.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.MAGNETOMETER.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.GYROSCOPE.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.PRESSURE_TEMPERATURE.sensorName);

        } else if (headerText.Equals("Shimmer GSR")) {

            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.LOW_NOISE_ACCELEROMETER);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.WIDE_RANGE_ACCELEROMETER);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.MAGNETOMETER);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.GYROSCOPE);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.PRESSURE_TEMPERATURE);
            singleSignalList.Add(SignalSelection.ShimmerSensorWithSignals.GSR);

            
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.LOW_NOISE_ACCELEROMETER.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.WIDE_RANGE_ACCELEROMETER.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.MAGNETOMETER.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.GYROSCOPE.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.PRESSURE_TEMPERATURE.sensorName);
            displayedSensorNames.Add(SignalSelection.ShimmerSensorWithSignals.GSR.sensorName); 
        } // else if (headerText.Equals("Muse")) {...}

        signalLists.Add(singleSignalList);
        Names_SignalList = displayedSensorNames;
    }
    
    // Equals DataInfo Class in SensorAPI
    public class ShimmerSensorWithSignals {

        public readonly string sensorName;
        public readonly int sensorBitFlag;
        public readonly List<string> signalNames;


        // Constructor
        public ShimmerSensorWithSignals(string sensorName, int sensorBitFlag, List<string> signalNames) {
            this.sensorName = sensorName;
            this.sensorBitFlag = sensorBitFlag;
            this.signalNames = signalNames;
        }


        public static readonly ShimmerSensorWithSignals LOW_NOISE_ACCELEROMETER = new ShimmerSensorWithSignals("Low Noise Accelerometer", (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL, new List<string> {Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_X, Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Y, Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Z});
        public static readonly ShimmerSensorWithSignals WIDE_RANGE_ACCELEROMETER = new ShimmerSensorWithSignals("Wide Range Accelerometer", (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_D_ACCEL, new List<string> {Shimmer3Configuration.SignalNames.WIDE_RANGE_ACCELEROMETER_X, Shimmer3Configuration.SignalNames.WIDE_RANGE_ACCELEROMETER_Y, Shimmer3Configuration.SignalNames.WIDE_RANGE_ACCELEROMETER_Z});
        public static readonly ShimmerSensorWithSignals MAGNETOMETER = new ShimmerSensorWithSignals("Magnetometer", (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_LSM303DLHC_MAG, new List<string> {Shimmer3Configuration.SignalNames.MAGNETOMETER_X, Shimmer3Configuration.SignalNames.MAGNETOMETER_Y, Shimmer3Configuration.SignalNames.MAGNETOMETER_Z});
        public static readonly ShimmerSensorWithSignals GYROSCOPE = new ShimmerSensorWithSignals("Gyroscope", (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_MPU9150_GYRO, new List<string> {Shimmer3Configuration.SignalNames.GYROSCOPE_X, Shimmer3Configuration.SignalNames.GYROSCOPE_Y, Shimmer3Configuration.SignalNames.GYROSCOPE_Z});
        public static readonly ShimmerSensorWithSignals PRESSURE_TEMPERATURE = new ShimmerSensorWithSignals("Pressure & Temperature", (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_BMP180_PRESSURE, new List<string> {Shimmer3Configuration.SignalNames.PRESSURE, Shimmer3Configuration.SignalNames.TEMPERATURE});
        
        // *** EXG ***

        /*** note that EXG1 and EXG2 can have different signalNames assigned in KeepObjectCluster
        however, in SignalNameArray those signals are referred to as the following ***/

        // EXG 1 > Channel 1 > can refer to... ECG lead Left Leg to Right Arm || EMG channel 1 || not specified
        // EXG 1 > Channel 2 > can refer to... ECG lead Left Arm to Right Arm || EMG channel 2 || not specified
        public static readonly ShimmerSensorWithSignals ECG_OR_EMG_1 = new ShimmerSensorWithSignals("ECG/EMG 1", (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG1_24BIT, new List<string> {Shimmer3Configuration.SignalNames.EXG1_STATUS, Shimmer3Configuration.SignalNames.EXG1_CH1, Shimmer3Configuration.SignalNames.EXG1_CH2});
        
        // EXG 2 > Channel 1 > can refer to... ECG Respiration Demodulation || not specified
        // EXG 2 > Channel 2 > can refer to... ECG vector signal from Wilson's Central Terminal to Vx (chest) position || not specified
        public static readonly ShimmerSensorWithSignals ECG_OR_EMG_2 = new ShimmerSensorWithSignals("ECG/EMG 2", (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG2_24BIT, new List<string> {Shimmer3Configuration.SignalNames.EXG2_STATUS, Shimmer3Configuration.SignalNames.EXG2_CH1, Shimmer3Configuration.SignalNames.EXG2_CH2});

        // *** GSR ***
        public static readonly ShimmerSensorWithSignals GSR = new ShimmerSensorWithSignals("GSR", (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR, new List<string> {Shimmer3Configuration.SignalNames.GSR, Shimmer3Configuration.SignalNames.GSR_CONDUCTANCE});

        
        public static List<ShimmerSensorWithSignals> allSensors = new List<ShimmerSensorWithSignals>
        {
            LOW_NOISE_ACCELEROMETER,
            WIDE_RANGE_ACCELEROMETER,
            MAGNETOMETER,
            GYROSCOPE,
            ECG_OR_EMG_1,
            ECG_OR_EMG_2,
            GSR,
            PRESSURE_TEMPERATURE,
        };

    }
}