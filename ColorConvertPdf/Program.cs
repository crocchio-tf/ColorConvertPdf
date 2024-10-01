 using Datalogics.PDFL;

 namespace ColorConvertPdf;

 static class ColorConvertPdf
 {
     public static void Main(string[] args)
     {
         Console.WriteLine("ColorConvertPdf Sample:");

         using (Library lib = new Library())
         {
             Console.WriteLine("Initialized the library.");

             // This should be the path to "test_file.pdf"
             var sInput = Library.ResourceDirectory + "../../../../test_file.pdf";
             var sOutput = "../ColorConvertPdf-out.pdf";
             
             if (args.Length > 0)
                 sInput = args[0];

             if (args.Length > 1)
                 sOutput = args[1];

             Console.WriteLine("Converting " + sInput + ", output file is " + sOutput);

             using (Document doc = new Document(sInput))
             {
                 ConvertToGrayscale(doc);
                 // Make a conversion parameters object
                 PDFAConvertParams pdfaParams = new PDFAConvertParams();
                 pdfaParams.AbortIfXFAIsPresent = true;
                 pdfaParams.IgnoreFontErrors = false;
                 pdfaParams.NoValidationErrors = false;
                 pdfaParams.ValidateImplementationLimitsOfDocument = true;

                 // Create a PDF/A compliant version of the document
                 PDFAConvertResult pdfaResult = doc.CloneAsPDFADocument(PDFAConvertType.RGB3b, pdfaParams);

                 // The conversion may have failed: we must check if the result has a valid Document
                 if (pdfaResult.PDFADocument == null)
                 {
                     Console.WriteLine("ERROR: Could not convert " + sInput + " to PDF/A.");
                 }
                 else
                 {
                     Console.WriteLine("Successfully converted " + sInput + " to PDF/A.");

                     Document pdfaDoc = pdfaResult.PDFADocument;

                     // The issue occurs when color converting a PDF/A document
                     ConvertToGrayscale(pdfaResult.PDFADocument);
                     pdfaDoc.Save(pdfaResult.PDFASaveFlags, sOutput);
                 }
             }
         }
     }
     
     private static void ConvertToGrayscale(Document document)
     {
         using var colorConvertAction = new ColorConvertActions();
         colorConvertAction.MustMatchAnyAttrs = ColorConvertObjAttrs.ColorConvAnyObject;
         colorConvertAction.MustMatchAnyCSAttrs = ColorConvertCSpaceType.ColorConvAnySpace;
         colorConvertAction.IntentToMatch = RenderIntent.UseProfileIntent;
         colorConvertAction.ConvertIntent = RenderIntent.UseProfileIntent;
         colorConvertAction.Action = ColorConvertActionType.ConvertColor;
         colorConvertAction.ConvertProfile = ColorProfile.DotGain20;

         using var colorConvertParams = new ColorConvertParams(new[] { colorConvertAction });

         document.ColorConvertPages(colorConvertParams);
     }
 }