using gamemanager.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace gamemanager.Code
{
    public class SteamPriceChecker
    {
        //public JArray SteamData { get; set; }

        /// <summary>
        /// This method gets the steam game listings
        /// </summary>
        /// <param name="name"></param>
        /// <returns>boolean indicating success</returns>
        //internal async Task<bool> GetSteamStoreData()
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            var httpResponse = await client.GetAsync("http://api.steampowered.com/ISteamApps/GetAppList/v0002/?format=json");
        //            string responseBody = await httpResponse.Content.ReadAsStringAsync();
        //            JObject steamReply = JObject.Parse(responseBody);
        //            SteamData = JArray.Parse(steamReply["applist"]["apps"].ToString());
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("There was a problem trying to download Steam store data", ex);
        //        }
        //    }            
        //}

        internal async Task<decimal> GetPrice(string appid)
        {   
            var gameDetails = await GetAppDetails(appid);

            if (gameDetails[appid]["data"]["price_overview"] == null)
            {
                return 0;
            }    
            else
            {
                if (gameDetails[appid]["data"]["price_overview"]["final"] == null)
                {
                    return 0;
                }
            }

            var priceInPence = gameDetails[appid]["data"]["price_overview"]["final"].ToString();
            decimal finalPrice = decimal.Parse(priceInPence);
            return finalPrice / 100;
        }

        private async Task<JObject> GetAppDetails(string appid)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var httpResponse = await client.GetAsync("https://store.steampowered.com/api/appdetails?appids=" + appid);
                    string responseBody = await httpResponse.Content.ReadAsStringAsync();
                    JObject steamReply = JObject.Parse(responseBody);
                    return steamReply;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
