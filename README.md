# cubemap6to1

A quick-and-dirty utility for stitching 6 separate cubemap faces into a single image file. Quick-and-dirty means it works but I make no guarantees. It should work cross-platform, although I've only used it from Windows to create the cubemap textures in my [Volt's Laboratory](https://github.com/MV10/volts-laboratory) repository. It has a few rules about the input files.

Usage is simple:

```cubemap6to1 [pathname] [png|jpg]```

The `pathname` argument should point to file zero. For example:

```c:\source\volts-laboratory\textures\Fancy Space Cubemap 256x256 0.png```

Use quotes if there are spaces anywhere in the pathname.

Files 1 through 5 must exist in the same location with the same name, file type, and dimensions.

The format argument is usually optional and can be `png` or `jpg`. If omitted, the source file type is used. However, several other source filetypes are supported, and in that case the format would be required.

The output filename is the same as the inputs with the trailing number omitted (and any trailing space omitted, which Windows doesn't support anyway).

The output file will have an horizontal resolution that is 4X the inputs, and a vertical resolution that is 3X the inputs. Keep in mind that if your source files have a dimension in the filename (as in the example above), this utility doesn't know anything about that, and the output file will still have that part even though it's a totally different resolution.

To be clear, this is the relationship of the numbered input files to the combined output, using [Shadertoy's Uffizi Gallery](https://www.shadertoy.com/view/tdjXDt) cubemap as the sample image:

![cubemap layout](https://raw.githubusercontent.com/MV10/volts-laboratory/master/misc/CubemapLayout.jpg)




