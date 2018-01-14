namespace Empiria.Land.Registration.Transactions {

  public interface IFiling {

    string UID {
      get;
    }

    void SetPaymentOrder(IPaymentOrder paymentOrder);

    IPaymentOrder TryGetPaymentOrder();

  }  // interface IFiling

}  // namespace Empiria.Land.Registration.Transactions
