using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MomoService : IMomoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public MomoService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<MomoPaymentResponseDto?> CreatePaymentAsync(OrderDto order, decimal totalAmount)
        {
            var momoConfig = _config.GetSection("MomoAPI");
            var partnerCode = momoConfig["PartnerCode"];
            var accessKey = momoConfig["AccessKey"];
            var secretKey = momoConfig["SecretKey"];
            var requestType = momoConfig["RequestType"];
            var notifyUrl = momoConfig["NotifyUrl"];
            var returnUrl = momoConfig["ReturnUrl"];
            var momoApiUrl = momoConfig["MomoApiUrl"];

            var requestId = Guid.NewGuid().ToString();
            var orderId = order.Id.ToString();

            var rawHash =
                $"accessKey={accessKey}&amount={totalAmount:0}&extraData=&ipnUrl={notifyUrl}&orderId={orderId}&orderInfo=Thanh toán đơn hàng {orderId}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType={requestType}";

            var signature = SignSHA256(rawHash, secretKey);

            var body = new
            {
                partnerCode,
                accessKey,
                requestId,
                amount = totalAmount.ToString("0"),
                orderId,
                orderInfo = $"Thanh toán đơn hàng {orderId}",
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                requestType,
                extraData = "",
                signature,
                lang = "vi"
            };

            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(momoApiUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<MomoPaymentResponseDto>(
                responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return result;
        }

        public async Task<bool> ValidateSignatureAsync(MomoIPNResponseDto ipnDto)
        {
            var momoConfig = _config.GetSection("MomoAPI");
            var secretKey = momoConfig["SecretKey"];

            var rawData =
                $"amount={ipnDto.Amount}&extraData={ipnDto.ExtraData}&message={ipnDto.Message}&orderId={ipnDto.OrderId}&orderInfo={ipnDto.OrderInfo}&orderType={ipnDto.OrderType}&partnerCode={ipnDto.PartnerCode}&payType={ipnDto.PayType}&requestId={ipnDto.RequestId}&responseTime={ipnDto.ResponseTime}&resultCode={ipnDto.ResultCode}&transId={ipnDto.TransId}";

            var mySignature = SignSHA256(rawData, secretKey);
            return mySignature == ipnDto.Signature;
        }

        private string SignSHA256(string rawData, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(rawData);
            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
