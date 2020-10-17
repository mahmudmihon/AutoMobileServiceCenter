namespace ASC.Models.BaseTypes
{
    public static class Constants
    {
        public const string Equal = "eq";
        public const string NotEqual = "ne";
        public const string GreaterThan = "gt";
        public const string GreaterThanOrEqual = "ge";
        public const string LessThan = "lt";
        public const string LessThanOrEqual = "le";
    }

    public enum Roles
    {
        Admin, Engineer, User
    }

    public enum MasterKeys
    {
        VehicleName, VehicleType
    }

    public enum Status
    {
        New, Denied, Pending, Initiated, InProgress, PendingCustomerApproval, RequestForInformation, Completed
    }
}
