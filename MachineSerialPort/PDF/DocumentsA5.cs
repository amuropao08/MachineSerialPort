using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using Border = MigraDoc.DocumentObjectModel.Border;
using Orientation = MigraDoc.DocumentObjectModel.Orientation;
using Paragraph = MigraDoc.DocumentObjectModel.Paragraph;
using Section = MigraDoc.DocumentObjectModel.Section;
using Table = MigraDoc.DocumentObjectModel.Tables.Table;

namespace MachineSerialPort.PDF
{
    class DocumentsA5
    {
        private ObservableCollection<LeatherArea> MeasureData = new();

        private string DocDate = string.Empty;
        private string Grade = string.Empty;
        private string MatDesc = string.Empty;
        private string MatNo = string.Empty;
        private string NGReason = string.Empty;
        private string OrderNo = string.Empty;
        private string TotalQty = string.Empty;

        public Document CreateDocument(List<DocParameter> parameters)
        {
            var document = new Document();

            foreach (var para in parameters)
            {
                document.Info.Title = $"ใบรายละเอียดยอดฟุต {para.OrderNo} Grade {para.Grade}";
                document.Info.Subject = $"Order No. {para.OrderNo} Grade {para.Grade}";
                document.Info.Author = "IHL";

                Styles.DefineStyles(document);

                DefineContentSection(document, para);
            }

            return document;
        }

