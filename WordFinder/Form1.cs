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
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                TextProcessor.Instance.Init(openFileDlg.FileName, UpdateProgres, SetStatus);
                TextProcessor.Instance.PreparyDictionary();
            }
        }

        private void UpdateProgres(float percent)
        {
            //так-как делали "ленивый" подсчет обработанных символов можем получить процент больше 100.
            var val = (int)(percent * 100);
            val = val < 100 ? val : 100;

            if (StatusStrip.InvokeRequired)
                StatusStrip.Invoke(new Action<int>((s) => ProgressBar.Value = s), val);
            else
                ProgressBar.Value = val;
        }

        private void SetStatus(string message)
        {
            if (StatusStrip.InvokeRequired)
                StatusStrip.Invoke(new Action<string>((s) => StatusLabel.Text = s), message);
            else
                StatusLabel.Text = message;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            TextProcessor.Instance.EndPreparing();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            var count = TextProcessor.Instance.FindWordFrequency(textToFind.Text);
            if (count > 0)
            {
                string strMsg = string.Format(@"Сдщво ""{0}"" встречается {1} раз.", textToFind.Text, count.ToString());
                MessageBox.Show(strMsg);
            }
            else
                MessageBox.Show(@"Задано неверное слово или процесс постоения словаря ещё не завершился");

        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            TextProcessor.Instance.EndPreparing();
        }
    }
}
