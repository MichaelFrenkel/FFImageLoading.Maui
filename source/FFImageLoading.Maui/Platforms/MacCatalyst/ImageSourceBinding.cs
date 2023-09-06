﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Foundation;

namespace FFImageLoading.Maui.Platform
{
    [Preserve(AllMembers= true)]
    internal class ImageSourceBinding : IImageSourceBinding
    {
        public ImageSourceBinding(FFImageLoading.Work.ImageSourceType imageSourceType, string path)
        {
            ImageSourceType = imageSourceType;
            Path = path;
        }

        public ImageSourceBinding(Func<CancellationToken, Task<Stream>> stream)
        {
            ImageSourceType = FFImageLoading.Work.ImageSourceType.Stream;
            Stream = stream;
        }

        public FFImageLoading.Work.ImageSourceType ImageSourceType { get; private set; }

        public string Path { get; private set; }

        public Func<CancellationToken, Task<Stream>> Stream { get; private set; }

        internal static ImageSourceBinding GetImageSourceBinding(IImageSource source, CachedImage element = null)
        {
            if (source == null)
            {
                return null;
            }

            var uriImageSource = source as UriImageSource;
            if (uriImageSource != null)
            {                
                var uri = uriImageSource.Uri?.OriginalString;
                if (string.IsNullOrWhiteSpace(uri))
                    return null;

                return new ImageSourceBinding(FFImageLoading.Work.ImageSourceType.Url, uri);
            }

            var fileImageSource = source as FileImageSource;
            if (fileImageSource != null)
            {
                if (string.IsNullOrWhiteSpace(fileImageSource.File))
                    return null;

                if (fileImageSource.File.StartsWith("/", StringComparison.InvariantCultureIgnoreCase) && File.Exists(fileImageSource.File))
                    return new ImageSourceBinding(FFImageLoading.Work.ImageSourceType.Filepath, fileImageSource.File);

                return new ImageSourceBinding(FFImageLoading.Work.ImageSourceType.CompiledResource, fileImageSource.File);
            }

            var streamImageSource = source as StreamImageSource;
            if (streamImageSource != null)
            {
                return new ImageSourceBinding(streamImageSource.Stream);
            }

            var embeddedResoureSource = source as EmbeddedResourceImageSource;
            if (embeddedResoureSource != null)
            {
                var uri = embeddedResoureSource.Uri?.OriginalString;
                if (string.IsNullOrWhiteSpace(uri))
                    return null;

                return new ImageSourceBinding(FFImageLoading.Work.ImageSourceType.EmbeddedResource, uri);
            }

            var dataUrlSource = source as DataUrlImageSource;
            if (dataUrlSource != null)
            {
                if (string.IsNullOrWhiteSpace(dataUrlSource.DataUrl))
                    return null;

                return new ImageSourceBinding(FFImageLoading.Work.ImageSourceType.Url, dataUrlSource.DataUrl);
            }

            var vectorSource = source as IVectorImageSource;
			if (vectorSource != null)
			{
                if (element != null && vectorSource.VectorHeight == 0 && vectorSource.VectorHeight == 0)
                {
                    if (element.Height > 0d && !double.IsInfinity(element.Height))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorHeight = (int)element.Height;
                    }
                    else if (element.Width > 0d && !double.IsInfinity(element.Width))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorWidth = (int)element.Width;
                    }
                    else if (element.HeightRequest > 0d && !double.IsInfinity(element.HeightRequest))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorHeight = (int)element.HeightRequest;
                    }
                    else if (element.WidthRequest > 0d && !double.IsInfinity(element.WidthRequest))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorWidth = (int)element.WidthRequest;
                    }
                    else if (element.MinimumHeightRequest > 0d && !double.IsInfinity(element.MinimumHeightRequest))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorHeight = (int)element.MinimumHeightRequest;
                    }
                    else if (element.MinimumWidthRequest > 0d && !double.IsInfinity(element.MinimumWidthRequest))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorWidth = (int)element.MinimumWidthRequest;
                    }
                }

                return GetImageSourceBinding(vectorSource.ImageSource, element);
            }

            throw new NotImplementedException("ImageSource type not supported");
        }

        public override bool Equals(object obj)
        {
            var item = obj as ImageSourceBinding;

            if (item == null)
            {
                return false;
            }

            return this.ImageSourceType == item.ImageSourceType && this.Path == item.Path && this.Stream == item.Stream;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this.ImageSourceType.GetHashCode();
                hash = hash * 23 + Path.GetHashCode();
                hash = hash * 23 + Stream.GetHashCode();
                return  hash;
            }
        }
    }
}

