using CNET_ERP_V7_VoucherPrintDialogue.Models;
using CNET_V7_Domain;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Misc.CommonTypes;
using Cnetv7BufferHolder;
using DevExpress.CodeParser;
using HulubejeBooking.Models;
using Microsoft.Extensions.Options;
using CNET_ERP_V7.WebConstants;
using System.Net;
using System.Security.AccessControl;

namespace HulubejeBooking.Helpers
{
    public class PrintDocumentVoucher
    {
        private readonly SharedHelpers _sharedHelpers;
        private readonly HttpClient _httpClient;
        private IWebHostEnvironment _env;
        private bool articleNew = true;
        string Objectstatedefination = "";
        private static OtherSettings _ftpSettings = new OtherSettings();
        public PrintDocumentVoucher(HttpClient httpClient,  SharedHelpers sharedHelpers, IOptions<OtherSettings> otherSettings)
        {
            _httpClient = httpClient;
            _sharedHelpers = sharedHelpers;
            _ftpSettings = otherSettings.Value;
        }
        string userName = "CHM_USER";
        string passWord = "AttACHeMenT5&@BBMF@TIIvsDNR";
        public byte[] logoPic { get; set; }
        public string logoPicUrl { get; set; }
        private int currentConsigneeUnit = 16;

        ArticleSpecificationPrint Spec = new ArticleSpecificationPrint();
        List<ArticleSpecificationPrint> Specification = new List<ArticleSpecificationPrint>();
        List<LineItemConversionValuesPrint> LineItemConversion = new List<LineItemConversionValuesPrint>();
        VoucherValues VoucherValues = new VoucherValues();
        ConsigneeInformationPrint ConsigneeRecord = new ConsigneeInformationPrint();
        NonCashTransactionInformationPrint NonCashPayment = new NonCashTransactionInformationPrint();
        //  VoucherInformationPrint VoucherInfo = new VoucherInformationPrint();
        public string Language { get; set; }
        public bool IsFirstTime { get; set; }
        public bool Hasendweightzero { get; set; }
        public bool IsOnTab { get; set; }
        public async Task<VoucherPrintModel> PrintLineItemVoucher(VoucherDetailDTO rdatasource)
        {
            #region Header
            VoucherPrintModel voucherPrint = new VoucherPrintModel();
            var NonListDataSource = rdatasource.VoucherHeader;
            voucherPrint.NonListDataSource = NonListDataSource;
            voucherPrint.voucherId = NonListDataSource.Id;
            var LineItemList = rdatasource.VoucherDetail;
            var activityList = rdatasource.ActivityDetail;

            Dictionary<string, List<LineItemConversionValues>> ConversionObj = new Dictionary<string, List<LineItemConversionValues>>();
            ArticleObjsPrint LineItemObj = new ArticleObjsPrint();
            List<ArticleObjsPrint> ListLineItemObj = new List<ArticleObjsPrint>();
            var configBuffer = await _sharedHelpers.GetFilterDynamicResponseModel<List<ConfigurationDTO>>("Configuration/dynamic", new Dictionary<string, string>() { { "reference", NonListDataSource.DefinitionId.ToString() } });
            #region print setting
            string attchmentUl = null;
            voucherPrint.attachmentPath = attchmentUl;
            ConsigneeDTO rOrganization = await _sharedHelpers.GetCompany();
            var voucherDefinition = NonListDataSource.Definition.ToString();
            voucherPrint.Type = "Template Type 1";
            List<ConfigurationDTO> value = configBuffer;
            string companyName = "CompanyName";
            foreach (var va in value)
            {
                var attribute = va.Attribute?.ToString()?.Trim().ToLower();
                try
                {
                    switch (attribute)
                    {
                        case "default printer":
                            voucherPrint.defaultPrinter = va.CurrentValue.ToString();
                            break;
                        case "enable e-invoice":
                            voucherPrint.enableEinvoice = bool.Parse(va.CurrentValue.ToString());
                            break;
                        case "print quantity sum":
                            voucherPrint.PrintQuantitySum = bool.Parse(va.CurrentValue);
                            break;
                        case "date format":
                            voucherPrint.DateFormat = va.CurrentValue.ToString();
                            break;
                        case "merge item code and description":
                            voucherPrint.MergreItemCodeAndDescription = bool.Parse(va.CurrentValue.ToString());
                            break;
                        case "max line item":
                            voucherPrint.MaxLineItem = uint.Parse(va.CurrentValue.ToString());
                            break;
                        case "print seasonal message":
                            voucherPrint.PrintSeasonalmessage = bool.Parse(va.CurrentValue.ToString());
                            break;
                        case "max no of printing":
                            voucherPrint.MaxNoOfPrinting = int.Parse(va.CurrentValue.ToString());
                            break;
                        case "no of line item per page":
                            voucherPrint.NoOfLineItemPerPage = uint.Parse(va.CurrentValue.ToString());
                            break;
                        case "voucher orientation":
                            voucherPrint.VoucherOrientation = va.CurrentValue.ToString();
                            break;
                        case "print lineitem conversion":
                            voucherPrint.EnableQtyConversion = bool.Parse(va.CurrentValue);
                            break;
                        case "print remote distribution":
                            voucherPrint.PrintRemoteDistribution = bool.Parse(va.CurrentValue);
                            break;
                        case "print copy distribution":
                            voucherPrint.PrintCopyDistribution = bool.Parse(va.CurrentValue.ToLower());
                            break;
                        case "print catalogue automatically":
                            voucherPrint.PrintCatalogueAutomatically = bool.Parse(va.CurrentValue.ToString());
                            break;
                        case "print article code":
                            voucherPrint.PrintArticleCode = bool.Parse(va.CurrentValue.ToString());
                            break;
                        case "print article picture":
                            voucherPrint.PrintArticlePicture = bool.Parse(va.CurrentValue);
                            break;
                        case "print article volume":
                            voucherPrint.PrintArticleVolume = bool.Parse(va.CurrentValue);
                            break;
                        case "print journal":
                            voucherPrint.PrintJournal = bool.Parse(va.CurrentValue);
                            break;
                        case "print sum. art. phy. dim.":
                            voucherPrint.PrintSum = bool.Parse(va.CurrentValue);
                            break;
                        case "print values":
                            voucherPrint.PrintValues = va.CurrentValue.ToString();
                            break;
                        case "paper size":
                            voucherPrint.PaperSize = va.CurrentValue.ToString();
                            break;
                        case "print immediate reference":
                            voucherPrint.PrintimmediateReference = bool.Parse(va.CurrentValue);
                            break;
                        case "print ancestor reference":
                            voucherPrint.PrintAncestorReference = bool.Parse(va.CurrentValue);
                            break;
                        case "print ancestor extension":
                            voucherPrint.PrintAncestorExtension = bool.Parse(va.CurrentValue);
                            break;
                        case "print water mark":
                            voucherPrint.PrintWaterMark = va.CurrentValue.ToString();
                            break;
                        case "print without preview":
                            voucherPrint.PrintWithoutPreview = bool.Parse(va.CurrentValue);
                            break;
                        case "print amount in word":
                            voucherPrint.PrintAmountInWord = bool.Parse(va.CurrentValue);
                            break;
                        case "sort line item":
                            voucherPrint.SortLineItem = va.CurrentValue.ToString();
                            break;
                        case "no of copies":
                            voucherPrint.NoOfCopies = short.Parse(va.CurrentValue.ToString());
                            break;
                        case "paper type":
                            voucherPrint.PaperType = va.CurrentValue.ToString();
                            break;
                        case "use darker lines":
                            voucherPrint.UseDarkerLines = bool.Parse(va.CurrentValue.ToString());
                            break;
                        case "enable payment option":
                            voucherPrint.EnablePaymentOptions = bool.Parse(va.CurrentValue.ToString());
                            break;
                        case "voucher user orientation":
                            voucherPrint.VoucherUserOrientation = va.CurrentValue.ToString();
                            break;
                        case "top margin":
                            voucherPrint.Top = string.IsNullOrEmpty(va.CurrentValue) ? 0 : Convert.ToInt16(va.CurrentValue);
                            break;
                        case "left margin":
                            voucherPrint.Left = string.IsNullOrEmpty(va.CurrentValue) ? 0 : Convert.ToInt16(va.CurrentValue);
                            break;
                        case "right margin":
                            voucherPrint.Right = string.IsNullOrEmpty(va.CurrentValue) ? 0 : Convert.ToInt16(va.CurrentValue);
                            break;
                        case "bottom margin":
                            voucherPrint.Right = string.IsNullOrEmpty(va.CurrentValue) ? 0 : Convert.ToInt16(va.CurrentValue);
                            break;
                        case "type":
                            voucherPrint.Type = va.CurrentValue.ToString();
                            break;
                        case "round digit total":
                            voucherPrint.RoundDigitTotal = va.CurrentValue;
                            break;
                        case "round digit quantity":
                            voucherPrint.RoundDigitQuantity = va.CurrentValue;
                            break;
                        case "round digit unit price":
                            voucherPrint.RoundDigitUnitPrice = va.CurrentValue;
                            break;
                        case "enable weight bridge":
                            voucherPrint.EnableBridgeWeight = bool.Parse(va.CurrentValue);
                            break;
                        case "enable term":
                            voucherPrint.EnableTerm = bool.Parse(va.CurrentValue);
                            break;
                        case "print bank info":
                            voucherPrint.PrintBankInfo = bool.Parse(va.CurrentValue);
                            break;
                        case "print batch":
                            voucherPrint.PrintBatch = bool.Parse(va.CurrentValue);
                            break;
                        case "print specification":
                            voucherPrint.PrintSpecification = va.CurrentValue;
                            break;
                        case "print expiry date":
                            voucherPrint.PrintExpiryDate = bool.Parse(va.CurrentValue);
                            break;
                        case "print production date":
                            voucherPrint.PrintProductionDate = bool.Parse(va.CurrentValue);
                            break;
                        case "print consignee code":
                            voucherPrint.PrintConsigneeCode = bool.Parse(va.CurrentValue);
                            break;
                        case "print activity reference":
                            voucherPrint.PrintReferenceActivity = va.CurrentValue;
                            break;
                        case "template document":
                            voucherPrint.TemplateDocument = va.CurrentValue;
                            break;
                        case "document header":
                            companyName = va.CurrentValue;
                            break;
                    }
                }
                catch { }
            }
            #endregion
            var taxTransBuffer = await _sharedHelpers.GetFilterDynamicResponseModel<List<TaxTransactionDTO>>("TaxTransaction/dynamic", new Dictionary<string, string>() { { "voucher", NonListDataSource.Id.ToString() } });
            var rDistribution = await _sharedHelpers.GetFilterDynamicResponseModel<List<DistributionDTO>>("Distribution/dynamic", new Dictionary<string, string>() { { "SystemConstant", NonListDataSource.DefinitionId.ToString() }, { "type", "1578" } });
            var terms = await _sharedHelpers.GetFilterDynamicResponseModel<List<VwVoucherTermViewDTO>>("VwVoucherTermView/dynamic", new Dictionary<string, string>() { { "Voucher", NonListDataSource.Id.ToString() } });
            var currencyTrans = await _sharedHelpers.GetFilterDynamicResponseModel<List<TransactionCurrencyDTO>>("TransactionCurrency/dynamic", new Dictionary<string, string>() { { "Voucher", NonListDataSource.Id.ToString() } });
            voucherPrint.TermListView = terms;
            var systemConstantBuffer = GeneralBufferHolder.SystemConstants;
            #endregion


            #region voucher Information and Voucher values
            if (voucherPrint.DateFormat?.ToLower() == "longdate")
                voucherPrint.IssuedDate = NonListDataSource.IssuedDate.ToString();
            else if (voucherPrint.DateFormat?.ToLower() == "mediumdate")
                voucherPrint.IssuedDate = NonListDataSource.IssuedDate.ToString("dd-MMM-yyyy");
            else
                voucherPrint.IssuedDate = NonListDataSource.IssuedDate.ToShortDateString();
            NumberToEnglish numToEng = new NumberToEnglish();
            var currencyBuffer = new List<CurrencyDTO>();
            var currency = new CurrencyDTO();
            if (NonListDataSource.CurrencyDescription != null)
            {
                currency.Description = NonListDataSource.CurrencyDescription;
                currency.IsDefault = true;
            }
            else
            {
                currency.Description = "Birr";
                currency.IsDefault = true;
            }
            currencyBuffer.Add(currency);
            VoucherValues voucherValue = new VoucherValues()
            {
                SubTotal = NonListDataSource.SubTotal,
                Remark = NonListDataSource.Remark,
                AdditionalCharge = NonListDataSource.AddCharge,
                AdditionalChargeLabel = rdatasource.AddChargeDescription,
                GrandTotal = decimal.Parse(String.Format("{0:n2}", NonListDataSource.GrandTotal)),
                GrandTotalInWords = numToEng.changeCurrencyToWords(String.Format("{0:n2}", NonListDataSource.GrandTotal), currencyBuffer),
                Discount = NonListDataSource.Discount,
            };

            voucherValue.VoucherTax = taxTransBuffer?.Where(x => x.Voucher == NonListDataSource.Id).ToList();
            voucherPrint.TaxBuffer = GeneralBufferHolder.AllTaxs;
            voucherPrint.voucherValues = voucherValue;
            voucherPrint.GrandTotalInWords = voucherValue.GrandTotalInWords;
            voucherPrint.withHoldingAmount = voucherValue.VoucherTax.FirstOrDefault(x => x.Tax == 6)?.TaxAmount ?? 0;
            if (voucherPrint.enableEinvoice)
            {
                var _invoice = await Get_EInvoice_By_VoucherId(NonListDataSource.Id, CNETConstants.Einvoice_RN_Type_Invoice);
                if (_invoice != null)
                {
                    voucherPrint.IRN = _invoice.Rn;
                    voucherPrint.SourceNo = _invoice.SourceNumber;
                }
            }
            decimal WithHoldingTax = 0;
            if (voucherValue != null)
            {

            }

            voucherPrint.VoucherString = NonListDataSource.Code;
            voucherPrint.IsVoid = NonListDataSource.IsVoid;
            voucherPrint.IsIssued = NonListDataSource.IsIssued;
            voucherPrint.printDialogueDocumentname = GeneralBufferHolder.SystemConstants?.FirstOrDefault(i => i.Id == NonListDataSource.DefinitionId)?.Description;
            voucherPrint.documentname = GeneralBufferHolder.SystemConstants?.FirstOrDefault(i => i.Id == NonListDataSource.DefinitionId)?.Description;
            voucherPrint.voucherDefinition = NonListDataSource.DefinitionId;
            voucherPrint.ConsigneeContactName = NonListDataSource.ContactPersonName;
            #endregion

            #region copy and department
            voucherPrint.CopyDescription = new List<string>();
            voucherPrint.OrganizationDepartment = new List<string>();
            voucherPrint.DistrbutionPrinterList = new List<string>();
            if (rDistribution != null && voucherPrint.PrintCopyDistribution)
            {
                DistributionDTO distributionDTO = new DistributionDTO();
                foreach (DistributionDTO objdistribution in rDistribution)
                {
                    try
                    {
                        voucherPrint.CopyDescription.Add(GeneralBufferHolder.AllLookups.FirstOrDefault(x => x.Id == objdistribution.Index).Description);
                    }
                    catch { }
                    voucherPrint.DistrbutionPrinterList.Add(objdistribution.Remark);
                    var _targetUnits = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string>() { { "Id", objdistribution.Destination?.ToString() } });
                    var _targetUnit = _targetUnits?.FirstOrDefault();
                    voucherPrint.OrganizationDepartment.Add(objdistribution.Destination != null ? _targetUnit?.Name : "");
                }
                if (voucherPrint.CopyDescription != null)
                {
                    string coppyDstributionString = "";
                    string CopyDepartmentString = "";
                    var index = 0;
                    bool mOC = false;
                    bool mFC = false;
                    bool mSC = false;
                    bool mTC = false;
                    bool mFrC = false;
                    bool mFiC = false;
                    if (voucherPrint.CopyDescription.Contains("Original copy") || voucherPrint.CopyDescription.Contains("Orignal copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("Orignal copy");
                        coppyDstributionString += "Original Copy -" + voucherPrint.OrganizationDepartment[index];
                    }
                    if (voucherPrint.CopyDescription.Contains("1st copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("1st copy");
                        if (mOC == true)
                            coppyDstributionString += " , 1st Copy - " + voucherPrint.OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 1st copy -" + voucherPrint.OrganizationDepartment[index];
                        mFC = true;
                    }
                    if (voucherPrint.CopyDescription.Contains("2nd copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("2nd copy");
                        if (mFC == true)
                            coppyDstributionString += " , 2nd Copy - " + voucherPrint.OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 2nd Copy - " + voucherPrint.OrganizationDepartment[index];
                        mSC = true;
                    }
                    if (voucherPrint.CopyDescription.Contains("3rd copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("3rd copy");
                        if (mOC == true | mFC == true | mSC == true)
                            coppyDstributionString += " , 3rd Copy - " + voucherPrint.OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 3rd Copy - " + voucherPrint.OrganizationDepartment[index];
                        mTC = true;
                    }
                    if (voucherPrint.CopyDescription.Contains("4th copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("4th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true)
                            coppyDstributionString += " , 4th Copy - " + voucherPrint.OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 4th Copy - " + voucherPrint.OrganizationDepartment[index];
                        mFrC = true;
                    }
                    if (voucherPrint.CopyDescription.Contains("5th copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("5th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true | mFrC == true)
                            coppyDstributionString += " , 5th Copy - " + voucherPrint.OrganizationDepartment[index];
                        else
                            coppyDstributionString += "5th Copy - " + voucherPrint.OrganizationDepartment[index];
                        mFiC = true;

                    }
                    if (voucherPrint.CopyDescription.Contains("6th copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("6th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true | mFrC == true | mFiC == true)
                            coppyDstributionString += "  , 6th Copy - " + voucherPrint.OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 6th Copy - " + voucherPrint.OrganizationDepartment[index];
                    }
                    voucherPrint.CopyDescriptionString = coppyDstributionString;
                }

                if (voucherPrint.OrganizationDepartment.Count() > 0)
                {
                    voucherPrint.NoOfCoppyDistributions = voucherPrint.OrganizationDepartment.Count();
                    #region Watermark
                    //var GetCount = GetPrintCount(voucherPrint, true);
                    voucherPrint.IsVoucherPrinted = false; //GetCount.Item1;
                    var act = await _sharedHelpers.GetFilterDynamicResponseModel<List<ActivityDTO>>("Activity/dynamic", new Dictionary<string, string>() { { "Reference", NonListDataSource.Id.ToString() }, { "Pointer", "3097" }, { "requiredFields", "Id" } });
                    if (act?.Count > 0)
                    {
                        voucherPrint.PrintCount = (int)act.Count();// GetCount.Item2;
                    }
                    string mDistTo = null;
                    if (voucherPrint.PrintWaterMark == "Standard")
                    {
                        if (voucherPrint.IsVoid == true)
                        {
                        }
                        else if (voucherPrint.IsPreview == true)
                        {
                        }
                        else if (voucherPrint.IsIssued != true)
                        {
                        }
                        else
                        {
                            if (voucherPrint.PrintCount == 0)
                            {
                                if (voucherPrint.CopyDescription != null)
                                {
                                    if (voucherPrint.CopyDescription.Contains("Orignal copy"))
                                    {
                                        int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("Orignal copy"));
                                        mDistTo = voucherPrint.OrganizationDepartment[index];
                                    }
                                }
                            }
                            else if (voucherPrint.PrintCount == 1)
                            {
                                if (voucherPrint.CopyDescription != null)
                                {
                                    if (voucherPrint.CopyDescription.Count > 0)
                                    {
                                        if (voucherPrint.CopyDescription.Contains("1st copy"))
                                        {
                                            int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("1st copy"));
                                            mDistTo = voucherPrint.OrganizationDepartment[index];
                                        }
                                    }
                                }
                            }
                            else if (voucherPrint.PrintCount == 2)
                            {
                                if (voucherPrint.CopyDescription != null)
                                {
                                    if (voucherPrint.CopyDescription.Count > 0)
                                    {
                                        if (voucherPrint.CopyDescription.Contains("2nd copy"))
                                        {
                                            int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("2nd copy"));
                                            mDistTo = voucherPrint.OrganizationDepartment[index];
                                        }
                                    }
                                }
                            }
                            else if (voucherPrint.PrintCount == 3)
                            {
                                if (voucherPrint.CopyDescription != null)
                                {
                                    if (voucherPrint.CopyDescription.Count > 0)
                                    {
                                        if (voucherPrint.CopyDescription.Contains("3rd copy"))
                                        {
                                            int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("3rd copy"));
                                            mDistTo = voucherPrint.OrganizationDepartment[index];
                                        }
                                    }
                                }
                            }
                            else if (voucherPrint.PrintCount == 4)
                            {
                                if (voucherPrint.CopyDescription != null)
                                {
                                    if (voucherPrint.CopyDescription.Count > 0)
                                    {
                                        if (voucherPrint.CopyDescription.Contains("4th copy"))
                                        {
                                            int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("4th copy"));
                                            mDistTo = voucherPrint.OrganizationDepartment[index];
                                        }
                                    }
                                }
                            }
                            else if (voucherPrint.PrintCount == 5)
                            {
                                if (voucherPrint.CopyDescription != null)
                                {
                                    if (voucherPrint.CopyDescription.Count > 0)
                                    {
                                        if (voucherPrint.CopyDescription.Contains("5th copy"))
                                        {
                                            int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("5th copy"));
                                            mDistTo = voucherPrint.OrganizationDepartment[index];
                                        }
                                    }
                                }
                            }
                            else if (voucherPrint.PrintCount == 6)
                            {
                                if (voucherPrint.CopyDescription != null)
                                {
                                    if (voucherPrint.CopyDescription.Count > 0)
                                    {
                                        if (voucherPrint.CopyDescription.Contains("6th copy"))
                                        {
                                            int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("6th copy"));
                                            mDistTo = voucherPrint.OrganizationDepartment[index];
                                        }
                                    }
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                    voucherPrint.OrganizationDepartmentString = mDistTo;
                    #endregion
                }

            }
            #endregion

            #region Consignees
            List<OtherConsigneeDetail> OtherConsList = new List<OtherConsigneeDetail>();
            List<ConsigneeUnitDTO> consigneeUnitBuffer = new List<ConsigneeUnitDTO>();
            try
            {
                var requiredGsls = await _sharedHelpers.GetFilterDynamicResponseModel<List<RequiredGslDTO>>("RequiredGsl/dynamic", new Dictionary<string, string>() { { "VoucherDefn", NonListDataSource.DefinitionId.ToString() }, { "type", "1835" } });
                if (requiredGsls?.Count > 0)
                {
                    int i = 0;
                    requiredGsls.ForEach(x => { x.Index = i; i++; });
                }
                if (NonListDataSource.Consignee1Id != null || requiredGsls?.Count > 0)
                {
                    var req1 = requiredGsls.FirstOrDefault(x => x.Index == 0);
                    if (voucherPrint.PrintConsigneeCode)
                        NonListDataSource.Consignee1FullName = NonListDataSource.Consignee1FullName + " (" + NonListDataSource.ConsigneeCode ?? "" + ")";
                    var Cons1 = new OtherConsigneeDetail()
                    {
                        consignee = NonListDataSource.Consignee1Id ?? 0,
                        consigneeFullName = NonListDataSource.Consignee1FullName,
                        consigneTin = NonListDataSource.Tin,
                        requiredGSlDesc = req1?.Description
                    };

                    OtherConsList.Add(Cons1);
                    var consUn = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string>() { { "Consignee", NonListDataSource.Consignee1Id.ToString() } });
                    if (consUn != null)
                        consigneeUnitBuffer.AddRange(consUn);
                }

                if (NonListDataSource.Consignee2Id != null || requiredGsls?.Count > 1)
                {
                    var req = requiredGsls.FirstOrDefault(x => x.Index == 1);
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = NonListDataSource.Consignee2Id ?? 0,
                        consigneeFullName = NonListDataSource.Consignee2FullName,
                        requiredGSlDesc = req?.Description
                    };

                    OtherConsList.Add(Cons);
                    var consUn = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string>() { { "Consignee", NonListDataSource.Consignee2Id.ToString() } });
                    if (consUn?.Count > 0)
                        consigneeUnitBuffer.Add(consUn.FirstOrDefault());
                }
                if (NonListDataSource.Consignee3Id != null || requiredGsls?.Count > 2)
                {
                    var req = requiredGsls.FirstOrDefault(x => x.Index == 2);
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = NonListDataSource.Consignee3Id ?? 0,
                        consigneeFullName = NonListDataSource.Consignee3FullName,
                        requiredGSlDesc = req?.Description
                    };

