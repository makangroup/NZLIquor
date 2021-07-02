using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Windcave;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Windcave;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Test.Windcave
{
    [TestClass]
    public class PaymentServiceTest
    {
        private ISettingService _settingService;
        private INopFileProvider _fileProvider;

        private Setting _pxPayUserId;
        private Setting _webServiceUrl;
        private Setting _pxPayKey;

        [TestInitialize]
        public void SetUp()
        {
            _pxPayUserId = new Setting { Name = "Payment.PxPayUserId", Value = "FenchurchLiquorREST_Dev" };
            _pxPayKey = new Setting { Name = "Payment.PxPayKey", Value = "a0155a53c4b40174500df6b893fd4bcdc34dd08847f555919312dbaa55148be2" };
            _webServiceUrl = new Setting { Name = "Payment.ServiceUrl", Value = "https://sec.windcave.com/pxaccess/pxpay.aspx" };

            var settingMock = new Mock<ISettingService>();
            settingMock.Setup(obj => obj.GetSetting("Payment.PxPayUserId", 0, false)).Returns(_pxPayUserId);
            settingMock.Setup(obj => obj.GetSetting("Payment.PxPayKey", 0, false)).Returns(_pxPayKey);
            settingMock.Setup(obj => obj.GetSetting("Payment.ServiceUrl", 0, false)).Returns(_webServiceUrl); 
             _settingService = settingMock.Object;

            var fileMock = new Mock<INopFileProvider>();
            fileMock.Setup(o => o.TempFileName()).Returns("3e7ee94a-0ffb-481d-8f3f-91a064fc6b30.txt");
            _fileProvider = fileMock.Object;
        }

        [TestMethod]
        public void GenerateRequest()
        {
            RequestInput input = new RequestInput();
            input.AmountInput = "1";
            input.CurrencyInput = "NZD";
            input.MerchantReference = "1234567890";
            input.TxnType = "Purchase";
            input.UrlFail = "http://localhost:15536/customer/WindCaveResponse";
            input.UrlSuccess = "http://localhost:15536/customer/WindCaveResponse";

            var paymentService = new WindcavePaymentService(_settingService, null);
            var output = paymentService.GenerateRequest(input);

            Assert.IsFalse(string.IsNullOrEmpty(output.Url));
        }

        [TestMethod]
        public void ProcessResponseTest()
        {
            string response = "<Response valid=\"1\"><AmountSettlement>82.09</AmountSettlement><TotalAmount></TotalAmount><AmountSurcharge></AmountSurcharge><AuthCode></AuthCode><CardName>Visa</CardName><CardNumber>499999........36</CardNumber><DateExpiry>0827</DateExpiry><DpsTxnRef>00000001243a9fd1</DpsTxnRef><SurchargeDpsTxnRef></SurchargeDpsTxnRef><Success>0</Success><ResponseText>DO NOT HONOUR</ResponseText><DpsBillingId></DpsBillingId><CardHolderName>RASIKA</CardHolderName><CurrencySettlement>NZD</CurrencySettlement><TxnData1></TxnData1><TxnData2></TxnData2><TxnData3></TxnData3><TxnType>Purchase</TxnType><CurrencyInput>NZD</CurrencyInput><MerchantReference>8</MerchantReference><ClientInfo>202.92.216.106</ClientInfo><TxnId></TxnId><EmailAddress></EmailAddress><BillingId></BillingId><TxnMac>037A8010</TxnMac><CardNumber2></CardNumber2><DateSettlement>20201209</DateSettlement><IssuerCountryId>0</IssuerCountryId><IssuerCountryCode></IssuerCountryCode><Cvc2ResultCode>U</Cvc2ResultCode><ReCo>05</ReCo><ProductSku></ProductSku><ShippingName></ShippingName><ShippingAddress></ShippingAddress><ShippingPostalCode></ShippingPostalCode><ShippingPhoneNumber></ShippingPhoneNumber><ShippingMethod></ShippingMethod><BillingName></BillingName><BillingPostalCode></BillingPostalCode><BillingAddress></BillingAddress><BillingPhoneNumber></BillingPhoneNumber><PhoneNumber></PhoneNumber><HomePhoneNumber></HomePhoneNumber><AccountInfo></AccountInfo><RiskScore>-1</RiskScore><RiskScoreText></RiskScoreText></Response>";
            var paymentService = new WindcavePaymentService(_settingService, _fileProvider);
            (ResponseOutput responseOutput, string fileName) = paymentService.ProcessResponse(response);
            Assert.Equals(responseOutput.MerchantReference, 5);
            Assert.Equals(fileName, "3e7ee94a-0ffb-481d-8f3f-91a064fc6b30.txt");
        }
    }
}