        void DefineContentSection(Document document, DocParameter para)
        {
            int rowsPerColumn = 24;
            int columnsPerPage = 5;
            int rowsPerPage = rowsPerColumn * columnsPerPage;
            int numRows = para.MeasureData.Count();
            double numPagesDb = (double)numRows / rowsPerPage;
            int numPages = numRows > rowsPerPage ? (int)Math.Ceiling(numPagesDb) : 1;
            int currentDataRow = 1;
            Dictionary<string, object> dmSt = new Dictionary<string, object>();
            string dynamicName;
            string titleTab;

            for (int s = 1; s <= numPages; s++)
            {
                double topMargin = 4.5;

                dynamicName = "stPage" + s;
                dmSt[dynamicName] = document.AddSection();
                Section stPage = (Section)dmSt[dynamicName];

                stPage.PageSetup.BottomMargin = "0.75cm";
                stPage.PageSetup.LeftMargin = "0.75cm";
                stPage.PageSetup.Orientation = Orientation.Portrait;
                stPage.PageSetup.PageFormat = PageFormat.A5;
                stPage.PageSetup.RightMargin = "3cm";
                stPage.PageSetup.TopMargin = "5.5cm";

                // Page No
                TextFrame hdPnF = stPage.AddTextFrame();
                hdPnF.Width = "33mm";
                hdPnF.RelativeHorizontal = RelativeHorizontal.Page;
                hdPnF.RelativeVertical = RelativeVertical.Page;
                hdPnF.Left = Unit.FromMillimeter(107);
                hdPnF.Top = Unit.FromMillimeter(5);

                Paragraph hdPnPg = hdPnF.AddParagraph();
                hdPnPg.Format.Font.Size = 12;
                hdPnPg.Format.Alignment = ParagraphAlignment.Right;
                hdPnPg.AddText($"Page {s} / {numPages}");

                // Grade
                TextFrame hdF = stPage.AddTextFrame();
                hdF.Width = "33mm";
                hdF.RelativeHorizontal = RelativeHorizontal.Page;
                hdF.RelativeVertical = RelativeVertical.Page;
                hdF.Left = Unit.FromMillimeter(103);
                hdF.Top = Unit.FromMillimeter(7);

                Paragraph pgG = hdF.AddParagraph();
                pgG.Format.Font.Size = 80;
                pgG.Format.Alignment = ParagraphAlignment.Right;
                pgG.AddFormattedText(para.Grade, TextFormat.Bold);

                // NG Reason
                TextFrame hdNGF = stPage.AddTextFrame();
                hdNGF.Width = "33mm";
                hdNGF.RelativeHorizontal = RelativeHorizontal.Page;
                hdNGF.RelativeVertical = RelativeVertical.Page;
                hdNGF.Left = Unit.FromMillimeter(103);
                hdNGF.Top = Unit.FromMillimeter(33);

                Paragraph pgNG = hdNGF.AddParagraph();
                pgNG.Format.Font.Size = 40;
                pgNG.Format.Alignment = ParagraphAlignment.Right;
                pgNG.AddFormattedText(para.NGReason, TextFormat.Bold);

                TextFrame hdBdF = stPage.AddTextFrame();
                hdBdF.Width = "33mm";
                hdBdF.RelativeHorizontal = RelativeHorizontal.Page;
                hdBdF.RelativeVertical = RelativeVertical.Page;
                hdBdF.Left = Unit.FromMillimeter(108);
                hdBdF.Top = Unit.FromMillimeter(14);

                Table hdTb = new Table();
                hdTb.AddColumn("33mm");
                hdTb.Borders.Width = "0.1mm";
                hdTb.Borders.Color = Colors.Black;
                hdTb.Borders.Visible = true;

                hdBdF.Elements.Add(hdTb);

                Row hdR = hdTb.AddRow();
                hdR.Height = Unit.FromMillimeter(21);
                hdR.HeightRule = RowHeightRule.AtLeast;

                // Order No.
                TextFrame hdF1 = stPage.AddTextFrame();
                hdF1.Width = "20mm";
                hdF1.RelativeHorizontal = RelativeHorizontal.Page;
                hdF1.RelativeVertical = RelativeVertical.Page;
                hdF1.Left = Unit.FromMillimeter(7.5);
                hdF1.Top = Unit.FromMillimeter(topMargin);

                Paragraph hdPg1 = hdF1.AddParagraph();
                hdPg1.Format.Font.Size = 16;
                hdPg1.AddFormattedText("Order No. :", TextFormat.Bold);

                TextFrame hdF2 = stPage.AddTextFrame();
                hdF2.Width = "30mm";
                hdF2.RelativeHorizontal = RelativeHorizontal.Page;
                hdF2.RelativeVertical = RelativeVertical.Page;
                hdF2.Left = Unit.FromMillimeter(29);
                hdF2.Top = Unit.FromMillimeter(topMargin);

                Paragraph hdPg2 = hdF2.AddParagraph();
                hdPg2.Format.Font.Size = 16;
                hdPg2.AddText(para.OrderNo);

                // Date
                TextFrame hdF3 = stPage.AddTextFrame();
                hdF3.Width = "12mm";
                hdF3.RelativeHorizontal = RelativeHorizontal.Page;
                hdF3.RelativeVertical = RelativeVertical.Page;
                hdF3.Left = Unit.FromMillimeter(72);
                hdF3.Top = Unit.FromMillimeter(topMargin);

                Paragraph hdPg3 = hdF3.AddParagraph();
                hdPg3.Format.Font.Size = 16;
                hdPg3.AddFormattedText("Date :", TextFormat.Bold);

                TextFrame hdF4 = stPage.AddTextFrame();
                hdF4.Width = "22.5mm";
                hdF4.RelativeHorizontal = RelativeHorizontal.Page;
                hdF4.RelativeVertical = RelativeVertical.Page;
                hdF4.Left = Unit.FromMillimeter(84);
                hdF4.Top = Unit.FromMillimeter(topMargin);

                Paragraph hdPg4 = hdF4.AddParagraph();
                hdPg4.Format.Font.Size = 16;
                hdPg4.AddText(para.DocDate);

                // Line
                TextFrame hdF5 = stPage.AddTextFrame();
                hdF5.Width = "132mm";
                hdF5.RelativeHorizontal = RelativeHorizontal.Page;
                hdF5.RelativeVertical = RelativeVertical.Page;
                hdF5.Left = Unit.FromMillimeter(7.5);
                hdF5.Top = Unit.FromMillimeter(topMargin + 2.5);

                Paragraph hdPg5 = hdF5.AddParagraph();
                Borders hdBd5 = hdPg5.Format.Borders;
                hdBd5.Bottom.Width = "0.1mm";
                hdBd5.Bottom.Color = Colors.Black;
                hdBd5.Bottom.Style = BorderStyle.Single;
                hdBd5.Bottom.Visible = true;

                // Mat No
                TextFrame hdF6 = stPage.AddTextFrame();
                hdF6.Width = "95mm";
                hdF6.RelativeHorizontal = RelativeHorizontal.Page;
                hdF6.RelativeVertical = RelativeVertical.Page;
                hdF6.Left = Unit.FromMillimeter(7.5);
                hdF6.Top = Unit.FromMillimeter(topMargin + 6);

                Paragraph hdPg6 = hdF6.AddParagraph();
                hdPg6.Format.Font.Size = 30;
                hdPg6.AddText(para.MatNo);

                // Mat Desc
                TextFrame hdF7 = stPage.AddTextFrame();
                hdF7.Width = "95mm";
                hdF7.RelativeHorizontal = RelativeHorizontal.Page;
                hdF7.RelativeVertical = RelativeVertical.Page;
                hdF7.Left = Unit.FromMillimeter(7.5);
                hdF7.Top = Unit.FromMillimeter(topMargin + 15);

                Paragraph hdPg7 = hdF7.AddParagraph();
                hdPg7.Format.LineSpacingRule = LineSpacingRule.Multiple;
                hdPg7.Format.LineSpacing = 0.65;
                hdPg7.Format.Font.Size = 30;
                hdPg7.AddText(para.MatDesc);

                // Line
                TextFrame hdF8 = stPage.AddTextFrame();
                hdF8.Width = "132mm";
                hdF8.RelativeHorizontal = RelativeHorizontal.Page;
                hdF8.RelativeVertical = RelativeVertical.Page;
                hdF8.Left = Unit.FromMillimeter(7.5);
                hdF8.Top = Unit.FromMillimeter(topMargin + 40);

                Paragraph hdPg8 = hdF8.AddParagraph();
                Borders hdBd8 = hdPg8.Format.Borders;
                hdBd8.Bottom.Width = "0.1mm";
                hdBd8.Bottom.Color = Colors.Black;
                hdBd8.Bottom.Style = BorderStyle.Single;
                hdBd8.Bottom.Visible = true;

                // Body Content
                Dictionary<string, object> dmTf = new Dictionary<string, object>();
                Dictionary<string, object> dmPg = new Dictionary<string, object>();
                
                int fontSize = 14;
                int i = 1;
                double posLeft = 6.5;
                double oriPostop = 51;
                double posTop = oriPostop;
                double rankW = 6.5;
                double areaW = 12.5;
                double gapRa = 2.5;
                double lnGap = 6;
                double vlPos = 6;
                double vlH = (lnGap * rowsPerColumn) - (lnGap / 2);
                double gapCol = 0.4;
                
                foreach (LeatherArea item in para.MeasureData)
                {
                    if (i < currentDataRow)
                    {
                        i++;
                        continue;
                    }

                    dynamicName = "fRank" + i;
                    dmTf[dynamicName] = stPage.AddTextFrame();
                    TextFrame tfRank = (TextFrame)dmTf[dynamicName];
                    tfRank.Width = $"{rankW}mm";
                    tfRank.RelativeHorizontal = RelativeHorizontal.Page;
                    tfRank.RelativeVertical = RelativeVertical.Page;
                    tfRank.Left = Unit.FromMillimeter(posLeft);
                    tfRank.Top = Unit.FromMillimeter(posTop);

                    dynamicName = "pgRank" + i;
                    dmPg[dynamicName] = tfRank.AddParagraph();
                    Paragraph pgRank = (Paragraph)dmPg[dynamicName];
                    pgRank.Format.Font.Size = fontSize;
                    pgRank.Format.Alignment = ParagraphAlignment.Right;
                    pgRank.AddText(item.Rank.ToString());

                    posLeft = posLeft + rankW + gapRa;
                    dynamicName = "fArea" + i;
                    dmTf[dynamicName] = stPage.AddTextFrame();
                    TextFrame tfArea = (TextFrame)dmTf[dynamicName];
                    tfArea.Width = $"{areaW}mm";
                    tfArea.RelativeHorizontal = RelativeHorizontal.Page;
                    tfArea.RelativeVertical = RelativeVertical.Page;
                    tfArea.Left = Unit.FromMillimeter(posLeft);
                    tfArea.Top = Unit.FromMillimeter(posTop);

                    dynamicName = "pgArea" + i;
                    dmPg[dynamicName] = tfArea.AddParagraph();
                    Paragraph pgArea = (Paragraph)dmPg[dynamicName];
                    pgArea.Format.Font.Size = fontSize;
                    pgArea.Format.Alignment = ParagraphAlignment.Right;
                    pgArea.AddText(item.Area.ToString());

                    if(i % rowsPerColumn == 0 && i % rowsPerPage != 0)
                    {
                        posLeft = posLeft + areaW + vlPos;
                        posTop = oriPostop;

                        TextFrame vlF = stPage.AddTextFrame();
                        vlF.RelativeHorizontal = RelativeHorizontal.Page;
                        vlF.RelativeVertical = RelativeVertical.Page;
                        vlF.Top = Unit.FromMillimeter(posTop + 1);
                        vlF.Left = Unit.FromMillimeter(posLeft);
                        vlF.Width = "1mm";
                        vlF.Height = $"{vlH}mm";

                        Table tb = new Table();
                        tb.AddColumn(vlF.Width);
                        tb.Borders.Left.Width = "0.1mm";
                        tb.Borders.Left.Color = Colors.Black;
                        tb.Borders.Left.Visible = true;

                        vlF.Elements.Add(tb);

                        Row r = tb.AddRow();
                        r.Height = Unit.FromMillimeter(vlH);
                        r.HeightRule = RowHeightRule.AtLeast;

                        posLeft = posLeft + gapCol;
                    }
                    else
                    {
                        posTop = posTop + lnGap;
                        posLeft = posLeft - rankW - gapRa;
                    }

                    if (i % rowsPerPage == 0)
                    {
                        currentDataRow++;
                        break;
                    }

                    i++;
                    currentDataRow++;
                }

                titleTab = "2cm";

                TextFrame tfFt1 = stPage.AddTextFrame();
                tfFt1.Width = "132mm";
                tfFt1.RelativeHorizontal = RelativeHorizontal.Page;
                tfFt1.RelativeVertical = RelativeVertical.Page;
                tfFt1.Left = Unit.FromMillimeter(7.5);
                tfFt1.Top = Unit.FromMillimeter(192);
                Paragraph pgFt1 = tfFt1.AddParagraph();
                Border bdFt1 = pgFt1.Format.Borders.Bottom;

                bdFt1.Width = "0.1mm";
                bdFt1.Color = Colors.Black;
                bdFt1.Style = BorderStyle.Single;

                if (s == numPages)
                {
                    TextFrame tfFt2 = stPage.AddTextFrame();
                    tfFt2.Width = "132mm";
                    tfFt2.RelativeHorizontal = RelativeHorizontal.Page;
                    tfFt2.RelativeVertical = RelativeVertical.Page;
                    tfFt2.Left = Unit.FromMillimeter(7.5);
                    tfFt2.Top = Unit.FromMillimeter(197);
                    Paragraph pgFt2 = tfFt2.AddParagraph();
                    pgFt2.Format.Font.Size = 16;
                    pgFt2.AddFormattedText("Grade:", TextFormat.Bold);

                    titleTab = "3cm";
                    pgFt2.Format.TabStops.AddTabStop(titleTab, TabAlignment.Left);
                    pgFt2.AddTab();
                    pgFt2.AddText(para.Grade);

                    titleTab = "5.5cm";
                    pgFt2.Format.TabStops.AddTabStop(titleTab, TabAlignment.Left);
                    pgFt2.AddTab();
                    pgFt2.AddFormattedText("Q'ty:", TextFormat.Bold);

                    titleTab = "7.5cm";
                    pgFt2.Format.TabStops.AddTabStop(titleTab, TabAlignment.Left);
                    pgFt2.AddTab();
                    pgFt2.AddText($"{numRows} / {para.TotalQty}");

                    titleTab = "11.05cm";
                    pgFt2.Format.TabStops.AddTabStop(titleTab, TabAlignment.Left);
                    pgFt2.AddTab();
                    pgFt2.AddFormattedText("PCS / SQ.FT", TextFormat.Bold);
                }
            }
        }

        public void SetData(DocParameter para)
        {
            DocDate = para.DocDate;
            Grade = para.Grade;
            NGReason = para.NGReason;
            MeasureData = para.MeasureData;
            MatDesc = para.MatDesc;
            MatNo = para.MatNo;
            OrderNo = para.OrderNo;
            TotalQty = para.TotalQty;
        }

        void SetHeaderData(Paragraph pg, string tt, string txt, Borders bd)
        {
            string titleTab = "2.5cm";

            pg.Format.Font.Size = 16;
            pg.AddFormattedText(tt, TextFormat.Bold);
            pg.Format.TabStops.AddTabStop(titleTab, TabAlignment.Left);
            pg.AddTab();
            pg.AddText($": {txt}");

            bd.Bottom.Width = "0.1mm";
            bd.Bottom.Color = Colors.Black;
            bd.Bottom.Style = BorderStyle.Single;
            bd.Bottom.Visible = true;
        }
    }
}
