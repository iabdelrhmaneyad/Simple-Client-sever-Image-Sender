using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ImgDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string filename = "";
            byte[] filenameLengthBytes = new byte[4];
            byte[] buffer = new byte[1024];
            int filenameLength;
            int bytesRead;
            using (Socket server = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    server.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050));
                }
                catch (SocketException a)
                {
                    MessageBox.Show("Unable to connect to server." + a.ToString());
                    return;
                }

                // Receive the filename length
                server.Receive(filenameLengthBytes, 4, SocketFlags.None);
                filenameLength = BitConverter.ToInt32(filenameLengthBytes, 0);

                // Receive the filename
                byte[] filenameBytes = new byte[filenameLength];
                server.Receive(filenameBytes, filenameLength, SocketFlags.None);
                filename = Encoding.ASCII.GetString(filenameBytes);

                // Receive the file
                using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    while ((bytesRead = server.Receive(buffer, buffer.Length, SocketFlags.None)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }

                pictureBox1.Image = new Bitmap(filename);
                textBox1.Text = filename;
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}