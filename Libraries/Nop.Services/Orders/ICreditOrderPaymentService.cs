using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Orders
{
    public interface ICreditOrderPaymentService
    {
        /// <summary>
        /// This method is used to insert CreditOrderPayment into the DB
        /// </summary>
        /// <param name="creditOrderPayment"></param>
        public void InsertCreditOrderPayment(CreditOrderPayment creditOrderPayment);

        /// <summary>
        /// This method is used to get CreditOrderPayment by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CreditOrderPayment GetCreditOrderPaymentById(int id);

        /// <summary>
        /// This method is used to update the existing CreditOrderPayment
        /// </summary>
        /// <param name="creditOrderPayment"></param>
        public void Update(CreditOrderPayment creditOrderPayment);
    }
}
