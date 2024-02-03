using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

using static Dalle3.Statics;
using Image = System.Drawing.Image;

public class Annotator
{
    public Graphics FakeGraphics { get; set; } = Graphics.FromImage(new Bitmap("../../image.png"));
    public Font Font { get; set; } = new Font("Gotham", FontSize, FontStyle.Regular);
    public Font MyFont { get; set; } = new Font("Gotham", 18, FontStyle.Bold);
    public static int LineSize { get; set; } = 45;
    public static int FontSize { get; set; } = 36;
    //Unused currently, but we should move text in a bit to make it more visible in twitter previews where now it's slightly cut off.
    public static int HorizontalBuffer { get; set; } = 10;

    //extra y to add to images in annotation section as a kind of vertical buffer.
    public static int TextExtraY { get; set; } = LineSize / 2 + 5;
    
    public List<string> GetTextInLines(string text, int pixelWidth)
    {
        //for some reason we need a  "real" graphics object to calculate text widths based off of.
        var remainingText = text;
        var parts = remainingText.Split('\n');
        var lines = new List<string>();

        foreach (var part in parts)
        {
            var remainingTextThisLine = part.Trim() + " ";
            while (!string.IsNullOrEmpty(remainingTextThisLine))
            {
                if (remainingTextThisLine == " ")
                {
                    break;
                }
                var testLength = remainingTextThisLine.Length - 1;
                while (true)
                {
                    if (testLength == 0)
                    {
                        break;
                    }
                    var nth = remainingTextThisLine[testLength];
                    if (nth != ' ' && nth != '\t')
                    {
                        testLength--;
                        continue;
                    }
                    var candidateText = remainingTextThisLine.Substring(0, testLength);

                    var w = FakeGraphics.MeasureString(candidateText, Font);
                    if (w.Width < pixelWidth)
                    {
                        remainingTextThisLine = remainingTextThisLine.Substring(testLength).Trim() + " ";
                        lines.Add(candidateText.Trim());
                        break;
                    }
                    testLength--;

                }
            }
        }

        return lines;
    }

    public async Task<string> Annotate(string source, string dest, string text)
    {
        var outputImageToAnnotate = Image.FromFile(source);
        Size outputSize;

        var outputOffsetX = 0;

        var maxYSeen = outputImageToAnnotate.Height;
        outputSize = outputImageToAnnotate.Size;

        var lines = GetTextInLines(text, outputSize.Width);

        var extraYPixels = LineSize * lines.Count + TextExtraY;

        var im = new Bitmap(outputSize.Width, outputSize.Height + extraYPixels);

        var graphics = Graphics.FromImage(im);
        graphics.Clear(Color.Black);

        graphics.DrawImage(outputImageToAnnotate, new Point(outputOffsetX, 0));

        outputImageToAnnotate.Dispose();
        var brush = new SolidBrush(Color.White);

        var ii = 0;
        foreach (var line in lines)
        {
            var pos = (float)Math.Floor((double)(maxYSeen + TextExtraY / 2 + ii * LineSize));
            graphics.DrawString(line, Font, brush, new PointF(0, pos));

            ii += 1;
        }

        //add my watermarking etc here.  Slightly annoying since to be perfect I should maybe calculate the remaining Y space left for my small annotation?
        //But that's annoying. Rather just add 10pix or so to the bottom by default and fill mine in there.
        var SAIconpos = (float)Math.Floor((double)(maxYSeen + TextExtraY / 2 + (ii - 1) * LineSize) + 16);

        var myFixedText = "Dalle3-cmdline";
        var w = FakeGraphics.MeasureString(myFixedText, MyFont);
        var p = new PointF(im.Width - w.Width - 4, SAIconpos);
        var verDarkGrey = Color.FromArgb(255, 36, 36, 36);
        var mybrush = new SolidBrush(verDarkGrey);
        graphics.DrawString(myFixedText, MyFont, mybrush, p);

        graphics.Save();
        im.Save(dest);
        Logger.Log($"Successfully saved new fp: {dest}");
        return dest;
    }
}
