using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownLoadStram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
              progressBar1.Minimum = 0;
            progressBar1.Maximum = 3000;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var urls = new[] {
            textBox1.Text,
           textBox2.Text
            };
            foreach (var item in urls)
            {

                var fileNme = new Random().Next(10);
                var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Get, item);
                //"https://nor-1.down-egy.xyz/The.Boss.Baby.Christmas.Bonus.2022.1080p.NF.WEB-DL.AR.mp4?Key=Jf-dNUC8uVWkE02rHuw0Yg&Expires=1683385598");use this link for test

           
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var stream = await response.Content.ReadAsStreamAsync();
                var buffer = new byte[2024];
                var observable = Observable.FromAsync(() => stream.ReadAsync(buffer, 0, buffer.Length))
                    .Repeat()
                    .TakeWhile(count => count > 0)
                    .Select(count => buffer.Take(count).ToArray())
                    .ObserveOn(SynchronizationContext.Current)//use this for thread management to avoid context switch 
                .Subscribe(
                    buffers =>
                    {
                    // Process received data
                    var fileStream = new FileStream("Movie_" + fileNme + ".mp4", FileMode.Append);
                        using (var writer = new BinaryWriter(fileStream))
                        {
                            foreach (var buffero in buffers)
                            {
                                writer.Write(buffero);
                            }
                        }
                        // Update progress bar
                        UpdateProgresBar(buffers.Length);
                    },
                    ex => MessageBox.Show($"Error: {ex.Message}"),
                    () => MessageBox.Show("Download completed.")
                );
            }
            //;
        }
        void UpdateProgresBar(int value)
        {
            //label2.Text = value.ToString();
            progressBar1.Value = value;
        }
    }
}
