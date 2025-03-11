// See https://aka.ms/new-console-template for more information
using IronOcr;

string pathForOCR = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "35-2.pdf"); // Keep the file in solution folder
MemoryStream stream = null;
const string IronOcrLicense = "Key";

try
{
    using (Stream tempStream = new FileStream(pathForOCR, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    {
        // copy stream to memorystream
        stream = new MemoryStream();
        if (tempStream.CanSeek)
        {
            int pos = (int)stream.Position;
            int length = (int)(tempStream.Length - tempStream.Position) + pos;
            stream.SetLength(length);

            while (pos < length)
                pos += tempStream.Read(stream.GetBuffer(), pos, length - pos);
        }
        else
            tempStream.CopyTo((Stream)stream);
    }


    FileInfo fielInfo = new FileInfo(pathForOCR);
    string fileExtension = fielInfo.Extension;

    string[] allowedOCRExtensions = { ".png", ".jpeg", ".jpg", ".tiff", ".pdf" };


    if (allowedOCRExtensions.Contains(fileExtension.ToLower()))
    {
        if (stream != null)
        {
            //Start: Code for OCR    
            IronOcr.Installation.LicenseKey = IronOcrLicense;
            bool isValidKey = IronOcr.License.IsValidLicense(IronOcrLicense);
            if (isValidKey)
            {
                var Ocr = new IronTesseract();
                Ocr.Language = OcrLanguage.English;
                using (var Input = new OcrInput())
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();

                    switch (fileExtension.ToLower())
                    {
                        case ".png":
                        case ".jpeg":
                        case ".jpg":
                        case ".tiff":
                            Input.DeNoise();
                            Input.LoadImage(stream);
                            break;
                        case ".pdf":
                            Input.LoadPdf(stream);
                            break;
                    }
                    var Result = Ocr.Read(Input);
                    Console.WriteLine( Result.Text);
                }
                // End: Code for OCR 
            }
            else
            {
                Console.WriteLine("Invalid OCR lincense key.");
            }
            stream.Close();
            stream.Dispose();
        }
        else
        {
            Console.WriteLine("Document not found.");
        }
    }

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}