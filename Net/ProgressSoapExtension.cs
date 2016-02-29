using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.IO;

namespace QuantumConcepts.Common.Net
{
    public class ProgressSoapExtension : SoapExtension
    {
        public delegate void ProgressChangedEventHandler(long bytesCompleted, long bytesTotal);

        private Stream _oldStream;
        private Stream _newStream;
        private bool _isAfterSerialization;

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.AfterSerialize:
                {
                    _isAfterSerialization = true;
                    break;
                }
                case SoapMessageStage.BeforeDeserialize:
                {
                    SoapClientMessage clientMessage = (SoapClientMessage)message;
                    IWebServiceWithProgress progressMessage = null;
                    long dataLength;
                    int readSize;
                    byte[] buffer;

                    dataLength = clientMessage.Stream.Length;

                    if (message is IWebServiceWithProgress)
                        progressMessage = (IWebServiceWithProgress)message;

                    if (progressMessage == null)
                        readSize = int.MaxValue;
                    else
                        readSize = (int)(dataLength / 100);

                    buffer = new byte[readSize];

                    while (true)
                    {
                        try
                        {
                            int bytesRead = _oldStream.Read(buffer, 0, readSize);

                            if (bytesRead == 0)
                            {
                                _newStream.Seek(0, SeekOrigin.Begin);
                                return;
                            }

                            _newStream.Write(buffer, 0, bytesRead);

                            if (progressMessage != null)
                                progressMessage.ProgressChangedHandler(_newStream.Length, dataLength);
                        }
                        catch
                        {
                            _newStream.Seek(0, SeekOrigin.Begin);
                            return;
                        }
                    }
                }
            }
        }

        public override Stream ChainStream(Stream stream)
        {
            if (_isAfterSerialization)
            {
                _oldStream = stream;
                _newStream = new MemoryStream();

                return _newStream;
            }

            return stream;
        }

        public override object GetInitializer(Type serviceType)
        {
            return null;
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {
            _isAfterSerialization = false;
        }
    }

    public interface IWebServiceWithProgress
    {
        ProgressSoapExtension.ProgressChangedEventHandler ProgressChangedHandler { get; }
    }
}