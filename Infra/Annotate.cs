using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

using static Dalle3.Statics;
using Image = System.Drawing.Image;
using System.Linq;
using System.Text;
using Dalle3;
using System.Drawing.Text;

public class Annotator
{

    public static int LineYPixels { get; set; } = 30;

    public static int FontSize { get; set; } = 18;
    public Font Font { get; set; } = new Font("Gotham", FontSize, FontStyle.Regular);
    public Font LabelFont { get; set; } = new Font("Gotham", FontSize / 2, FontStyle.Bold);

    //Unused currently, but we should move text in a bit to make it more visible in twitter previews where now it's slightly cut off.
    public static int HorizontalBuffer { get; set; } = 10;

    /// <summary>
    /// just split it up into lines based on the width you have to display it in.
    /// Luckily you can easily adjust this to make it wrap around the image some too.
    /// remainingXPixelsLastLine is so that if you want to jam some stuff in right-aligned along the bottom, you know if you can or not.
    /// lastLineYHeight this is so we can expand to contain the descenders, etc.
    /// </summary>
    public List<string> GetTextInLines(string text, int pixelWidth, out int remainingXPixelsLastLine, out int lastLineYHeight)
    {
        remainingXPixelsLastLine = 0;
        lastLineYHeight = 0;
        //for some reason we need a "real" graphics object to calculate text widths based off of.
        //okay so sometimes things show up with a variety of weird newlines. Basically I want to coalesce all of those into a standard set.        
        var remainingText = CoalesceAllNewlineLikeSections(text);

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
                        words = words.Skip(words.Count() - wordsToSkipAtEnd).ToArray();
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

        //deepTrim
        while (true)
        {
            if (string.IsNullOrEmpty(lines.Last()))
            {
                lines.RemoveAt(lines.Count - 1);
            }
            else
            {
                break;
            }
        }

        var llMeasurement = FakeGraphics.MeasureString(lines.Last(), Font);
        remainingXPixelsLastLine = (int)(pixelWidth - llMeasurement.Width);
        lastLineYHeight = (int)llMeasurement.Height;

        return lines;
    }

