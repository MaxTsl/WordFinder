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

                _textProcessor = new TextProcessor(openFileDlg.FileName, UpdateProgres, OnError);
                _textProcessor.PreparyDictionary();
            }
        }

        private void UpdateProgres(float percent)
        {
            var val = (int)(percent * 100);
            val = val < 100 ? val : 100;

            if (ProgressBar.InvokeRequired)
                ProgressBar.Invoke(new Action<int>((s) => ProgressBar.Value = s), val);
            else
                ProgressBar.Value = val;
        }

        private void OnError(string message)
        {
            System.Windows.Forms.MessageBox.Show(message);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_textProcessor != null)
                _textProcessor.EndPreparing();
        }
    }
}
