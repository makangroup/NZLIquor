namespace Nop.Core.Domain.Payments
{
    /// <summary>
    /// Represents a payment status enumeration
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Authorized
        /// </summary>
        Authorized = 20,

        /// <summary>
        /// Paid
        /// </summary>
        Paid = 30,

        /// <summary>
        /// Partially Refunded
        /// </summary>
        PartiallyRefunded = 35,

        /// <summary>
        /// Refunded
        /// </summary>
        Refunded = 40,

        /// <summary>
        /// Voided
        /// </summary>
        Voided = 50,

        /// <summary>
        /// The order is on credit
        /// </summary>
        Credit = 60
    }
}