    public static string CoalesceAllNewlineLikeSections(string input)
    {
        //the input may have sections of \r, \n, \n\n, \r\n, etc. and we want to convert the entirety of any such contiguous section to a single one.
        //we do this so they don't get doubled up.
        if (string.IsNullOrEmpty(input))
        {
            input = "";
        }
        input = input.Replace("\r\n", "\n").Replace("\r", "\n");

        var parts = input.Split(new string[] { "\r", "\n", "|||" }, StringSplitOptions.None);
        var sb = new StringBuilder();
        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part))
            {
                sb.Append("\r\n");
            }
            else
            {
                sb.Append(part + "\r\n");
            }
        }

        var res = sb.ToString();

        var any = false;

        //condense any 2+ empty lines into 2.
        while (true)
        {
            any = false;
            if (res.IndexOf("\r\n\r\n\r\n") != -1)
            {
                res = res.Replace("\r\n\r\n\r\n", "\r\n\r\n");
                any = true;
            }
            if (!any)
            {
                break;
            }
        }

        return res;
    }

    /// <summary>
    /// Annotate an image file at source with black text and a light text below (optional), expanding the height of the image and
    /// trying not to lose any pixels.
    /// </summary>
    public async Task<string> Annotate(string srcFp, string destFp, string text, bool includeSourceLabel, IEnumerable<SequentialTextPieceToDraw> instructions = null)
    {

        var outputOffsetX = 0;
        //this is the smart way to draw where we also overlay colored boxes and stuff.

        //the extra amount we increase the image size no matter what to make it so that descenders from the text don't overlap too much.
        var inputImageToAnnotate = Image.FromFile(srcFp);
        var outputSize = inputImageToAnnotate.Size;

        //lets characterize this as SOLEY figuring out the special areas, and drawing lighter boxes behind them.
        //if (instructions != null)
        //{
        //    var prvateLines = GetTextInLines(text, outputSize.Width, out int AremainingXPixelsLastLine, out int BlastLineYHeight);
        //    //okay so here we have the full text, plus various subsets of it.

        //    //these will be tracked based on splitting up the input as we go.
        //    var myStartLocationX = 0;
        //    var myStartLocationY = 0;
        //    foreach (var section in instructions)
        //    {
        //        //lets just characterize this as, for every nonfixed part of the input instructions, just figure out what line and prefix point (Y) it starts at.
        //        //Then magically draw a lighter box behind that. The later code will never know.
        //        var privateSection = GetTextInLines(section.text, outputSize.Width, out int aa, out int bb);
        //        if (section.isFixed)
        //        {

        //        }
        //    }
        //}



        var sourceLabelText = "dalle3-cmdline-csharp";
        var sourceLabelWidth = FakeGraphics.MeasureString(sourceLabelText, LabelFont);

        var lines = GetTextInLines(text, outputSize.Width, out int remainingXPixelsLastLine, out int lastLineYHeight);

        var labelExtraYPixels = 0.0f;

        var lastLineOverhangYPixels = Math.Max(lastLineYHeight - LineYPixels, 0);
        //normally it just fits in at the bottom right of the lowest line. But if not enough space, add a bit to fill that out.
        if (includeSourceLabel && remainingXPixelsLastLine < sourceLabelWidth.Width + 40)
        {
            //let's say the label need at least half a line.
            labelExtraYPixels = (float)(LineYPixels * 0.5);
        }

        //two things can expand the height: if the last line has descenders,
        var totalExtraPixels = (int)(LineYPixels * lines.Count + Math.Max(labelExtraYPixels, lastLineOverhangYPixels)) + 2;

        var annotatedImageTotalYPixels = outputSize.Height + totalExtraPixels;
        var im = new Bitmap(outputSize.Width, annotatedImageTotalYPixels);

        var graphics = Graphics.FromImage(im);
        graphics.Clear(Color.Black);

        graphics.DrawImage(inputImageToAnnotate, new Point(outputOffsetX, 0));

        inputImageToAnnotate.Dispose();
        var whiteBrush = new SolidBrush(Color.White);
        var yellowBrush = new SolidBrush(Color.GreenYellow);

        var usingBrush = whiteBrush;
        //create a brush which has a certain background color, also:
        //var brushY=new Brush(brush);

        //var qq = new SolidBrush(
        var lastLineWrittenYPixels = outputSize.Height;
        SequentialTextPieceToDraw currentInstruction = null;
        var instructionIndex = 0;
        if (instructions != null)
        {
            currentInstruction = instructions.First();
            instructionIndex++;
        }
        var ii = 0;
        foreach (var line in lines)
        {
            graphics.DrawString(line, Font, brush, new PointF(0, lastLineWrittenYPixels));

            //if (currentInstruction != null)
            //{
            //    if (currentInstruction.text.Length > line.Length)
            //    {
            //        if (currentInstruction.isFixed)
            //        {
            //            usingBrush = whiteBrush;
            //        }
            //        else
            //        {
            //            usingBrush = yellowBrush;
            //        }
            //        currentInstruction.text = currentInstruction.text.Replace(line, "").Trim();
            //    }
            //    else
            //    {
            //        //just the first part of this line is an instruction.
            //    }
            //}
            lastLineWrittenYPixels += LineYPixels;
        }

        //try not to add too much extra space to the bottom.
        if (includeSourceLabel)
        {
            //add my watermarking etc here.  Slightly annoying since to be perfect I should maybe calculate the remaining Y space left for my small annotation?
            //okay just place it above the floor either way; if we bumped previously or not.
            var labelYPos = (float)Math.Floor((double)(annotatedImageTotalYPixels - LineYPixels * 0.5));

            var p = new PointF(im.Width - sourceLabelWidth.Width - 2, labelYPos);
            var verDarkGrey = Color.FromArgb(255, 36, 36, 36);
            var mybrush = new SolidBrush(verDarkGrey);
            graphics.DrawString(sourceLabelText, LabelFont, mybrush, p);
        }

        graphics.Save();
        if (System.IO.File.Exists(destFp))
        {
            Console.WriteLine($"File already exists. Not overwriting. fp: {destFp}");
        }
        im.Save(destFp);
        return destFp;
    }

}
