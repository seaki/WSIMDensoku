using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WSIMDensoku
{
    public partial class Form1 : Form
    {
        private SerialPort _port = null;
        private bool _stopped = false;

        public Form1()
        {
            InitializeComponent();
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            _stopped = false;
            // TODO: 別スレッドでポートを開き、5秒ごとに AT@K20 と AT@@LVL を送る
            openPort(comPortTextBox.Text);
            await Task.Run(async () =>
            {
                while (!_stopped)
                {
                    sendData("AT@K20");
                    await Task.Delay(5000);
                    sendData("AT@@LVL");
                    await Task.Delay(1000);
                }
            });
            
        }

        private void openPort(string portName)
        {
            _port = new SerialPort
            {
                PortName = portName,
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                DtrEnable = true,
                RtsEnable = true,
                NewLine = "\r",
                Encoding = Encoding.ASCII,
            };
            if (_port.IsOpen) { _port.Close(); }
            _port.DataReceived += _port_DataReceived;
            _port.Open();
        }

        private void sendData(string text)
        {
            //Invoke((Action)(() => {
            //    resultTextBox.Text += string.Format("{0:s} >> ", DateTime.Now) + text + Environment.NewLine;
            //}));
            _port.WriteLine(text);
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string text = _port.ReadExisting();
            Invoke((Action)(() => {
                //resultTextBox.AppendText(string.Format("{0:s} >> {2}{1}{2}", DateTime.Now, text, Environment.NewLine));
                resultTextBox.AppendText(text);
                resultTextBox.ScrollToCaret();
            }));
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _port.Close();
            _port.Dispose();
            _port = null;
            _stopped = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (null != _port)
            {
                _port.Dispose();
            }
        }
    }
}
