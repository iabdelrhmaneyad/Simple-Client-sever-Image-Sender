using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ImgServer
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Image Files(*.jpg; *.jpeg; *.gif;*.bmp;)|*.jpg; *.jpeg; *.gif; *.bmp;";
            if (of.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = of.FileName;
                pictureBox1.Image = new Bitmap(of.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = textBox1.Text;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
            Socket newsock = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            newsock.Bind(ipep);
            newsock.Listen(10);
            Socket client = newsock.Accept();

            // Send the filename to the client
            byte[] filenameBytes = Encoding.ASCII.GetBytes(Path.GetFileName(filename));
            client.Send(BitConverter.GetBytes(filenameBytes.Length), 4, SocketFlags.None);

            client.Send(filenameBytes, filenameBytes.Length, SocketFlags.None);

            // Send the file to the client
            using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    client.Send(buffer, bytesRead, SocketFlags.None);
                }
            }
            MessageBox.Show("File sent to client");


            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}