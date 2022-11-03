using System.Drawing;

if (args.Length == 1 || args.Length == 2)
{
    string path = args[0];
    int width = 0;
    if (args.Length==2 && args[1].Length > 0)
    {
        int.TryParse(args[1], out width);
    }
    FileInfo info = new FileInfo(path);
    // BMP, GIF, EXIF, JPG, PNG, and TIFF
    string[] allowedExts = { ".jpeg", ".jpg", ".png", ".gif", ".exif", ".tiff", ".bmp" };
    if (!info.Exists)
    {
        Console.WriteLine("File not found");
        return -1;
    }
    else if (info.Extension.Length == 0 || !allowedExts.Contains(info.Extension))
    {
        Console.WriteLine("Extension not found or invlaid. Please use an image file.");
        return -1;
    }
    string name = info.Name.Replace(info.Extension, "");
    Console.WriteLine("Printing '" + name + "'...\n");
    Bitmap bmp = new Bitmap(info.FullName);
    bmp.PrintASCII();
    bmp.PrintASCII(width);
    bmp.PrintASCII(width,false);
    bmp.PrintASCII(width, true, true);

    return 0;
}
else
{
    Console.WriteLine("Create an ASCII representation of an image. Must be run from the command line. Arguments:\n1st. Path to local image file.\n2nd. Minimum side in characters, default: 80.");
    return -1;
}
