using Hangfire.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FactorialSumm
{
    class MainViewModel : INotifyPropertyChanged
    {
        private double result;
        private int counter;
        public double Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }
        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        private Thread _thread;
        private CancellationTokenSource _tokenSource;

        public MainViewModel()
        {
            counter = 1;
            result = 0;
            StartCommand = new DelegateCommand((p) =>
            {
                _tokenSource = new CancellationTokenSource();
                _thread = new Thread(Worker) { IsBackground = true };
                _thread.Start(_tokenSource.Token);
            },
            p => _thread == null);

            PauseCommand = new DelegateCommand(p =>
            {
                _tokenSource.Cancel();
                _tokenSource = null;
                _thread = null;
            }, p => _thread != null);
            StopCommand = new DelegateCommand((p) =>
            {
                _tokenSource.Cancel();
                _tokenSource = null;
                _thread = null;
                counter = 1;
                result = 0;
            }, p => _thread != null);
        }

        private void Worker(object state)
        {
            var token = (CancellationToken)state;
            while (!token.IsCancellationRequested)
            {
                Result+=(double)1/Factorial(counter++);
                Thread.Sleep(1000);
            }
        }
        static int Factorial(int x)
        {
            if (x == 0)
            {
                return 1;
            }
            else
            {
                return x * Factorial(x - 1);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

