using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Imagegram.API.Application.Enums.Constants;

namespace Imagegram.API.Helpers
{
    public class ImageFormatter : IImageFormatter
    {
        public byte[] CropAndConvert(byte[] imageBytes, int height, int width, ImageFormat format)
        {
            using (var image = Image.Load(imageBytes))
            {
                using (var ms1 = new MemoryStream())
                {
                    image.Mutate(x => x.Crop(width, image.Height));
                    image.SaveAsJpeg(ms1);

                    return ms1.ToArray();
                }
            }
        }
            
    }

    public interface IImageFormatter
    {
        byte[] CropAndConvert(byte[] imageBytes, int height, int width, ImageFormat format);
    }
}
