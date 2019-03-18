using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.ServiceReference1;

namespace Client
{
    public partial class Form1 : Form
    {
        private StringServiceClient client;
        public Form1()
        {
            client = new StringServiceClient();
            InitializeComponent();
        }

        private void DefineClass(object sender, EventArgs e)
        {
            string set = textBox2.Text;
            var response = client.DefineClass(set);
            this.label4.Text = response;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DatasetInfo response = client.GetDatasetInformation();
            cols.Text = response.ColumnNumber.ToString();
            rows.Text = response.RowsNumber.ToString();

            string classDistribution = "";

            foreach (var keyValuePair in response.ClassDistribution)
            {
                classDistribution += "class: " + keyValuePair.Key + " values: " + keyValuePair.Value + "\n";
            }

            classes.Text = classDistribution;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void AddString(object sender, EventArgs e)
        {
            string addingSet = textBox1.Text;
            string response = client.AddString(addingSet);
            label3.Text = response;
        }
        
    }
}
