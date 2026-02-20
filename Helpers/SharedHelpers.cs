using CNET_ERP_V7.WebConstants;
using CNET_V7_Domain.Domain.AccountingSchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Misc.CommonTypes;
using Cnetv7BufferHolder;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace HulubejeBooking.Helpers
{
    public class SharedHelpers
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor; // [Added]
        private ConsigneeDTO? _organization;
        private UserDTO? _loggedInUser;
        public SharedHelpers(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }
        public virtual async Task<ConsigneeDTO?> GetCompany()
        {
            if (_organization != null)
                return _organization;

            var response = await _httpClient.GetAsync("Consignee/filter?gslType=1");
            if (!response.IsSuccessStatusCode)
                return null;

            var jconsignee = await response.Content.ReadAsStringAsync();
            var _consigneeDTO = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jconsignee);

            _organization = _consigneeDTO != null && _consigneeDTO.Count > 0 ? _consigneeDTO.FirstOrDefault() : null;

            return _organization;
        }

        public virtual async Task<List<ActivityViewDTO>> GetActivitiesByUserAndDateAsync(int userId, DateTime timestamp, bool isAdmin)
        {
            string formattedDate = timestamp.ToString();
            string url;
            if (isAdmin)
            {
                url = $"ActivityView/filter?timestamp={Uri.EscapeDataString(formattedDate)}";
            }
            else
            {
                url = $"ActivityView/filter?user={userId}&timestamp={Uri.EscapeDataString(formattedDate)}";
            }

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new List<ActivityViewDTO>();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<List<ActivityViewDTO>>(jsonResponse);

            return activities ?? new List<ActivityViewDTO>();
        }


        public async Task<TResponse> SendReqAsync<TRequest, TResponse>(string url, TRequest requestData)
        {
            try
            {
                string jsonRequestData = JsonConvert.SerializeObject(requestData);
                var requestContent = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Content = requestContent;

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        var contentStream = await response.Content.ReadAsStringAsync();
                        var jsonResponseData = JsonConvert.DeserializeObject<TResponse>(contentStream);
                        return jsonResponseData;

                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return default;
            }
        }

        public virtual async Task<UserDTO?> GetUserByUserName(string _userName)
        {
            if (_loggedInUser != null)
                return _loggedInUser;

            var response = await _httpClient.GetAsync("User/filter?userName=" + _userName);
            if (!response.IsSuccessStatusCode)
                return null;

            var juser = await response.Content.ReadAsStringAsync();
            var usernameUser = JsonConvert.DeserializeObject<List<UserDTO>>(juser);

            _loggedInUser = usernameUser != null && usernameUser.Count > 0 ? usernameUser.FirstOrDefault() : null;

            return _loggedInUser;
        }
          
        public async Task<UserRoleMapperDTO?> GetUserRoleM(int _userID)
        {
            var xrolemapper = new UserRoleMapperDTO();
            var response = await _httpClient.GetAsync("UserRoleMapper/filter?user=" + _userID);
            if (!response.IsSuccessStatusCode)
                return null;

            var juser = await response.Content.ReadAsStringAsync();
            var userrolemapper = JsonConvert.DeserializeObject<List<UserRoleMapperDTO>>(juser);

            xrolemapper = userrolemapper != null && userrolemapper.Count > 0 ? userrolemapper.FirstOrDefault() : null;

            return xrolemapper;
        }
        public virtual async Task<List<SystemConstantDTO>> GetSytemConstantBytype(string _type)
        {

            var response = await _httpClient.GetAsync("SystemConstant/filter?type=" + _type);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var sysconstDto_ = JsonConvert.DeserializeObject<List<SystemConstantDTO>>(jsysconstDto_);

            return sysconstDto_;
        }
        public virtual async Task<List<VoucherAccountDTO>> GetVoucherAccount()
        {

            var response = await _httpClient.GetAsync("VoucherAccount/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var artCdto = JsonConvert.DeserializeObject<List<VoucherAccountDTO>>(jsysconstDto_);

            return artCdto;
        }  
        public virtual async Task<List<ArticleUserDTO>> GetArticleUsers()
        {

            var response = await _httpClient.GetAsync("ArticleUser/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var artCdto = JsonConvert.DeserializeObject<List<ArticleUserDTO>>(jsysconstDto_);

            return artCdto;
        }
        public virtual async Task<List<RoleActivityDTO>> GetVoucherAccount(int role)
        {

            var response = await _httpClient.GetAsync("RoleActivity/dynamic?role=" + role);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var artCdto = JsonConvert.DeserializeObject<List<RoleActivityDTO>>(jsysconstDto_);

            return artCdto;
        }
        public async Task<decimal> GetRoleAccessLevel(int _roleid)
        {
            decimal accesslevel = 0;
            var response = await _httpClient.GetAsync("CommonLibrary/get_access_level?role=" + _roleid);
            if (!response.IsSuccessStatusCode)
                return 0;

            var juser = await response.Content.ReadAsStringAsync();
            accesslevel = Convert.ToDecimal(juser);

            return accesslevel;
        }

        public virtual async Task<List<ConsigneeUnitDTO>?> GetCompanyBranchsByTin(string? _tin)
        {
            var _CompanyBranchs = new List<ConsigneeUnitDTO>();

            var response = await _httpClient.GetAsync("SysInitialize?deviceName=web&tin=" + _tin + "&platform=0&isWeb=true");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysDto = await response.Content.ReadAsStringAsync();
            var sysDto_ = JsonConvert.DeserializeObject<ResponseModel<SystemInitDTO>>(jsysDto);

            _CompanyBranchs = sysDto_.Data != null ? sysDto_?.Data.branches?.ToList() : null;

            return _CompanyBranchs;
        }
        public virtual async Task<List<ConsigneeDTO>?> GetAllBanks()
        {
            var _Banks = new List<ConsigneeDTO>();

            var response = await _httpClient.GetAsync("Consignee/dynamic?gslType=38");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysDto = await response.Content.ReadAsStringAsync();
            _Banks = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jsysDto);

            return _Banks;
        }
        public virtual async Task<List<ConsigneeUnitDTO>?> GetAllBranches()
        {
            var _BankBranches = new List<ConsigneeUnitDTO>();

            var response = await _httpClient.GetAsync("ConsigneeUnit/dynamic?type=1719");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysDto = await response.Content.ReadAsStringAsync();
            _BankBranches = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(jsysDto);


            return _BankBranches;
        }

        public virtual async Task<SystemInitDTO> GetCompanyInfo(string? _tin)
        {
            var response = await _httpClient.GetAsync("SysInitialize?deviceName=web&tin=" + _tin + "&platform=0&isWeb=true");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysDto = await response.Content.ReadAsStringAsync();
            var sysDto_ = JsonConvert.DeserializeObject<ResponseModel<SystemInitDTO>>(jsysDto);
            return sysDto_ != null ? sysDto_.Data : new SystemInitDTO();
        }
        public virtual async Task<List<PeriodDTO>> GetAllPeriods()
        {
            var response = await _httpClient.GetAsync("Period/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jperiodDto = await response.Content.ReadAsStringAsync();
            var periodDto_ = JsonConvert.DeserializeObject<List<PeriodDTO>>(jperiodDto);

            return periodDto_;
        }
        public virtual async Task<SystemConstantDTO> GetSytemConstantByID(int _id)
        {
            var response = await _httpClient.GetAsync("SystemConstant/dynamic?Id=" + _id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var sysconstDto_ = JsonConvert.DeserializeObject<List<SystemConstantDTO>>(jsysconstDto_);

            return sysconstDto_?.FirstOrDefault();
        } 
        public virtual async Task<ConversionDefinitionDTO> GetArticleUnitConversion(int article)
        {
            var response = await _httpClient.GetAsync("ConversionDefinition/dynamic?Article=" + article);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var sysconstDto_ = JsonConvert.DeserializeObject<List<ConversionDefinitionDTO>>(jsysconstDto_);

            return sysconstDto_?.FirstOrDefault();
        }
        public virtual async Task<List<ConversionDefinitionDTO>> GetArticleUnitConversionList(int article)
        {
            var response = await _httpClient.GetAsync("ConversionDefinition/dynamic?Article=" + article);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var sysconstDto_ = JsonConvert.DeserializeObject<List<ConversionDefinitionDTO>>(jsysconstDto_);

            return sysconstDto_;
        }
        public virtual async Task<RouteAssignmentDTO> GetRouteSale(int _id)
        {
            var response = await _httpClient.GetAsync("RouteAssignment/dynamic?Id=" + _id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var sysconstDto_ = JsonConvert.DeserializeObject<List<RouteAssignmentDTO>>(jsysconstDto_);

            return sysconstDto_?.FirstOrDefault();
        }
        public async Task<VoucherDTO> GetVoucherById(int id)
        {
            var response = await _httpClient.GetAsync("Voucher/dynamic?Id=" + id);
            if (!response.IsSuccessStatusCode)
                return null;
            try
            {
                var jvoucherDto_ = await response.Content.ReadAsStringAsync();
                var voucherDto_ = JsonConvert.DeserializeObject<List<VoucherDTO>>(jvoucherDto_);

                return voucherDto_?.FirstOrDefault() ?? new VoucherDTO();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public virtual async Task<List<TaxTransactionDTO>> GetTaxTransaction(int voucherid)
        {
            var response = await _httpClient.GetAsync("TaxTransaction/filter?voucher=" + voucherid);
            if (!response.IsSuccessStatusCode)
                return null;


            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var SysCdto = JsonConvert.DeserializeObject<List<TaxTransactionDTO>>(jsysconstDto_);

            return SysCdto;
        }
        public async Task<VoucherDTO> GetVoucherByCode(string id)
        {
            var response = await _httpClient.GetAsync("Voucher/dynamic?Code=" + id);
            if (!response.IsSuccessStatusCode)
                return null;
            try
            {
                var jvoucherDto_ = await response.Content.ReadAsStringAsync();
                var voucherDto_ = JsonConvert.DeserializeObject<List<VoucherDTO>>(jvoucherDto_);

                return voucherDto_?.FirstOrDefault() ?? new VoucherDTO();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<List<JournalDetailDTO>> GetJournalDetailByVoucher(int id)
        {
            var response = await _httpClient.GetAsync("JournalDetail/dynamic?Voucher=" + id);
            if (!response.IsSuccessStatusCode)
                return null;
            try
            {
                var jvoucherDto_ = await response.Content.ReadAsStringAsync();
                var journalDetail_ = JsonConvert.DeserializeObject<List<JournalDetailDTO>>(jvoucherDto_);

                return journalDetail_ ?? new List<JournalDetailDTO>();
            }
            catch (Exception)
            {
                return null;
            }

        }  
        public async Task<List<VwJournalDetailLightDTO>> GetJournalDetailByReferencedId(int id)
        {
            var response = await _httpClient.GetAsync("VwJournalDetailLight/dynamic?ReferencedVoucherId=" + id);
            if (!response.IsSuccessStatusCode)
                return null;
            try
            {
                var jvoucherDto_ = await response.Content.ReadAsStringAsync();
                var journalDetail_ = JsonConvert.DeserializeObject<List<VwJournalDetailLightDTO>>(jvoucherDto_);

                return journalDetail_ ?? new List<VwJournalDetailLightDTO>();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<VoucherDTO> GetVoucherByPaymentRefNum(string checkNum)
        {
            var response = await _httpClient.GetAsync("Voucher/dynamic?PaymentRefNumber=" + checkNum + "&PaymentMethod=1751");
            if (!response.IsSuccessStatusCode)
                return null;
            try
            {
                var jvoucherDto_ = await response.Content.ReadAsStringAsync();
                var voucherDto_ = JsonConvert.DeserializeObject<List<VoucherDTO>>(jvoucherDto_);

                return voucherDto_?.FirstOrDefault() ?? new VoucherDTO();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<LineItemDTO> GetLineitemById(int id)
        {
            var response = await _httpClient.GetAsync("Lineitem/dynamic?Id=" + id);
            if (!response.IsSuccessStatusCode)
                return null;
            try
            {
                var jlineitemDto_ = await response.Content.ReadAsStringAsync();
                var lineitemDto_ = JsonConvert.DeserializeObject<List<LineItemDTO>>(jlineitemDto_);

                return lineitemDto_?.FirstOrDefault() ?? new LineItemDTO();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<List<TransactionReferenceDTO>> GetTransactionReferenceByRefering(int? referring)
        {
            if (referring == null)
                return new List<TransactionReferenceDTO>();
            var dictionaryValue = new Dictionary<string, string>
            {
                //["ReferencingVoucherDefn"] = vd.ToString(),
                ["Referring"] = referring.ToString(),
            };

            return await GetFilterData<List<TransactionReferenceDTO>>("TransactionReference", dictionaryValue);
        }
        public async Task<List<LineItemDTO>> GetLineitemByVoucher(int id)
        {
            var response = await _httpClient.GetAsync("Lineitem/dynamic?Voucher=" + id);
            if (!response.IsSuccessStatusCode)
                return null;
            try
            {
                var jlineitemDto_ = await response.Content.ReadAsStringAsync();
                var lineitemDto_ = JsonConvert.DeserializeObject<List<LineItemDTO>>(jlineitemDto_);

                return lineitemDto_ ?? new List<LineItemDTO>();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public virtual async Task<List<SystemConstantDTO>> GetAllSytemConstants()
        {
            var response = await _httpClient.GetAsync("SystemConstant/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var sysconstDto_ = JsonConvert.DeserializeObject<List<SystemConstantDTO>>(jsysconstDto_);

            return sysconstDto_;
        }
        public virtual async Task<List<TaxDTO>> GetAllTaxs()
        {
            var response = await _httpClient.GetAsync("Tax/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var taxconstDto_ = JsonConvert.DeserializeObject<List<TaxDTO>>(jsysconstDto_);

            return taxconstDto_;
        }
        public virtual async Task<List<IdsettingDTO>> GetAllIdSetting()
        {
            var response = await _httpClient.GetAsync("Idsetting/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var taxconstDto_ = JsonConvert.DeserializeObject<List<IdsettingDTO>>(jsysconstDto_);

            return taxconstDto_;
        }

        public virtual async Task<List<SystemConstantDTO>> GetSytemConstants()
        {

            var response = await _httpClient.GetAsync("SystemConstant");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var sysconstDto_ = JsonConvert.DeserializeObject<List<SystemConstantDTO>>(jsysconstDto_);

            return sysconstDto_;
        }
        


        public virtual async Task<List<FieldFormatDTO>?> DocumetntFieldFormat(int? _ref)
        {
            var _listofFieldFormat = new List<FieldFormatDTO>();

            var response = await _httpClient.GetAsync("FieldFormat/filter?reference=" + _ref + "&type=" + 1617);
            if (!response.IsSuccessStatusCode)
                return null;

            var jfiDto = await response.Content.ReadAsStringAsync();
            var fiDto = JsonConvert.DeserializeObject<List<FieldFormatDTO>>(jfiDto);

            _listofFieldFormat = fiDto.OrderBy(i => i.Index) != null ? fiDto?.ToList() : null;

            return _listofFieldFormat;
        }
        
        //public async Task<GlobalParamsModel> GetGlobalParams(UserDTO? userdto)
        //{
        //    if (userdto == null || string.IsNullOrEmpty(userdto.Remark))
        //    {
        //        return null;
        //    }

        //    string[] splitRem = userdto.Remark.Split('_');
        //    string _rpx = splitRem.Length >= 2 ? splitRem[1]?.Trim() : null;
        //    string _rpB = splitRem[0];
        //    var comp = await GetCompany();
        //    string endpoint = "SysInitialize/authenticate";
        //    string queryString = $"?userName={userdto.UserName}&password={HttpUtility.UrlEncode(Decrypt(_rpx))}&tin={comp.Tin}";
        //    string requestUrl = $"{endpoint}{queryString}";

        //    HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return null;
        //    }

        //    string jsonResponse = await response.Content.ReadAsStringAsync();
        //    var userValidation = JsonConvert.DeserializeObject<ResponseModel<LoginResponse>>(jsonResponse);

        //    var globalParams = new GlobalParamsModel
        //    {
        //        navigatorList = userValidation.Data?.navigatorList,
        //        personInfo = userValidation.Data?.personInfo
        //    };

        //    return globalParams;
        //}
        public virtual async Task<List<AccountDTO>> GetAllAccount()
        {
            var response = await _httpClient.GetAsync("Account/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<AccountDTO>>(jsysconstDto_);
            return accDTO;

        }
        public async Task<T> GetFilterdReponseModel<T>(string Database, Dictionary<string, string> keyValuePairs)
        {
            string Filtervalues = "";
            int totalcount = keyValuePairs.Count;
            int count = 0;
            foreach (var values in keyValuePairs)
            {
                if (count > 0)
                    Filtervalues += "&";

                Filtervalues += values.Key + "=" + values.Value;

                count++;
            }
            string uri = string.Format(Database + "/?{0}", Filtervalues);
            HttpResponseMessage response = await _httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<T>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<T>(responseJson);
                return responseData;
            }
        }
        public async Task<T> GetFilterdReponseModel<T>(string Database)
        {
            string uri = string.Format(Database);
            HttpResponseMessage response = await _httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<T>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<T>(responseJson);
                return responseData;
            }
        }
        public async Task<T> GetFilterDynamicData<T>(string Database, Dictionary<string, string> keyValuePairs)
        {
            string Filtervalues = "";
            int totalcount = keyValuePairs.Count;
            int count = 0;
            foreach (var values in keyValuePairs)
            {
                if (count > 0)
                    Filtervalues += "&";

                Filtervalues += values.Key + "=" + values.Value;

                count++;
            }
            string uri = string.Format(Database + "/?{0}", Filtervalues);

            return await GetReqAsync<T>(uri);
        }
        public async Task<T> GetFilterDynamicResponseModel<T>(string Database, Dictionary<string, string> keyValuePairs)
        {
            string Filtervalues = "";
            int totalcount = keyValuePairs.Count;
            int count = 0;
            foreach (var values in keyValuePairs)
            {
                if (count > 0)
                    Filtervalues += "&";

                Filtervalues += values.Key + "=" + values.Value;

                count++;
            }
            string uri = string.Format(Database + "/?{0}", Filtervalues);

            HttpResponseMessage response = await _httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<T>(responseJson);
                    return responseData;
                }
                catch { return default; }
            }
            return default;
        }
        public async Task<T> GetFilterData<T>(string Database, Dictionary<string, string> keyValuePairs)
        {
            string Filtervalues = "";
            int totalcount = keyValuePairs.Count;
            int count = 0;
            foreach (var values in keyValuePairs)
            {
                if (count > 0)
                    Filtervalues += "&";

                Filtervalues += values.Key + "=" + values.Value;

                count++;
            }
            string uri = string.Format(Database + "/filter?{0}", Filtervalues);

            return await GetReqAsync<T>(uri);
        }
        public async Task<T> GetDynamicData<T>(string Database, Dictionary<string, string> keyValuePairs)
        {
            string Filtervalues = "";
            int totalcount = keyValuePairs.Count;
            int count = 0;
            foreach (var values in keyValuePairs)
            {
                if (count > 0)
                    Filtervalues += "&";

                Filtervalues += values.Key + "=" + values.Value;

                count++;
            }
            string uri = string.Format(Database + "/dynamic/?{0}", Filtervalues);

            return await GetReqAsync<T>(uri);
        }
        public async Task<TResponse> GetReqAsync<TResponse>(string path)
        {
            var baseUrl = _httpContextAccessor.HttpContext?
                .Session
                .GetString("EInvoiceBaseUrl");

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new Exception("EInvoiceBaseUrl not found in session.");

            var fullUrl = $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";

            var response = await _httpClient.GetAsync(fullUrl);

            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(responseJson);

            return JsonConvert.DeserializeObject<TResponse>(responseJson)!;
        }

        public async Task<TResponse> GetReqAsyncx<TResponse>(string path)
        {

            HttpResponseMessage response = await _httpClient.GetAsync(path);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<TResponse>(responseJson);
            return responseData;

        }

        public async Task<TResponse> SendReqAsync<TRequest, TResponse>(string path, HttpMethod httpMethod, TRequest requestBody = default)
        {
            var request = new HttpRequestMessage(httpMethod, path);
            if (requestBody != null)
            {
                var json = JsonConvert.SerializeObject(requestBody);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<TResponse>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<TResponse>(responseJson);
                return responseData;
            }
        }
        public async Task<TResponse> SendReqAsyncQueyParm<TResponse>(string path, HttpMethod httpMethod, Dictionary<string, string> queryParams = null)
        {
            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams.Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));
                path += "?" + queryString;
            }

            var request = new HttpRequestMessage(httpMethod, path);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<TResponse>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<TResponse>(responseJson);
                return responseData;
            }
        }
        public async Task<TResponse> SendReqDataModelRespAsync<TRequest, TResponse>(string path, HttpMethod httpMethod, TRequest requestBody = default)
        {
            var request = new HttpRequestMessage(httpMethod, path);
            if (requestBody != null)
            {
                var json = JsonConvert.SerializeObject(requestBody);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            var response = await _httpClient.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<TResponse>(responseJson);
            return responseData;

        }

        #region create and update
        public async Task<BeginingTransactionDTO> createBeginningTransaction(BeginingTransactionDTO _begnnigTransaction)
        {
            var begTransaction = new BeginingTransactionDTO();
            var xretun = await SendReqAsync<BeginingTransactionDTO, BeginingTransactionDTO>("BeginingTransaction", HttpMethod.Post, _begnnigTransaction);
            if (xretun != null)
            {
                begTransaction = xretun;
            }
            return begTransaction;
        }
        public async Task<TaxTransactionDTO> createtaxtransaction(TaxTransactionDTO _taxtransaction)
        {
            var begTransaction = new TaxTransactionDTO();
            var xretun = await SendReqAsync<TaxTransactionDTO, TaxTransactionDTO>("TaxTransaction", HttpMethod.Post, _taxtransaction);
            if (xretun != null)
            {
                begTransaction = xretun;
            }
            return begTransaction;
        }
        public async Task<BeginingTransactionDTO> updateBeginningTransaction(BeginingTransactionDTO _begnnigTransaction)
        {
            var begTransaction = new BeginingTransactionDTO();
            var xretun = await SendReqAsync<BeginingTransactionDTO, BeginingTransactionDTO>("BeginingTransaction", HttpMethod.Put, _begnnigTransaction);
            if (xretun != null)
            {
                begTransaction = xretun;
            }
            return begTransaction;
        }
        public async Task<TaxTransactionDTO> updateTaxTransaction(TaxTransactionDTO _begnnigTransaction)
        {
            var begTransaction = new TaxTransactionDTO();
            var xretun = await SendReqAsync<TaxTransactionDTO, TaxTransactionDTO>("TaxTransaction", HttpMethod.Put, _begnnigTransaction);
            if (xretun != null)
            {
                begTransaction = xretun;
            }
            return begTransaction;
        }
        #endregion

        #region Consignee
        public async Task<BeginingTransactionDTO> BeginingTransactionByCode(int id)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "id", id.ToString() } };
                var result = await GetFilterData<BeginingTransactionDTO>("BeginingTransaction", Dictionaryvalue);
                return result;
            }
            catch
            {
                return new BeginingTransactionDTO();
            }

        }
        public async Task<ArticleDTO> GetArticleById(int id)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "id", id.ToString() } };
                var result = await GetFilterData<List<ArticleDTO>>("Article", Dictionaryvalue);
                return result?.FirstOrDefault() ?? new ArticleDTO();
            }
            catch
            {
                return new ArticleDTO();
            }

        }
        public async Task<ArticleDTO> GetArticleByLocalCode(string localcode)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "LocalCode", localcode } };
                var result = await GetFilterData<List<ArticleDTO>>("Article", Dictionaryvalue);
                return result?.FirstOrDefault() ?? new ArticleDTO();
            }
            catch
            {
                return new ArticleDTO();
            }

        }

        public async Task<List<LookupDTO>> GetLookupsbyDescriptionAndType(int id, string type)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "type", type }, { "id", id.ToString() } };
            var result = await GetFilterData<List<LookupDTO>>("Lookup", Dictionaryvalue);
            return result;
        }
        public async Task<List<ConsigneeDTO>> GetConsineesByGslTypeAndId(int id, int type)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "id", id.ToString() }, { "Gsltype", type.ToString() }, { "requiredfields", "" } };
            var consingeeByGlsType = await GetFilterDynamicData<List<ConsigneeDTO>>("Consignee", Dictionaryvalue);
            return consingeeByGlsType;
        }
        public async Task<VwConsigneeViewDTO> GetConsignee(int id)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "id", id.ToString() } };
                var consingeeByGlsType = await GetFilterDynamicData<List<VwConsigneeViewDTO>>("VwConsigneeView/filter", Dictionaryvalue);
                return consingeeByGlsType?.FirstOrDefault() ?? new VwConsigneeViewDTO();
            }
            catch { return new VwConsigneeViewDTO(); }
        }

        public virtual async Task<bool> SaveRelation(RelationDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Relation", obj);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public async Task<List<RelationDTO>> selectrelationByRefferingAndReferenceObject(int refereing, int referenced)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "ReferringObject", refereing.ToString() }, { "ReferencedObject", referenced.ToString() } };
            var consingeeByGlsType = await GetFilterData<List<RelationDTO>>("Relation", Dictionaryvalue);
            return consingeeByGlsType;
        }
        public virtual async Task<List<AttachmentDTO>> AttachmentByReference(int? refe)
        {
            var response = await _httpClient.GetAsync("Attachment/filter?reference=" + refe);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var idDendto = JsonConvert.DeserializeObject<List<AttachmentDTO>>(jsysconstDto_);
            return idDendto;

        }
        public virtual async Task<List<RelationDTO>> getRelationByreference(int? refereceObject)
        {
            string endpoint = "Relation/filter";
            string queryString = $"?referringObject={refereceObject}";
            string requestUrl = $"{endpoint}{queryString}";

            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var redto = JsonConvert.DeserializeObject<List<RelationDTO>>(jsysconstDto_);

            return redto;
        }
        public virtual async Task<List<RelationDTO>> GetRelationByreferencedObj(int? referecedObject)
        {
            string endpoint = "Relation/filter";
            string queryString = $"?ReferringObject={referecedObject}";
            string requestUrl = $"{endpoint}{queryString}";

            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var redto = JsonConvert.DeserializeObject<List<RelationDTO>>(jsysconstDto_);

            return redto;
        }
        public virtual async Task<List<GsltaxDTO>> GetGslTaxByrefernce(int? refe)
        {
            var response = await _httpClient.GetAsync("Gsltax/filter?reference=" + refe);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var SysCdto = JsonConvert.DeserializeObject<List<GsltaxDTO>?>(jsysconstDto_);

            return SysCdto;
        }
        public virtual async Task<List<ObjectStateDTO>> GetObjeectStateByrefe(int? refrence)
        {

            var response = await _httpClient.GetAsync("ObjectState/filter?reference=" + refrence);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var objDTO = JsonConvert.DeserializeObject<List<ObjectStateDTO>>(jsysconstDto_);

            return objDTO;
        }
        public virtual async Task<List<CountryDTO>> GetAllCountries()
        {
            var response = await _httpClient.GetAsync("Country/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var CountryDTO = JsonConvert.DeserializeObject<List<CountryDTO>>(jsysconstDto_);

            return CountryDTO;
        }
        public virtual async Task<List<ConsigneeDTO>> GetCustomer()
        {
            var response = await _httpClient.GetAsync("Consignee/dynamic?gslType=28");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consigneedto = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jsysconstDto_);

            return consigneedto;
        }
        public virtual async Task<List<ConsigneeDTO>> GetConsignee()
        {
            var response = await _httpClient.GetAsync("Consignee/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consigneedto = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jsysconstDto_);

            return consigneedto;
        }
        public virtual async Task<List<SubCountryDTO>> Getsubountries()
        {
            var response = await _httpClient.GetAsync("SubCountry/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var CountryDTO = JsonConvert.DeserializeObject<List<SubCountryDTO>>(jsysconstDto_);

            return CountryDTO;
        }
        public virtual async Task<List<VwAccountViewDTO>> GetVwAccountView_()
        {

            var response = await _httpClient.GetAsync("VwAccountView/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jaccountvie = await response.Content.ReadAsStringAsync();
            var accounviewdto = JsonConvert.DeserializeObject<List<VwAccountViewDTO>>(jaccountvie);

            return accounviewdto != null ? accounviewdto : new List<VwAccountViewDTO>();
            //return accountList;
        }
        public virtual async Task<List<ConsigneeDTO>> GetConsinee()
        {
            var response = await _httpClient.GetAsync("Consignee/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<List<ConsigneeUnitDTO>> GetConsigneeByconsignee(int? _id)
        {
            var response = await _httpClient.GetAsync("ConsigneeUnit/filter?consignee=" + _id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consDto = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(jsysconstDto_);
            return consDto;

        }
        public virtual async Task<List<ConsigneeUnitDTO>> GetConsigneeByconsigneeandTyope(int? _id, int? tyoe)
        {

            string endpoint = "ConsigneeUnit/filter";
            string queryString = $"?consignee={_id}&type={tyoe}";
            string requestUrl = $"{endpoint}{queryString}";
            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consDto = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(jsysconstDto_);
            return consDto;

        }
        public virtual async Task<List<SystemConstantDTO>> GetSystemConstantByCat(string category)
        {

            var response = await _httpClient.GetAsync("SystemConstant/filter?category=" + category);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var SysCdto = JsonConvert.DeserializeObject<List<SystemConstantDTO>>(jsysconstDto_);

            return SysCdto;
        }
        public virtual async Task<List<ConsigneeUnitDTO>> GetConsigneeUnitById(int? _id)
        {
            var response = await _httpClient.GetAsync("ConsigneeUnit/filter?id=" + _id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consDto = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(jsysconstDto_);
            return consDto;

        }
        public virtual async Task<List<ConsigneeUnitDTO>> GetConsigneeUnitByType(int? _id)
        {
            var response = await _httpClient.GetAsync("ConsigneeUnit/filter?Type=" + _id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consDto = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(jsysconstDto_);
            return consDto;

        }
        public virtual async Task<List<ConsigneeUnitDTO>> GetConsigneeUnitByConsignee(int? _id)
        {
            var response = await _httpClient.GetAsync("ConsigneeUnit/filter?Consignee=" + _id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consDto = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(jsysconstDto_);
            return consDto;

        }
        public virtual async Task<List<ConsigneeDTO>> GetConsigneeById(int? _id)
        {
            var response = await _httpClient.GetAsync("Consignee/filter?id=" + _id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consDto = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jsysconstDto_);
            return consDto;
        }
        public virtual async Task<List<ConsigneeDTO>> GetConsigneeByGslType(int? _id)
        {
            var response = await _httpClient.GetAsync("Consignee/filter?GslType=" + _id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consDto = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jsysconstDto_);
            return consDto;
        }
        public virtual async Task<List<ConsigneeDTO>> GetConsigneeByTin(string _id)
        {
            var response = await _httpClient.GetAsync("Consignee/filter?tin=" + _id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consDto = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jsysconstDto_);
            return consDto;
        }
        public virtual async Task<List<ConsigneeDTO>> GetOrganizationConsinee()
        {
            var response = await _httpClient.GetAsync("Consignee/filter?IsPerson=false&GslType=28");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var consDto = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jsysconstDto_);
            return consDto;
        }
        public virtual async Task<List<SystemConstantDTO>> GetsystemConstantById(int? id)
        {
            var response = await _httpClient.GetAsync("SystemConstant/filter?id=" + id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var SysCdto = JsonConvert.DeserializeObject<List<SystemConstantDTO>>(jsysconstDto_);

            return SysCdto;
        }
        public async Task<List<PreferenceDTO>> GetPreferenceByType(int id)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "reference", id.ToString() } };
            var ByGlsType = await GetFilterData<List<PreferenceDTO>>("Preference", Dictionaryvalue);
            return ByGlsType;
        }
        public async Task<List<SystemConstantDTO>> GetSystemConstantsCategory(string category)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "category", category } };
            var ByGlsType = await GetFilterData<List<SystemConstantDTO>>("SystemConstant", Dictionaryvalue);
            return ByGlsType;
        }
        public async Task<List<LookupDTO>> GetLookUpByType(string type)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "type", type } };
            var consingeeByGlsType = await GetFilterData<List<LookupDTO>>("Lookup", Dictionaryvalue);
            return consingeeByGlsType;
        }
        //public async Task<List<ObjectStateDefinitionDTO>> GetObjectStateDefinitionByType(string type)
        //{
        //    //ToType
        //    Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "type", type } };
        //    var consingeeByGlsType = await GetFilterData<List<ObjectStateDefinitionDTO>>("ObjectStateDefinition", Dictionaryvalue);
        //    return consingeeByGlsType;
        //}
        public virtual async Task<List<ConsigneeUnitDTO>> GetConsigneeunit()
        {
            var response = await _httpClient.GetAsync("ConsigneeUnit/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<List<LookupDTO>> GetLookup()
        {
            var response = await _httpClient.GetAsync("Lookup/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<LookupDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<List<ScheduleHeaderDTO>> GetScheduleHeaders()
        {
            var response = await _httpClient.GetAsync("ScheduleHeader/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<ScheduleHeaderDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<List<GslacctRequirementDTO>> GetGSLAcctRequirementBygslType(int gsltype)
        {
            var response = await _httpClient.GetAsync("GslacctRequirement/filter?gslTypeList=" + gsltype);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<GslacctRequirementDTO>>(jsysconstDto_);
            return accDTO;

        }
        public virtual async Task<List<VwAccountMapViewDTO>> getAccountView()
        {
            var response = await _httpClient.GetAsync("VwAccountMapView/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<VwAccountMapViewDTO>>(jsysconstDto_);
            return accDTO;

        }

        public virtual async Task<List<VwAccountViewDTO>> getAccount_View()
        {
            var response = await _httpClient.GetAsync("VwAccountView/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<VwAccountViewDTO>>(jsysconstDto_);
            return accDTO;

        }
        public virtual async Task<List<BankAccountDetailDTO>> GetBankAccountDetailBuffer()
        {
            var response = await _httpClient.GetAsync("BankAccountDetail/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<BankAccountDetailDTO>>(jsysconstDto_);
            return accDTO;

        }
        public virtual async Task<List<AccountMapDTO>> GetGSLAccReqforAccountMap(string prefcode)
        {
            var response = await _httpClient.GetAsync("Account/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<AccountMapDTO>>(jsysconstDto_);
            return accDTO;

        }
        public virtual async Task<ConsigneeBuffer> SaveConsigneeBuffer(ConsigneeBuffer confnDTO)
        {
            try
            {


                HttpResponseMessage response = new HttpResponseMessage();
                if (confnDTO?.consignee?.Id != 0)
                {
                    response = await _httpClient.PutAsJsonAsync(requestUri: "ConsigneeBuffer", value: confnDTO);
                }
                else
                {

                    response = await _httpClient.PostAsJsonAsync("ConsigneeBuffer", confnDTO);
                }
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<ConsigneeBuffer>(responseJson);
                var test = responseData;
                return responseData ?? new ConsigneeBuffer();


            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }
            return new ConsigneeBuffer();
        }
        public async Task<List<ActivityDefinitionDTO>> GetAllActivityDefns()
        {
            var response = await _httpClient.GetAsync("ActivityDefinition/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<ActivityDefinitionDTO>>(jsysconstDto_);
            return accDTO;
        }
        public async Task<ActivityDefinitionDTO?> CreateActivityDefinition(ActivityDefinitionDTO activityDto)
        {
            if (activityDto == null)
                throw new ArgumentNullException(nameof(activityDto));

            try
            {
                var response = await _httpClient.PostAsJsonAsync("ActivityDefinition", activityDto);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Failed to create activity definition: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ActivityDefinitionDTO>(content);
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP error in CreateActivityDefinition: {httpEx.Message}");
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                Debug.WriteLine($"Deserialization error in CreateActivityDefinition: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in CreateActivityDefinition: {ex.Message}");
            }

            return null;
        }

        public virtual async Task<List<ActivityDefinitionDTO>> GetActivityDefinitionByDesc(int desc)
        {

            var response = await _httpClient.GetAsync("ActivityDefinition/filter?description=" + desc.ToString());
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var _actCdto = JsonConvert.DeserializeObject<List<ActivityDefinitionDTO>>(jsysconstDto_);

            return _actCdto;
        }
        public virtual async Task<ActivityDefinitionDTO> GetActivityDefinitionByID(int desc)
        {

            var response = await _httpClient.GetAsync("ActivityDefinition/filter?id=" + desc.ToString());
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var _actCdto = JsonConvert.DeserializeObject<List<ActivityDefinitionDTO>>(jsysconstDto_);

            return _actCdto?.FirstOrDefault();
        }
        public virtual async Task<List<AccountMapDTO>> GetAccountMapById(int id)
        {
            var response = await _httpClient.GetAsync("AccountMap/filter?id=" + id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<AccountMapDTO>>(jsysconstDto_);
            return accDTO;

        }
        public virtual async Task<List<RouteAssignmentDTO>> GetRouteSaleBySpace(int id)
        {
            var response = await _httpClient.GetAsync("RouteAssignment/filter?space=" + id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<RouteAssignmentDTO>>(jsysconstDto_);
            return accDTO;

        }
        public virtual async Task<List<AccountDTO>> GetAccountById(int id)
        {
            var response = await _httpClient.GetAsync("Account/filter?id=" + id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<AccountDTO>>(jsysconstDto_);
            return accDTO;

        }
        public virtual async Task<List<ControlAccountDTO>> GetControlAccount()
        {
            var response = await _httpClient.GetAsync("ControlAccount/dynamic");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<ControlAccountDTO>>(jsysconstDto_);
            return accDTO;

        }
        public virtual async Task<List<ControlAccountDTO>> GetControlAccount(int id)
        {
            var response = await _httpClient.GetAsync("ControlAccount/filter?id=" + id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<ControlAccountDTO>>(jsysconstDto_);
            return accDTO;

        }
        public virtual async Task<List<VwAccountMapViewDTO>> GetAccountview(int id)
        {
            var response = await _httpClient.GetAsync("VwAccountMapView/filter?AccountId=" + id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var accDTO = JsonConvert.DeserializeObject<List<VwAccountMapViewDTO>>(jsysconstDto_);
            return accDTO;
        }
        public virtual async Task<List<ConsigneeDTO>> GetConsigneeByType(int type)
        {
            var response = await _httpClient.GetAsync("Consignee/filter?gslType=" + type);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<List<ConsigneeUnitDTO>> GetConsigneeByType_()
        {
            var response = await _httpClient.GetAsync("ConsigneeUnit/filter?type=1719&consignee=39763");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<List<AttachmentDTO>> sellectAttachmentByReference(int type)
        {
            var response = await _httpClient.GetAsync("Attachment/filter?reference=" + type);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<AttachmentDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<List<AttachmentDTO>> GetattachmentbyCode(int id)
        {
            var response = await _httpClient.GetAsync("Attachment/filter?id=" + id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<AttachmentDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<List<AttachmentDTO>> GetattachmentbyReference(int id)
        {
            var response = await _httpClient.GetAsync("Attachment/filter?Reference=" + id );
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<AttachmentDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<List<AttachmentDTO>> GetattachmentbyReferenceandType(int id)
        {
            var response = await _httpClient.GetAsync("Attachment/filter?Pointer=760&Reference=" + id);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var Taxto = JsonConvert.DeserializeObject<List<AttachmentDTO>>(jsysconstDto_);
            return Taxto;
        }
        public virtual async Task<AttachmentDTO?> CreateAttachment(AttachmentDTO attxh)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Attachment", attxh);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        #region Delete
        public virtual async Task<bool> DeleteCosigneeunit(int _idd)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("ConsigneeUnit/" + _idd);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public virtual async Task<int> Deleteaccountmap(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("AccountMap/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }


            return 0;
        }
        public virtual async Task<bool> DeleteLineitem(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("Lineitem/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }


            return true;
        }

        public virtual async Task<int> DeleteTransactionLimitById(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("TransactionLimit/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }


            return 0;
        }
        public virtual async Task<int> DeleteBankAccountDetail(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("BankAccountDetail/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }


            return 0;
        }
        public virtual async Task<int> DeleteRelationById(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("Relation/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }


            return 0;
        }
        public virtual async Task<int> DeleteSpaceByCode(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("Space/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response content: " + responseContent);
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }


            return 0;
        }
        public virtual async Task<int> DeleteRouteSaleBy(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("RouteAssignment/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }


            return 0;
        }
        public virtual async Task<int> DeleteBeginingBalance(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("BeginingBalance/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return 0;
        }
        public virtual async Task<int> DeleteBeginingTransaction(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("BeginingTransaction/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return 0;
        }
        public virtual async Task<bool> DeleteVoucher(int _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("Voucher/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public virtual async Task<bool> DeleteVoucherAccount(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("VoucherAccount/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public virtual async Task<int> DeleteAttachment(int? _id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("Attachment/" + _id);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }


            return 0;
        }

        #endregion
        #endregion

        #region Update
        public virtual async Task<RelationDTO?> Updaterelation(RelationDTO relaSate)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("Relation", relaSate);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public virtual async Task<RelationDTO?> Createrelation(RelationDTO relaSate)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Relation", relaSate);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public virtual async Task<RouteAssignmentDTO?> CreateRouteSale(RouteAssignmentDTO relaSate)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("RouteAssignment", relaSate);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RouteAssignmentDTO>();
                    return result;
                }
                else
                {
                    Console.WriteLine("Error Calling Web API");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        public virtual async Task<ConsigneeUnitDTO?> UpdateOrganizationunit(ConsigneeUnitDTO consignee)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("ConsigneeUnit", consignee);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public virtual async Task<RouteAssignmentDTO?> UpdateRouteSale(RouteAssignmentDTO consignee)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("RouteAssignment", consignee);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public virtual async Task<AccountMapDTO> Createaccountmap(AccountMapDTO accountMap)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("AccountMap", accountMap);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public virtual async Task<AccountMapDTO> Updateaccountmap(AccountMapDTO accountMap)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("AccountMap", accountMap);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        #endregion


        #region art

        #region Select

        public virtual async Task<List<ArticleDTO>> SelectArticle(string articleCode)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "LocalCode", articleCode } };
                var response = await GetFilterData<List<ArticleDTO>>("Article", Dictionaryvalue);
                return response;
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public virtual async Task<List<ValueFactorDTO>> SelectValueFactorByreference(string reference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "Reference", reference } };
                var response = await GetFilterData<List<ValueFactorDTO>>("ValueFactor", Dictionaryvalue);
                return response;
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public async Task<List<DiscountFactorDTO>> SelectDiscountFactoreByArticle(int? art)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("DiscountFactor/filter?Article=" + art);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<DiscountFactorDTO>>(responseJson);

                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }


        #endregion

        #region Create

        public virtual async Task<bool> CreateArticle(ArticleDTO _article)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Article", _article);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool> CreateGSLTax(GsltaxDTO gltaxdTO)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Gsltax", gltaxdTO);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool> CreateActivity(ActivityDTO activityDTO)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Activity", activityDTO);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<ActivityDTO?> CreateActivity_(ActivityDTO activityDTO)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Activity", activityDTO);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var createdActivity = System.Text.Json.JsonSerializer.Deserialize<ActivityDTO>(
                        responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return createdActivity;
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    Console.WriteLine($"Response: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }

            return null;
        }
    
        public virtual async Task<ValueChangeLogDTO?> CreatValueLog(ValueChangeLogDTO ValueChangeLogDTO)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("ValueChangeLog", ValueChangeLogDTO);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var createdActivity = System.Text.Json.JsonSerializer.Deserialize<ValueChangeLogDTO>(
                        responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return createdActivity;
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    Console.WriteLine($"Response: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }

            return null;
        }


        public virtual async Task<bool> CreateActivityWithIdReturn(ActivityDTO activityDTO)
        {
            try
            {
                var _vouResult = await SendReqAsync<ActivityDTO, ActivityDTO>("Activity", HttpMethod.Post, activityDTO);
                var response = await _httpClient.PostAsJsonAsync("Activity", activityDTO);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> CreateAccountMap(List<AccountMapDTO> accountM)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("AccountMap", accountM);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public virtual async Task<bool> CreateSchedule(ScheduleDTO _schedule)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Schedule", _schedule);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool> CreateOptionalCode(ArticleDTO _schedule)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Article", _schedule);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool> CreatSerialDefinition(SerialDefinitionDTO _schedule)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("SerialDefinition", _schedule);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool> CreateObjectState(ObjectStateDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("ObjectState", obj);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> CreatePriceValue(ValueDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Value", obj);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool> CreateValuefactor(ValueFactorDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("ValueFactorDefinition", obj);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> CreateDiscountFactor(DiscountFactorDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("DiscountFactor", obj);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> CreateStockBalance(BeginingBalanceDTO listOfBegining)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("BeginingBalance", listOfBegining);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> CreateStocklevel(StockLevelDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("StockLevel", obj);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public virtual async Task<bool?> CreateLocation(List<VwSpaceLocationDTO> locationlist)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("VwSpaceLocation", locationlist);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public virtual async Task<bool?> CreateSpecification1(SpecificationDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Specification", obj);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        public virtual async Task<bool?> CreateConversionDefinition(ConversionDefinitionDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("ConversionDefinition", obj);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        #endregion

        #region Update Article
        public virtual async Task<bool?> UpdateArticle(ArticleDTO obj)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("Article", obj);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> UpdateGslTax(GsltaxDTO obj)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("Gsltax", obj);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> SaveGslTax(GsltaxDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Gsltax", obj);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> UpdateValueFactor(ValueFactorDTO vf)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("ValueFactor", vf);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> SaveValueFactor(ValueFactorDTO obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("ValueFactor", obj);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> UpdateDiscountFactor(DiscountFactorDTO vf)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("DiscountFactor", vf);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> UpdateBeginning(BeginingBalanceDTO vf)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("BeginingBalance", vf);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> UpdateStockLevel(StockLevelDTO vf)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("StockLevel", vf);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> UpdateLocation(LocationDTO vf)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("Location", vf);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> UpdateSpecification(SpecificationDTO vf)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("Specification", vf);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> UpdateConversion(ConversionDefinitionDTO vf)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("ConversionDefinition", vf);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }



        #endregion

        public virtual async Task<List<ViewPriceTagDTO>> GetAllPriceTag()
        {

            var response = await _httpClient.GetAsync("VwArticlePrice");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var sysconstDto_ = JsonConvert.DeserializeObject<List<ViewPriceTagDTO>>(jsysconstDto_);

            return sysconstDto_;
        }
        public async Task<List<PreferenceDTO>> GetPrefrenceByRef()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Preference/dynamic?requiredFields=id,reference");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<PreferenceDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<PeriodDTO>> GetPeriod()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Period");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<PeriodDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        public async Task<List<ConsigneeUnitDTO>> GetAllOrg()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("ConsigneeUnit/dynamic?id=1719");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<DeviceDTO>> GetDevice()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Device");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<DeviceDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }


        public async Task<List<SystemConstantDTO>> GetUom()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("SystemConstant");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<SystemConstantDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        public async Task<List<StockLevelDTO>> GetStockLevel()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("StockLevel/dynamic");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<StockLevelDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        public async Task<List<StockLevelDTO>> GetStockLevel(int? id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("StockLevel/filter?id=" + id);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<StockLevelDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        public async Task<List<ConsigneeUnitDTO>> GetConsineeUnit()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("ConsigneeUnit/dynamic?");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<ConsigneeUnitDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<ConsigneeDTO>> GetAllPerson()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Consignee");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<PreferenceDTO>> SelectPreferenceByreference()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Preference/dynamic?requiredFields=");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<PreferenceDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        
        public async Task<List<ArticleDTO>> Getoptionalcode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Article/dynamic?");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<ArticleDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<ValueFactorDefinitionDTO>> GetValueFactorDefinition()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("ValueFactorDefinition/dynamic");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<ValueFactorDefinitionDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        } 
        public async Task<List<ValueFactorDefinitionDTO>> GetValueFactorDefinitionByType(int type)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("ValueFactorDefinition/dynamic?type=" + type.ToString());
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<ValueFactorDefinitionDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }



        public async Task<List<BeginingBalanceDTO>> GetBeginingBalance()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("BeginingBalance/dynamic?requiredFields=period,value,isProvisional,remark");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<BeginingBalanceDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<StockLevelDTO>> GetStockBalanceStore()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("StockLevel/dynamic?requiredFields=store,quantity");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<StockLevelDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<BeginingBalanceDTO>> GetStockBeginningBalance()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("BeginingBalance/dynamic?requiredFields=value &id=1");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<BeginingBalanceDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<StockLevelDTO>> GetStockLevelView()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("StockLevel/dynamic?requiredFields=minLevel,maxLevel,economicOrderQty,leadTimeDays,remark");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<StockLevelDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }


        public async Task<List<VoucherDTO>> getGslNote()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Voucher/dynamic?requiredFields=note");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<VoucherDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<VwSpaceLocationDTO>> GetSpaceLocationsView()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("VwSpaceLocation/dynamic?requiredFields=spaceDescription,storeDescription");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<VwSpaceLocationDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        public async Task<List<SpaceDTO>> GetAllSpaces()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Space/dynamic");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<SpaceDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<SpecificationDTO>> SelectSpecificationByArticle()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Specification/dynamic?requiredFields=value,attribute,type,remark");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<SpecificationDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<ConversionDefinitionDTO>> SelectConversionDefinitionByarticle()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("ConversionDefinition/dynamic?requiredFields=fromQty,toQty,remark,index");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<ConversionDefinitionDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

        public async Task<List<SerialDefinitionDTO>> GetSerialDefinition(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("SerialDefinition/dynamic?article=" + id);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<SerialDefinitionDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        public async Task<List<VwAccountMapViewDTO>> GetAccountMapViewDTOs()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("VwAccountMapView/dynamic?requiredFields=AccountMapdescription,consigneeUnitId,accountCode");
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<VwAccountMapViewDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        public async Task<List<VwArticlePriceDTO>> GetArticlePrice(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("VwArticlePrice/dynamic?article=" + id);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<VwArticlePriceDTO>>(responseJson);

                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        public async Task<List<LookupDTO>> GetLookUpDef(string category)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>() { { "type", category } };
            var ByGlsType1 = await GetFilterData<List<LookupDTO>>("LookUp", Dictionaryvalue);
            return ByGlsType1;
        }
        public virtual async Task<bool?> UpdateSerialDefinition(SerialDefinitionDTO vf)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("SerialDefinition", vf);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> NewSerialDefinition(SerialDefinitionDTO vf)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("SerialDefinition", vf);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<bool?> DeleteSerialdefinitionBycode(int Id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync("SerialDefinition/" + Id);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Error Calling Web Api");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        [HttpPost]
        public virtual async Task<bool> SaveArticle(ArticleDTO confnDTO)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Article", confnDTO);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public async Task<List<ValueFactorDTO>> GetValueFactor(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("ValueFactor/dynamic?&reference=" + id);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<ValueFactorDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        public async Task<List<ValueDTO>> GetArticleVaueByReference(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Value/dynamic?&article=" + id);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<ValueDTO>>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }
        #endregion

        #region  __Wubit
        public virtual async Task<List<BeginingBalanceDTO>> GetBeginingBalanceByrefernce(int? refe)
        {
            var response = await _httpClient.GetAsync("BeginingBalance/filter?reference=" + refe);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysconstDto_ = await response.Content.ReadAsStringAsync();
            var SysCdto = JsonConvert.DeserializeObject<List<BeginingBalanceDTO>?>(jsysconstDto_);

            return SysCdto;
        }
        public virtual async Task<bool> updateConsigneeUnit(ConsigneeUnitDTO confnDTO)
        {
            try
            {
                // confnDTO.Code = null;
                var response = await _httpClient.PutAsJsonAsync("ConsigneeUnit", confnDTO);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<ConsigneeUnitDTO>(responseJson);
                    var test = responseData;
                    return true;
                }

            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public virtual async Task<ConsigneeUnitDTO> CreateConsigneeUnit(ConsigneeUnitDTO confnDTO)
        {
            try
            {
                // confnDTO.Code = null;
                var response = await _httpClient.PostAsJsonAsync("ConsigneeUnit", confnDTO);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<ConsigneeUnitDTO>(responseJson);
                    return responseData;
                }

            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }
            return null;
        }


        #endregion

        #region Generate VouchercCode
        public async Task<string> CNETIdGenerater(string objType, int reference, int genType, int conunit)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();

                Dictionaryvalue.Add("objectType", objType.ToString());
                Dictionaryvalue.Add("reference", reference.ToString());
                Dictionaryvalue.Add("generationType", genType.ToString());
                Dictionaryvalue.Add("consigneeUnit", conunit.ToString());
                Dictionaryvalue.Add("isWeb", true.ToString());
                var generated = await GetFilterDynamicData<ResponseModel<string>>("IDGenerate", Dictionaryvalue);

                return generated?.Data ?? generated.Ex;

            }
            catch
            {
                return null;
            }
        }
        #endregion

        private string Decrypt(string encryptedText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encryptedText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return encryptedText;
        }
        public async Task<List<int>> Mistir_Articles(int authUser)
        {
            var notAllowedArticles = await GetArticleUsers();
            if (notAllowedArticles == null || notAllowedArticles.Count() == 0)
                return new List<int>();

            var formulationConfigs = GeneralBufferHolder.ConfigrationSettings
                .FirstOrDefault(u => u.Reference == CNET_WebConstantes.FORMULATION.ToString() &&
                                     u.Attribute.Equals("use only assigned article", StringComparison.OrdinalIgnoreCase));

            if (formulationConfigs?.CurrentValue == null)
                return new List<int>();

            if (!bool.TryParse(formulationConfigs.CurrentValue, out var useAssignedArticles) || !useAssignedArticles)
                return new List<int>();

            var roleM = await GetUserRoleM(authUser);

            return notAllowedArticles?
                .Where(u => u.Role != roleM?.Role /*&& u.Type == CNET_WebConstantes.SECRTE_RECIPE_TYPE*/)
                .Select(al => al.Article)
                .ToList() ?? new List<int>();
        }
        
        public virtual async Task<List<StockLedgerHeader>> GetStockLedgerofArticleByEnquiryDate(int? articleId,DateTime enqDate)
        {
            List<StockLedgerHeader> _ledgerDetailList = new List<StockLedgerHeader>();
            DateTime endDate = enqDate;
            Dictionary<string, string> stockLedgDic = new Dictionary<string, string>()
            {
                {"enquiryDate", endDate.ToString("MM/dd/yyyy")},
            };

            if (articleId != null)
            {
                stockLedgDic.Add("articleId", articleId.ToString());
            }
          
            try
            {
                ResponseModel<List<StockLedgerHeader>> _result = await GetFilterDynamicData<ResponseModel<List<StockLedgerHeader>>>("StockReport/stockLedgerDetail", stockLedgDic);
                _ledgerDetailList = _result.Data;
            }
            catch { }
            return _ledgerDetailList;
        }
        public async Task<bool> CheckProductCodesInArticleUser(string localcode, int authuser)
        {
            try
            {
                var secretItems = await Mistir_Articles(authuser);
                if (secretItems != null && secretItems.Count() > 0)
                {
                    var article = await GetArticleByLocalCode(localcode);
                    if (article != null && article.Id > 0)
                    {
                        if (secretItems.Contains(article.Id))
                        {
                            return true;
                        }
                    }
                }

                return false; // No secret article found
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error: {ex.Message}");
                return false; // Return false in case of an error
            }
        }
        public virtual async Task<UserDTO?> UpdateUser(UserUpdateDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            try
            {
                var response = await _httpClient.PostAsJsonAsync("CommonLibrary/update_user", userDto);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"UpdateUser failed: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseModel<UserDTO>>(content);

                return result?.Data;
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP error in UpdateUser: {httpEx.Message}");
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                Debug.WriteLine($"Deserialization error in UpdateUser: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in UpdateUser: {ex.Message}");
            }

            return null;
        }

    }

}
