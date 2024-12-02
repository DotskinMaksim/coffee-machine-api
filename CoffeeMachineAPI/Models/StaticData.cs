namespace coffee_machine_api.Models
{
    public static class MachineStatus
    {
        public static readonly string Idle = "Idle"; // Machine is on but not in use
        public static readonly string Brewing = "Brewing"; // Machine is currently brewing coffee
        public static readonly string OutOfWater = "Out Of Water"; // Water tank is empty and needs refilling
        public static readonly string OutOfBeans = "Out Of Beans"; // Coffee beans are empty and need refilling
        public static readonly string NeedsCleaning = "Needs Cleaning"; // Machine requires cleaning
        public static readonly string Error = "Error"; // Machine encountered an error or malfunction
        public static readonly string Maintenance = "Maintenance"; // Machine is under maintenance
        public static readonly string Off = "Off"; // Machine is turned off
        public static readonly string HeatingUp = "Heating Up"; // Machine is heating up before use
        public static readonly string Descaling = "Descaling"; // Machine is performing descaling process
        public static readonly string ReadyToServe = "Ready To Serve"; // Machine is fully ready to serve
    }
    public static class OrderStatus
    {
        public static readonly string Completed = "Completed"; 
        public static readonly string Cancelled  = "Cancelled"; 
        public static readonly string Failed  = "Failed";
    }

    public static class PaymentType
    {
        public static readonly string Cash = "Cash";
        public static readonly string Card = "Card";
        public static readonly string Transfer = "Transfer";
    }
    namespace coffee_machine_api.Models
    {
        public static class PaymentResult
        {
            public static readonly string Completed = "Completed";
            public static readonly string InsufficientFunds = "Insufficient Funds";
            public static readonly string Failed = "Failed";
            public static readonly string Cancelled = "Cancelled";
            public static readonly string Timeout = "Timeout";
            public static readonly string Unauthorized = "Unauthorized";
            public static readonly string CardExpired = "Card Expired";
            public static readonly string NetworkError = "Network Error";
            public static readonly string LimitExceeded = "Limit Exceeded";
            public static readonly string Refunded = "Refunded";
        }
    }
}