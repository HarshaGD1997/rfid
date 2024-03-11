using System;
using System.Windows.Forms;
using System.IO.Ports;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text;
using System.Linq;
using System.IO;

namespace rfid
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;
        IMongoCollection<BsonDocument> collection;

        private string subname;
        public Form1(string subname)
        {
            this.subname = subname;
            InitializeComponent();

            try
            {
                string jsonString = File.ReadAllText("E:\\arduino-nano\\rfid_app\\rfid\\appsettings.json");

                int startIndex = jsonString.IndexOf("ConnectionString") + "ConnectionString".Length + 4; // Add 4 for ": ": characters
                int endIndex = jsonString.IndexOf('"', startIndex);

                string connectionString = jsonString.Substring(startIndex, endIndex - startIndex);

                MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
                MongoClient client = new MongoClient(settings);

                client.ListDatabaseNames();

                IMongoDatabase database = client.GetDatabase("RFID");
                collection = database.GetCollection<BsonDocument>("data");
            }
            catch (MongoConnectionException ex)
            {
                MessageBox.Show("Error connecting to MongoDB: " + ex.Message, "MongoDB Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            try
            {

                serialPort = new SerialPort("COM4", 9600);
                serialPort.Encoding = Encoding.ASCII; 
                                                      
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Serial Port Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                string rfidTag = serialPort.ReadLine();
                string name = GetNameFromDatabase(rfidTag);
                UpdateTextBox(name);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Data Receive Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetNameFromDatabase(string rfidTag)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("RFID_tag", rfidTag.Trim());
            var document = collection.Find(filter).FirstOrDefault();

            if (document != null)
            {
                var updateFilter = Builders<BsonDocument>.Filter.Eq("_id", document["_id"]);
                var update = Builders<BsonDocument>.Update.Push("attendance", new BsonDocument
        {
            { "dateTime", DateTime.Now },
            { "subname", subname.ToUpper() }
        }); 
                collection.UpdateOne(updateFilter, update);

                UpdateTextBox(textBox1, document["name"].AsString);
                UpdateTextBox(textBox2, "Success");

                return document["name"].AsString;
            }
            else
            {
                Console.WriteLine("No document found for RFID tag: " + rfidTag); 
                return "RFID Tag not found in database";
            }
        }

        private void UpdateTextBox(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateTextBox), text);
                return;
            }
            textBox1.Text = text; 
        }

        private void UpdateTextBox(TextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new Action<TextBox, string>(UpdateTextBox), textBox, text);
                return;
            }
            textBox.Text = text;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Visible)
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }

                Application.Exit();
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            home homepage = new home();

            homepage.Show();

            this.Hide();
        }
    }
}
