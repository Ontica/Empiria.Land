using System.Data;
using System.Web.Services;
using Empiria.Land.Registration.Data;
using Empiria.Services;

namespace Empiria.Land.WebServices {

  [WebService(Namespace = "http://empiria.ontica.org/web.services/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class PaymentServices : EmpiriaWebService {

    public PaymentServices() {
      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public DataSet GetTransaction(string transactionKey) {
      return TransactionData.GetLRSTransactionWithKey(transactionKey);
    }

  } //class DataIntegrationServices

} // namespace Empiria.WebServices
