using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace quan_ly_chi_tieu
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationContext appContext = new ApplicationContext();

            login loginForm = new login();
            appContext.MainForm = loginForm;
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                main newForm = new main(loginForm.userID, loginForm.connectionString);
                newForm.Show();
                appContext.MainForm = newForm;
                
            }
            else
            {
                appContext.ExitThread();
                appContext.MainForm.Close();
                Application.Exit();
                return;
            }

            Application.Run(appContext);

        }
    }
}
