namespace HulubejeBooking.Controllers.Payment
{
    public class OperationModeController
    {
        public enum OperationMode
        {
            Synchronous = 1,
            Asynchronous = 2,
            AuthorizationTransaction = 4,
            Transaction = 8,
            CustomerInitiated = 16,
            Redirected = 32,
            QrCode = 64
        }

        public static Models.PaymentModels.OperationMode FlagChecker(int operationMode)
        {
            bool isSynchronous = (operationMode & (int)OperationMode.Synchronous) != 0;
            bool isAsynchronous = (operationMode & (int)OperationMode.Asynchronous) != 0;

            var result = new Models.PaymentModels.OperationMode
            {
                Asynchronous = isAsynchronous,
                Synchronous = isSynchronous
            };

            return result;
        }
    }
}
