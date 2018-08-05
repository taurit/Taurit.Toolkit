using System;
using System.IO;
using JetBrains.Annotations;
using Taurit.Toolkit.FileProcessors;
using Taurit.Toolkit.FileProcessors.ConversionProcessors;

namespace Taurit.Toolkit.ConvertBlogpostImages
{
    internal class BlogpostImageFolder
    {
        [NotNull]
        private readonly String _rootPath;

        public BlogpostImageFolder([NotNull] String rootPath)
        {
            if (rootPath == null)
                throw new ArgumentNullException(nameof(rootPath));
            if (!Directory.Exists(rootPath)) throw new ArgumentException("Directory does not exist");

            _rootPath = rootPath;
        }


        public void GenerateDerivedImages()
        {
            String[] postDirectories = Directory.GetDirectories(_rootPath); // each post has its own subdirectory
            foreach (String postDirectory in postDirectories)
            {
                var conversionOptions = new IFileProcessor[]
                {
                    // generate compressed representation of a wide image
                    new ConvertToJpegProcessor(@"\.1200x627\.hq\.jpg$",
                        new JpegFileQuality(70),
                        new ReplaceEndStrategy(".hq.jpg", ".jpg"),
                        new EmptyConversionStrategy()),

                    // generate compressed representation of a square image
                    new ConvertToJpegProcessor(@"\.600x600\.hq\.jpg$",
                        new JpegFileQuality(70),
                        new ReplaceEndStrategy(".hq.jpg", ".jpg"),
                        new EmptyConversionStrategy()),

                    // generate compressed and downsized representation of a square image
                    new ConvertToJpegProcessor(@"\.600x600\.hq\.jpg$",
                        new JpegFileQuality(70),
                        new ReplaceEndStrategy(".600x600.hq.jpg", ".150x150.jpg"),
                        new ResizeStrategy(150, 150)
                    ),

                    // generate downsized representation of a square image converted to WebP
                    new ConvertToWebpProcessor(@"\.600x600\.hq\.jpg$",
                        new WebpFileQuality(80),
                        Int32.MaxValue,
                        new ReplaceEndStrategy(".600x600.hq.jpg", ".150x150.webp"),
                        new ResizeStrategy(150, 150),
                        new ConsoleLoggingStrategy(ConsoleLoggingStrategy.LogLevel.Actions)
                    ),

                    // in case thumbnail is png, and not jpg
                    new ConvertToWebpProcessor(@"\.600x600\.hq\.png$",
                        new WebpFileQuality(80),
                        Int32.MaxValue,
                        new ReplaceEndStrategy(".600x600.hq.png", ".150x150.webp"),
                        new ResizeStrategy(150, 150),
                        new ConsoleLoggingStrategy(ConsoleLoggingStrategy.LogLevel.Actions)
                    ),

                    // in case thumbnail is png, and not jpg
                    new ConvertToPngProcessor(@"\.600x600\.hq\.png$",
                        new PngFileQuality(100),
                        new ReplaceEndStrategy(".hq.png", ".png"),
                        new EmptyConversionStrategy())
                };
                IConversionSource conversionSource =
                    ConversionSourceFactory.GetConversionSource(postDirectory, conversionOptions);
                conversionSource.Process();
            }
        }
    }
}