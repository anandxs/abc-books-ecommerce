namespace AbcBooks.Utilities;

public static class ProjectConstants
{
	public const string ADMIN = "Admin";
	public const string CUSTOMER = "Customer";

	public const string STATUS_PENDING = "Pending";
	public const string STATUS_APPROVED = "Approved";
	public const string STATUS_IN_PROCESS = "Processing";
	public const string STATUS_SHIPPED = "Shipped";
	public const string STATUS_DELIVERED = "Delivered";
	public const string STATUS_CANCELLED = "Cancelled";
	public const string STATUS_RETURN_REQUESTED = "Return Requested";
	public const string STATUS_RETURN_APPROVED = "Return Approved";
	public const string STATUS_RETURN_REJECTED = "Return Declined";
	public const string STATUS_RETURN_COMPLETE = "Returned";

	public const string PAYMENT_STATUS_PENDING = "Pending";
	public const string PAYMENT_STATUS_APPROVED = "Approved";
	public const string PAYMENT_STATUS_REJECTED = "Rejected";
	public const string PAYMENT_STATUS_CANCELLED = "Cancelled";
	public const string PAYMNET_STATUS_REFUND_INITIATED = "Refund Initiated";
	public const string PAYMNET_STATUS_REFUND_COMPLETE = "Refunded";

	public const string PAYMENT_METHOD_CASH_ON_DELIVERY = "CashOnDelivery";
	public const string PAYMENT_METHOD_ONLINE = "OnlinePayment";
	public const string PAYMENT_METHOD_WALLET = "WalletPayment";

	public const string SESSION_CART = "SessionShoppingCart";

	public const string TRANSFER_CREDIT = "Credit";
	public const string TRANSFER_DEBIT = "Debit";
}
