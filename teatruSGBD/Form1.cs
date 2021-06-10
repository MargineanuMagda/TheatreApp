using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace teatruSGBD
{
    public partial class Form1 : Form
    {

        //string connectionString = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
        string connectionString = "Server=DESKTOP-VS7B26P\\SQLEXPRESS;Database=Teatru;Integrated Security=true";
        SqlConnection connection = new SqlConnection("Server=DESKTOP-VS7B26P\\SQLEXPRESS;Database=Teatru;Integrated Security=true");
        SqlDataAdapter dataAdapt = new SqlDataAdapter();
        DataSet dataSet = new DataSet();
        SqlDataAdapter dataAdapt2 = new SqlDataAdapter();
        DataSet dataSet2 = new DataSet();
        string ptableName = ConfigurationSettings.AppSettings["ParentTableName"];
        string ctableName = ConfigurationSettings.AppSettings["ChildTableName"];
        public Form1()
        {
            InitializeComponent();
            Init1();
        }

        private void Init1()
        {
            //init labels
            
            table1Name.Text = ptableName;
            table2Name.Text = ctableName;

            //add textboxes
            int pointX = 130;
            int pointY = 10;
            string[] columns = ConfigurationSettings.AppSettings["ParentColumns"].Split(',');
            foreach (string column in columns)
            {
                Label l1 = new Label();
                l1.Text = column;
                l1.Name = column+"lbl";
                l1.Location=new Point(0, pointY);
                l1.Show();
                
                TextBox t = new TextBox();
                t.Width = 100;
                t.Name = column;
                t.Visible = true;
                t.Location = new Point(pointX, pointY);
                pointY += 30;
                t.Show();
                panel1.Controls.Add(l1);
                panel1.Controls.Add(t);

            }

            dataAdapt.SelectCommand = new SqlCommand("SELECT * FROM "+ptableName, connection);
            dataSet.Clear();
            dataAdapt.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];

            //init textboxes for child table
            int point2X = 130;
            int point2Y = 10;
            string[] columnsChild = ConfigurationSettings.AppSettings["ChildColumns"].Split(',');
                       
            foreach (string column in columnsChild)
            {
                
                Label l1 = new Label();
                l1.Text = column;
                l1.Name = column + "lbl";
                l1.Location = new Point(0, point2Y);
                l1.Show();

                TextBox t = new TextBox();
                t.Width = 100;
                t.Name = column+"Field";
                t.Visible = true;
                t.Location = new Point(point2X, point2Y);
                point2Y += 30;
                t.Show();
                panel2.Controls.Add(l1);
                panel2.Controls.Add(t);

            }



        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            button4.Enabled = true;
            int index = dataGridView1.CurrentCell.RowIndex;

            DataGridViewRow row = dataGridView1.Rows[index];
            string [] columns = ConfigurationSettings.AppSettings["ParentColumns"].Split(',');
            foreach (string column in columns){
                TextBox textBox = (TextBox)panel1.Controls[column];
                textBox.Text= Convert.ToString(row.Cells[column].Value);
            }

            string columnRel = ConfigurationSettings.AppSettings["parentIdColumn"];
            string id = Convert.ToString(row.Cells[columnRel].Value);

            
           
            initTab2(id);
        }

        private void initTab2(string id)
        {
            string columnRel = ConfigurationSettings.AppSettings["parentIdColumn"];
            dataAdapt2.SelectCommand = new SqlCommand("SELECT * FROM "+ctableName+" WHERE "+columnRel+"=@id", connection);
            dataAdapt2.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = Int32.Parse(id);
            dataSet2.Clear();
            dataAdapt2.Fill(dataSet2);
            dataGridView2.DataSource = dataSet2.Tables[0];
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = dataGridView2.CurrentCell.RowIndex;

            DataGridViewRow row = dataGridView2.Rows[index];

            string[] columns = ConfigurationSettings.AppSettings["ChildColumns"].Split(',');
            foreach (string column in columns)
            {
                TextBox textBox = (TextBox)panel2.Controls[column+"Field"];
                textBox.Text = Convert.ToString(row.Cells[column].Value);
            }
            
            



        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            { //ADAUGA IN DB
                int ok = 0;
                string insertParams = ConfigurationSettings.AppSettings["insert"];

                string cols = ConfigurationSettings.AppSettings["ChildColumns"];
                dataAdapt2.InsertCommand = new SqlCommand("INSERT INTO "+ ctableName+" ("+cols+") VALUES ("+insertParams+")", con);

                string[] columns = cols.Split(',');
                string [] paramsI = insertParams.Split(',');

                int nrColumns = int.Parse(ConfigurationSettings.AppSettings["nrChildColumns"]);

                
                for (int i = 0; i < nrColumns; i++)
                {
                    TextBox textBox = (TextBox)panel2.Controls[columns[i] + "Field"];
                    if (textBox.Text != "")
                    {
                        try
                        {
                            dataAdapt2.InsertCommand.Parameters.AddWithValue(paramsI[i], textBox.Text);
                            ok++;
                            Console.WriteLine("parametrul {0} ia valoarea: {1}",paramsI[i],textBox.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                    else
                    {
                        MessageBox.Show(columns[i]+ " SHOULD NOT BE NULL!");
                    }
                }

                
                if (ok == nrColumns)
                {
                    try
                    {

                        con.Open();
                        dataAdapt2.InsertCommand.ExecuteNonQuery();
                        MessageBox.Show("Inserted succesfully!");
                        con.Close();

                        dataSet2.Clear();
                        dataAdapt2.Fill(dataSet2);
                        dataGridView2.DataSource = dataSet2.Tables[0];


                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }



            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            { //ADAUGA IN DB
                int ok = 0;
                string updateStr = ConfigurationSettings.AppSettings["update"];
                dataAdapt2.UpdateCommand = new SqlCommand(updateStr, con);

                string[] columns = ConfigurationSettings.AppSettings["ChildColumns"].Split(',');
                
                string[] paramsU = ConfigurationSettings.AppSettings["insert"].Split(',');

                int nrColumns = int.Parse(ConfigurationSettings.AppSettings["nrChildColumns"]);


                for (int i = 0; i < nrColumns; i++)
                {
                    TextBox textBox = (TextBox)panel2.Controls[columns[i] + "Field"];
                    if (textBox.Text != "")
                    {
                        try
                        {
                            dataAdapt2.UpdateCommand.Parameters.AddWithValue(paramsU[i], textBox.Text);
                            ok++;
                            Console.WriteLine("parametrul {0} ia valoarea: {1}", paramsU[i], textBox.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                    else
                    {
                        MessageBox.Show(columns[i] + " SHOULD NOT BE NULL!");
                    }
                }


                if (ok == nrColumns)
                {
                    try
                    {

                        con.Open();
                        dataAdapt2.UpdateCommand.ExecuteNonQuery();
                        MessageBox.Show("Update done succesfully!");
                        con.Close();

                        dataSet2.Clear();
                        dataAdapt2.Fill(dataSet2);
                        dataGridView2.DataSource = dataSet2.Tables[0];


                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

            }

        }

       


        private void button6_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                TextBox textBox = (TextBox)panel2.Controls[ConfigurationSettings.AppSettings["childIdColumn"]+"Field"];
                dataAdapt2.DeleteCommand = new SqlCommand("DELETE FROM "+ctableName+ " where "+ConfigurationSettings.AppSettings["childIdColumn"]+"=@id", con);
                if (textBox.Text != "")
                {
                    try
                    {
                        con.Open();
                        dataAdapt2.DeleteCommand.Parameters.AddWithValue("@id",textBox.Text);
                        dataAdapt2.DeleteCommand.ExecuteNonQuery();
                        MessageBox.Show("Item delete succesfully!");
                        con.Close();
                        dataSet2.Clear();
                        dataAdapt2.Fill(dataSet2);
                        dataGridView2.DataSource = dataSet2.Tables[0];


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ID should be an integer!\n" + ex);
                    }

                }
                else
                {
                    MessageBox.Show("ID SHOULD NOT BE NULL!");
                }


            }
        }
    }
}
