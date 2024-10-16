using System.Windows;

namespace TestAPI
{
    public partial class InputWindow : Window
    {
        public string UserInput { get; private set; }

        public InputWindow()
        {
            InitializeComponent();
        }

        private void OnClick_SubmitBTAddress(object sender, RoutedEventArgs e)
        {
            // Capture the input from the TextBox
            // known addresses fc:0f:e7:b5:6a:66; 44:b7:d0:2d:a8:a2
            UserInput = inputTextBox.Text;

            // Close the window after input
            this.DialogResult = true;  // Sets the window result to indicate successful submission
            this.Close();
        }
    }
}
