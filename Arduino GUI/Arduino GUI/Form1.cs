using SuperSimpleTcp;
using System.Text;

namespace Arduino_GUI
{
    public partial class Form1 : Form
    {

        static SimpleTcpClient client;

        bool isRedLEDPressed = false;
        bool isGreenLEDPressed = false;
        bool isBlueLEDPressed = false;

        string imgPath = "C:\\Images\\";

        public Form1()
        {
            InitializeComponent();
            sensorProgressBar.Value = 0;
            sensorProgressBar.Minimum = 0;
            sensorProgressBar.Maximum = 100;

            btnConnect.Enabled = false;
            btnRed.Enabled = false;
            btnGreen.Enabled = false;
            btnBlue.Enabled = false;
            btnSend.Enabled = false;
            btnClear.Enabled = false;
            textBox1.Enabled = false;
            servoSlider.Enabled = false;

            txtBoxPort.TextChanged += isTextBoxEmpty;
            txtBoxIP.TextChanged += isTextBoxEmpty;
        }

        private void isTextBoxEmpty(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBoxPort.Text) && !string.IsNullOrEmpty(txtBoxIP.Text))
            {
                btnConnect.Enabled = true;
            }

        }
        private bool Connect()
        {
            try
            {
                client = new SimpleTcpClient(txtBoxIP.Text + ":" + txtBoxPort.Text);

                client.Connect();

                btnRed.Enabled = true;
                btnBlue.Enabled = true;
                btnGreen.Enabled = true;
                btnSend.Enabled = true;
                btnClear.Enabled = true;
                textBox1.Enabled = true;
                servoSlider.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (Connect())
            {
                client.Events.Connected += Connected;
                client.Events.Disconnected += Disconnected;
                client.Events.DataReceived += DataReceived;
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }
        private static void Connected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected to the server.");
        }
        private static void Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected from the server. Trying to reconnect...");
        }
        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                Console.Write($"{Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count)}");
                string data = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);

                if (Form.ActiveForm is Form1 form)
                {
                    form.Invoke((MethodInvoker)delegate
                    {
                        form.sensorProgressBar.Value = Convert.ToInt32(data);
                        form.sensorProgressBar.Text = data;
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }
        private void sendData(Button btn, string data, string img)
        {
            Image image = Image.FromFile(imgPath + img);

            btn.Image = image;
            btn.ImageAlign = ContentAlignment.MiddleCenter;
            client.Send(data);
        }

        private void btnRed_Click(object sender, EventArgs e)
        {
            if (isRedLEDPressed)
            {
                sendData(btnRed, "ROFF", "ledRedOff.png");
            }
            else
            {
                sendData(btnRed, "RON", "ledRedOn.png");
            }
            isRedLEDPressed = !isRedLEDPressed;
        }

        private void btnGreen_Click(object sender, EventArgs e)
        {
            if (isGreenLEDPressed)
            {
                sendData(btnGreen, "GOFF", "ledGreenOff.png");
            }
            else
            {
                sendData(btnGreen, "GON", "ledGreenOn.png");
            }
            isGreenLEDPressed = !isGreenLEDPressed;
        }

        private void btnBlue_Click(object sender, EventArgs e)
        {
            if (isBlueLEDPressed)
            {
                sendData(btnBlue, "BOFF", "ledBlueOff.png");
            }
            else
            {
                sendData(btnBlue, "BON", "ledBlueOn.png");
            }
            isBlueLEDPressed = !isBlueLEDPressed;
        }

        private void servoSlider_Scroll(object sender, EventArgs e)
        {
            TrackBar trackBar = (TrackBar)sender;
            int value = trackBar.Value;
            client.Send(value.ToString() + "\n");
            labelSlider.Text = value.ToString();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            client.Send(textBox1.Text.ToString());
            textBox1.Text = "";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            client.Send("clr");
        }
    }
}