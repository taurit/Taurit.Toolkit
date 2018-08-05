using System;
using JetBrains.Annotations;

namespace Taurit.Toolkit.ConvertBlogpostImages
{
    internal static class Program
    {
        private static void Main([NotNull] String[] args)
        {
            if (args.Length < 1) throw new ArgumentException("inputFileOrDirectory should be passed as an argument");
            String postImagesDirectory = args[0];

            var imageFolder = new BlogpostImageFolder(postImagesDirectory);
            imageFolder.GenerateDerivedImages();

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}