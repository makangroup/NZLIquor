using Nop.Core.Domain.Windcave;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Windcave
{
    public interface IWindcavePaymentService
    {
        /// <summary>
        /// This method is used to generate the RequestOutput using RequestInput
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public RequestOutput GenerateRequest(RequestInput input);

        /// <summary>
        /// This method is used to generate the RequestOutput using responce string from Windcave
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public (ResponseOutput, string) ProcessResponse(string result);
    }
}
