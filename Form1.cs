using System;
using System.Windows.Forms;
using System.IO.Ports;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text;
using System.Linq;

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
                // MongoDB connection string
                string connectionString = "mongodb+srv://harsha:harsha01@rfid.efilaqh.mongodb.net/?retryWrites=true&w=majority";
                MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
                MongoClient client = new MongoClient(settings);

                // Check if the server is up and accessible
                client.ListDatabaseNames();

                // If no exception is thrown, connection is successful
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
                serialPort.Encoding = Encoding.ASCII; // or Encoding.ASCII, Encoding.UTF7, Encoding.UTF32, etc.
                                                      // Change COM4 to the appropriate port
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
                // Update attendance for the student
                var updateFilter = Builders<BsonDocument>.Filter.Eq("_id", document["_id"]);
                var update = Builders<BsonDocument>.Update.Push("attendance", new BsonDocument
        {
            { "dateTime", DateTime.Now },
            { "subname", subname.ToUpper() }
        }); // Add an object containing dateTime and subname to the attendance array
                collection.UpdateOne(updateFilter, update);

                return document["name"].AsString;
            }
            else
            {
                Console.WriteLine("No document found for RFID tag: " + rfidTag); // Debug statement
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



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }

            Application.Exit();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            // Unsubscribe from the event handler to prevent memory leaks
            this.FormClosing -= Form1_FormClosing;
        }

    }
}