                    OtherConsList.Add(Cons);
                    var consUn = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string>() { { "Consignee", NonListDataSource.Consignee3Id.ToString() } });
                    if (consUn?.Count > 0)
                        consigneeUnitBuffer.Add(consUn.FirstOrDefault());
                }
                if (NonListDataSource.Consignee4Id != null || requiredGsls?.Count > 3)
                {
                    var req = requiredGsls.FirstOrDefault(x => x.Index == 3);
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = NonListDataSource.Consignee4Id ?? 0,
                        consigneeFullName = NonListDataSource.Consignee4FullName,
                        requiredGSlDesc = req?.Description
                    };

                    OtherConsList.Add(Cons);
                    var consUn = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string>() { { "Consignee", NonListDataSource.Consignee4Id.ToString() } });
                    if (consUn?.Count > 0)
                        consigneeUnitBuffer.Add(consUn.FirstOrDefault());
                }
                if (NonListDataSource.Consignee5Id != null || requiredGsls?.Count > 4)
                {
                    var req = requiredGsls.FirstOrDefault(x => x.Index == 4);
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = NonListDataSource.Consignee5Id ?? 0,
                        consigneeFullName = NonListDataSource.Consignee5FullName,
                        requiredGSlDesc = req?.Description
                    };

                    OtherConsList.Add(Cons);
                    var consUn = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string>() { { "Consignee", NonListDataSource.Consignee5Id.ToString() } });
                    if (consUn?.Count > 0)
                        consigneeUnitBuffer.Add(consUn.FirstOrDefault());
                }
                if (NonListDataSource.Consignee6Id != null || requiredGsls?.Count == 5)
                {
                    var req = requiredGsls.FirstOrDefault(x => x.Index == 5);
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = NonListDataSource.Consignee6Id ?? 0,
                        consigneeFullName = NonListDataSource.Consignee6FullName,
                        requiredGSlDesc = req?.Description
                    };

                    OtherConsList.Add(Cons);
                    var consUn = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string>() { { "Consignee", NonListDataSource.Consignee6Id.ToString() } });
                    if (consUn?.Count > 0)
                        consigneeUnitBuffer.Add(consUn.FirstOrDefault());
                }
            }
            catch
            {

            }
            voucherPrint.OtherConsigneeDetail = OtherConsList;
            voucherPrint.ConsigneeUnitBuffer = consigneeUnitBuffer;
            #endregion
            #region Lineitem and Journal
            if (LineItemList != null)
            {
                ArticleSpecification Spec = new ArticleSpecification();
                LineItemConversionValues LICv = new LineItemConversionValues();
                List<ArticleSpecification> Specification = new List<ArticleSpecification>();
                List<LineItemConversionValues> LineItemConversion = new List<LineItemConversionValues>();
                Dictionary<string, List<ArticleSpecification>> SpecObj = new Dictionary<string, List<ArticleSpecification>>();
                var sn = 1;
                foreach (var objectADD in LineItemList)
                {
                    if (objectADD != null)
                    {

                        if (rOrganization.Tin == "0000028457")
                        {
                            objectADD.ArticleCode = string.IsNullOrWhiteSpace(objectADD.LocalCode) ? objectADD.BarCode : objectADD.LocalCode;
                        }
                        if (ConversionObj.ContainsKey(objectADD.LineItemId.ToString()))
                        {
                            LineItemConversion = ConversionObj[objectADD.LineItemId.ToString()];
                            if (LineItemConversion.Any(c => c.code == objectADD.LineItemId.ToString()))
                            {
                                LICv = new LineItemConversionValues();
                                LICv.code = objectADD.LineItemId.ToString();
                                LICv.uom = objectADD?.Uom.ToString();
                                LICv.UnitAmount = objectADD.UnitAmount != null ? (decimal)objectADD.UnitAmount : 0;
                                LICv.Quantity = objectADD.Quantity != null ? (double)objectADD.Quantity : 0;
                                LICv.UOMLookupDescription = systemConstantBuffer.Where(x => x.Id == objectADD.Uom)?.FirstOrDefault()?.Description;
                                LineItemConversion.Add(LICv);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(objectADD.LineItemId.ToString()))
                            {
                                LineItemConversion = new List<LineItemConversionValues>();
                                LICv = new LineItemConversionValues();
                                LICv.code = objectADD.LineItemId.ToString();

                                LICv.uom = objectADD.Uom.ToString();
                                LICv.UnitAmount = objectADD.UnitAmount != null ? (decimal)objectADD.UnitAmount : 0;
                                LICv.Quantity = objectADD.Quantity != null ? (double)objectADD.Quantity : 0;
                                LICv.UOMLookupDescription = systemConstantBuffer.Where(x => x.Id == objectADD.Uom)?.FirstOrDefault()?.Description; ;
                                LineItemConversion.Add(LICv);
                                ConversionObj.Add(objectADD.LineItemId.ToString(), LineItemConversion);
                                LineItemObj.LineItemConversion = ConversionObj;
                            }
                        }
                        if (!ListLineItemObj.Any(l => l.LineItemCode == objectADD.LineItemId.ToString()))
                        {
                            int rndQty = 0;
                            int.TryParse(voucherPrint.RoundDigitQuantity, out rndQty);
                            if (rndQty <= 0 || rndQty > 8)
                                rndQty = 2;

                            int rndUA = 0;
                            int.TryParse(voucherPrint.RoundDigitUnitPrice, out rndUA);
                            if (rndUA <= 0 || rndUA > 8)
                                rndUA = 2;

                            int rndTA = 0;
                            int.TryParse(voucherPrint.RoundDigitTotal, out rndTA);
                            if (rndTA <= 0 || rndTA > 8)
                                rndTA = 2;

                            LineItemObj.LineItemCode = objectADD.LineItemId.ToString();
                            LineItemObj.Catagory = objectADD.ArticleChildDesc;
                            LineItemObj.Article = objectADD.ArticleCode;
                            if (!string.IsNullOrWhiteSpace(voucherPrint.PrintSpecification) && voucherPrint.PrintSpecification != "None")
                            {
                                var speDic = new Dictionary<string, string>() { { "Article", objectADD.ArticleId.ToString() } };
                                if (voucherPrint.PrintSpecification == "Allspecificationtype") { }
                                else if (voucherPrint.PrintSpecification == "Keyspecificationtype")
                                    speDic.Add("Type", "1900");
                                else if (voucherPrint.PrintSpecification == "NonKeyspecificationtype")
                                    speDic.Add("Type", "1901");
                                var _specs = await _sharedHelpers.GetFilterDynamicResponseModel<List<SpecificationDTO>>("Specification/dynamic", speDic);
                                if (_specs?.Count > 0)
                                {
                                    var txtspec = "";
                                    _specs.ForEach(c => txtspec += String.Format("    {0}{1}", c.Attribute, System.Environment.NewLine));
                                    objectADD.LineItemDescription = String.Format("{0}{1}{2}", objectADD.LineItemDescription, System.Environment.NewLine, txtspec);
                                }
                            }
                            LineItemObj.Description = objectADD.LineItemDescription;
                            LineItemObj.Quantity = Math.Round(objectADD.Quantity, rndQty);
                            LineItemObj.UnitAmnt = Math.Round(objectADD.UnitAmount, rndUA);
                            LineItemObj.UOM = systemConstantBuffer.Where(x => x.Id == objectADD?.Uom)?.FirstOrDefault()?.Description;
                            LineItemObj.size1 = objectADD.Size1;
                            LineItemObj.size2 = objectADD.Size2;

                            LineItemObj.sn = sn;
                            LineItemObj.Description = voucherPrint.PaperSize == "A5" ? string.Format("{0} [{1}]", LineItemObj.Description, LineItemObj.UOM) : LineItemObj.Description;
                            LineItemObj.TotalAmount = Math.Round(objectADD.TotalAmount, rndTA);
                            if (rOrganization.Tin == "0025239533" && voucherPrint.voucherDefinition == 116)
                            {
                                string? ftpFilePath2 = null;
                                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
                                {
                                    try
                                    {
                                        var artDefURL = (await _sharedHelpers.GetFilterDynamicResponseModel<List<ArticleDTO>>("Article/dynamic", new Dictionary<string, string>() { { "id", objectADD.ArticleId.ToString() }, { "requiredFields", "DefaultImageUrl" } }))?.FirstOrDefault();
                                        if (!string.IsNullOrWhiteSpace(artDefURL?.DefaultImageUrl))
                                        {
                                            ftpFilePath2 = String.Format("{0}{1}", _ftpSettings.FtpFilePathIP, artDefURL.DefaultImageUrl);
                                            var task = Task.Run(async () =>
                                            {
                                                System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(ftpFilePath2);
                                                request.Method = System.Net.WebRequestMethods.Ftp.GetFileSize;
                                                request.Credentials = new System.Net.NetworkCredential(userName, passWord);
                                                System.Net.FtpWebResponse response = await request.GetResponseAsync() as System.Net.FtpWebResponse;
                                                request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(ftpFilePath2);
                                                request.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
                                                request.Credentials = new System.Net.NetworkCredential(userName, passWord);

                                                using (Stream responseStream = request.GetResponse().GetResponseStream())
                                                {
                                                    using (MemoryStream memoryStream = new MemoryStream())
                                                    {
                                                        responseStream.CopyTo(memoryStream);
                                                        LineItemObj.articlePic = memoryStream.ToArray();

                                                    }
                                                }
                                            }
                                            , cancellationTokenSource.Token);
                                            task.Wait(cancellationTokenSource.Token);
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                            ListLineItemObj.Add(LineItemObj);
                            LineItemObj = new ArticleObjsPrint();
                            if (objectADD.LineitemOverride == true)
                            {
                                LineItemObj.Description = LineItemObj.LineItemNote;
                                if (!string.IsNullOrWhiteSpace(LineItemObj.Description))
                                {
                                    LineItemObj.Description.Replace("\n", System.Environment.NewLine);
                                }
                            }
                            if (voucherPrint.voucherDefinition == 284 && rOrganization.Tin == "0025239533")
                            {
                                var lineitemEx = GeneralBufferHolder.AllVoucherExtDefinitions.Where(x => x.Type == 1963 && x.VoucherDefinition == 284)?.ToList();
                                if (lineitemEx?.Count > 0)
                                {
                                    var dict = new Dictionary<int, string>();
                                    var inx = 0;
                                    lineitemEx.ForEach(x => { dict.Add(inx, objectADD.Extension1 ?? ""); inx++; });
                                    LineItemObj.LineItemExtension = dict;
                                }
                            }
                            sn++;
                        }


                    }
                }

            }

            voucherPrint.OtherConsigneeDetail = OtherConsList;
            voucherPrint.ListLineItemObj = ListLineItemObj;
            voucherPrint.JournalDetObj = rdatasource.JournalDetailRefering;
            #endregion

            #region Voucher Operators, Terms, and extensions
            if (activityList != null)
            {
                List<string> dateList = new List<string>();
                List<bool> isManualList = new List<bool>();
                List<string> opList = new List<string>();
                List<string> actList = new List<string>();
                foreach (var val in activityList)
                {
                    if (val.IsPrint == true)
                    {
                        opList.Add(val.UserName ?? "");
                        string dt = val.ActivityDate != null ? val.ActivityDate?.ToString("dd-MM-yyyy") : "";
                        dateList.Add(dt);
                        actList.Add(val.ActivityDefDesc ?? "");
                        isManualList.Add(val.IsManual ?? false);
                    }

                }
                voucherPrint.Voucheroperators = opList;
                voucherPrint.ActivityDate = dateList;
                voucherPrint.ActivityDefDesc = actList;
                voucherPrint.WorkflowManual = isManualList;
                bool isPrintActivitiyReferenceSet = (!string.IsNullOrEmpty(voucherPrint.PrintReferenceActivity) && !voucherPrint.PrintReferenceActivity.ToLower().Equals("notapplicable")) ? true : false;
                if (!string.IsNullOrEmpty(voucherPrint.SortLineItem))
                {
                    if (voucherPrint.SortLineItem.ToLower() == "asentered")
                    {

                    }
                    else if (voucherPrint.SortLineItem.ToLower() == "bynamealphabetically")
                        voucherPrint.ListLineItemObj = voucherPrint.ListLineItemObj.OrderBy(x => x.Description).ToList();
                    else if (voucherPrint.SortLineItem.ToLower() == "bycodealphabetically")
                        voucherPrint.ListLineItemObj = voucherPrint.ListLineItemObj.OrderBy(x => x.Article).ToList();
                }
                int sn = 1;
                voucherPrint.ListLineItemObj.ForEach(x => { x.sn = sn; sn++; });
                voucherPrint.printDialogueConsineeCode = voucherPrint.ConsigneeCode;
                voucherPrint.storeString = "";
                string mTextToPrint = "";

                if (NonListDataSource.SourceStoreDescription != null)
                    mTextToPrint = NonListDataSource.SourceStoreDescription;
                if (NonListDataSource.DestinationStoreDescription != null)
                {
                    mTextToPrint += "=>";
                    mTextToPrint += NonListDataSource.DestinationStoreDescription;
                }
                if (!string.IsNullOrWhiteSpace(mTextToPrint))
                    voucherPrint.storeString = mTextToPrint;


                voucherPrint.Remark = NonListDataSource.Remark;
                voucherPrint.Note = NonListDataSource.Note;
                voucherPrint.Purpose = NonListDataSource.Purpose != null ? GeneralBufferHolder.AllLookups.FirstOrDefault(x => x.Id == NonListDataSource.Purpose)?.Description : null;

                if (voucherPrint.voucherDefinition == 241)
                    voucherPrint.Remark = rdatasource.VoucherHeader.ConsigneeUnit1Description;

                voucherPrint.transactionRemark = NonListDataSource.Remark;
                if (NonListDataSource.Cart != null)
                    voucherPrint.cart = NonListDataSource?.Cart.ToString();

            }
            if (!voucherPrint.PrintAmountInWord)
                voucherPrint.GrandTotalInWords = "";
            string oppText = "";

            if (voucherPrint?.VoucherUserOrientation == "Horizontal")
            {
                if (voucherPrint.ActivityDefDesc?.Count > 0)
                {
                    var opereratorString = new List<string>();
                    var operarators = voucherPrint.Voucheroperators;
                    var activityDesc = voucherPrint.ActivityDefDesc;
                    var activityDate = voucherPrint.ActivityDate;
                    if (activityDesc?.Count > 0)
                    {
                        for (int z = 0; z < activityDesc.Count; z++)
                        {
                            try
                            {
                                oppText += voucherPrint.ActivityDefDesc[z];
                                oppText += " by  ";
                                oppText += voucherPrint.WorkflowManual[z] ? " ________________  " : voucherPrint.Voucheroperators[z];
                                oppText += " on  " + activityDate[z] + "  ";
                                oppText += voucherPrint.WorkflowManual[z] ? "__________" : "";
                                opereratorString.Add(oppText);
                            }
                            catch { }
                        }
                    }
                }
            }
            voucherPrint.VoucheroperatorsString = oppText;
            if (voucherPrint?.ActivityDate != null)
            {
                foreach (var dt in voucherPrint.ActivityDate)
                {
                    if (string.IsNullOrWhiteSpace(voucherPrint?.DateString))
                        voucherPrint.DateString = dt;
                    else
                        voucherPrint.DateString = voucherPrint.DateString + "," + dt;
                }
            }
            voucherPrint.fsNo = NonListDataSource.FsNumber;
            if (voucherPrint.EnablePaymentOptions)
            {
                voucherPrint.payment_method = NonListDataSource.PaymentMethodDesc;
            }
            if (voucherPrint.EnablePaymentOptions)
            {
                string _paytxt = NonListDataSource.PaymentMethodDesc ?? "";
                if (NonListDataSource.PaymentMethod != null && NonListDataSource.PaymentMethod != 1748)
                {
                    if (NonListDataSource.ProcessingEntityName != null)
                        _paytxt += String.Format(", {0}", NonListDataSource.ProcessingEntityName);
                    if (NonListDataSource.PaymentRefNumber != null)
                        _paytxt += String.Format(", {0}", NonListDataSource.PaymentRefNumber);
                    if (NonListDataSource.PaymentIssueDate != null)
                        _paytxt += String.Format(", Maturity: {0}", NonListDataSource.PaymentIssueDate.Value.ToString("MM/dd/yyyy"));
                }
                voucherPrint.payment_method = _paytxt;
            }
            voucherPrint.mrsNo = NonListDataSource.Mrc;
            Dictionary<string, string?> RefAndExt = new Dictionary<string, string?>();
            #endregion

            #region Transaction References and voucherextensions
            var refText = "";
            if (rdatasource.tranReferenced?.Count > 0)
            {
                foreach (var TrRef in rdatasource.tranReferenced)
                {
                    refText += TrRef.Remark + ",";
                    if (TrRef.ReferencedVoucherDefn == 354 && TrRef.Referenced != null)
                    {
                        var voucher = await _sharedHelpers.GetFilterDynamicData<List<VoucherDTO>>("Voucher/dynamic", new Dictionary<string, string>() { { "id", TrRef.Referenced.ToString() }, { "requiredFields", "Id,Code,Definition" } });
                        voucherPrint.RegNo = voucher.FirstOrDefault().Code.ToString();
                        var registrationDet = await _sharedHelpers.GetFilterDynamicData<List<RegistrationDetailDTO>>("RegistrationDetail/dynamic", new Dictionary<string, string>() { { "Voucher", voucher.FirstOrDefault().Id.ToString() }, { "requiredFields", "Id,Room" } });
                        if (registrationDet?.Count > 0)
                        {
                            var roomDet = await _sharedHelpers.GetFilterDynamicData<List<RoomDetailDTO>>("RoomDetail/dynamic", new Dictionary<string, string>() { { "Id", registrationDet.FirstOrDefault().Room.ToString() }, { "requiredFields", "Id,Description" } });
                            if (roomDet?.Count > 0)
                                voucherPrint.RoomNo = roomDet.FirstOrDefault().Description;
                        }
                    }
                }
                refText = refText.Substring(0, refText.Length - 1);
            }
            if (refText != null && refText != "")
                RefAndExt.Add("References", refText);

            var extensionDefinition = GeneralBufferHolder.AllVoucherExtDefinitions.Where(x => x.Type == 1965 && x.VoucherDefinition == NonListDataSource.DefinitionId).ToList();
            if (extensionDefinition?.Count > 0)
            {
                var i = 0;
                try
                {
                    extensionDefinition.ForEach(x =>
                    {
                        RefAndExt.Add(x.Descritpion, getDescription(NonListDataSource, x.ExDataType, i));
                        i++;
                    });
                    if (rOrganization.Tin == "0000008573" && voucherPrint.voucherDefinition == 127)
                        RefAndExt.Add("Exchange_Rate", LineItemList?.FirstOrDefault()?.Size1?.ToString());
                }
                catch { }
            }
            voucherPrint.referencesAndExtensions = RefAndExt;
            voucherPrint.TransactionCurrency = currencyTrans?.FirstOrDefault();
            #endregion


            #region companyInformation
            if (companyName == "CompanyName" || string.IsNullOrWhiteSpace(companyName))
                voucherPrint.CompanyName = rOrganization.FirstName;
            if (companyName == "BranchName")
                voucherPrint.CompanyName = rdatasource.VoucherHeader.OriginConsigneeFullName ?? rOrganization.FirstName;

            voucherPrint.TINNo = rOrganization.Tin;
            var consIdentification = await _sharedHelpers.GetFilterDynamicResponseModel<List<IdentificationDTO>>("Identification/dynamic", new Dictionary<string, string>() { { "Type", "535" }, { "Consignee", rOrganization.Id.ToString() } });
            voucherPrint.VATNo = consIdentification?.FirstOrDefault()?.IdNumber;
            var _currentBranch = await _sharedHelpers.GetFilterDynamicResponseModel<List<ConsigneeUnitDTO>>("ConsigneeUnit/dynamic", new Dictionary<string, string>() { { "id", currentConsigneeUnit.ToString() } });

            if (_currentBranch == null) _currentBranch = new List<ConsigneeUnitDTO>();
            var currentBranch = _currentBranch.FirstOrDefault();
            int? country = null;
            int? subCity = null;
            int? city = null;
            string wereda = null;
            if (currentBranch != null)
            {
                if (currentBranch.Country != null) country = currentBranch.Country;
                if (currentBranch.Subcity != null) subCity = currentBranch.Subcity;
                if (currentBranch.HouseNumber != null) voucherPrint.CompanyHouseNo = currentBranch.HouseNumber;
                if (currentBranch.City != null) city = currentBranch.City;
                if (currentBranch.Phone1 != null) voucherPrint.CompanyTel = currentBranch.Phone1;
                if (currentBranch.Phone2 != null) voucherPrint.CompanyFax = currentBranch.Phone2;
                if (currentBranch.Website != null) voucherPrint.CompanyWeb = currentBranch.Website;
                if (currentBranch.PoBox != null) voucherPrint.CompanyPOBox = currentBranch.PoBox;
                if (currentBranch.Email != null) voucherPrint.CompanyEmail = currentBranch.Email;

            }

            string mtextToPrint = "";
            try
            {
                if (subCity != null) { mtextToPrint += subCity + ", "; }
                if (wereda != null)
                    mtextToPrint += "Wereda " + wereda + ", ";
                if (voucherPrint.CompanyHouseNo != null)
                    mtextToPrint += voucherPrint?.CompanyHouseNo;
            }
            catch
            {
            }
            voucherPrint.CompanyAddress = mtextToPrint;
            if (voucherPrint.PaperType != "Continuous")
                voucherPrint.logoPath = await GetLogoImage(rOrganization.Tin);
            #endregion

            return voucherPrint;
        }
        public async Task<VoucherPrintModel> EInvoiceLineItemVoucher(VoucherDetailDTO rdatasource)
        {
            #region Header
            VoucherPrintModel voucherPrint = new VoucherPrintModel();
            var NonListDataSource = rdatasource.VoucherHeader;
            voucherPrint.NonListDataSource = NonListDataSource;
            voucherPrint.voucherId = NonListDataSource.Id;
            voucherPrint.voucherDefinition = NonListDataSource.DefinitionId;
            var LineItemList = rdatasource.VoucherDetail;
            var activityList = rdatasource.ActivityDetail;

            Dictionary<string, List<LineItemConversionValues>> ConversionObj = new Dictionary<string, List<LineItemConversionValues>>();
            ArticleObjsPrint LineItemObj = new ArticleObjsPrint();
            List<ArticleObjsPrint> ListLineItemObj = new List<ArticleObjsPrint>();
            var configBuffer = await _sharedHelpers.GetFilterDynamicResponseModel<List<ConfigurationDTO>>("Configuration/dynamic", new Dictionary<string, string>() { { "reference", NonListDataSource.DefinitionId.ToString() } });
            #region print setting
            string attchmentUl = null;
            voucherPrint.attachmentPath = attchmentUl;
            ConsigneeDTO rOrganization = await _sharedHelpers.GetCompany();
            var voucherDefinition = NonListDataSource.Definition.ToString();
            voucherPrint.Type = "Template Type 1";
            List<ConfigurationDTO> value = configBuffer;
            string companyName = "CompanyName";
            foreach (var va in value)
            {
                switch (va.Attribute.ToString())
                {
                    case "Paper Type":
                        voucherPrint.PaperType = va.CurrentValue.ToString();
                        break;
                    case "Paper Size":
                        voucherPrint.PaperSize = va.CurrentValue.ToString();
                        break;
                }
            }
            #endregion
            #endregion

            var consIdentification = await _sharedHelpers.GetFilterDynamicResponseModel<List<IdentificationDTO>>("Identification/dynamic", new Dictionary<string, string>() { { "Type", "535" }, { "Consignee", rOrganization.Id.ToString() } });
            voucherPrint.VATNo = consIdentification?.FirstOrDefault()?.IdNumber;
            voucherPrint.logoPath = await GetLogoImage(rOrganization.Tin);
            voucherPrint.IsVoid = NonListDataSource.IsVoid;
            voucherPrint.enableEinvoice = true;
            return voucherPrint;
        }
        private  async Task<EinvoiceDTO> Get_EInvoice_By_VoucherId(int voucherId, int type)
        {
            var res =  await _sharedHelpers.GetFilterDynamicResponseModel<List<EinvoiceDTO>>("Einvoice/dynamic", new Dictionary<string, string>() { { "VoucherId", voucherId.ToString() },{ "Type", type.ToString() } });
            if (res != null && res.Count > 0)
                return res.FirstOrDefault();
            else
                return null;
        }
        private string? getDescription(VwVoucherHeaderDTO NonListDataSource, string type, int i)
        {
            string? data = "";
         
            if (i == 0)
                data = NonListDataSource.Extension1;
            else if (i == 1)
                data = NonListDataSource.Extension2;
            else if (i == 2)
                data = NonListDataSource.Extension3;
            else if (i == 3)
                data = NonListDataSource.Extension4;
            else if (i == 4)
                data = NonListDataSource.Extension5;
            else if (i == 5)
                data = NonListDataSource.Extension6;

            if (type == "Date Time")
                data = NonListDataSource.StartDate?.ToString("MM/dd/yyyy") + " To " + NonListDataSource.EndDate?.ToString("MM/dd/yyyy");
            return data;
        }
        public async Task<HeaderDTO> PrintNoneLineItemVoucher(VoucherDetailDTO rdatasource)
        {
            #region Header
            HeaderDTO voucherPrint = new HeaderDTO();
            var NonListDataSource = rdatasource.VoucherHeader;
            var LineItemList = rdatasource.VoucherDetail;
            var activityList = rdatasource.ActivityDetail;
            Dictionary<string, List<LineItemConversionValues>> ConversionObj = new Dictionary<string, List<LineItemConversionValues>>();
            ArticleObjsPrint LineItemObj = new ArticleObjsPrint();
            List<ArticleObjsPrint> ListLineItemObj = new List<ArticleObjsPrint>();
            var configBuffer = await _sharedHelpers.GetFilterDynamicData<List<ConfigurationDTO>>("Configuration/filter", new Dictionary<string, string>() { { "reference", NonListDataSource.DefinitionId.ToString() } });
            var taxTransBuffer = await _sharedHelpers.GetFilterData<List<TaxTransactionDTO>>("TaxTransaction", new Dictionary<string, string>() { { "voucher", NonListDataSource.Id.ToString() } });
            var rDistribution = await _sharedHelpers.GetFilterDynamicData<List<DistributionDTO>>("Distribution/filter", new Dictionary<string, string>() { { "SystemConstant", NonListDataSource.DefinitionId.ToString() }, { "type", "1578" } });
            var systemConstantBuffer = GeneralBufferHolder.SystemConstants;
            voucherPrint.JournalDetObj = rdatasource.JournalDetailRefering;
            #endregion

            #region print setting
            var voucherDefinition = NonListDataSource.Definition.ToString();
            ConsigneeDTO rOrganization = await _sharedHelpers.GetCompany();
            List<ConfigurationDTO> value = configBuffer;
            string DateFormat = "";
            bool EnablePaymentOptions = false;
            bool PrintAncestorReference = false;
            string companyName = "CompanyName";
            var enInvoice = true;
            foreach (var va in value)
            {
                switch (va.Attribute.ToString())
                {
                    case "Date Format":
                        DateFormat = va.CurrentValue.ToString();
                        break;
                    case "Print Journal":
                        if (NonListDataSource.DefinitionId == 203)
                        {
                            voucherPrint.PrintJournal = true;
                        }
                        else
                        {
                            voucherPrint.PrintJournal = bool.Parse(va.CurrentValue ?? "false");
                        }
                        break;
                    case "Enable Payment Option":
                        EnablePaymentOptions = bool.Parse(va.CurrentValue ?? "false");
                        break;
                    case "Print Ancestor Reference":
                        PrintAncestorReference = bool.Parse(va.CurrentValue ?? "false");
                        break;
                    case "Print Water mark":
                        voucherPrint.waterMark = va.CurrentValue.ToString();
                        break;
                    case "Paper Type":
                        voucherPrint.paperType = va.CurrentValue.ToString();
                        break;
                    case "Paper Size":
                        voucherPrint.PaperSize = va.CurrentValue.ToString();
                        break;
                    case "Print Copy Distribution":
                        voucherPrint.PrintCopyDistribution = bool.Parse(va.CurrentValue ?? "false");
                        break; 
                    case "Show Net On Total":
                        voucherPrint.ShowNetOnTotal = bool.Parse(va.CurrentValue ?? "false");
                        break;
                    case "No Of Line Item Per Page":
                        voucherPrint.NoOfLineItemPerPage = int.Parse(va.CurrentValue ?? "0");
                        break;
                    case "Voucher Orientation":
                        voucherPrint.VoucherOrientation = va.CurrentValue;
                        break;
                    case "Voucher User Orientation":
                        voucherPrint.VoucherUserOrientation = va.CurrentValue.ToString();
                        break;
                    case "Print Consignee Code":
                        voucherPrint.PrintConsigneeCode = bool.Parse(va.CurrentValue);
                        break;
                    case "Top Margin":
                        voucherPrint.Top = string.IsNullOrEmpty(va.CurrentValue) ? 0 : Convert.ToInt16(va.CurrentValue);
                        break;
                    case "Left Margin":
                        voucherPrint.Left = string.IsNullOrEmpty(va.CurrentValue) ? 0 : Convert.ToInt16(va.CurrentValue);
                        break;
                    case "Right Margin":
                        voucherPrint.Right = string.IsNullOrEmpty(va.CurrentValue) ? 0 : Convert.ToInt16(va.CurrentValue);
                        break;
                    case "Bottom Margin":
                        voucherPrint.Bottom = string.IsNullOrEmpty(va.CurrentValue) ? 0 : Convert.ToInt16(va.CurrentValue);
                        break;
                    case "Document Header":
                        companyName = va.CurrentValue;
                        break;
                }
            }
            #endregion
            #region cprv
            if (NonListDataSource.DefinitionId == 211)
            {
                var cprv = await _sharedHelpers.GetFilterDynamicResponseModel<ResponseModel<List<PettyCashReplenishmentVoucherDTO>>>("TransactionLibrary/petty_cash_replenishment_voucher", new Dictionary<string, string>() { { "voucherId", NonListDataSource.Id.ToString() } });
                voucherPrint.PCPRGridList = cprv?.Data ?? new List<PettyCashReplenishmentVoucherDTO>();
            }
            #endregion
            #region voucher Information and Voucher values
            if (DateFormat.ToLower() == "longdate")
                voucherPrint.IssueDate = NonListDataSource.IssuedDate.ToString();
            else if (DateFormat.ToLower() == "mediumdate")
                voucherPrint.IssueDate = NonListDataSource.IssuedDate.ToString("dd-MMM-yyyy");
            else
                voucherPrint.IssueDate = NonListDataSource.IssuedDate.ToShortDateString();

            if (NonListDataSource.Consignee1Id != null)
            {
                if (voucherPrint.PrintConsigneeCode)
                    NonListDataSource.Consignee1FullName = NonListDataSource.Consignee1FullName + " (" + NonListDataSource.ConsigneeCode + ")";
                voucherPrint.consignee = NonListDataSource.Consignee1FullName;
                voucherPrint.consigneeTitle = NonListDataSource.Consignee1PrefDesc;
                if (NonListDataSource.DefinitionId == 197 | NonListDataSource.DefinitionId == 200)
                    voucherPrint.consigneeTitle = "Received From: ";
                else if (NonListDataSource.DefinitionId == 210
                    || NonListDataSource.DefinitionId == 196
                    || NonListDataSource.DefinitionId == CNETConstants.BANK_DEPOSIT_VOUCHER ||
                     NonListDataSource.DefinitionId == 205)
                    voucherPrint.consigneeTitle = "Pay To: ";
                else if (NonListDataSource.DefinitionId == 194)
                    voucherPrint.consigneeTitle = "Debit To: ";
                else if (NonListDataSource.DefinitionId == 344)
                    voucherPrint.consigneeTitle = "Credit To: ";
                else if (NonListDataSource.DefinitionId == 121)
                    voucherPrint.consigneeTitle = "Refund To: ";
                else
                    voucherPrint.consigneeTitle = "To: ";
            }
            else if(NonListDataSource.Consignee1Id == null && NonListDataSource.Article > 0)
            {
                var _art = await _sharedHelpers.GetArticleById(Convert.ToInt32(NonListDataSource.Article));
                voucherPrint.consignee = _art.Name;
            }


            var taxTransactionList = taxTransBuffer?.Where(x => x.Voucher == NonListDataSource.Id).ToList();

            decimal withholdingTax = 0;
            decimal IncomeTax = 0;
            decimal newAmt = NonListDataSource.GrandTotal;
            if (taxTransactionList != null)
            {
                withholdingTax = taxTransactionList.FirstOrDefault(x => x.Tax == 6)?.TaxAmount ?? 0;
                IncomeTax = taxTransactionList.FirstOrDefault(x => x.Tax == 5)?.TaxAmount ?? 0;

                //if (NonListDataSource.DefinitionId == 196 || NonListDataSource.DefinitionId == 197)
                //    newAmt = NonListDataSource.GrandTotal;
                //else
                //    newAmt = (NonListDataSource.GrandTotal - withholdingTax - IncomeTax);

            }
            NumberToEnglish numToEng = new NumberToEnglish();
            var currencyBuffer = new List<CurrencyDTO>();
            var currency = new CurrencyDTO();
            if (NonListDataSource.CurrencyDescription != null)
            {
                currency.Description = NonListDataSource.CurrencyDescription;
                currency.IsDefault = true;
            }
            else
            {
                currency.Description = "Birr";
                currency.IsDefault = true;
            }
            if (NonListDataSource.SubTotal == 0 && NonListDataSource.GrandTotal != 0)
                NonListDataSource.SubTotal = NonListDataSource.GrandTotal;
            currencyBuffer.Add(currency);
            voucherPrint.VoucherCode = NonListDataSource.Code;
            voucherPrint.VoucherId = NonListDataSource.Id;
            if (voucherPrint.ShowNetOnTotal)
            {
                voucherPrint.amount_in_word = numToEng.changeCurrencyToWords(string.Format("{0:n2}", NonListDataSource.GrandTotal), currencyBuffer);
                voucherPrint.grandTotal = decimal.Parse(string.Format("{0:n2}", newAmt));
                voucherPrint.subtotal = decimal.Parse(string.Format("{0:n2}", NonListDataSource.SubTotal));
            }
            else
            {
                voucherPrint.amount_in_word = numToEng.changeCurrencyToWords(string.Format("{0:n2}", NonListDataSource.SubTotal), currencyBuffer);
                voucherPrint.grandTotal = decimal.Parse(string.Format("{0:n2}", NonListDataSource.SubTotal));
                voucherPrint.subtotal = decimal.Parse(string.Format("{0:n2}", newAmt));
            }
           

            voucherPrint.withHoldingAmount = withholdingTax;
            voucherPrint.incomeAmount = IncomeTax;
            voucherPrint.vouchernote = NonListDataSource.Note;
            voucherPrint.remark = NonListDataSource.Remark;
            voucherPrint.voucherDefinition = NonListDataSource.DefinitionId;
            voucherPrint.voucherType = NonListDataSource.Definition;
            voucherPrint.sourceBank = NonListDataSource.SrcBankDesc;
            voucherPrint.destinationBank = NonListDataSource.DstBankDesc;
            voucherPrint.PaymentRef = NonListDataSource.PaymentRefNumber;
            if (EnablePaymentOptions)
            {
                if (NonListDataSource.PaymentMethod != null)
                {
                    var _text = NonListDataSource.SrcBankDesc != null ? NonListDataSource.SrcBankDesc + ", " : "";
                    _text += NonListDataSource.PaymentMethodDesc;
                    _text += NonListDataSource.PaymentRefNumber != null ? ": "+NonListDataSource.PaymentRefNumber + ", " : ""; ;
                    _text += NonListDataSource.PaymentIssueDate != null ? "Due: " + NonListDataSource.PaymentIssueDate : "";
                    voucherPrint.payment_method = _text;
                    //voucherPrint.payment_method = NonListDataSource.SrcBankDesc + ", " + NonListDataSource.PaymentMethodDesc + NonListDataSource.PaymentRefNumber + ", Due: " + NonListDataSource.PaymentIssueDate?.ToString("MM/dd/yyyy");
                }
                else
                    voucherPrint.payment_method = " ";

            }
            voucherPrint.isIssued = NonListDataSource.IsIssued;
            voucherPrint.isVoid = NonListDataSource.IsVoid;

            #endregion

            #region transaction References
            var refText = "";
            if (rdatasource.tranReferenced?.Count > 0)
            {
                foreach (var TrRef in rdatasource.tranReferenced)
                    refText += TrRef.Remark + ",";
            }
            List<TransactionReferenceDTO> refList = new List<TransactionReferenceDTO>();
            if (PrintAncestorReference)
            {
                if (rdatasource.tranReferenced != null && rdatasource.tranReferenced.Count != 0)
                {
                    foreach (TransactionReferenceDTO TrF in rdatasource.tranReferenced)
                    {
                        var AncesestorRef = await _sharedHelpers.GetFilterDynamicData<List<TransactionReferenceDTO>>("TransactionReference/filter", new Dictionary<string, string>() { { "Referring", TrF.Referenced.ToString() } });
                        if (AncesestorRef?.Count > 0)
                            refList.AddRange(AncesestorRef);
                    }
                }
            }
            if (refList.Count > 0)
            {
                foreach (var TrRef in refList)
                {
                    var voucher = await _sharedHelpers.GetFilterDynamicData<List<VoucherDTO>>("Voucher/dynamic", new Dictionary<string, string>() { { "id", TrRef.Referenced.ToString() }, { "requiredFields", "Code" } });
                    if (voucher?.Count > 0)
                        refText += voucher.FirstOrDefault().Code + ",";
                }
                try
                {
                    refText.Remove(refText.Length - 1);
                }
                catch { }
            }
            if (refText != "")
                refText = refText.Substring(0, refText.Length - 1);
            voucherPrint.RefNo = refText;
            #endregion

            #region operators
            string oppText = "";
            if (activityList != null && activityList.Count > 0)
            {
                List<string> dateList = new List<string>();
                List<bool> isManual = new List<bool>();
                List<string> opList = new List<string>();
                List<string> actList = new List<string>();
                foreach (var val in activityList)
                {
                    if (val.IsPrint == true)
                    {
                        isManual.Add(val.IsManual ?? false);
                        opList.Add(val.UserName ?? "");
                        string dt = val.ActivityDate != null ? val.ActivityDate?.ToString("dd-MM-yyyy") : "";
                        dateList.Add(dt);
                        actList.Add(val.ActivityDefDesc != null ? val.ActivityDefDesc : "");
                    }
                }

                voucherPrint.Voucheroperators = opList;
                voucherPrint.ActivityDate = dateList;
                voucherPrint.ActivityDefDesc = actList;
                voucherPrint.isManual = isManual;
                if (voucherPrint.ActivityDefDesc.Count > 0)
                {
                    var opereratorString = new List<string>();
                    var operarators = voucherPrint.Voucheroperators;
                    var activityDesc = voucherPrint.ActivityDefDesc;
                    var activityDate = voucherPrint.ActivityDate;
                    if (activityDesc?.Count > 0)
                    {
                        for (int z = 0; z < activityDesc.Count; z++)
                        {
                            try
                            {
                                oppText += voucherPrint.ActivityDefDesc[z];
                                oppText += " by  ";
                                oppText += voucherPrint.isManual[z] ? "  ________________  " : voucherPrint.Voucheroperators[z];
                                oppText += " on  " + activityDate[z]+" ";
                                oppText += voucherPrint.isManual[z] ? " __________" : "";
                                opereratorString.Add(oppText);
                            }
                            catch { }
                        }
                    }
                }
            }


            voucherPrint.VoucheroperatorsString = oppText;
            #endregion

            #region bank Deposit
            if (rdatasource.VoucherHeader.DefinitionId == CNETConstants.BANK_DEPOSIT_VOUCHER)
            {
                List<VwBankAccountDetailDTO> accountdetailByConsignee = await _sharedHelpers.GetFilterdReponseModel<List<VwBankAccountDetailDTO>>("VwBankAccountDetail/dynamic", new Dictionary<string, string>() { { "BankAccountId", rdatasource.VoucherHeader.DstBankAcctDetailId.ToString() } });//acctMap
                var accountdet = accountdetailByConsignee?.FirstOrDefault();
                if (accountdet != null)
                {
                    voucherPrint.accountNo = accountdet.AccountNo;
                    voucherPrint.bankDescription = accountdet.Description; //string.Format("{0} ({1})", accountdet.ConsigneeName, accountdet.PaymentProcessorUnitName); ;
                    voucherPrint.AccountTypeDescription = accountdet.TypeDescription;
                }
                var _denominations = await _sharedHelpers.GetReqAsync<List<DenominationDTO>>("Denomination/dynamic");
            }
            #endregion
            #region companyInformation
            if (companyName == "CompanyName" || string.IsNullOrWhiteSpace(companyName))
                voucherPrint.CompanyName = rOrganization.FirstName;
            if (companyName == "BranchName")
                voucherPrint.CompanyName = rdatasource.VoucherHeader.OriginConsigneeFullName ?? rOrganization.FirstName;

            voucherPrint.TINNo = rOrganization.Tin;
            var branches = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string>() { { "Consignee", rOrganization.Id.ToString() } });
            var consIdentification = await _sharedHelpers.GetReqAsync<List<IdentificationDTO>>("Identification/dynamic?Type=535&Consignee=" + rOrganization.Id);

            voucherPrint.VATNo = consIdentification?.FirstOrDefault()?.IdNumber;
            var currentBranch = branches?.FirstOrDefault(x => x.Id == currentConsigneeUnit);
            if (currentBranch == null) currentBranch = new ConsigneeUnitDTO();
            string mtextToPrint = "";
            if (currentBranch.Phone1 != null)
                mtextToPrint += "Tel " + currentBranch.Phone1 + ", ";
            if (currentBranch.PoBox != null)
                mtextToPrint += "P.O.Box " + currentBranch.PoBox + ", ";
            voucherPrint.CompanyAddress = mtextToPrint;
            if (currentBranch != null)
            {
                if (currentBranch.Phone1 != null) voucherPrint.CompanyTel = currentBranch.Phone1;
                if (currentBranch.Phone2 != null) voucherPrint.CompanyFax = currentBranch.Phone2;
                if (currentBranch.Website != null) voucherPrint.CompanyWeb = currentBranch.Website;
                if (currentBranch.PoBox != null) voucherPrint.CompanyPOBox = currentBranch.PoBox;
                if (currentBranch.Email != null) voucherPrint.CompanyEmail = currentBranch.Email;

            }
            #endregion
            #region copy and department
            var CopyDescription = new List<string>();
            var OrganizationDepartment = new List<string>();
            var DistrbutionPrinterList = new List<string>();
            if (rDistribution != null && voucherPrint.PrintCopyDistribution)
            {
                DistributionDTO distributionDTO = new DistributionDTO();
                foreach (DistributionDTO objdistribution in rDistribution)
                {
                    try
                    {
                        CopyDescription.Add(GeneralBufferHolder.AllLookups.FirstOrDefault(x => x.Id == objdistribution.Index).Description);
                    }
                    catch { }
                    DistrbutionPrinterList.Add(objdistribution.Remark);
                    OrganizationDepartment.Add(objdistribution.Destination != null ? branches.Where(x => x.Id == objdistribution.Destination).FirstOrDefault()?.Name : "");
                }
                if (CopyDescription != null)
                {
                    string coppyDstributionString = "";
                    string CopyDepartmentString = "";
                    var index = 0;
                    bool mOC = false;
                    bool mFC = false;
                    bool mSC = false;
                    bool mTC = false;
                    bool mFrC = false;
                    bool mFiC = false;
                    if (CopyDescription.Contains("Orignal copy"))
                    {
                        index = CopyDescription.IndexOf("Orignal copy");
                        coppyDstributionString += "Original Copy -" + OrganizationDepartment[index];
                    }
                    if (CopyDescription.Contains("1st copy"))
                    {
                        index = CopyDescription.IndexOf("1st copy");
                        if (mOC == true)
                            coppyDstributionString += " , 1st Copy - " + OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 1st copy -" + OrganizationDepartment[index];
                        mFC = true;
                    }
                    if (CopyDescription.Contains("2nd copy"))
                    {
                        index = CopyDescription.IndexOf("2nd copy");
                        if (mFC == true)
                            coppyDstributionString += " , 2nd Copy - " + OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 2nd Copy - " + OrganizationDepartment[index];
                        mSC = true;
                    }
                    if (CopyDescription.Contains("3rd copy"))
                    {
                        index = CopyDescription.IndexOf("3rd copy");
                        if (mOC == true | mFC == true | mSC == true)
                            coppyDstributionString += " , 3rd Copy - " + OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 3rd Copy - " + OrganizationDepartment[index];
                        mTC = true;
                    }
                    if (CopyDescription.Contains("4th copy"))
                    {
                        index = CopyDescription.IndexOf("4th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true)
                            coppyDstributionString += " , 4th Copy - " + OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 4th Copy - " + OrganizationDepartment[index];
                        mFrC = true;
                    }
                    if (CopyDescription.Contains("5th copy"))
                    {
                        index = CopyDescription.IndexOf("5th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true | mFrC == true)
                            coppyDstributionString += " , 5th Copy - " + OrganizationDepartment[index];
                        else
                            coppyDstributionString += "5th Copy - " + OrganizationDepartment[index];
                        mFiC = true;

                    }
                    if (CopyDescription.Contains("6th copy"))
                    {
                        index = CopyDescription.IndexOf("6th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true | mFrC == true | mFiC == true)
                            coppyDstributionString += "  , 6th Copy - " + OrganizationDepartment[index];
                        else
                            coppyDstributionString += " 6th Copy - " + OrganizationDepartment[index];
                    }
                    voucherPrint.CopyDistributionString = coppyDstributionString;
                }
            }
            #endregion
            if (voucherPrint.paperType != "Continuous")
                voucherPrint.logoPath = await GetLogoImage(rOrganization.Tin);
            return voucherPrint;
        }
        private async Task<byte[]?> GetLogoImage(string tin)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

            string ftpBasePath = $"{_ftpSettings.FtpFilePathIP}/{tin}/CompanyProfile/logo/";


            try
            {
                var listRequest = (FtpWebRequest)WebRequest.Create(ftpBasePath);
                listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                listRequest.Credentials = new NetworkCredential(userName, passWord);
                listRequest.Timeout = 5000; // 5 seconds

                List<string> imageFiles = new();

                using (var listResponse = (FtpWebResponse)await listRequest.GetResponseAsync())
                using (var stream = listResponse.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string ext = Path.GetExtension(line).ToLowerInvariant();
                        if (imageExtensions.Contains(ext))
                            imageFiles.Add(line);
                    }
                }

                if (imageFiles.Count == 0)
                    return null; 

                string logoFile = imageFiles.LastOrDefault();
                string logoUrl = $"{ftpBasePath}{logoFile}";

                var downloadRequest = (FtpWebRequest)WebRequest.Create(logoUrl);
                downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                downloadRequest.Credentials = new NetworkCredential(userName, passWord);
                downloadRequest.Timeout = 5000;

                using var response = (FtpWebResponse)await downloadRequest.GetResponseAsync();
                using var responseStream = response.GetResponseStream();
                using var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            catch 
            {
                return null;
            }
        }


        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if (number % 10 > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }
            return words;
        }
    }
}

