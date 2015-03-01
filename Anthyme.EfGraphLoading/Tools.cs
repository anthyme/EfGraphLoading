using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Anthyme.EfGraphLoading
{
    class PerfCounter : IDisposable
    {
        public PerfCounter(string startMessage, string endMessage)
        {
            _message = endMessage;
            _start = DateTime.Now;
            Console.Write(string.Format(startMessage + " ", _start));
        }
        private readonly DateTime _start;
        private readonly string _message;

        public void Dispose()
        {
            var end = DateTime.Now;
            Console.WriteLine(string.Format(_message, end - _start));
        }
    }

    class NoLazyLoadingScope : IDisposable
    {
        private bool lazyLoadingActivated;
        private DbContext context;

        public NoLazyLoadingScope(DbContext context)
        {
            this.context = context;
            lazyLoadingActivated = context.Configuration.LazyLoadingEnabled;

            if (lazyLoadingActivated)
            {
                context.Configuration.LazyLoadingEnabled = false;
            }
        }

        public void Dispose()
        {
            if (lazyLoadingActivated)
            {
                context.Configuration.LazyLoadingEnabled = true;
            }
        }
    }
}
