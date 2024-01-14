using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

//public class Annotator
//{
//    public List<string> GetTextInLines(string text, int pixelWidth)
//    {
//        //for some reason we need a  "real" graphics object to calculate text widths based off of.
//        var remainingText = text;
//        var parts = remainingText.Split('\n');
//        var lines = new List<string>();

//        foreach (var part in parts)
//        {
//            var remainingTestThisLine = part.Trim() + " ";
//            while (!string.IsNullOrEmpty(remainingTestThisLine))
//            {
//                if (remainingTestThisLine == " ")
//                {
//                    break;
//                }
//                var testLength = remainingTestThisLine.Length - 1;
//                while (true)
//                {
//                    if (testLength == 0)
//                    {
//                        break;
//                    }
//                    var nth = remainingTestThisLine[testLength];
//                    if (nth != ' ' && nth != '\t')
//                    {
//                        testLength--;
//                        continue;
//                    }
//                    var candidateText = remainingTestThisLine.Substring(0, testLength);

//                    var w = FakeGraphics.MeasureString(candidateText, Font);
//                    if (w.Width < pixelWidth)
//                    {
//                        remainingTestThisLine = remainingTestThisLine.Substring(testLength).Trim() + " ";
//                        lines.Add(candidateText.Trim());
//                        break;
//                    }
//                    testLength--;

//                }
//            }
//        }

//        return lines;
//    }

//    public async Task<string> Annotate(string source, string dest, string text)
//    {
//        var outputImageToAnnotate = Image.FromFile(source);
//        var holdImages = new List<Image>();
//        Size outputSize;
//        //output is the original size, plus some Y increase to fit the text.
//        //plus, if the image were constructed from other images through "blend" or similar
//        //expand the X axis much more to fit them in too.

//        var outputOffsetX = 0;

//        var maxYSeen = outputImageToAnnotate.Height;
//        outputSize = outputImageToAnnotate.Size;

//        var lines = GetTextInLines(text, outputSize.Width);

//        var extraYPixels = LineSize * lines.Count() + TextExtraY;

//        var im = new Bitmap(outputSize.Width, outputSize.Height + extraYPixels);

//        var graphics = Graphics.FromImage(im);
//        graphics.Clear(Color.Black);

//        var holdOffsetX = 0;
//        foreach (var holdImage in holdImages)
//        {
//            graphics.DrawImage(holdImage, new Point(holdOffsetX, 0));
//            holdOffsetX += holdImage.Width;
//        }

//        graphics.DrawImage(outputImageToAnnotate, new Point(outputOffsetX, 0));

//        outputImageToAnnotate.Dispose();

//        var ii = 0;
//        var brush = new SolidBrush(Color.White);

//        //note that we draw the text fully left-aligned which may be weird for blend images.
//        foreach (var line in lines)
//        {
//            var pos = (float)Math.Floor((double)(maxYSeen + TextExtraY / 2 + ii * LineSize));
//            graphics.DrawString(line, Font, brush, new PointF(0, pos));

//            ii += 1;
//        }

//        //add my watermarking etc here.  Slightly annoying since to be perfect I should maybe calculate the remaining Y space left for my small annotation?  But that's annoying. Rather just add 10pix or so to the bottom by default and fill mine in there.
//        var SAIconpos = (float)Math.Floor((double)(maxYSeen + TextExtraY / 2 + (ii - 1) * LineSize) + 16);

//        var myFixedText = "Dalle3/SocialAI";
//        var w = FakeGraphics.MeasureString(myFixedText, MyFont);
//        var p = new PointF(im.Width - w.Width - 4, SAIconpos);
//        var verDarkGrey = Color.FromArgb(255, 36, 36, 36);
//        var mybrush = new SolidBrush(verDarkGrey);
//        graphics.DrawString(myFixedText, MyFont, mybrush, p);

//        graphics.Save();
//        im.Save(dest);
//        Console.WriteLine($"Successfully saved new fp: {dest}");
//        return dest;
//    }

//    public Class1()
//    {
//    }
//}
