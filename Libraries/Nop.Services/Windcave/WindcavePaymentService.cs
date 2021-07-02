using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Windcave;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Xml;

namespace Nop.Services.Windcave
{
    public class WindcavePaymentService : IWindcavePaymentService
    {
        #region Member Fields
        protected readonly ISettingService _settingService;
        private readonly INopFileProvider _nopFileProvider;

        private static Setting _webServiceUrl;
        private static Setting _pxPayUserId;
        private static Setting _pxPayKey;

        private const string FilePath = "windcave/logs";
        #endregion

        #region Properties
        private Setting PayUserId
        {
            get
            {
                if (_pxPayUserId != null)
                    return _pxPayUserId;

                _pxPayUserId = _settingService.GetSetting("Payment.PxPayUserId");
                return _pxPayUserId;
            }
        }

        public Setting PayKey
        {
            get 
            {
                if (_pxPayKey != null)
                    return _pxPayKey;

                _pxPayKey = _settingService.GetSetting("Payment.PxPayKey");
                return _pxPayKey;
            }
        }

        public Setting WebServiceUrl
        {
            get
            {
                if (_webServiceUrl != null)
                    return _webServiceUrl;
                _webServiceUrl = _settingService.GetSetting("Payment.ServiceUrl");
                return _webServiceUrl;
            }
        }
        #endregion

        #region Constructors
        public WindcavePaymentService(ISettingService settingService, 
                                      INopFileProvider nopFileProvider)
        {
            _settingService = settingService;
            _nopFileProvider = nopFileProvider;
        }
        #endregion

        public RequestOutput GenerateRequest(RequestInput input)
        {
            _pxPayUserId = _settingService.GetSetting("Payment.PxPayUserId");
            if (_pxPayUserId == null)
                throw new ArgumentException("Payment.PxPayUserId does not have value");

            _pxPayKey = _settingService.GetSetting("Payment.PxPayKey");
            if (_pxPayKey == null)
                throw new ArgumentException("Payment.PxPayKey does not have value");

            _webServiceUrl = _settingService.GetSetting("Payment.ServiceUrl");
            if (_webServiceUrl == null)
                throw new ArgumentException("Payment.ServiceUrl does not have value");


            RequestOutput result = new RequestOutput(SubmitXml(GenerateRequestXml(input)));
            return result;
        }

        public (ResponseOutput, string) ProcessResponse(string result)
        {
            //Save the rerpnse message on the disk
            var fileName = _nopFileProvider.TempFileName();
            var filePath = _nopFileProvider.MapPath(FilePath);
            var fullFilePath = _nopFileProvider.Combine(filePath, fileName);
            _nopFileProvider.CreateFile(fullFilePath);
            _nopFileProvider.WriteAllText(fullFilePath, result, Encoding.UTF8);
            var response = SubmitXml(ProcessResponseXml(result));
            _nopFileProvider.AppendAllText(fullFilePath, response, Encoding.UTF8);


            ResponseOutput myResult = new ResponseOutput(response);
            return (myResult, fileName);
        }

        /// <summary>
        /// Generates the XML required for a ProcessResponse call
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private string ProcessResponseXml(string result)
        {

            StringWriter sw = new StringWriter();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = true;

            using (XmlWriter writer = XmlWriter.Create(sw, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("ProcessResponse");
                writer.WriteElementString("PxPayUserId", PayUserId.Value);
                writer.WriteElementString("PxPayKey", PayKey.Value);
                writer.WriteElementString("Response", result);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }

            return sw.ToString();
        }

        private string SubmitXml(string InputXml)
        {
            const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
            const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
            ServicePointManager.SecurityProtocol = Tls12;
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(_webServiceUrl.Value);
            webReq.Method = "POST";

            byte[] reqBytes;

            reqBytes = System.Text.Encoding.UTF8.GetBytes(InputXml);
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.ContentLength = reqBytes.Length;
            webReq.Timeout = 5000;
            Stream requestStream = webReq.GetRequestStream();
            requestStream.Write(reqBytes, 0, reqBytes.Length);
            requestStream.Close();

            HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();
            using (StreamReader sr = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.ASCII))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Generates the XML required for a GenerateRequest call
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GenerateRequestXml(RequestInput input)
        {

            StringWriter sw = new StringWriter();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = true;

            using (XmlWriter writer = XmlWriter.Create(sw, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("GenerateRequest");
                writer.WriteElementString("PxPayUserId", PayUserId.Value);
                writer.WriteElementString("PxPayKey", PayKey.Value);

                PropertyInfo[] properties = input.GetType().GetProperties();

                foreach (PropertyInfo prop in properties)
                {
                    if (prop.CanWrite)
                    {
                        string val = (string)prop.GetValue(input, null);

                        if (val != null || val != string.Empty)
                        {

                            writer.WriteElementString(prop.Name, val);
                        }
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }

            return sw.ToString();
        }
    }
}
