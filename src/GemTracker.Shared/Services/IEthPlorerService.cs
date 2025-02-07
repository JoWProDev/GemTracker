﻿using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services.Responses.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace GemTracker.Shared.Services
{
    public interface IEthPlorerService
    {
        Task<SingleServiceResponse<TopHolderList>> FetchTopHoldersAsync(string contractAddress, int numberOfHolders = 100);
        Task<SingleServiceResponse<TokenInfo>> FetchTokenInfoAsync(string contractAddress);
    }
    public class EthPlorerService : IEthPlorerService
    {
        private readonly string _baseUrl = "https://api.ethplorer.io/";
        private readonly string _apiKey;
        public EthPlorerService(
            string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<SingleServiceResponse<TokenInfo>> FetchTokenInfoAsync(string contractAddress)
        {
            var result = new SingleServiceResponse<TokenInfo>();
            var parameters = new Dictionary<string, object>();
            try
            {
                using var client = new WebClient();
                string httpApiResult = await client.DownloadStringTaskAsync(
                    ConstructRequest("getTokenInfo", contractAddress, parameters));

                result.ObjectResponse = JsonSerializer.Deserialize<TokenInfo>(httpApiResult);
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }

        public async Task<SingleServiceResponse<TopHolderList>> FetchTopHoldersAsync(string contractAddress, int numberOfHolders = 100)
        {
            var result = new SingleServiceResponse<TopHolderList>();
            var parameters = new Dictionary<string, object>()
            {
                {"limit", numberOfHolders }
            };

            try
            {
                using var client = new WebClient();
                string httpApiResult = await client.DownloadStringTaskAsync(
                    ConstructRequest("getTopTokenHolders", contractAddress, parameters));

                result.ObjectResponse = new TopHolderList
                {
                    Holders = JsonSerializer.Deserialize<TopHolderList>(httpApiResult).Holders
                };
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }
        private string ConstructRequest(string endPoint, string contractAddress, Dictionary<string, object> parameters)
        {
            parameters.Add("apiKey", _apiKey);
            string requestUrl = _baseUrl + endPoint + "/" + contractAddress + "?" + string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
            return requestUrl;
        }
    }
}