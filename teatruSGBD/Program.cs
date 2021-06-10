using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.Data;

namespace teatruSGBD
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        [STAThread]
        static void InitWin()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
                Application.EnableVisualStyles();

                Application.SetCompatibleTextRenderingDefault(false);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        static void Main()
        {
            //DEADLOCK
            // Creating and initializing threads

            /*
            var noTries = 2;
            
            Console.WriteLine("AM INCEPUT");
            for(int i=0;i<noTries;i++)
            {
                Thread thr1 = new Thread(method1);
                Thread thr2 = new Thread(method2);
                thr1.Start();

                thr2.Start();

                
            }
            
            
            Console.WriteLine("ABORTED ...");
            */
                
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void method2()
        {
            using (var conn = new SqlConnection("Server=DESKTOP-VS7B26P\\SQLEXPRESS;Database=Teatru;Integrated Security=true"))
            using (var command = new SqlCommand("Deadlock_2", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                try
                {
                    Console.WriteLine("THREAD 2"); conn.Open();
                    var rows = command.ExecuteNonQuery();
                    Console.WriteLine("Thread2 rows: " + rows);
                    conn.Close();
                   
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    if (e.Number == 1205)
                    {
                        // Deadlock 
                        Console.WriteLine("DEADLOCK");
                    }
                    
                     Console.WriteLine("Deadlock on thread 2");
                   
                }



            }
        }

        private static void method1()
        {
            using (var conn = new SqlConnection("Server=DESKTOP-VS7B26P\\SQLEXPRESS;Database=Teatru;Integrated Security=true"))
            using (var command = new SqlCommand("Deadlock_1", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                try
                {
                    Console.WriteLine("THREAD 1");
                    conn.Open();
                    var rows = command.ExecuteNonQuery();
                    Console.WriteLine("Thread1 rows: " + rows);
                    conn.Close();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    if (e.Number == 1205)
                    {
                        // Deadlock 
                        Console.WriteLine("DEADLOCK");
                    }

                    Console.WriteLine("Deadlock on thread 1");
                    
                }





            }
        }
    }
}
