using Extreme.Net;
using System.Diagnostics;
using System.Windows.Forms;

namespace rubotNet.Lcheck
{
    public class LicenseCheck
    {
        public static string startPurch;

        public static string endPurch;

        public static string email;

        public static void Connectvkabinu(Panel panel, UserControl login1)
        {
            panel.Controls.Remove(login1);
            MainFrame value = new MainFrame();
            panel.Controls.Add(value);
        }
    }
}

		