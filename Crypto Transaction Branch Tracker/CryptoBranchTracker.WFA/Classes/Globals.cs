using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBranchTracker.WFA.Classes
{
    public class Globals
    {
        //Compress a string down to a base64 format
        public static string Compress(string text)
        {
            string compressedString;

            try
            {
                byte[] compressedBytes;

                using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
                {
                    using (var compressedStream = new MemoryStream())
                    {
                        using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
                            uncompressedStream.CopyTo(compressorStream);

                        compressedBytes = compressedStream.ToArray();
                    }
                }

                compressedString = Convert.ToBase64String(compressedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred compressing string: {ex}");
            }

            return compressedString;
        }

        //Decompress a string from the Compress method back to the readable string
        public static string Decompress(string compressedString)
        {
            string decompressedString;

            try
            {
                byte[] decompressedBytes;

                var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

                using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        decompressorStream.CopyTo(decompressedStream);

                        decompressedBytes = decompressedStream.ToArray();
                    }
                }

                decompressedString = Encoding.UTF8.GetString(decompressedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred decompressing string: {ex}");
            }

            return decompressedString;
        }
    }
}
