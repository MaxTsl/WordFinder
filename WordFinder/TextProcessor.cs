using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    public delegate void StatusInformerDeligate(string x);

    class TextProcessor
    {
        UpdateProgressDeligate _updateProgress;
        StatusInformerDeligate _statusInformer;

        string _fileName;
        Dictionary<String, WordCount> _dict;

        Task _preapering = null;
        CancellationToken _token;
        CancellationTokenSource _tokenSource;

        bool _ready;

        private static TextProcessor instance;

        private TextProcessor() { }

        public static TextProcessor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TextProcessor();
                }
                return instance;
            }
        }

        public void Init(string FileName, UpdateProgressDeligate UpdateProgress, StatusInformerDeligate StatusInformer)
        {
            if (UpdateProgress == null)
                _updateProgress = s => { };
            else
                _updateProgress = UpdateProgress;

            if (StatusInformer == null)
                _statusInformer = s => { };
            else
                _statusInformer = StatusInformer;

            _fileName = FileName;
            _ready = false;
        }

        public void PreparyDictionary()
        {
            if (_preapering == null || _ready)
            {
                InitNewTask();
            }
            else
            {
                _statusInformer("Дождитесь завершения текущего процесса");
            }
        }

        private void InitNewTask()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            _preapering = new Task(Processing, _token);
            _preapering.Start();
            _statusInformer("Начата подготовка словаря");
        }

        public void EndPreparing()
        {
            if (_preapering != null && !_preapering.IsCompleted)
            {
                //в любом варианте все завершится без нас, но так будет лучше, только вот ещё надо ловить исключение прерывания выполнения...

                try
                {
                    _tokenSource.Cancel();
                    _updateProgress(0);
                    _ready = true;
                    Thread.Sleep(200);
                    _preapering.Wait(_token);
                    
                }
                catch (Exception ex)
                {
                    _statusInformer(ex.Message);
                }
                finally
                {
                    _preapering = null;
                    _tokenSource.Dispose();

                }

            }
        }

        public void Processing()
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
                WordCount count = new WordCount(1);
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

                    if (_token.IsCancellationRequested)
                    {
                        _token.ThrowIfCancellationRequested();
                    }

                    _updateProgress(readed / totalCount);
                }

                _updateProgress(0);
                strReader.Close();
                _statusInformer("Готово");
                _ready = true;
            }
            catch (Exception ex)
            {
                _statusInformer(ex.Message);
                if (strReader != null)
                    strReader.Close();
            }
            return;
        }

        public int FindWordFrequency(string str)
        {
            if (_ready && str != null)
            {
                WordCount count = new WordCount(0);
                if (_dict.TryGetValue(str, out count))
                    return count.Count;
            }
            return -1;
        }

    }
}
