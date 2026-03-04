namespace SMART.ERP.Domain.Enums
{
    public enum CartStatus
    {
        Active = 0,
        ReceiptSubmitted = 1,
        PaymentLinkRequested = 2,
        PaymentLinkSent = 3,
        Verified = 4,
        Rejected = 5
    }
}
