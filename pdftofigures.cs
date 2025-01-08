using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;

class Program
{
    static void Main(string[] args)
    {
        string pdfPath = "pdfdocument.pdf";
        var figures = ExtractCoordinatesFromPdf(pdfPath);

        foreach (var figure in figures)
        {
            Console.WriteLine($"Page: {figure.Page}, Points: [{string.Join(", ", figure.Points)}]");
        }
    }

    public static List<Figure> ExtractCoordinatesFromPdf(string pdfPath)
    {
        List<Figure> figures = new List<Figure>();
        PdfDocument document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.ReadOnly);

        for (int i = 0; i < document.PageCount; i++)
        {
            var page = document.Pages[i];
            var content = ContentReader.ReadContent(page);

            string pageText = ExtractText(content);

            // Regex to match coordinates like (x, y)
            Regex regex = new Regex(@"\\((\\d+\\.\\d*),\\s*(\\d+\\.\\d*)\\)");
            var matches = regex.Matches(pageText);

            List<(double, double)> points = new List<(double, double)>();
            foreach (Match match in matches)
            {
                double x = double.Parse(match.Groups[1].Value);
                double y = double.Parse(match.Groups[2].Value);
                points.Add((x, y));
            }

            if (points.Count > 0)
            {
                figures.Add(new Figure { Page = i + 1, Points = points });
            }
        }

        return figures;
    }

    public static string ExtractText(CObject contentObject)
    {
        if (contentObject is COperator cOperator && cOperator.Operands != null)
        {
            string result = "";
            foreach (var op in cOperator.Operands)
            {
                result += ExtractText(op);
            }
            return result;
        }
        else if (contentObject is CString cString)
        {
            return cString.Value;
        }

        return string.Empty;
    }
}

public class Figure
{
    public int Page { get; set; }
    public List<(double, double)> Points { get; set; }
}
