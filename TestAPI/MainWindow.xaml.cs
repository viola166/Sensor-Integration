using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShimmerAPI;
using SensorAPI;
using LiveCharts;
using System.Windows.Automation.Peers;
using Microsoft.Windows.Themes;
using System.Windows.Threading;
using MathNet.Numerics.LinearAlgebra.Solvers;

namespace TestAPI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{

    public ObservableCollection<SensorInfos> SensorInformation { get; set; } = new ObservableCollection<SensorInfos>{
        new SensorInfos("Shimmer ECG",false),
        new SensorInfos("Shimmer EMG",false),
        new SensorInfos("Shimmer IMU",false),
        new SensorInfos("Shimmer GSR",false),
        new SensorInfos("Muse",false),
        new SensorInfos("Embrace Plus",false)
    };
    

    // eventhandler for dropbox
    public event PropertyChangedEventHandler PropertyChanged;

    // number of sensors selected
    private int sensorCount;

    // Timer to set sampling rate for data
    private DispatcherTimer updateTimer;

    // enabled Shimmer Sensors
    // private int enabledShimmerSensors;

    // all selected sensors with names, and indication whether selected [all selected] (not actual unit)
    public List<SensorInfos> selectedSensors = new List<SensorInfos>();

    // all selected sensors as actual units
    public List<SensorUnit> initializedSensors = new List<SensorUnit>();

    // all listboxes (with signal names) of selected sensors
    public List<ListBox> listBoxesSensorSignals = new List<ListBox>();

    public List<List<TextBlock>> signalTextBlocks = new List<List<TextBlock>>();

    // text at dropbox top
    private string _contentText;
    // Property
    public string ContentText
    {
        get => _contentText;            // getter
        set                             // setter
        {
            if (value != _contentText)
            {
                _contentText = value;
                OnPropertyChanged();    // call eventhandler method when property/content changes
            }
        }
    }

    private bool _enabledStartButton = false;

