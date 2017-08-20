using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordFinder
{
    public partial class Form1 : Form
    {
        TextProcessor _textProcessor;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                _textProcessor = new TextProcessor(openFileDlg.FileName);
                _textProcessor.PreparyDictionary();

            }
        }
    }
}
