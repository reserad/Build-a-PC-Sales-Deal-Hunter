using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class UrlShortService
    {
        private string _APIKEY = "291f9a38a775855f8063e293e9d04b02"; //YOUR API KEY HERE
        private string _UID = "12001095"; //YOUR UID HERE

        public enum AdvertType { Banner, FullPage };
        public enum Domain { Adf_ly, q_gs };
        private string _AdvType;
        private string _Domain;
        private string _UrlBaseAddress;
        private string _RequestURL;

        private string GetAdvType(AdvertType adv)
        {
	        string tmpAdType;
	        if (adv == AdvertType.FullPage)
		        tmpAdType = "int";
	        else
		        tmpAdType = "banner";
	        return tmpAdType;
        }

        private string GetDomainName(Domain domain)
        {
	        switch (domain)
	        {
		        case Domain.Adf_ly:
			        return "adf.ly";
		        case Domain.q_gs:
			        return "q.gs";
		        default:
			        return "adf.ly";
	        }
        }

        public UrlShortService(string APPKEY, string UID, AdvertType AdvType, Domain DomainType)
        {
	        _UrlBaseAddress = "http://api.adf.ly/api.php";
	        _APIKEY = APPKEY;
	        _UID = UID;
	        _AdvType = GetAdvType(AdvType);
	        _Domain = GetDomainName(DomainType);
	        _RequestURL = string.Format("{0}?key={1}&uid={2}&advert_type={3}&domain={4}&url=", _UrlBaseAddress, _APIKEY, _UID, _AdvType, _Domain);
        }

        public UrlShortService(AdvertType AdvType)
        {
	        _UrlBaseAddress = "http://api.adf.ly/api.php";
	        _AdvType = GetAdvType(AdvType);
	        _Domain = GetDomainName(Domain.Adf_ly);
	        _RequestURL = string.Format("{0}?key={1}&uid={2}&advert_type={3}&domain={4}&url=", _UrlBaseAddress, _APIKEY, _UID, _AdvType, _Domain);
        }

        public UrlShortService()
        {
	        _UrlBaseAddress = "http://api.adf.ly/api.php";
	        _AdvType = GetAdvType(AdvertType.Banner);
	        _Domain = GetDomainName(Domain.Adf_ly);
	        _RequestURL = string.Format("{0}?key={1}&uid={2}&advert_type={3}&domain={4}&url=", _UrlBaseAddress, _APIKEY, _UID, _AdvType, _Domain);
        }

        public string GenerateShortUrl(string UrlName)
        {
	        string Result;
	        try
	        {
		        HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(_RequestURL + UrlName);
                httpReq.Proxy = null;
		        HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();

		        Result = new StreamReader(httpResp.GetResponseStream()).ReadToEnd().Trim();

		        if (Result.ToLower() == "error")
		        {
			        throw new Exception("ShortUrl Response Error");
		        }
	        }
	        catch (Exception err)
	        {
		        throw new Exception("ShortUrl Conversion Error: " + err.Message);
	        }

	        return Result;
        }
    }
}