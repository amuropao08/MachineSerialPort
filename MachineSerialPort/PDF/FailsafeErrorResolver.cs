using PdfSharp.Fonts;
using System;
using System.Reflection;
using System.IO;

namespace MachineSerialPort.PDF
{
    public class FailsafeErrorResolver : IFontResolver
    {
        // Define a unique internal name for your font family
        private const string FALLBACK_FAMILY = "THSarabunNew";
        private const string ROOT_RESOURCE_PATH = "MachineSerialPort.PDF."; // Adjust

        public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Courier New", StringComparison.OrdinalIgnoreCase) || familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
            {
                string faceName = FALLBACK_FAMILY;
                if (isBold && isItalic) faceName += " BoldItalic";
                else if (isBold) faceName += " Bold";
                else if (isItalic) faceName += " Italic";
                else faceName += "";

                return new FontResolverInfo(faceName);
            }
            
            return null;
        }

        public byte[]? GetFont(string faceName)
        {
            if (faceName.StartsWith(FALLBACK_FAMILY))
            {
                // 1. Map the internal face name to the physical filename
                string fileName = "";

                if (faceName.EndsWith(" BoldItalic"))
                    fileName = "THSarabunNew BoldItalic.ttf";
                else if (faceName.EndsWith(" Bold"))
                    fileName = "THSarabunNew Bold.ttf";
                else if (faceName.EndsWith(" Italic"))
                    fileName = "THSarabunNew Italic.ttf";
                else // Must be Regular
                    fileName = "THSarabunNew.ttf";

                // 2. Construct the full resource path
                string resourceName = ROOT_RESOURCE_PATH + fileName;

                // 3. Load the bytes from the embedded resource
                var assembly = Assembly.GetExecutingAssembly();
                using Stream? stream = assembly.GetManifestResourceStream(resourceName);

                if (stream == null)
                {
                    throw new InvalidOperationException($"Embedded font resource not found: {resourceName}");
                }

                byte[] fontData = new byte[stream.Length];
                stream.Read(fontData, 0, (int)stream.Length);
                return fontData;
            }

            return null; // Font name not handled by this resolver
        }
    }
}