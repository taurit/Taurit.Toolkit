﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Taurit.Toolkit.CompressVideo
{
    // draft of video converter wrapper for easy use in totalcmd
    internal class CompressVideo
    {
        private static void Main(String[] args)
        {
            String inputFileOrDirectory = args[0];

            new CompressVideo().CompressVideos(inputFileOrDirectory);
        }

        private void CompressVideos(String inputFileOrDirectory)
        {
            ReadOnlyCollection<String> filesToConvert = GetFilesInDirectory(inputFileOrDirectory).AsReadOnly();
            foreach (String file in filesToConvert)
            {
                String outputFileName = file + ".mp4";
                ConvertSingleFile(file, outputFileName);
            }
        }

        [JetBrains.Annotations.Pure]
        private static List<String> GetFilesInDirectory(String directoryOrFilePath)
        {
            if (File.Exists(directoryOrFilePath))
            {
                return new List<String> {directoryOrFilePath};
            }

            return Directory.GetFiles(directoryOrFilePath).ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="vcodec">x264 seems to be the best option today, and widely accepted standard</param>
        /// <param name="acodec">aac chosen as default for its good quality https://trac.ffmpeg.org/wiki/Encode/HighQualityAudio</param>
        /// <param name="crf">
        ///     "The range of the quantizer scale is 0-51: where 0 is lossless, 23 is default, and 51 is worst
        ///     possible. A lower value is a higher quality and a subjectively sane range is 18-28. Consider 18 to be visually
        ///     lossless or nearly so: it should look the same or nearly the same as the input but it isn't technically lossless. "
        ///     https://trac.ffmpeg.org/wiki/Encode/H.264#crf
        /// </param>
        private void ConvertSingleFile(String inputFile, String outputFile, String vcodec = "libx264",
            String acodec = "aac", Int32 crf = 23)
        {
            Contract.Assert(inputFile != null);
            Contract.Assert(outputFile != null);
            Contract.Assert(!Directory.Exists(inputFile));

            var ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.FileName = "ffmpeg";
            ffmpegProcess.StartInfo.Arguments =
                $"-hide_banner -vcodec {vcodec} -acodec {acodec} -crf {crf} \"{outputFile}\" -i \"{inputFile}\"";
            ffmpegProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            ffmpegProcess.Start();
            ffmpegProcess.WaitForExit();
        }
    }
}