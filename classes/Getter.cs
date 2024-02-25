using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

using static Dalle3.Statics;

namespace Dalle3
{
    /// <summary>
    /// Actually get the file and handle errors.
    /// </summary>
    public static class Getter
    {
        //Google photo uploader and other programs will incorrectly start messing with files before they're finished.
        //1. if you save them with another extension which would be ignored, then you get locking issues when trying to move them.
        //2. so instead i just save them out of view then move them back.
        //3. we also delay creating the unique filename.
        //4. also save an annotated version?
        public static void DownloadCompleted(object sender, AsyncCompletedEventArgs e, OptionsModel optionsModel, string srcFp, string destFp,
            string prompt, string revisedPrompt, IEnumerable<InternalTextSection> ungroupedTextSections)
        {
            try
            {
                //the new reservation system? ugh.
                if (!System.IO.File.Exists(destFp))
                {
                    throw new Exception("No reservation.");
                }
                var fa = new FileInfo(destFp);
                if (fa.Length > 0)
                {
                    throw new Exception("too bad.'");
                }

                try
                {
                    System.IO.File.Copy(srcFp, destFp, true);
                    System.IO.File.Delete(srcFp);
                    //optionsModel.IncStr("DownloadedCount");
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    //we have already moved the file where its supposed to go.
                    Statics.Logger.Log($"Disappeared so going on: {destFp}");
                    optionsModel.IncStr("ErrorCount");
                    //break;

                }
                catch (System.IO.IOException ex)
                {
                    //well, sometimes we call download completed... before the image is completely downloaded. yah that's how things go.
                    Task.WaitAll(Task.Delay(1000));
                    Statics.Logger.Log($"IOexcpeton. {ex} {destFp}");
                    optionsModel.IncStr("ErrorCount");
                    //continue;
                }

                catch (Exception ex)
                {
                    //either it was busy, or the target existed? not sure about that latter thing.
                    Statics.Logger.Log($"\t{destFp}\tFailed with ex: {ex}.");
                    Task.WaitAll(Task.Delay(1000));
                    optionsModel.IncStr("ErrorCount");
                    //continue;
                }

                UpdateWithFilterResult(optionsModel, ungroupedTextSections, TextChoiceResultEnum.Okay);
                //we need to redo the target name cause its gotten taken in the meantime.

                var ann = new Annotator();
                //assume the file exists at uniquefp now.

                //we have two targets: annotated is the FULL annotation, revised is just the revised prompt, not the (possibly massive) entire prompt and training leading up to it..
                var annotatedFp = destFp.Replace(".png", "_annotated.png");

                var aDirectory = Path.GetDirectoryName(annotatedFp);
                var aFilename = Path.GetFileName(annotatedFp);

                annotatedFp = aDirectory + "/annotated/" + aFilename;

                var annotatedCompletePrompt = prompt;
                if (!string.IsNullOrEmpty(revisedPrompt) && revisedPrompt != prompt)
                {
                    //Console.WriteLine($"{revisedPrompt}");
                    revisedPrompt = revisedPrompt.Replace("|||", "\r\n\r\n");
                    if (revisedPrompt.IndexOf("\\") != -1)
                    {
                        revisedPrompt = revisedPrompt.Replace("\\r\\n", "\r\n");
                    }
                    annotatedCompletePrompt += $"\r\n\r\nGPT revised it to: ==> \r\n\r\n\"{revisedPrompt}\"";
                }
                
                ann.Annotate(destFp, annotatedFp, annotatedCompletePrompt, true);

                var revisedAnnotationFp = destFp.Replace(".png", "_revised.png");
                var rDirectory = Path.GetDirectoryName(revisedAnnotationFp);
                var rFilename = Path.GetFileName(revisedAnnotationFp);
                revisedAnnotationFp = rDirectory + "/revised/" + rFilename;

                ann.Annotate(destFp, revisedAnnotationFp, revisedPrompt, true);
                Statics.Logger.Log($"Saved revised version. fp: {revisedAnnotationFp}");
                DoReport(optionsModel);
            }
            catch (Exception ex)
            {
                Statics.Logger.Log("weird error." + ex.ToString());
                optionsModel.IncStr("ErrorCount");
                DoReport(optionsModel);
            }
        }
    }
}
