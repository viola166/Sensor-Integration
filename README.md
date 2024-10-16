# Sensor-Integration

## Downloads
* Download the library folder (comprising the SensorAPI) and the TestAPI folder; make sure both folders are located in the same parent directory

## Prerequisites
*	Shimmer Sensors: All Shimmer Sensors that you possibly want to use in the program must have been connected to the client/laptop before you run the program
  >	Connect via Bluetooth upon App usage: 
    -	Open the Bluetooth settings on your device and switch on Bluetooth
    -	Turn on the Shimmer unit by toggling the switch on its side
    -	The Shimmer unit’s upper LED light shall now be showing the “RTC not set” pattern (see LED patterns in the Shimmer User Manual)
    -	Now add a new device in the Bluetooth settings of your client
    -	The Shimmer unit will be displayed as “Shimmer3-[Radio ID]” (you can find each Shimmer’s Radio ID on the unit’s backside)
    -	Select the Shimmer and establish connection using the code “1234”
    -	The Shimmer will shortly connect and then disconnect again from the client
*	Find out Shimmer unit’s full Bluetooth Address:
  -	Open the Device Manager on your client
  -	In the “Bluetooth” section, identify the Shimmer units, right click on each one and open its properties
  -	For each Shimmer unit switch to the “Details” tab and find the value of “Association Endpoint Address”
  -	Write down these values somewhere where you can access them fast while running the program
* Platform Support:
  -	This application has only been tested on Windows yet
  -	However, the project uses the .NET 8 SDK and should therefore provide cross-platform support

## Run Program
*	Open the project TestAPI in your editor of choice
*	Run the program by executing the code “dotnet run” in the open project

## Guide through Program
*	First, select the sensor units you want to use in this trial using the dropdown menu in the top left corner; after you selected all sensors successfully, submit by pressing the submit button next to the menu
*	Below, all corresponding sensor units will be displayed in one tab each
*	Navigate through the tabs and select the sensor signals you want to enable for this trial; Once you selected all sensor signals, submit by pressing the submit button below the tabs
*	For each Shimmer unit, a separate window will open one by one and ask you for entering the Shimmer address. Here, please enter all addresses respectively that you wrote down earlier
*	The sensor units with the respective signal settings will be connecting to the client now – this may take a few minutes depending on the number of selected sensors
*	After successful connection (connection statuses indicated in the bottom left corner), the live streamed data of the chosen sensor signals will show up on the right side of the window (sampling rate: 1/sec) 
*	You can now stop the streaming and logging process by clicking the respective button on the bottom right corner and start it again with the corresponding button there too
