using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using SapOdGrdTt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using Border = MigraDoc.DocumentObjectModel.Border;
using Column = MigraDoc.DocumentObjectModel.Tables.Column;
using Orientation = MigraDoc.DocumentObjectModel.Orientation;
using Paragraph = MigraDoc.DocumentObjectModel.Paragraph;
using Section = MigraDoc.DocumentObjectModel.Section;
using Table = MigraDoc.DocumentObjectModel.Tables.Table;

namespace MachineSerialPort.PDF
{
    class DocumentsA4
    {
        private MSEG[] TotalGradeQty = Array.Empty<MSEG>();

        private string DocDate = string.Empty;
        private string MatDesc = string.Empty;
        private string MatNo = string.Empty;
        private string OrderNo = string.Empty;
        private string PQty = string.Empty;
        private string Qty = string.Empty;

        public Document CreateDocument()
        {
            // Create a new MigraDoc document.
            var document = new Document
            {
                Info =
                {
                    Title = $"ใบสรุปยอดฟุตทั้ง Order {OrderNo}",
                    Subject = $"Order No. {OrderNo}",
                    Author = "IHL"
                }
            };

            Styles.DefineStyles(document);

            DefineContentSection(document);

            return document;
        }

        void DefineContentSection(Document document)
        {
            double curVPos = 6.2;
            double fontSize = 30;
            double moveDown = 10;
            double objW;
            double pageW = 190;
            double startLeft = 10;
            double curHPos = startLeft;
            double colW;
            decimal pQtyDecimal;
            decimal qtyPercent;

            int colNum;

            string gradeText = "";

            Section stP = document.AddSection();
            stP.PageSetup.Orientation = Orientation.Portrait;
            stP.PageSetup.PageFormat = PageFormat.A4;

            // Order No
            TextFrame tfOdTt = stP.AddTextFrame();
            tfOdTt.Left = Unit.FromMillimeter(curHPos);
            tfOdTt.RelativeHorizontal = RelativeHorizontal.Page;
            tfOdTt.RelativeVertical = RelativeVertical.Page;
            tfOdTt.Top = Unit.FromMillimeter(curVPos);

            objW = 40;
            tfOdTt.Width = Unit.FromMillimeter(objW);

            Paragraph pgOdTt = tfOdTt.AddParagraph();
            pgOdTt.AddFormattedText("Order No. :", TextFormat.Bold);
            pgOdTt.Format.Font.Size = fontSize;

            TextFrame tfOdVl = stP.AddTextFrame();
            tfOdVl.RelativeHorizontal = RelativeHorizontal.Page;
            tfOdVl.RelativeVertical = RelativeVertical.Page;
            tfOdVl.Top = Unit.FromMillimeter(curVPos);

            curHPos = curHPos + objW;
            tfOdVl.Left = Unit.FromMillimeter(curHPos);

            objW = 50;
            tfOdVl.Width = Unit.FromMillimeter(objW);

            Paragraph pgOdVl = tfOdVl.AddParagraph();
            pgOdVl.AddText(OrderNo);
            pgOdVl.Format.Font.Size = fontSize;

            // Date
            TextFrame tfDtTt = stP.AddTextFrame();
            tfDtTt.RelativeHorizontal = RelativeHorizontal.Page;
            tfDtTt.RelativeVertical = RelativeVertical.Page;
            tfDtTt.Top = Unit.FromMillimeter(curVPos);

            curHPos = curHPos + objW + 40;
            tfDtTt.Left = Unit.FromMillimeter(curHPos);

            objW = 25;
            tfDtTt.Width = Unit.FromMillimeter(objW);

            Paragraph pgDtTt = tfDtTt.AddParagraph();
            pgDtTt.AddFormattedText("Date :", TextFormat.Bold);
            pgDtTt.Format.Font.Size = fontSize;

            TextFrame tfDtVl = stP.AddTextFrame();
            tfDtVl.RelativeHorizontal = RelativeHorizontal.Page;
            tfDtVl.RelativeVertical = RelativeVertical.Page;
            tfDtVl.Top = Unit.FromMillimeter(curVPos);

            curHPos = curHPos + objW;
            tfDtVl.Left = Unit.FromMillimeter(curHPos);

            objW = 40;
            tfDtVl.Width = Unit.FromMillimeter(objW);

            Paragraph pgDtVl = tfDtVl.AddParagraph();
            pgDtVl.AddText(DocDate);
            pgDtVl.Format.Font.Size = fontSize;

            // Line
            TextFrame tfOdLn = stP.AddTextFrame();
            tfOdLn.RelativeHorizontal = RelativeHorizontal.Page;
            tfOdLn.RelativeVertical = RelativeVertical.Page;
            tfOdLn.Width = Unit.FromMillimeter(pageW);

            curHPos = startLeft;
            tfOdLn.Left = Unit.FromMillimeter(curHPos);

            curVPos = curVPos + moveDown;
            tfOdLn.Top = Unit.FromMillimeter(curVPos);

            Paragraph pgOdLn = tfOdLn.AddParagraph();
            Borders bdOdLn = pgOdLn.Format.Borders;
            bdOdLn.Bottom.Color = Colors.Black;
            bdOdLn.Bottom.Style = BorderStyle.Single;
            bdOdLn.Bottom.Visible = true;
            bdOdLn.Bottom.Width = Unit.FromMillimeter(0.1);

            // Material No
            TextFrame tfMnTt = stP.AddTextFrame();
            tfMnTt.Left = Unit.FromMillimeter(curHPos);
            tfMnTt.RelativeHorizontal = RelativeHorizontal.Page;
            tfMnTt.RelativeVertical = RelativeVertical.Page;
            tfMnTt.Width = Unit.FromMillimeter(pageW);

            curVPos = curVPos + moveDown - 5;
            tfMnTt.Top = Unit.FromMillimeter(curVPos);

            Paragraph pgMnTt = tfMnTt.AddParagraph();
            pgMnTt.AddText(MatNo);
            pgMnTt.Format.Font.Size = fontSize + 10;

            // Material Desc
            TextFrame tfMdTt = stP.AddTextFrame();
            tfMdTt.Left = Unit.FromMillimeter(curHPos);
            tfMdTt.RelativeHorizontal = RelativeHorizontal.Page;
            tfMdTt.RelativeVertical = RelativeVertical.Page;
            tfMdTt.Width = Unit.FromMillimeter(pageW);

            curVPos = curVPos + moveDown + 2;
            tfMdTt.Top = Unit.FromMillimeter(curVPos);

            Paragraph pgMdTt = tfMdTt.AddParagraph();
            pgMdTt.AddText(MatDesc);
            pgMdTt.Format.Font.Size = fontSize + 10;
            pgMdTt.Format.LineSpacing = 0.75;
            pgMdTt.Format.LineSpacingRule = LineSpacingRule.Multiple;

            // Line
            TextFrame tfMnLn = stP.AddTextFrame();
            tfMnLn.Left = Unit.FromMillimeter(curHPos);
            tfMnLn.RelativeHorizontal = RelativeHorizontal.Page;
            tfMnLn.RelativeVertical = RelativeVertical.Page;
            tfMnLn.Width = Unit.FromMillimeter(pageW);

            curVPos = curVPos + moveDown + moveDown + 8;
            tfMnLn.Top = Unit.FromMillimeter(curVPos);

            Paragraph pgMnLn = tfMnLn.AddParagraph();
            Borders bdMnLn = pgMnLn.Format.Borders;
            bdMnLn.Bottom.Color = Colors.Black;
            bdMnLn.Bottom.Style = BorderStyle.Single;
            bdMnLn.Bottom.Visible = true;
            bdMnLn.Bottom.Width = Unit.FromMillimeter(0.1);

            //// Qty
            //TextFrame tfQtyTt = stP.AddTextFrame();
            //tfQtyTt.Left = Unit.FromMillimeter(curHPos);
            //tfQtyTt.RelativeHorizontal = RelativeHorizontal.Page;
            //tfQtyTt.RelativeVertical = RelativeVertical.Page;

            //curVPos = curVPos + moveDown - 4;
            //tfQtyTt.Top = Unit.FromMillimeter(curVPos);

            //objW = 40;
            //tfQtyTt.Width = Unit.FromMillimeter(objW);

            //Paragraph pgQtyTt = tfQtyTt.AddParagraph();
            //pgQtyTt.AddFormattedText("Q'ty. :", TextFormat.Bold);
            //pgQtyTt.Format.Font.Size = fontSize;

            //TextFrame tfQtyVl = stP.AddTextFrame();
            //tfQtyVl.RelativeHorizontal = RelativeHorizontal.Page;
            //tfQtyVl.RelativeVertical = RelativeVertical.Page;
            //tfQtyVl.Top = Unit.FromMillimeter(curVPos);

            //curHPos = curHPos + objW;
            //tfQtyVl.Left = Unit.FromMillimeter(curHPos);

            //objW = 150;
            //tfQtyVl.Width = Unit.FromMillimeter(objW);

            //Paragraph pgQtyVl = tfQtyVl.AddParagraph();
            //pgQtyVl.AddText($"{PQty} PCS / {Qty} SQ.FT");
            //pgQtyVl.Format.Font.Size = fontSize;

            //// Line
            //TextFrame tfQtyLn = stP.AddTextFrame();
            //tfQtyLn.RelativeHorizontal = RelativeHorizontal.Page;
            //tfQtyLn.RelativeVertical = RelativeVertical.Page;
            //tfQtyLn.Width = Unit.FromMillimeter(pageW);

            //curHPos = startLeft;
            //tfQtyLn.Left = Unit.FromMillimeter(curHPos);

            //curVPos = curVPos + moveDown + 1;
            //tfQtyLn.Top = Unit.FromMillimeter(curVPos);

            //Paragraph pgQtyLn = tfQtyLn.AddParagraph();
            //Borders bdQtyLn = pgQtyLn.Format.Borders;
            //bdQtyLn.Bottom.Color = Colors.Black;
            //bdQtyLn.Bottom.Style = BorderStyle.Single;
            //bdQtyLn.Bottom.Visible = true;
            //bdQtyLn.Bottom.Width = Unit.FromMillimeter(0.1);

            // Grade Qty
            string dmN;
            int i = 3;
            Dictionary<string, object> dmTbR = new Dictionary<string, object>();
            Dictionary<string, object> dmTbRC = new Dictionary<string, object>();
            startLeft = startLeft + 12.5;

            // Add Table Structure
            TextFrame tfTbQty = stP.AddTextFrame();

            curHPos = curHPos + 1.2;
            tfTbQty.Left = Unit.FromMillimeter(curHPos);
            tfTbQty.RelativeHorizontal = RelativeHorizontal.Page;
            tfTbQty.RelativeVertical = RelativeVertical.Page;

            curVPos = curVPos + moveDown + 1;
            tfTbQty.Top = Unit.FromMillimeter(curVPos);

            objW = 190;
            colNum = 4;
            colW = objW / colNum;
            tfTbQty.Width = Unit.FromMillimeter(objW);

            Table tb = tfTbQty.AddTable();
            tb.Borders.Visible = true;

            Column tbC1 = tb.AddColumn(Unit.FromMillimeter(colW));
            tbC1.Format.Alignment = ParagraphAlignment.Center;
            tbC1.Format.Font.Size = fontSize;

            Column tbC2 = tb.AddColumn(Unit.FromMillimeter(colW));
            tbC2.Format.Alignment = ParagraphAlignment.Center;
            tbC2.Format.Font.Size = fontSize;

            Column tbC3 = tb.AddColumn(Unit.FromMillimeter(colW));
            tbC3.Format.Alignment = ParagraphAlignment.Center;
            tbC3.Format.Font.Size = fontSize;

            Column tbC4 = tb.AddColumn(Unit.FromMillimeter(colW));
            tbC4.Format.Alignment = ParagraphAlignment.Center;
            tbC4.Format.Font.Size = fontSize;

            // Add Table Header
            Row tbR1Hd = tb.AddRow();

            Paragraph tbR1HdC1 = tbR1Hd.Cells[0].AddParagraph();
            tbR1HdC1.AddFormattedText("Grade", TextFormat.Bold);
            tbR1Hd.Cells[0].MergeDown = 1;
            tbR1Hd.VerticalAlignment = VerticalAlignment.Center;

            Paragraph tbR1HdC2 = tbR1Hd.Cells[1].AddParagraph();
            tbR1HdC2.AddFormattedText("Q'ty", TextFormat.Bold);
            tbR1Hd.Cells[1].MergeRight = 2;

            Row tbR2Hd = tb.AddRow();

            Paragraph tbR2HdC2 = tbR2Hd.Cells[1].AddParagraph();
            tbR2HdC2.AddFormattedText("PCS", TextFormat.Bold);
            Paragraph tbR2HdC3 = tbR2Hd.Cells[2].AddParagraph();
            tbR2HdC3.AddFormattedText("SQ.FT", TextFormat.Bold);
            Paragraph tbR2HdC4 = tbR2Hd.Cells[3].AddParagraph();
            tbR2HdC4.AddFormattedText("%", TextFormat.Bold);

            moveDown = 14.3;
            curVPos += (moveDown * 2);
            foreach (var item in TotalGradeQty)
            {
                dmN = $"tbR{i}";
                dmTbR[dmN] = tb.AddRow();
                Row tbR = (Row)dmTbR[dmN];

                if (item.SGTXT == "NG")
                {
                    gradeText = $"{item.SGTXT} ({item.EQUNR})";
                }
                else
                {
                    gradeText = item.SGTXT;
                }

                tbR.Cells[0].AddParagraph(gradeText);
                tbR.Cells[1].AddParagraph(item._CWM_MENGE.ToString("#,#"));
                tbR.Cells[2].AddParagraph(item.MENGE.ToString("#,##0.000"));

                _ = decimal.TryParse(PQty, out pQtyDecimal);
                qtyPercent = item._CWM_MENGE * 100 / pQtyDecimal;
                tbR.Cells[3].AddParagraph(qtyPercent.ToString("0.00"));

                //// Grade
                //dmN = "tfGrdTt" + i;
                //dmTf[dmN] = stP.AddTextFrame();
                //TextFrame tfGrdTt = (TextFrame)dmTf[dmN];
                //tfGrdTt.RelativeHorizontal = RelativeHorizontal.Page;
                //tfGrdTt.RelativeVertical = RelativeVertical.Page;

                //curHPos = startLeft;
                //tfGrdTt.Left = Unit.FromMillimeter(curHPos);

                //curVPos = curVPos + moveDown - 3;
                //tfGrdTt.Top = Unit.FromMillimeter(curVPos);

                //objW = 27;
                //tfGrdTt.Width = Unit.FromMillimeter(objW);

                //dmN = "pgGrdTt" + i;
                //dmTf[dmN] = tfGrdTt.AddParagraph();
                //Paragraph pgGrdTt = (Paragraph)dmTf[dmN];
                //pgGrdTt.AddFormattedText("Grade :", TextFormat.Bold);
                //pgGrdTt.Format.Font.Size = fontSize;

                //dmN = "tfGrdVl" + i;
                //dmTf[dmN] = stP.AddTextFrame();
                //TextFrame tfGrdVl = (TextFrame)dmTf[dmN];
                //tfGrdVl.RelativeHorizontal = RelativeHorizontal.Page;
                //tfGrdVl.RelativeVertical = RelativeVertical.Page;
                //tfGrdVl.Top = Unit.FromMillimeter(curVPos);

                //curHPos = curHPos + objW;
                //tfGrdVl.Left = Unit.FromMillimeter(curHPos);

                //objW = 20;
                //tfGrdVl.Width = Unit.FromMillimeter(objW);

                //dmN = "pgGrdVl" + i;
                //dmTf[dmN] = tfGrdVl.AddParagraph();
                //Paragraph pgGrdVl = (Paragraph)dmTf[dmN];
                //pgGrdVl.AddText(item.SGTXT);
                //pgGrdVl.Format.Alignment = ParagraphAlignment.Center;
                //pgGrdVl.Format.Font.Size = fontSize;

                //Borders bdGrdVl = pgGrdVl.Format.Borders;
                //bdGrdVl.Bottom.Color = Colors.Black;
                //bdGrdVl.Bottom.Style = BorderStyle.Single;
                //bdGrdVl.Bottom.Visible = true;
                //bdGrdVl.Bottom.Width = Unit.FromMillimeter(0.1);

                //// Qty
                //dmN = "tfGrdQtyTt" + i;
                //dmTf[dmN] = stP.AddTextFrame();
                //TextFrame tfGrdQtyTt = (TextFrame)dmTf[dmN];
                //tfGrdQtyTt.RelativeHorizontal = RelativeHorizontal.Page;
                //tfGrdQtyTt.RelativeVertical = RelativeVertical.Page;
                //tfGrdQtyTt.Top = Unit.FromMillimeter(curVPos);

                //curHPos = curHPos + objW;
                //tfGrdQtyTt.Left = Unit.FromMillimeter(curHPos);

                //objW = 20;
                //tfGrdQtyTt.Width = Unit.FromMillimeter(objW);

                //dmN = "pgGrdQtyTt" + i;
                //dmTf[dmN] = tfGrdQtyTt.AddParagraph();
                //Paragraph pgGrdQtyTt = (Paragraph)dmTf[dmN];
                //pgGrdQtyTt.AddFormattedText("Q'ty :", TextFormat.Bold);
                //pgGrdQtyTt.Format.Font.Size = fontSize;

                //dmN = "tfGrdQtyVl" + i;
                //dmTf[dmN] = stP.AddTextFrame();
                //TextFrame tfGrdQtyVl = (TextFrame)dmTf[dmN];
                //tfGrdQtyVl.RelativeHorizontal = RelativeHorizontal.Page;
                //tfGrdQtyVl.RelativeVertical = RelativeVertical.Page;
                //tfGrdQtyVl.Top = Unit.FromMillimeter(curVPos);

                //curHPos = curHPos + objW;
                //tfGrdQtyVl.Left = Unit.FromMillimeter(curHPos);

                //objW = pageW - curHPos - 42.5;
                //tfGrdQtyVl.Width = Unit.FromMillimeter(objW);

                //dmN = "pgGrdQtyVl" + i;
                //dmTf[dmN] = tfGrdQtyVl.AddParagraph();
                //Paragraph pgGrdQtyVl = (Paragraph)dmTf[dmN];
                //pgGrdQtyVl.AddText($"{item._CWM_MENGE.ToString("#,#")} / {item.MENGE.ToString("#,##0.000")}");
                //pgGrdQtyVl.Format.Alignment = ParagraphAlignment.Center;
                //pgGrdQtyVl.Format.Font.Size = fontSize;

                //Borders bdGrdQtyVl = pgGrdQtyVl.Format.Borders;
                //bdGrdQtyVl.Bottom.Color = Colors.Black;
                //bdGrdQtyVl.Bottom.Style = BorderStyle.Single;
                //bdGrdQtyVl.Bottom.Visible = true;
                //bdGrdQtyVl.Bottom.Width = Unit.FromMillimeter(0.1);

                //dmN = "tfGrdQtyUom" + i;
                //dmTf[dmN] = stP.AddTextFrame();
                //TextFrame tfGrdQtyUom = (TextFrame)dmTf[dmN];
                //tfGrdQtyUom.RelativeHorizontal = RelativeHorizontal.Page;
                //tfGrdQtyUom.RelativeVertical = RelativeVertical.Page;
                //tfGrdQtyUom.Top = Unit.FromMillimeter(curVPos);

                //curHPos = curHPos + objW;
                //tfGrdQtyUom.Left = Unit.FromMillimeter(curHPos);

                //objW = 40;
                //tfGrdQtyUom.Width = Unit.FromMillimeter(objW);

                //dmN = "pgGrdQtyUom" + i;
                //dmTf[dmN] = tfGrdQtyUom.AddParagraph();
                //Paragraph pgGrdQtyUom = (Paragraph)dmTf[dmN];
                //pgGrdQtyUom.AddText("PCS / SQ.FT");
                //pgGrdQtyUom.Format.Alignment = ParagraphAlignment.Right;
                //pgGrdQtyUom.Format.Font.Size = fontSize;

                //Borders bdGrdQtyUom = pgGrdQtyUom.Format.Borders;
                //bdGrdQtyUom.Bottom.Color = Colors.Black;
                //bdGrdQtyUom.Bottom.Style = BorderStyle.Single;
                //bdGrdQtyUom.Bottom.Visible = true;
                //bdGrdQtyUom.Bottom.Width = Unit.FromMillimeter(0.1);

                curVPos = curVPos + moveDown;
            }

            // Total Qty
            Row tbRFt = tb.AddRow();

            Paragraph tbRFtC1 = tbRFt.Cells[0].AddParagraph();
            tbRFtC1.AddFormattedText("Total", TextFormat.Bold);
            Paragraph tbRFtC2 = tbRFt.Cells[1].AddParagraph();
            tbRFtC2.AddFormattedText(PQty, TextFormat.Bold);
            Paragraph tbRFtC3 = tbRFt.Cells[2].AddParagraph();
            tbRFtC3.AddFormattedText(Qty, TextFormat.Bold);
            Paragraph tbRFtC4 = tbRFt.Cells[3].AddParagraph();
            tbRFtC4.AddFormattedText("", TextFormat.Bold);

            curVPos = curVPos + moveDown;

            // Line
            TextFrame tfGrdLn = stP.AddTextFrame();
            tfGrdLn.RelativeHorizontal = RelativeHorizontal.Page;
            tfGrdLn.RelativeVertical = RelativeVertical.Page;
            tfGrdLn.Width = Unit.FromMillimeter(pageW);

            startLeft = 10;
            curHPos = startLeft;
            tfGrdLn.Left = Unit.FromMillimeter(curHPos);

            curVPos = curVPos + 2;
            tfGrdLn.Top = Unit.FromMillimeter(curVPos);

            Paragraph pgGrdLn = tfGrdLn.AddParagraph();
            Borders bdGrdLn = pgGrdLn.Format.Borders;
            bdGrdLn.Bottom.Color = Colors.Black;
            bdGrdLn.Bottom.Style = BorderStyle.Single;
            bdGrdLn.Bottom.Visible = true;
            bdGrdLn.Bottom.Width = Unit.FromMillimeter(0.1);

            // Note
            TextFrame tfNtTt = stP.AddTextFrame();
            tfNtTt.Left = Unit.FromMillimeter(curHPos);
            tfNtTt.RelativeHorizontal = RelativeHorizontal.Page;
            tfNtTt.RelativeVertical = RelativeVertical.Page;

            curVPos = curVPos + 10;
            tfNtTt.Top = Unit.FromMillimeter(curVPos);

            pageW = 190;
            tfNtTt.Width = Unit.FromMillimeter(pageW);

            Paragraph pgNtTt = tfNtTt.AddParagraph();
            pgNtTt.AddFormattedText("Note :", TextFormat.Bold);
            pgNtTt.Format.Font.Size = fontSize;
        }

        public void SetData(DocParameter para)
        {
            DocDate = para.DocDate;
            MatDesc = para.MatDesc;
            MatNo = para.MatNo;
            OrderNo = para.OrderNo;
            PQty = para.PQty;
            Qty = para.Qty;
            TotalGradeQty = para.TotalGradeQty;
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
