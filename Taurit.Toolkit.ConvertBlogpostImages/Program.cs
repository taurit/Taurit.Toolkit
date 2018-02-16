using System;

namespace Taurit.Toolkit.ConvertBlogpostImages
{
    internal static class Program
    {
        private static void Main(String[] args)
        {
            String postImagesDirectory = args[0];

            var imageFolder = new BlogpostImageFolder(postImagesDirectory);
            imageFolder.GenerateDerivedImages();

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}