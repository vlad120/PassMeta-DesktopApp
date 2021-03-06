using AvaloniaGif.Decoding;
using System;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AvaloniaGif
{
    public class GifInstance : IDisposable
    { 
        public Stream Stream { get; private set; }
        public Progress<int> Progress { get; private set; }
        
#pragma warning disable 414
        bool _streamCanDispose;
#pragma warning restore 414
        private GifDecoder _gifDecoder;
        private GifBackgroundWorker _bgWorker;
        private WriteableBitmap _targetBitmap;
        private bool _hasNewFrame;
        private bool _isDisposed;
        
        public void SetSource(object newValue)
        {
            var sourceUri = newValue as Uri;
            var sourceStr = newValue as Stream;

            Stream stream = null;

            if (sourceUri != null)
            {
                _streamCanDispose = true;
                Progress = new Progress<int>();

                if (sourceUri.OriginalString.Trim().StartsWith("resm"))
                {
                    var assetLocator = AvaloniaLocator.Current.GetService<IAssetLoader>();
                    stream = assetLocator.Open(sourceUri);
                }

            }
            else if (sourceStr != null)
            {
                stream = sourceStr;
            }
            else
            {
                throw new InvalidDataException("Missing valid URI or Stream.");
            }

            Stream = stream;
            _gifDecoder = new GifDecoder(Stream);
            _bgWorker = new GifBackgroundWorker(_gifDecoder);
            var pixSize = new PixelSize(_gifDecoder.Header.Dimensions.Width, _gifDecoder.Header.Dimensions.Height);
            
            _targetBitmap = new  WriteableBitmap(pixSize, new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
            _bgWorker.CurrentFrameChanged += FrameChanged;
            GifPixelSize = pixSize;
            Run();
        }

        public PixelSize GifPixelSize { get; private set; }
 
        public WriteableBitmap GetBitmap()
        {
            WriteableBitmap ret = null;

            if (_hasNewFrame)
            {
                _hasNewFrame = false;
                ret = _targetBitmap;
            }

            return ret;
        }
        
        private void FrameChanged()
        {
            if (_isDisposed) return;
            _hasNewFrame = true;
            
            using (var lockedBitmap = _targetBitmap?.Lock())
                _gifDecoder?.WriteBackBufToFb(lockedBitmap.Address);
          
        }

        private void Run()
        {
            if (!Stream.CanSeek)
                throw new ArgumentException("The stream is not seekable");

            _bgWorker?.SendCommand(BgWorkerCommand.Play);
        }

        public void Dispose()
        {
            _isDisposed = true;
            _bgWorker?.SendCommand(BgWorkerCommand.Dispose);
            _targetBitmap?.Dispose();
        }
    }
}
