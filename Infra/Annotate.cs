using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

using static Dalle3.Statics;
using Image = System.Drawing.Image;
using System.Linq;

public class Annotator
{
    public Graphics FakeGraphics { get; set; } = Graphics.FromImage(new Bitmap("../../image.png"));

    public static int LineSize { get; set; } = 27;
    public static int FontSize { get; set; } = 18;
    public Font Font { get; set; } = new Font("Gotham", FontSize, FontStyle.Regular);
    public Font LabelFont { get; set; } = new Font("Gotham", FontSize / 2, FontStyle.Bold);

    //Unused currently, but we should move text in a bit to make it more visible in twitter previews where now it's slightly cut off.
    public static int HorizontalBuffer { get; set; } = 10;

    /// <summary>
    /// just split it up into lines based on the width you have to display it in.
    /// Luckily you can easily adjust this to make it wrap around the image some too.
    /// </summary>
    public List<string> GetTextInLines(string text, int pixelWidth)
    {
        //for some reason we need a "real" graphics object to calculate text widths based off of.
        var remainingText = text.Trim();
        var parts = remainingText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        var lines = new List<string>();

        foreach (var part in parts)
        {
            var remainingTextThisLine = part;
            if (string.IsNullOrWhiteSpace(remainingTextThisLine))
            {
                //if it is just a newline, add it as a line.
                lines.Add(remainingTextThisLine);
                continue;
            }
            while (!string.IsNullOrEmpty(remainingTextThisLine))
            {
                remainingTextThisLine = remainingTextThisLine.TrimEnd();
                if (string.IsNullOrWhiteSpace(remainingTextThisLine))
                {
                    break;
                }

                //just try to fit it all.
                var w = FakeGraphics.MeasureString(remainingTextThisLine, Font);
                if (w.Width < pixelWidth)
                {
                    lines.Add(remainingTextThisLine);
                    break;
                }

                //test breaking off words from the end.
                var words = remainingTextThisLine.Split(' ');
                var wordsToSkipAtEnd = 1;
                var shouldRetryAllSplitAttemptsFromBeginning = false;
                while (true)
                {
                    var joined = string.Join(" ", words.Take(words.Count() - wordsToSkipAtEnd));
                    var w2 = FakeGraphics.MeasureString(joined, Font);
                    if (w2.Width < pixelWidth)
                    {
                        words = words.Skip(words.Count()- wordsToSkipAtEnd).ToArray();
                        remainingTextThisLine = string.Join(" ", words);
                        lines.Add(joined);
                        shouldRetryAllSplitAttemptsFromBeginning = true;
                        break;
                    }
                    wordsToSkipAtEnd++;
                }
                if (shouldRetryAllSplitAttemptsFromBeginning)
                {
                    continue;
                }

                //now just hard cut since it both doesn't fully fit, and there are no interior word breaks which work either.
                var testLength = remainingText.Length - 1;
                while (true)
                {
                    if (testLength == 0)
                    {
                        //how can this happen? the output image is just too narrow?
                        throw new Exception("While fitting text into image, even taking 1 character at a time wasn't able to fit. hmm this has got to be a bug.");
                    }
                    var candidateText2 = remainingTextThisLine.Substring(0, testLength);
                    var w3 = FakeGraphics.MeasureString(candidateText2, Font);
                    if (w3.Width < pixelWidth)
                    {
                        lines.Add(candidateText2);
                        remainingTextThisLine = remainingTextThisLine.Substring(0, testLength);

                        //we break so we try the full splitting attempt thing again. this is important.
                        shouldRetryAllSplitAttemptsFromBeginning = true;
                        break;
                    }
                    testLength--;
                }

                if (shouldRetryAllSplitAttemptsFromBeginning)
                {
                    continue;
                }

                throw new Exception("Shouldn't be able to get here.");
            }
        }

        return lines;
    }

    /// <summary>
    /// Annotate an image file at source with black text and a light text below (optional), expanding the height of the image and
    /// trying not to lose any pixels.
    /// </summary>
    public async Task<string> Annotate(string srcFp, string destFp, string text, bool includeSourceLabel)
    {
        var outputOffsetX = 0;
        var sourceLabelExtraYPixels = 0;

        var inputImageToAnnotate = Image.FromFile(srcFp);
        var outputSize = inputImageToAnnotate.Size;
        var originalImageYHeightPixels = inputImageToAnnotate.Height;

        var lines = GetTextInLines(text, outputSize.Width);

        if (includeSourceLabel)
        {
            sourceLabelExtraYPixels = LineSize / 2 + 20;
        }

        var extraYPixels = LineSize * lines.Count + sourceLabelExtraYPixels;

        var im = new Bitmap(outputSize.Width, outputSize.Height + extraYPixels);

        var graphics = Graphics.FromImage(im);
        graphics.Clear(Color.Black);

        graphics.DrawImage(inputImageToAnnotate, new Point(outputOffsetX, 0));

        inputImageToAnnotate.Dispose();
        var brush = new SolidBrush(Color.White);

        var ii = 0;
        foreach (var line in lines)
        {
            var pos = (float)Math.Floor((double)(originalImageYHeightPixels + ii * LineSize));
            graphics.DrawString(line, Font, brush, new PointF(0, pos));

            ii += 1;
        }

        if (includeSourceLabel)
        {
            //add my watermarking etc here.  Slightly annoying since to be perfect I should maybe calculate the remaining Y space left for my small annotation?
            //But that's annoying. Rather just add 10pix or so to the bottom by default and fill mine in there.
            var labelPos = (float)Math.Floor((double)(originalImageYHeightPixels + (lines.Count - 1) * LineSize + 2 + sourceLabelExtraYPixels));

            var myFixedText = "dalle3-cmdline-csharp";
            var w = FakeGraphics.MeasureString(myFixedText, LabelFont);
            var p = new PointF(im.Width - w.Width - 4, labelPos);
            var verDarkGrey = Color.FromArgb(255, 36, 36, 36);
            var mybrush = new SolidBrush(verDarkGrey);
            graphics.DrawString(myFixedText, LabelFont, mybrush, p);
        }

        graphics.Save();
        if (System.IO.File.Exists(destFp))
        {
            Console.WriteLine($"File already exists. Not overwriting. fp: {destFp}");
        }
        im.Save(destFp);
        //Logger.Log($"Saved annotated version. fp: {destFp}");
        return destFp;
    }
}
