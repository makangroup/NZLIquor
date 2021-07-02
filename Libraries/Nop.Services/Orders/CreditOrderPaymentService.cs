using Nop.Core.Domain.Orders;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Orders
{
    public class CreditOrderPaymentService : ICreditOrderPaymentService
    {
        public readonly IRepository<CreditOrderPayment> _creditOrderPaymentRepository;

        public CreditOrderPaymentService(IRepository<CreditOrderPayment> creditOrderPayments)
        {
            _creditOrderPaymentRepository = creditOrderPayments;
        }

        public void InsertCreditOrderPayment(CreditOrderPayment creditOrderPayment)
        {
            _creditOrderPaymentRepository.Insert(creditOrderPayment);
        }

        public CreditOrderPayment GetCreditOrderPaymentById(int id)
        {
            return _creditOrderPaymentRepository.GetById(id);
        }

        public void Update(CreditOrderPayment creditOrderPayment)
        {
            _creditOrderPaymentRepository.Update(creditOrderPayment);
        }
    }
}
