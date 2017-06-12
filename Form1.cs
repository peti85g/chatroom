using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace TcpIpTry
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader streamR;
        public StreamWriter streamW;
        public string recive;
        public String textToSend;

        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIp)
            {
                if (address.AddressFamily==AddressFamily.InterNetwork)
                {
                    textBox3.Text = address.ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(textBox4.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            streamR = new StreamReader(client.GetStream());
            streamW = new StreamWriter(client.GetStream());
            streamW.AutoFlush = true;
            backgroundWorker1.RunWorkerAsync();  //itt a lenyeg
            backgroundWorker2.WorkerSupportsCancellation = true;  //le allithato a Thread
            button3.Enabled = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recive = streamR.ReadLine();
                    this.textBox2.Invoke(new MethodInvoker(delegate() { textBox2.AppendText("Chat partner :" + recive + "\n"); }));
                    recive = "";
                }
                catch(Exception y)
                {
                    MessageBox.Show(y.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                streamW.WriteLine(textToSend);
                this.textBox2.Invoke(new MethodInvoker(delegate() { textBox2.AppendText("En: " + textToSend + "\n"); }));
            }
            else
            {
                MessageBox.Show("A kuldes nem sikerult");
            }
            backgroundWorker2.CancelAsync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(textBox5.Text),int.Parse(textBox6.Text));
            try
            {
                client.Connect(IpEnd);
                if (client.Connected)
                {
                    textBox2.AppendText("Connected to server"+"\n");
                    streamW=new StreamWriter(client.GetStream());
                    streamR=new StreamReader(client.GetStream());
                    streamW.AutoFlush=true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;

                }
            }catch(Exception x)
            {
                MessageBox.Show(x.Message.ToString());
            }
            button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text!="")
            {
                textToSend = textBox1.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            textBox1.Text = "";
        }
    }
}
