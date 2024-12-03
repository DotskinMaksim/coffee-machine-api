namespace coffee_machine_api.Models
{
    public static class MachineStatuses
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



    public static class PaymentType
    {
        public static readonly string Cash = "Cash";
        public static readonly string Card = "Card";
        public static readonly string Transfer = "Transfer";
    }

    public static class OrderStatuses
    {
        public const string Pending = "Pending"; 
        public const string Processing = "Processing"; 
        public const string InPayment = "In the payment process"; 
        public const string Completed = "Completed"; 
        public const string Cancelled = "Cancelled"; 
        public const string Failed = "Failed"; 
        public const string OnHold = "OnHold"; 
        public const string Refunded = "Refunded"; 
        public const string PaymentError = "Payment error"; 

    }

    public static class PaymentStatuses
    {
        public const string Pending = "Pending";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string Cancelled = "Cancelled";
        public const string Refunded = "Refunded";
        public const string Authorized = "Authorized";
        public const string PartiallyRefunded = "PartiallyRefunded";
    }
}
