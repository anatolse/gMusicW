﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.GoogleMusic.Diagnostics
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Windows.Storage;

    public class FileLogWriter : ILogWriter, IDisposable
    {
        private readonly object locker = new object();

        private StreamWriter writer;

        public bool IsEnabled
        {
            get
            {
                return true;
            }
        }

        public void Dispose()
        {
            lock (this.locker)
            {
                if (this.writer != null)
                {
                    this.ClearStream();
                }
            }
        }

        public void Log(string level, string context, string message, params object[] parameters)
        {
            lock (this.locker)
            {
                if (this.writer == null)
                {
                    this.EnableLogging();
                }

                if (this.writer != null)
                {
                    this.writer.WriteLine("{0}::: {1} --- {2}", level, context, string.Format(message, parameters));
                    this.writer.Flush();
                }
            }
        }

        private void EnableLogging()
        {
            var enableLoggingAsync = this.EnableLoggingAsync();
            TaskEx.WaitAllSafe(enableLoggingAsync);

            if (enableLoggingAsync.IsCompleted)
            {
                lock (this.locker)
                {
                    this.writer = enableLoggingAsync.Result;   
                }
            }
        }

        private void ClearStream()
        {
            lock (this.locker)
            {
                this.writer.Flush();
                this.writer.Dispose();
                this.writer = null;
            }
        }

        private async Task<StreamWriter> EnableLoggingAsync()
        {
            var storageFile =
                await ApplicationData.Current.LocalFolder.CreateFileAsync(string.Format("{0:yyyy-MM-dd-HH-mm-ss-ffff}.log", DateTime.Now), CreationCollisionOption.OpenIfExists).AsTask();

            var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite).AsTask();

            return new StreamWriter(stream.AsStream());
        }
    }
}
