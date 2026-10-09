using Azure;
using MachineSerialPort.Databases;
using MachineSerialPort.Databases.Tables;
using MachineSerialPort.Helpers;
using MachineSerialPort.PDF;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using SapGtMtClAt;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MachineSerialPort
{
    public class DataService(
    IDbContextFactory<HuefulDBDbContext> huefulDBFactory,
    IDbContextFactory<MaterialInfoDbContext> materialInfoFactory,
    IDbContextFactory<MeasurementLeatherContext> measurementLeatherFactory,
    IDbContextFactory<SapDbContext> sapFactory)
    {
        private readonly IDbContextFactory<HuefulDBDbContext> _huefulDBFactory = huefulDBFactory;
        private readonly IDbContextFactory<MaterialInfoDbContext> _materialInfoFactory = materialInfoFactory;
        private readonly IDbContextFactory<MeasurementLeatherContext> _measurementLeatherFactory = measurementLeatherFactory;
        private readonly IDbContextFactory<SapDbContext> _sapFactory = sapFactory;

        public Dictionary<string, string> ArraySapEndpoint { get; } = new();
        public string SapUser { get; set; } = "IHL_RFC";

        private Z_GET_MAT_COL_ARTResponse _response = new Z_GET_MAT_COL_ARTResponse();
        public Z_GET_MAT_COL_ARTResponse Response
        {
            get => _response;
            set => SetProperty(ref _response, value);
        }

        private void AddEndpoint(string host, string client, string serviceName)
        {
            string url = $"http://{host}.interhides.com:8000/sap/bc/srt/rfc/sap/{serviceName}/{client}/{serviceName}/{serviceName}";
            ArraySapEndpoint[serviceName] = url;
        }

        public void SetSapEndpoint(string sapClient)
        {
            ArraySapEndpoint.Clear();
            string host = sapClient == "PRD" ? "IHL-PRD02" : "IHL-QAS01";
            string client = sapClient == "PRD" ? "900" : "500";

            AddEndpoint(host, client, "z_bapi_migo");
            AddEndpoint(host, client, "z_check_measure_op");
            AddEndpoint(host, client, "z_get_hu_info");
            AddEndpoint(host, client, "z_get_mat_col_art");
            AddEndpoint(host, client, "z_get_order_grade_total");
            AddEndpoint(host, client, "z_ws_cal_activities");
        }

        public Dictionary<string, string> GetSapEndPoint(string sapClient)
        {
            SetSapEndpoint(sapClient);
            return ArraySapEndpoint;
        }

        public async Task<string> GetCustomerColorArticle(string materialNo)
        {
            string colArt = string.Empty;
            string customerColor = string.Empty;
            string errMsg = string.Empty;

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue
            };
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            if (!ArraySapEndpoint.TryGetValue("z_get_mat_col_art", out var endpointUrl))
            {
                throw new InvalidOperationException("SAP endpoint 'z_get_mat_col_art' is not configured.");
            }

            var endpoint = new EndpointAddress(endpointUrl);
            var client = new Z_GET_MAT_COL_ARTClient(binding, endpoint);

            try
            {
                client.ClientCredentials.UserName.UserName = SapUser;

                var pwdResult = GetRfcPwd();

                if (!pwdResult.GetValue().IsSuccess)
                {
                    throw new InvalidOperationException("Could not retrieve RFC password: " + pwdResult.GetValue().Message);
                }
                
                client.ClientCredentials.UserName.Password = pwdResult.GetValue().Message;

                var pdoParams = new Z_GET_MAT_COL_ARTRequest
                {
                    I_MAT_NO = materialNo
                };

                var response = await client.Z_GET_MAT_COL_ARTAsync(pdoParams);

                if (!string.IsNullOrEmpty(response.E_CUS_MAT_NO))
                {
                    string[] custColCode = response.E_CUS_MAT_NO.Split('/');
                    if (custColCode.Length > 2)
                    {
                        colArt = custColCode[1];
                        customerColor = custColCode[2];
                    }
                }

                if (!string.IsNullOrEmpty(response.E_CUS_COL_CODE))
                {
                    customerColor = response.E_CUS_COL_CODE;
                }

                await client.CloseAsync();
                return string.Join("/", new[] { colArt, customerColor }.Where(s => !string.IsNullOrEmpty(s)));
            }
            catch (System.ServiceModel.FaultException faultEx)
            {
                client.Abort();

                if (faultEx.Message == "NOT_FOUND")
                {
                    errMsg = "Customer Color Article Not Found";
                }
                else
                {
                    errMsg = faultEx.Message;
                }

                await ConfigHelper.ShowSmartDialog(
                    "InfoTemplate",
                    errMsg,
                    PackIconKind.WarningCircle,
                    Brushes.DarkOrange
                );
            }
            catch
            {
                client.Abort();
                throw;
            }

            return colArt;
        }

        public async Task<List<DocParameter>> GetLeatherMeasureByMatDoc(string orderNo, string docYear, string matDocNoStart, string matDocNoEnd)
        {
            if (!int.TryParse(docYear.Trim(), out int year)) return new List<DocParameter>();

            using var context = _measurementLeatherFactory.CreateDbContext();

            string startDoc = matDocNoStart.Trim();
            string endDoc = matDocNoEnd.Trim();
            string trimmedOrder = orderNo.Trim();

            var rawData = await (from md in context.MsMatDocs.AsNoTracking()
                                 join s in context.MsSummaries.AsNoTracking() on md.MsId equals s.MsId
                                 where s.OrderNo == trimmedOrder &&
                                       s.DocDate.Year == year &&
                                       string.Compare(md.MatDocNo, startDoc) >= 0 &&
                                       string.Compare(md.MatDocNo, endDoc) <= 0
                                 select new
                                 {
                                     S_OrderNo = s.OrderNo,
                                     S_DocDate = s.DocDate,
                                     S_MatNo = s.MatNo,
                                     S_MatDesc = s.MatDesc,
                                     MD_MsMdId = md.MsMdId,
                                     MD_MatDocNo = md.MatDocNo,
                                     MD_Grade = md.Grade,
                                     MD_NgReason = md.NgReason
                                 }).ToListAsync();

            if (!rawData.Any()) return new List<DocParameter>();

            var mdIds = rawData.Select(x => x.MD_MsMdId).ToList();
            var allDetails = await context.MsDatas
                .AsNoTracking()
                .Where(d => mdIds.Contains(d.MsMdId))
                .ToListAsync();

            // ... ส่วนที่เหลือเหมือนเดิมทั้งหมด (ไม่แตะ context อีกแล้วหลังจากนี้)
            var distinctMatNos = rawData.Select(x => x.S_MatNo).Distinct().ToList();
            var colorMap = new Dictionary<string, string>();
            foreach (var matNo in distinctMatNos)
            {
                try { colorMap[matNo] = await GetCustomerColorArticle(matNo); }
                catch { colorMap[matNo] = string.Empty; }
            }

            var sortedRawData = rawData.OrderBy(x => x.MD_Grade).ThenBy(x => x.MD_MatDocNo);
            var result = new List<DocParameter>();

            foreach (var item in sortedRawData)
            {
                var docDetails = allDetails.Where(d => d.MsMdId == item.MD_MsMdId).OrderBy(d => d.Rank).ToList();
                var leatherAreas = docDetails.Select(d => new LeatherArea { Rank = d.Rank, Area = d.Area, Group = d.PackingGroup }).ToList();
                colorMap.TryGetValue(item.S_MatNo, out var colorArticle);
                var descParts = new[] { item.S_MatDesc, colorArticle }.Where(s => !string.IsNullOrWhiteSpace(s));

                result.Add(new DocParameter
                {
                    OrderNo = item.S_OrderNo,
                    DocDate = item.S_DocDate.ToString("yyyy-MM-dd"),
                    MatNo = item.S_MatNo,
                    MatDesc = string.Join("/", descParts),
                    Grade = item.MD_Grade,
                    NGReason = item.MD_NgReason,
                    TotalQty = docDetails.Sum(d => d.Area).ToString("N2"),
                    MeasureData = new ObservableCollection<LeatherArea>(leatherAreas)
                });
            }

            return result;
        }

        public ReturnResult GetMaterialDescription(string materialNo)
        {
            var result = new ReturnResult();

            if (string.IsNullOrWhiteSpace(materialNo))
            {
                result.SetValue(false, "Material number is required.");
                return result;
            }

            string[] matNoParts = materialNo.Split('-');
            if (matNoParts.Length < 3 || matNoParts[2].Length < 4)
            {
                result.SetValue(false, "Invalid Material Number format.");
                return result;
            }

            string criThickness = matNoParts[2].Substring(0, 1);
            string criCustomer = matNoParts[2].Substring(1, 3);
            string articleCode = matNoParts[1];

            using var huefulContext = _huefulDBFactory.CreateDbContext();
            using var materialContext = _materialInfoFactory.CreateDbContext();

            var mstrArticle = huefulContext.MstrArticle
                .FirstOrDefault(ma => ma.AtclCodeU == articleCode && ma.AtclVisible == "T" && ma.AtclActive == "Y");

            var mstrThickness = huefulContext.MstrThickness
                .FirstOrDefault(mt => mt.ThknsAcrn2 == criThickness && mt.ThknsVisible == "T" && mt.ThknsActive == "Y");

            var mstrCustomer = huefulContext.MstrCustomer
                .FirstOrDefault(mc => mc.CustAcrn2 == criCustomer && mc.CustVisible == "T" && mc.CustActive == "Y");

            var materialHistory = materialContext.material_creation_history
                .FirstOrDefault(mch => mch.material_code == materialNo);

            List<string> errors = new();
            if (mstrArticle == null) errors.Add("Article not found.");
            if (mstrThickness == null) errors.Add("Thickness not found.");
            if (mstrCustomer == null) errors.Add("Customer Description not found.");
            if (materialHistory == null) errors.Add("Color Name not found.");

            if (errors.Count > 0)
            {
                result.SetValue(false, string.Join("\n", errors));
            }
            else
            {
                string[] matDesc = {
                mstrArticle?.AtclDesc1 ?? string.Empty,
                mstrThickness?.ThknsDesc3 ?? string.Empty,
                mstrCustomer?.CustDesc1 ?? string.Empty,
                materialHistory?.color_name ?? string.Empty
            };
                result.SetValue(true, string.Join(' ', matDesc));
            }

            return result;
        }

        public async Task SaveToSql(MsSummary? summary, MsMatDoc matDoc, List<MsData> measurements, int existingId = 0)
        {
            using var context = _measurementLeatherFactory.CreateDbContext(); // ← สด ใหม่ ทุกครั้ง!

            matDoc.Details = measurements;

            if (existingId > 0)
            {
                var existingSummary = await context.MsSummaries
                    .Include(x => x.MatDocs)
                    .FirstOrDefaultAsync(x => x.MsId == existingId);

                if (existingSummary != null)
                {
                    existingSummary.MatDocs.Add(matDoc);
                }
            }
            else if (summary != null)
            {
                summary.MatDocs.Add(matDoc);
                context.MsSummaries.Add(summary);
            }

            await context.SaveChangesAsync();
        }

        public ReturnResult GetRfcPwd()
        {
            var result = new ReturnResult();
            try
            {
                using var context = _sapFactory.CreateDbContext();
                var rfcPwd = context.Rfc_pass
                    .OrderByDescending(p => p.Createdate)
                    .FirstOrDefault();

                if (rfcPwd != null && !string.IsNullOrEmpty(rfcPwd.Pwd))
                    result.SetValue(true, rfcPwd.Pwd);
                else
                    result.SetValue(false, "RFC password record was not found or is empty.");
            }
            catch (Exception ex)
            {
                result.SetValue(false, ex.Message);
            }

            return result;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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
