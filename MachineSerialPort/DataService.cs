using MachineSerialPort.Databases;
using MachineSerialPort.Databases.Tables;
using MachineSerialPort.Helpers;
using MachineSerialPort.PDF;
using MailKit.Search;
using MaterialDesignThemes.Wpf;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MigraDoc.DocumentObjectModel;
using SapChkMsOp;
using SapGtMtClAt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MachineSerialPort
{
    public class DataService(HuefulDBDbContext huefulDBContext, MaterialInfoDbContext materialInfoContext, MeasurementLeatherContext measurementLeatherContext, SapDbContext sapContext)
    {
        private readonly HuefulDBDbContext _huefulDBContext = huefulDBContext;
        private readonly MaterialInfoDbContext _materialInfoContext = materialInfoContext;
        private readonly MeasurementLeatherContext _measurementLeatherContext = measurementLeatherContext;
        private readonly SapDbContext _sapContext = sapContext;
        public ReturnResult returnResult = new();
        public Dictionary<string, string> arraySapEndpoint = new Dictionary<string, string>();
        public string SapUser = "IHL_RFC";
        private string ErrMsg = string.Empty;

        private Z_GET_MAT_COL_ARTResponse _response = new Z_GET_MAT_COL_ARTResponse();
        public Z_GET_MAT_COL_ARTResponse Response
        {
            get => _response;
            set => SetProperty(ref _response, value);
        }

        private void AddEndpoint(string host, string client, string serviceName)
        {
            string url = $"http://{host}.interhides.com:8000/sap/bc/srt/rfc/sap/{serviceName}/{client}/{serviceName}/{serviceName}";
            arraySapEndpoint[serviceName] = url;
        }

        public void ClearCache()
        {
            Response = new Z_GET_MAT_COL_ARTResponse();
            arraySapEndpoint.Clear();
        }

        public Dictionary<string, string> GetSapEndPoint(string sapClient)
        {
            SetSapEndpoint(sapClient);
            return arraySapEndpoint;
        }

        // Then your method becomes very clean:
        public void SetSapEndpoint(string sapClient)
        {
            string host = (sapClient == "PRD") ? "IHL-PRD02" : "IHL-QAS01";
            string client = (sapClient == "PRD") ? "900" : "500";

            AddEndpoint(host, client, "z_bapi_migo");
            AddEndpoint(host, client, "z_check_measure_op");
            AddEndpoint(host, client, "z_get_mat_col_art");
            AddEndpoint(host, client, "z_get_order_grade_total");
            AddEndpoint(host, client, "z_ws_cal_activities");
        }

        public async Task<string> GetCustomerColorArticle(string MaterialNo)
        {
            string ColArt = string.Empty;
            string CustomerColor = string.Empty;

            try
            {
                // Define your SAP user credentials
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.MaxReceivedMessageSize = 2147483647; // Set to 2GB (Max Int32)
                binding.MaxBufferSize = 2147483647;

                // Also update the ReaderQuotas if you have large strings or arrays
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                binding.ReaderQuotas.MaxArrayLength = 2147483647;

                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                var endpoint = new EndpointAddress(arraySapEndpoint["z_get_mat_col_art"]);

                var pdoParams = new Z_GET_MAT_COL_ARTRequest
                {
                    I_MAT_NO = MaterialNo
                };

                var client = new Z_GET_MAT_COL_ARTClient(binding, endpoint);

                client.ClientCredentials.UserName.UserName = SapUser;
                returnResult = GetRfcPwd();

                if (returnResult.GetValue().IsSuccess)
                {
                    client.ClientCredentials.UserName.Password = returnResult.GetValue().Message;
                }
                else
                {
                    throw new System.NullReferenceException("Could not get RFC password");
                }

                Response = await client.Z_GET_MAT_COL_ARTAsync(pdoParams);

                if (!string.IsNullOrEmpty(Response.E_CUS_MAT_NO))
                {
                    string[] CustColCode = Response.E_CUS_MAT_NO.Split('/');
                    if (CustColCode.Length > 2)
                    {
                        ColArt = CustColCode[1];
                        CustomerColor = CustColCode[2];
                    }
                }

                if (Response.E_CUS_COL_CODE != "")
                {
                    CustomerColor = Response.E_CUS_COL_CODE;
                }

                // Create a list of the parts, filter out the empty ones, and join them with "/"
                ColArt = string.Join("/", new[] { ColArt, CustomerColor }.Where(s => !string.IsNullOrEmpty(s)));
            }
            catch (System.ServiceModel.FaultException faultEx)
            {
                if (faultEx.Message == "NOT_FOUND")
                {
                    ErrMsg = "Customer Color Article Not Found";
                }
                else
                {
                    ErrMsg = faultEx.Message;
                }

                await ConfigHelper.ShowSmartDialog(
                    "InfoTemplate",
                    ErrMsg,
                    PackIconKind.WarningCircle,
                    Brushes.DarkOrange
                );
            }
            
            return ColArt;
        }

        public async Task<List<DocParameter>> GetLeatherMeasureByMatDoc(string OrderNo, string DocYear, string MatDocNoStart, string MatDocNoEnd)
        {
            if (!int.TryParse(DocYear.Trim(), out int year)) return new List<DocParameter>();

            // 1. Select into an anonymous object to bypass EF's internal relationship manager
            var rawData = await (from md in _measurementLeatherContext.MsMatDocs.AsNoTracking()
                                 join s in _measurementLeatherContext.MsSummaries.AsNoTracking() on md.MsId equals s.MsId
                                 where s.OrderNo == OrderNo.Trim() &&
                                       s.DocDate.Year == year &&
                                       md.MatDocNo.CompareTo(MatDocNoStart.Trim()) >= 0 &&
                                       md.MatDocNo.CompareTo(MatDocNoEnd.Trim()) <= 0
                                 select new
                                 {
                                     // Flattening the data into a simple structure
                                     S_OrderNo = s.OrderNo,
                                     S_DocDate = s.DocDate,
                                     S_MatNo = s.MatNo,
                                     S_MatDesc = s.MatDesc,
                                     MD_MsMdId = md.MsMdId,
                                     MD_MatDocNo = md.MatDocNo,
                                     MD_Grade = md.Grade,
                                     MD_NgReason = md.NgReason
                                 }).ToListAsync();

            System.Diagnostics.Debug.WriteLine($"FLATTENED DATA COUNT: {rawData.Count}");

            if (!rawData.Any()) return new List<DocParameter>();

            // 2. Fetch the Details (MsData) using the IDs we just found
            var mdIds = rawData.Select(x => x.MD_MsMdId).ToList();
            var allDetails = await _measurementLeatherContext.MsDatas
                .AsNoTracking()
                .Where(d => mdIds.Contains(d.MsMdId))
                .ToListAsync();

            var result = new List<DocParameter>();

            var sortedRawData = rawData
                .OrderBy(x => x.MD_Grade)
                .ThenBy(x => x.MD_MatDocNo)
                .ToList();

            // 3. Manual Mapping
            foreach (var item in sortedRawData)
            {
                // 1. Get the details first
                var docDetails = allDetails
                    .Where(d => d.MsMdId == item.MD_MsMdId)
                    .OrderBy(d => d.Rank) // Good practice for PDF reports
                    .ToList();

                // Debug check: See if details exist for this specific ID
                System.Diagnostics.Debug.WriteLine($"Doc {item.MD_MatDocNo} (ID:{item.MD_MsMdId}) has {docDetails.Count} detail rows.");

                // 2. Map to a temporary list first to ensure no threading/UI lag
                var leatherAreas = docDetails.Select(d => new LeatherArea
                {
                    Rank = d.Rank,
                    Area = d.Area,
                    Group = d.PackingGroup
                }).ToList();

                string ColorArticle = await GetCustomerColorArticle(item.S_MatNo);

                // 3. Create the parameter with a fully populated collection
                var descParts = new[] { item.S_MatDesc, ColorArticle }
                    .Where(s => !string.IsNullOrWhiteSpace(s));
                result.Add(new DocParameter
                {
                    OrderNo = item.S_OrderNo,
                    DocDate = item.S_DocDate.ToString("yyyy-MM-dd"),
                    MatNo = item.S_MatNo,
                    MatDesc = string.Join("/", descParts),
                    Grade = item.MD_Grade,
                    NGReason = item.MD_NgReason,
                    TotalQty = docDetails.Sum(d => d.Area).ToString("N2"),

                    // Use the list to initialize the collection
                    MeasureData = new ObservableCollection<LeatherArea>(leatherAreas)
                });
            }

            var totalRows = result.Sum(r => r.MeasureData.Count);
            System.Diagnostics.Debug.WriteLine($"TOTAL ROWS READY FOR PDF: {totalRows}");
            return result;
        }

        public ReturnResult GetMaterialDescription(string MaterialNo)
        {
            string[] mat_no = MaterialNo.Split('-');
            List<string> chk_error = new List<string>();
            string cri_thickness = mat_no[2].Substring(0, 1);
            string cri_customer = mat_no[2].Substring(1, 3);

            var mstr_article = _huefulDBContext.MstrArticle
                                               .Where(ma => ma.AtclCodeU == mat_no[1] && ma.AtclVisible == "T" && ma.AtclActive == "Y")
                                               .FirstOrDefault();

            var mstr_thickness = _huefulDBContext.MstrThickness
                                               .Where(mt => mt.ThknsAcrn2 == cri_thickness && mt.ThknsVisible == "T" && mt.ThknsActive == "Y")
                                               .FirstOrDefault();

            var mstr_customer = _huefulDBContext.MstrCustomer
                                               .Where(mc => mc.CustAcrn2 == cri_customer && mc.CustVisible == "T" && mc.CustActive == "Y")
                                               .FirstOrDefault();

            var material_creation_history = _materialInfoContext.material_creation_history
                                               .Where(mch => mch.material_code == MaterialNo)
                                               .FirstOrDefault();

            // Check if any of the results are null and return a meaningful message
            if (mstr_article == null)
            {
                chk_error.Add("Article not found.");
            }
            if (mstr_thickness == null)
            {
                chk_error.Add("Thickness not found.");
            }
            if (mstr_customer == null)
            {
                chk_error.Add("Customer Description not found.");
            }
            if (material_creation_history == null)
            {
                chk_error.Add("Color Name not found.");
            }

            if (chk_error != null && chk_error.Count > 0)
            {
                returnResult.SetValue(false, string.Join("\n", chk_error));
            }
            else
            {
                string[] mat_desc = { mstr_article.AtclDesc1, mstr_thickness.ThknsDesc3, mstr_customer.CustDesc1, material_creation_history.color_name };
                returnResult.SetValue(true, string.Join(' ', mat_desc));
            }
            
            return returnResult;
        }

        public ReturnResult GetRfcPwd()
        {
            try
            {
                var rfc_pwd = _sapContext.Rfc_pass
                                     .OrderByDescending(p => p.Createdate)
                                     .FirstOrDefault();
                returnResult.SetValue(true, rfc_pwd.Pwd);
            }
            catch (Exception ex)
            {
                returnResult.SetValue(false, ex.Message);
            }

            return returnResult;
        }

        public async Task SaveToSql(MsSummary summary, MsMatDoc matDoc, List<MsData> measurements, int existingId = 0)
        {
            matDoc.Details = measurements;

            if (existingId > 0)
            {
                // 1. Find the existing summary in the Database
                var existingSummary = await _measurementLeatherContext.MsSummaries
                    .FirstOrDefaultAsync(x => x.MsId == existingId);

                if (existingSummary != null)
                {
                    // 2. Add the new MatDoc to the existing Summary's collection
                    existingSummary.MatDocs.Add(matDoc);
                }
            }
            else if (summary != null)
            {
                // 3. Brand new session
                summary.MatDocs.Add(matDoc);
                _measurementLeatherContext.MsSummaries.Add(summary);
            }

            await _measurementLeatherContext.SaveChangesAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            // If the value is the same as the current field, don't do anything
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            // Set the new value
            field = value;

            // "Shout" to the UI that it changed
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
