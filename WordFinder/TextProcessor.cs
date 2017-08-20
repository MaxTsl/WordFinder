using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordFinder
{

    class WordCount
    {
        public int Count { get; set; }

        public WordCount(int NewCount)
        {
            Count = NewCount;
        }
    }

    public delegate void UpdateProgressDeligate(float x);
    public delegate void ErrorInformerDeligate(string x);

    class TextProcessor
    {
        UpdateProgressDeligate _updateProgress;
        ErrorInformerDeligate _errorInformer;

        string _fileName;
        Dictionary<String, WordCount> _dict;

        public TextProcessor(string FileName, UpdateProgressDeligate UpdateProgress, ErrorInformerDeligate ErrorInformer)
        {
            if (UpdateProgress == null)
                _updateProgress = s => { };
            else
                _updateProgress = UpdateProgress;

            if (ErrorInformer == null)
                _errorInformer = s => { };
            else
                _errorInformer = ErrorInformer;

            _fileName = FileName;
        }

        public void PreparyDictionary()
        {
            StreamReader strReader = null;
            try
            {
                long totalCount = 0;
                var fInfo = new FileInfo(_fileName);
                if (fInfo.Exists)
                    totalCount = fInfo.Length / sizeof(char);
                else
                    return;

                strReader = new StreamReader(_fileName);

                _dict = new Dictionary<String, WordCount>();
                float readed = 0;
                WordCount count = new WordCount(0);
                while (strReader.EndOfStream != true)
                {
                    foreach (var el in strReader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (_dict.TryGetValue(el, out count))
                            ++count.Count;
                        else
                            _dict[el] = new WordCount(1);

                        readed += el.Length + 1; // разделитель пробел, по этому будет правильнее его учитывать. Не совсем честный подсчет, но и не колайдер делаем
                    }

                    _updateProgress(readed / totalCount);

                }

                _updateProgress(0);
                strReader.Close();
            }
            catch (Exception ex)
            {
                _errorInformer(ex.Message);
                if (strReader != null)
                    strReader.Close();
            }
            return;
        }
    }
}
