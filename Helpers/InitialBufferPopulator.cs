using CNET_V7_Domain.Domain.AccountingSchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using Newtonsoft.Json;

namespace HulubejeBooking.Helpers
{
    public class InitialBufferPopulator
    {
        private readonly HttpClient _httpClient;
        public InitialBufferPopulator(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EinvoiceClient");
        }
        public async Task<List<SystemConstantDTO>> GetAllSystemConstants()
        {
            var systemConstants = await GetDynamicData<List<SystemConstantDTO>>("SystemConstant");
            return systemConstants ?? new List<SystemConstantDTO>();
        }
        public async Task<List<TaxDTO>> GetAllTaxs()
        {
            var taxs = await GetDynamicData<List<TaxDTO>>("Tax");
            return taxs ?? new List<TaxDTO>();
        }
        public async Task<List<UserRoleMapperDTO>> GetAllUserRoleMapper()
        {
            var userRoleMapper = await GetDynamicData<List<UserRoleMapperDTO>>("UserRoleMapper");
            return userRoleMapper ?? new List<UserRoleMapperDTO>();
        }
        public async Task<List<DeviceDTO>> GetAllDevices()
        {
            var devices = await GetDynamicData<List<DeviceDTO>>("Device");
            return devices ?? new List<DeviceDTO>();
        }  
        public async Task<List<SubCountryDTO>> GetAllSubCountry()
        {
            var devices = await GetDynamicData<List<SubCountryDTO>>("SubCountry");
            return devices ?? new List<SubCountryDTO>();
        }
        public async Task<List<VoucherStoreMappingDTO>> GetAllMappedStores()
        {
            var devices = await GetDynamicData<List<VoucherStoreMappingDTO>>("VoucherStoreMapping");
            return devices ?? new List<VoucherStoreMappingDTO>();
        }
        public async Task<List<VwFixedAssetDepViewDTO>> GetAllFixedAsset()
        {
            var fixedAsset = await GetDynamicData<List<VwFixedAssetDepViewDTO>>("vwFixedAssetDepView");
            return fixedAsset ?? new List<VwFixedAssetDepViewDTO>();
        }
        public async Task<List<ConfigurationDTO>> GetAllConfigration()
        {
            var configs = await GetDynamicData<List<ConfigurationDTO>>("Configuration");
            return configs ?? new List<ConfigurationDTO>();
        }
        public async Task<List<ActivityDefinitionDTO>> GetAllActivityDefns()
        {
            var actDefns = await GetDynamicData<List<ActivityDefinitionDTO>>("ActivityDefinition");
            return actDefns ?? new List<ActivityDefinitionDTO>();
        }
        public async Task<List<LookupDTO>> GetLookUps()
        {
            var lookups = await GetDynamicData<List<LookupDTO>>("Lookup");
            return lookups ?? new List<LookupDTO>();
        }
        public async Task<List<ValueFactorDefinitionDTO>> GetAllValueFactorDefinitions()
        {
            var valueFactorDef = await GetDynamicData<List<ValueFactorDefinitionDTO>>("ValueFactorDefinition");
            return valueFactorDef ?? new List<ValueFactorDefinitionDTO>();
        }
        public async Task<List<PeriodDTO>> GetPeriod()
        {
            var periods = await GetDynamicData<List<PeriodDTO>>("Period");
            return periods ?? new List<PeriodDTO>();
        }
        public async Task<List<RelationalStateDTO>> GetAllRelationStates()
        {
            var relationstates = await GetDynamicData<List<RelationalStateDTO>>("RelationalState");
            return relationstates ?? new List<RelationalStateDTO>();
        }
        public async Task<List<VoucherExtensionDefinitionDTO>> GetAllVoucherExtDefns()
        {
            var relationstates = await GetDynamicData<List<VoucherExtensionDefinitionDTO>>("VoucherExtensionDefinition");
            return relationstates ?? new List<VoucherExtensionDefinitionDTO>();
        }
        public async Task<List<TermDefinitionDTO>> GetAllVoucherTermDefns()
        {
            var alltermdfn = await GetDynamicData<List<TermDefinitionDTO>>("TermDefinition");
            return alltermdfn ?? new List<TermDefinitionDTO>();
        }
        public async Task<List<TermDefinitionDTO>> GetAllTermDefns()
        {
            var consigneeunits = await GetDynamicData<List<TermDefinitionDTO>>("TermDefinition");
            return consigneeunits ?? new List<TermDefinitionDTO>();
        }
        public async Task<List<AccountMapDTO>> GetAllAccountMap()
        {
            var consigneeunits = await GetDynamicData<List<AccountMapDTO>>("AccountMap");
            return consigneeunits ?? new List<AccountMapDTO>();
        }
        public async Task<List<SerialDefinitionDTO>> GetAllSerialDefns()
        {
            var consigneeunits = await GetDynamicData<List<SerialDefinitionDTO>>("SerialDefinition");
            return consigneeunits ?? new List<SerialDefinitionDTO>();
        }
        public async Task<List<ViewFunctWithAccessMDTO>> GetFunctionWithAccessMatrix()
        {
            var consigneeunits = await GetDynamicData<List<ViewFunctWithAccessMDTO>>("viewFunctWithAccessM");
            return consigneeunits ?? new List<ViewFunctWithAccessMDTO>();
        }
        public async Task<List<RelationDTO>> GetAllRelationss()
        {
            var alltermdfn = await GetDynamicData<List<RelationDTO>>("Relation");
            return alltermdfn ?? new List<RelationDTO>();
        }
        public async Task<List<PreferenceDTO>> GetAllPreferences()
        {
            var relationstates = await GetDynamicData<List<PreferenceDTO>>("Preference");
            return relationstates ?? new List<PreferenceDTO>();
        }

        public async Task<List<ActivityDefinitionDTO>> GetAllActiviyDefinitions()
        {
            var relationstates = await GetDynamicData<List<ActivityDefinitionDTO>>("ActivityDefinition");
            return relationstates ?? new List<ActivityDefinitionDTO>();
        }
        public async Task<List<CurrencyDTO>> GetAllCurrencies()
        {
            var currency = await GetDynamicData<List<CurrencyDTO>>("Currency");
            return currency ?? new List<CurrencyDTO>();
        }
        public async Task<List<ConsigneeUnitDTO>> GetAllConsigneeUnits(int companyId)
        {
            string _filter = $"?consignee={companyId}";
            var consigneeunits = await GetDynamicData<List<ConsigneeUnitDTO>>("ConsigneeUnit", _filter);
            return consigneeunits ?? new List<ConsigneeUnitDTO>();

    
        }
        public async Task<List<LookupDTO>> GetAllLookup()
        {
            var consigneeunits = await GetDynamicData<List<LookupDTO>>("Lookup");
            return consigneeunits ?? new List<LookupDTO>();
        }

        public async Task<T> GetDynamicData<T>(string Database,string? addtionalfiter ="")
        {
            string uri = string.Format(Database + "/dynamic" + addtionalfiter);

            return await GetReqAsync<T>(uri);
        }

        private async Task<TResponse> GetReqAsync<TResponse>(string path)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<TResponse>(responseJson);
                return responseData;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                throw new Exception(responseJson);
            }
        }

    }
}
