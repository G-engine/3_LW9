using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private string city;
        private double lat;
        private double lon;
        public Form1()
        {
            InitializeComponent();
            List<string>cities = new List<string>();
            StreamReader file = new StreamReader("D:\\projects\\C#\\WindowsFormsApp1\\city.txt");
            while(!file.EndOfStream) 
                cities.Add(file.ReadLine());
            listBox1.Items.AddRange(cities.ToArray());
            file.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using HttpClient client = new HttpClient();
            await ParseString();
            var content = await client.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&APPID=f9db82f0483aa6ce398baf3e85d4d59f");
            Weather w = new Weather(content);
            Weather.Text = w.Print();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            city = listBox1.SelectedItem.ToString();
        }

        private Task ParseString()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                StringBuilder s = new StringBuilder();
                bool isRead = false;
                int whatValue = 0;
                for (int i = 0; i < city.Length; i++)
                {
                    while (city[i] == '0' || city[i] == '1' || city[i] == '2' || city[i] == '3' ||
                           city[i] == '4' || city[i] == '5' || city[i] == '6' || city[i] == '7' ||
                           city[i] == '8' || city[i] == '9' || city[i] == '.' || (city[i] == '-' &&
                               (city[i + 1] == '0' || city[i + 1] == '1' || city[i + 1] == '2' || city[i + 1] == '3' ||
                                city[i + 1] == '4' || city[i + 1] == '5' || city[i + 1] == '6' || city[i + 1] == '7' ||
                                city[i + 1] == '8' || city[i + 1] == '9')))
                    {
                        s.Append(city[i]);
                        isRead = true;
                        if (i == city.Length - 1)
                            break;
                        ++i;
                    }

                    if (isRead)
                    {
                        s.Replace('.', ',');
                        if (whatValue == 0)
                        {
                            lat = Double.Parse(s.ToString());
                            whatValue = 1;
                            s.Clear();
                        }
                        else if (whatValue == 1)
                            lon = Double.Parse(s.ToString());

                        isRead = false;
                    }
                }
            });
            return task;
        }
    }
}