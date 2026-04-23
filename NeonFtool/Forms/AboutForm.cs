using System.Windows.Forms;

namespace NeonFtool.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            label2.Text = $"Copyright © {System.DateTime.Now.Year} \r\nVersion {Classes.Constants.VERSION}";
        }
    }
}
