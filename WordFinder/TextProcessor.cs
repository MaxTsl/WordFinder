using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordFinder
{
    class TextProcessor
    {

        string _fileName;
        Dictionary<String, int> _dict;

        public TextProcessor(string FileName)
        {
            _fileName = FileName;
        }

        public void PreparyDictionary()
        {
            StreamReader strReader = null;
            try
            {
                strReader = new StreamReader(_fileName);

                _dict = new Dictionary<String, int>();
                int count = 0;
                while (strReader.EndOfStream != true)
                {
                    foreach (var el in strReader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (_dict.TryGetValue(el, out count))
                            _dict[el] = count+1;
                        else
                            _dict[el] = 1;
                    }
                }
                strReader.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                if (strReader != null)
                    strReader.Close();
            }
            return;
        }
    }
}
