using StbImageSharp;
using StbImageWriteSharp;
using System;

namespace cubemap6to1;

// Probably has Debug Properties set to this:
// "c:\source\volts-laboratory\textures\Shadertoy St Peters Basilica 256x256 0.jpg"

internal class Program
{
    static ImageResult[] image;
    static int iw, ih, ow, oh;
    static byte[] output;

    static void Main(string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
        {
            Console.WriteLine("Invalid argument count.");
            ShowHelp();
            return;
        }

        if (!File.Exists(args[0]))
        {
            Console.WriteLine($"File not found:\n{args[0]}");
            ShowHelp();
            return;
        }

        var in_type = Path.GetExtension(args[0]).Substring(1);
        var out_type = ((args.Length == 1) ? in_type : args[1]).ToLowerInvariant();
        if(!out_type.Equals("jpg") && !out_type.Equals("png"))
        {
            Console.WriteLine("Output format must be \"jpg\" or \"png\".");
            ShowHelp();
            return;
        }

        var path = Path.GetDirectoryName(args[0]);
        var filename = Path.GetFileNameWithoutExtension(args[0]);
        if(!filename.EndsWith("0"))
        {
            Console.WriteLine("Pathname must refer to a file named zero of six (numbered 0 through 5).");
            ShowHelp();
            return;
        }

        var basefilename = filename.Substring(0, filename.Length - 1); // not trimmed here

        image = new ImageResult[6];
        for(int i = 0; i < 6; i++)
        {
            var pathname = Path.Combine(path, $"{basefilename}{i}.{in_type}");
            Console.WriteLine($"Reading {pathname}");

            if (!File.Exists(pathname))
            {
                Console.WriteLine($"Unable to locate file {i} in the series.");
                ShowHelp();
                return;
            }

            using (var stream = File.OpenRead(pathname))
            {
                image[i] = ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
            }

            if (image[i] is null)
            {
                Console.WriteLine("Unsupported image file type.");
                ShowHelp();
                return;
            }

            if(i > 0 && image[i].Width != image[0].Width || image[i].Height != image[0].Height)
            {
                Console.WriteLine($"File {i} doesn't match the dimensions of file 0.");
                ShowHelp();
                return;
            }
        }

        iw = image[0].Width;
        ih = image[0].Height;
        ow = iw * 4;
        oh = ih * 3;
        Console.WriteLine($"Loaded {iw} x {ih} images, outputting {ow} x {oh} image.");

        output = new byte[4 * ow * oh];
        CopyBytes(1, 0, 1);
        CopyBytes(2, 1, 0);
        CopyBytes(4, 1, 1);
        CopyBytes(3, 1, 2);
        CopyBytes(0, 2, 1);
        CopyBytes(5, 3, 1);



        var outpathname = Path.Combine(path, $"{basefilename.TrimEnd()}.{out_type}");
        Console.WriteLine($"Writing {outpathname}");

        using var ostream = File.OpenWrite(outpathname);
        var writer = new ImageWriter();
        if (out_type.Equals("jpg"))
        {
            const int JpegQuality = 90;
            writer.WriteJpg(output, ow, oh, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, ostream, JpegQuality);
        }
        else
        {
            writer.WritePng(output, ow, oh, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, ostream);
        }
    }

    static void CopyBytes(int srcindex, int xgrid, int ygrid)
    {
        int xoffset = xgrid * iw * 4;
        int yoffset = ygrid * ih * iw * 16;

        for (int y = 0; y < ih; y++)
        {
            for (int x = 0; x < iw; x++)
            {
                int ei = (x * 4) + (y * iw * 4);
                int eo = (x * 4 + xoffset) + (y * ow * 4 + yoffset);
                for (int rgba = 0; rgba < 4; rgba++)
                {
                    output[eo + rgba] = image[srcindex].Data[ei + rgba];
                }
            }
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("\nUsage:\ncubemap6to1 [pathname] [png|jpg]\n\nThe output format is optional unless the input isn't PNG or JPG.\n\nSee repository README for details:\nhttps://github.com/MV10/cubemap6to1\n");
    }
}
