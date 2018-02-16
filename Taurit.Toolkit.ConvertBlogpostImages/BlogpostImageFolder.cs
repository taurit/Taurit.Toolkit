using System;
using System.IO;
using JetBrains.Annotations;
using Taurit.Toolkit.FileProcessors;
using Taurit.Toolkit.FileProcessors.ConversionProcessors;

namespace Taurit.Toolkit.ConvertBlogpostImages
{
    internal class BlogpostImageFolder
    {
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
                    new ConvertToJpegProcessor(@"\.1200x627\.hq\.jpg$", new JpegFileQuality(70),
                        new ReplaceEndStrategy(".hq.jpg", ".jpg"))
                };
                IConversionSource conversionSource =
                    ConversionSourceFactory.GetConversionSource(postDirectory, conversionOptions);
                conversionSource.Process();
            }
        }
    }
}