    public bool EnabledStartButton
    {
        get => _enabledStartButton;
        set 
        {
            if (value != _enabledStartButton) {
                _enabledStartButton = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _enabledStopButton = true;
    public bool EnabledStopButton
    {
        get => _enabledStopButton;
        set 
        {
            if (value != _enabledStopButton) {
                _enabledStopButton = value;
                OnPropertyChanged();
            }
        }
    }

    public MainWindow()
    {
        this.InitializeComponent();
        this.Loaded += MainWindow_Loaded;

        this.DataContext = this;
    }


    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    void OnClick_SubmitSensors(object sender, RoutedEventArgs e) {
        InitializeSensorTabs(SensorInformation);
    }

    async void OnClick_SubmitSignals(object sender, RoutedEventArgs e) {
        await InitializeSensors();
        InitializePlot();
        SetUpLiveStreaming();
        SampleData();
        StartAndStopStreamingAndLogButtons();
    }

    void OnClick_StartStreamingAndLogging(object sender, RoutedEventArgs e) {
        EnabledStartButton = false;
        foreach (SensorUnit unit in initializedSensors) {
            if (unit is BTShimmer) {
                BTShimmer shimmer = (BTShimmer) unit;
                shimmer.StartStreamingandLog();
            }
        }
        EnabledStopButton = true;
    }

    void OnClick_StopStreamingAndLogging(object sender, RoutedEventArgs e) {
        EnabledStopButton = false;
        foreach (SensorUnit unit in initializedSensors) {
            if (unit is BTShimmer) {
                BTShimmer shimmer = (BTShimmer) unit;
                shimmer.StopStreamingandLog();
            }
        }
        EnabledStartButton = true;
    }


    /* Called after submitting sensor selectiion
    Initializes listboxes for each selected sensor in different tabs */

    private void InitializeSensorTabs(ObservableCollection<SensorInfos> SensorInformation) {

        selectedSensors.Clear();

        // get final selection of sensors
        foreach(SensorInfos unit in SensorInformation) {
            if (unit.IsChecked) {
                selectedSensors.Add(unit);
            }
        }

        foreach(SensorInfos unit in selectedSensors) {
            // create tab
            TabItem newTab = new TabItem();
            string headerText = unit.Name;
            newTab.Header = headerText;

            // create listbox (list with signal names)
            ListBox listBox = new ListBox();
            // create the list dependent on the sensor
            SignalSelection list = new SignalSelection();

            list.populate_SignalSelectionList(headerText);
            listBox.SetBinding(ListBox.ItemsSourceProperty, new Binding("Names_SignalList"));
            listBox.SelectionMode = SelectionMode.Multiple;

            listBox.DataContext = list;

            newTab.Content = listBox;

            tabControl.Items.Add(newTab);

            listBoxesSensorSignals.Add(listBox);
        }

    }

    private void InitializePlot(){
    }

    private void SetUpLiveStreaming() {
        // get dataChannels to be streamed dependent on signal selection 
        for (int i = 0; i < selectedSensors.Count; i++) {
            // inner list: length -> num of sensors | entries -> signals per sensor; outer list: units
            List<List<string>> signalsToStream = new List<List<string>>();
            // for each selected signal in the listbox of sensor i
            foreach (var signal in listBoxesSensorSignals[i].SelectedItems) {
                // if sensor i is a Shimmer sensor
                if (selectedSensors[i].Name.Contains("Shimmer")) {
                    // get the ShimmerSensorWithSignals member with the respective signal name
                    var sensor = SignalSelection.ShimmerSensorWithSignals.allSensors.SingleOrDefault(s => s.sensorName.Equals((string)signal));
                    signalsToStream.Add(sensor.signalNames);
                }
            }

            string[] signalsToStreamArray = signalsToStream.SelectMany(sublist => sublist).ToArray();
            // start streaming specified signals of that sensor
            initializedSensors[i].StreamSignalData(signalsToStreamArray);
        }
    }

    private void SampleData() {
        for(int i = 0; i < selectedSensors.Count; i++) {
            
            // find corresponding Container (StackPanel) to edit in xaml file
            string containerName = "";

            if (selectedSensors[i].Name.Equals("Shimmer ECG")) {
                containerName = "dataShimmerECG";
            } else if (selectedSensors[i].Name.Equals("Shimmer EMG")) {
                containerName = "dataShimmerEMG";   
            } else if (selectedSensors[i].Name.Equals("Shimmer IMU")) {
                containerName = "dataShimmerIMU";
            } else if (selectedSensors[i].Name.Equals("Shimmer GSR")) {
                containerName = "dataShimmerGSR";
            } else if (selectedSensors[i].Name.Equals("Muse")) {
                containerName = "dataMuse"; 
            } else if (selectedSensors[i].Name.Equals("Embrace Plus")) {
                containerName = "dataEmbracePlus"; 
            }

            var textBlockContainer = (StackPanel)this.FindName(containerName);

            // get sensor unit
            SensorUnit unit = initializedSensors[i];
            BTShimmer shimmerUnit = (BTShimmer) unit;

            List<TextBlock> signalTextBlock = new List<TextBlock>();

            // lock(shimmerUnit.lockObj) {
                // get number of signals that this sensor unit streams
                int numSignals = unit.LastStreamedData.Count;
                
                textBlockContainer.Visibility = Visibility.Visible;
                
                for (int j = 0; j < numSignals; j++) {

                    string dataChannel = unit.LastStreamedData[j].dataChannel;
                    double data = unit.LastStreamedData[j].data;

                    TextBlock newTextBlock = new TextBlock
                    {
                        Text = dataChannel + ": " + data,
                    };

                    textBlockContainer.Children.Add(newTextBlock);

                    signalTextBlock.Add(newTextBlock);
                }

                signalTextBlocks.Add(signalTextBlock);

                // textBlockContainer.Visibility = Visibility.Visible;
            // }
        }
        StartPeriodicUpdate();
    }

    private void StartPeriodicUpdate() {
        // Set up a timer to update the TextBlock every few seconds
        updateTimer = new DispatcherTimer();
        updateTimer.Interval = TimeSpan.FromSeconds(1);  // Update every 2 seconds
        updateTimer.Tick += (s, e) => {
            for (int i = 0; i < initializedSensors.Count; i++) {
                SensorUnit unit = initializedSensors[i];
                int numSignals = unit.LastStreamedData.Count;
                // Update the TextBlock with the current value of the BTShimmer property
                for (int j = 0; j < numSignals; j++) {
                    signalTextBlocks[i][j].Text = unit.LastStreamedData[j].dataChannel + ": " + unit.LastStreamedData[j].data;
                }
            }
        };

        updateTimer.Start();
    }

    private void StartAndStopStreamingAndLogButtons() {
        startStreamingAndLoggingButton.Visibility = Visibility.Visible;
        stopStreamingAndLoggingButton.Visibility = Visibility.Visible;
    }

    private async Task InitializeSensors() {
        textConnecting.Visibility = Visibility.Visible;
        List<string> shimmerAddresses = new List<string>();
        List<TextBlock> textsConnecting = new List<TextBlock>();
        int listIndex = 0;
        for (int i = 0; i < selectedSensors.Count; i++) {
            string sensorName = selectedSensors[i].Name;

            // indicate processing status in UI
            string textBlockName = "";
            
            if (sensorName.Equals("Shimmer ECG")) {
                textBlockName = "textConnectingShimmerECG";
                shimmerAddresses.Add(OpenandReadTextBox(sensorName));
            } else if (sensorName.Equals("Shimmer EMG")) {
                textBlockName = "textConnectingShimmerEMG";
                shimmerAddresses.Add(OpenandReadTextBox(sensorName));
            } else if (sensorName.Equals("Shimmer IMU")) {
                textBlockName = "textConnectingShimmerIMU";
                shimmerAddresses.Add(OpenandReadTextBox(sensorName));
            } else if (sensorName.Equals("Shimmer GSR")) {
                textBlockName = "textConnectingShimmerGSR";
                shimmerAddresses.Add(OpenandReadTextBox(sensorName));
            } else if (sensorName.Equals("Muse")) {
                textBlockName = "textConnectingMuse";
            } else if (sensorName.Equals("Embrace Plus")) {
                textBlockName = "textConnectingEmbracePlus";
            }

            TextBlock thisTextBlock = (TextBlock)this.FindName(textBlockName);
            thisTextBlock.Visibility = Visibility.Visible;
            textsConnecting.Add(thisTextBlock);
            
        }

        for (int i = 0; i < selectedSensors.Count; i++) {
            int enabledSensors = 0x00;
            // try connecting to selected sensors one by one
            for (int j = 0; j < listBoxesSensorSignals[i].Items.Count; j++) {

                // get individual signal of signal listbox
                var signal = listBoxesSensorSignals[i].Items[j];

                // check if signal is selected
                if (listBoxesSensorSignals[i].SelectedItems.Contains(signal)) {
                    
                    // get selected signal
                    object sensorSignal = SignalSelection.signalLists[i][j];

                    // if selected signal is listed in a Shimmer listbox ...
                    // set enabled sensors of that shimmer and connect to unit
                    if (selectedSensors[i].Name.Contains("Shimmer")) {
                        SignalSelection.ShimmerSensorWithSignals? shimmerSensor = sensorSignal as SignalSelection.ShimmerSensorWithSignals;
                        enabledSensors = (enabledSensors | shimmerSensor.sensorBitFlag);
                    } // else if (selectedSensors[i].Name.Contain("Muse")) {...}
                }
            }

            // initialize shimmer sensors and connect
            if (selectedSensors[i].Name.Contains("Shimmer")) {
                string shimmerAddress = shimmerAddresses[listIndex];
                listIndex++;

                BTShimmer shimmerBTunit = InitializeShimmer(selectedSensors[i].Name, shimmerAddress, enabledSensors);
                initializedSensors.Add(shimmerBTunit);
     

                // connect
                await Task.Run(() => shimmerBTunit.Connect());
                await Task.Run(() => shimmerBTunit.StartStreamingandLog());



                if (shimmerBTunit.GetState() == ShimmerBluetooth.SHIMMER_STATE_CONNECTED) {
                    textsConnecting[i].Text = textsConnecting[i].Text.Replace("Connecting", "Connected");
                    textsConnecting[i].Foreground = new SolidColorBrush(Colors.LightGray);
                } else {
                    textsConnecting[i].Text = textsConnecting[i].Text.Replace(" Connecting", ": Connection Failed");
                    textsConnecting[i].Foreground = new SolidColorBrush(Colors.Red);
                }
                
            }
        }

        await WaitAsync(5000);

        // update UI
        textConnectingShimmerECG.Visibility = Visibility.Collapsed;
        textConnectingShimmerEMG.Visibility = Visibility.Collapsed;
        textConnectingShimmerIMU.Visibility = Visibility.Collapsed;
        textConnectingShimmerGSR.Visibility = Visibility.Collapsed;
        textConnectingMuse.Visibility = Visibility.Collapsed;
        textConnectingEmbracePlus.Visibility = Visibility.Collapsed;
        textConnecting.Visibility = Visibility.Hidden;
    }


    private static async Task WaitAsync(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }


    private string OpenandReadTextBox(string shimmerName) {
        InputWindow inputWindow = new InputWindow();
        inputWindow.sensorToConnect.Text = "Enter " + shimmerName + " Address";
        string userInput = "";
        // Show the input window modally
        if (inputWindow.ShowDialog() == true)  // Wait for the user to close the input window
        {
            // After closing, retrieve the user input
            userInput = inputWindow.UserInput;

            // Display or process the input
            // MessageBox.Show("User entered: " + userInput);
        }

        return userInput;
    }

    private BTShimmer InitializeShimmer(string devName, string shimmerAddress, int enabledSensors) {
        double samplingRate = 51.2;

        byte[] defaultECGReg1 = ShimmerBluetooth.SHIMMER3_DEFAULT_TEST_REG1;
        byte[] defaultECGReg2 = ShimmerBluetooth.SHIMMER3_DEFAULT_TEST_REG2;

        BTShimmer shimmerBTunit = new BTShimmer(devName, shimmerAddress, samplingRate, 0, ShimmerBluetooth.GSR_RANGE_AUTO, enabledSensors, false, false, false, 1, 0, defaultECGReg1, defaultECGReg2, false);

        return shimmerBTunit;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        showSensors();
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        showSensors();
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        showSensors();
    }


    private void showSensors()
    {
        ContentText = null;
        sensorCount = 0;
        for (int i = 0; i < SensorInformation.Count; i++)
        {
            SensorInfos item = SensorInformation[i];
            if (item.IsChecked)
            {
                string sensorName = item.Name;
                string sensorName_noW = Whitespace_Reg().Replace(sensorName, "");

                if (sensorName.Contains("Shimmer")) {
                    string shortSensorName = Shimmer_Reg().Replace(sensorName_noW, "");
                    ContentText = ContentText + shortSensorName + ", ";
                } else if (sensorName.Contains("Embrace")) {
                    string shortSensorName = Embrace_Reg().Replace(sensorName_noW, "E");
                    ContentText = ContentText + shortSensorName + ", ";
                } else {
                    ContentText = ContentText + sensorName_noW + ", ";
                }
                sensorCount++;
            }
        }

        if (sensorCount == SensorInformation.Count())
        {
            ContentText = "All Sensors selected";
        }
        else if (sensorCount == 0)
        {
            ContentText = "Please select Sensors";
        }
    }

    [GeneratedRegex(@"\s")]
    private static partial Regex Whitespace_Reg();
    [GeneratedRegex(@"Shimmer")]
    private static partial Regex Shimmer_Reg();
    [GeneratedRegex(@"Embrace")]
    private static partial Regex Embrace_Reg();
}