namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Models
{
    public abstract class RLBaseStoredProcedurePresenter
    {
        public abstract string StoredProcedureName { get; }

        public abstract string MsSqlBody { get; }

        public abstract string MySqlBody { get; }
    }
}
