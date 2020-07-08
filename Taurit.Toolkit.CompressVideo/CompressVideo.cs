using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Taurit.Toolkit.CompressVideo
{
    // draft of video converter wrapper for easy use in totalcmd
    internal static class CompressVideo
    {
        private static void Main([NotNull] String[] args)
        {
            if (args.Length < 1) throw new ArgumentException("inputFileOrDirectory should be passed as an argument");
            String inputFileOrDirectory = args[0];

            CompressVideo.CompressVideos(inputFileOrDirectory);
        }

        private static void CompressVideos(String inputFileOrDirectory)
        {
            ReadOnlyCollection<String> filesToConvert = CompressVideo.GetFilesInDirectory(inputFileOrDirectory).AsReadOnly();

            Int32 totalFiles = filesToConvert.Count;

            if (totalFiles != 1)
            {
                Console.WriteLine($"Found {totalFiles} files to convert:");
                foreach (String fileName in filesToConvert) Console.WriteLine($"* {fileName}");
                Console.WriteLine(" ");
            }

            var fileNumber = 0;
            foreach (String file in filesToConvert)
            {
                String outputFileName = (file + ".min.h265.mp4").Replace(".mp4.min", ".min");

                if (totalFiles != 1)
                    Console.WriteLine($"Converting {file} to {outputFileName} (file {++fileNumber}/{totalFiles})...");

                CompressVideo.ConvertSingleFile265(file, outputFileName);
            }
        }

        [JetBrains.Annotations.Pure]
        private static List<String> GetFilesInDirectory(String directoryOrFilePath)
        {
            if (File.Exists(directoryOrFilePath)) return new List<String> {directoryOrFilePath};

            return Directory.GetFiles(directoryOrFilePath)
                .Where(fileName => !fileName.Contains(".min."))
                .Where(fileName => !fileName.Contains(".h265."))
                .ToList();
        }

        /// <param name="outputFile"></param>
        /// <param name="vcodec">
        ///     x264 used to be the best option and a widely accepted standard before its successor was developed
        ///     :)
        /// </param>
        /// <param name="acodec">aac chosen as default for its good quality https://trac.ffmpeg.org/wiki/Encode/HighQualityAudio</param>
        /// <param name="crf">
        ///     "The range of the quantizer scale is 0-51: where 0 is lossless, 23 is default, and 51 is worst
        ///     possible. A lower value is a higher quality and a subjectively sane range is 18-28. Consider 18 to be visually
        ///     lossless or nearly so: it should look the same or nearly the same as the input but it isn't technically lossless. "
        ///     https://trac.ffmpeg.org/wiki/Encode/H.264#crf
        /// </param>
        /// <param name="inputFile"></param>
        // ReSharper disable once UnusedMember.Local
        private static void ConvertSingleFile264(String inputFile, String outputFile, String vcodec = "libx264",
            String acodec = "aac", Int32 crf = 23)
        {
            Contract.Assert(inputFile != null);
            Contract.Assert(outputFile != null);
            Contract.Assert(!Directory.Exists(inputFile));

            using var ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.FileName = "d:\\ProgramData\\Tools\\ffmpeg\\bin\\ffmpeg.exe";
            ffmpegProcess.StartInfo.Arguments =
                $"-hide_banner -vcodec {vcodec} -acodec {acodec} -crf {crf} \"{outputFile}\" -i \"{inputFile}\"";
            ffmpegProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            try
            {
                ffmpegProcess.Start();
                ffmpegProcess.WaitForExit();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine($"Could not run ffmpeg: {e.Message}");
                Console.ReadLine();
            }
        }

        /// <param name="outputFile"></param>
        /// <param name="vcodec">x264 seems to be the best option today, and widely accepted standard</param>
        /// <param name="acodec">aac chosen as default for its good quality https://trac.ffmpeg.org/wiki/Encode/HighQualityAudio</param>
        /// <param name="crf">https://trac.ffmpeg.org/wiki/Encode/H.265</param>
        /// <param name="inputFile"></param>
        private static void ConvertSingleFile265(String inputFile, String outputFile, String vcodec = "libx265",
            String acodec = "aac", Int32 crf = 28)
        {
            Contract.Assert(inputFile != null);
            Contract.Assert(outputFile != null);
            Contract.Assert(!Directory.Exists(inputFile));

            using var ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.FileName = "cmd.exe";
            ffmpegProcess.StartInfo.Arguments =
                $"/C ffmpeg-bar -hide_banner  -i \"{inputFile}\" -vcodec {vcodec} -preset slow -crf {crf} -acodec {acodec} \"{outputFile}\"";
            // I wasnt able to produce good quality output with a GPU hardware acceleration enabled, this seems to be true: https://www.reddit.com/r/pcgaming/comments/6vdmv6/guide_hardware_accelerated_h265hevc_encoding_for/dlziqai?utm_source=share&utm_medium=web2x
            // I tested many options, but the video quality was not good enough (despite very fast encoding).
            // Software encoders seems to produce just better results with the same options (or maybe GPU encoder does not support all options as of 2020-07)

            ffmpegProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            try
            {
                ffmpegProcess.Start();
                ffmpegProcess.WaitForExit();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine($"Could not run the command: {e.Message}");
                Console.ReadLine();
            }
        }
    }
}