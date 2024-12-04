namespace CoffeeMachineAPI.Data
{
    // Masina olekute staatuste määratlus
    public static class MachineStatuses
    {
        public static readonly string Idle = "Idle"; // Masin on sisse lülitatud, kuid ei ole kasutuses
        public static readonly string Brewing = "Brewing"; // Masin on praegu kohvi valmistamas
        public static readonly string OutOfWater = "Out Of Water"; // Veepaak on tühi ja vajab täitmist
        public static readonly string OutOfBeans = "Out Of Beans"; // Kohviubade paak on tühi ja vajab täitmist
        public static readonly string NeedsCleaning = "Needs Cleaning"; // Masin vajab puhastamist
        public static readonly string Error = "Error"; // Masinal esines viga või rike
        public static readonly string Maintenance = "Maintenance"; // Masin on hoolduses
        public static readonly string Off = "Off"; // Masin on välja lülitatud
        public static readonly string HeatingUp = "Heating Up"; // Masin soojeneb enne kasutamist
        public static readonly string Descaling = "Descaling"; // Masin teeb katlakivi eemaldamise protsessi
        public static readonly string ReadyToServe = "Ready To Serve"; // Masin on täielikult valmis teenindamiseks
    }

    // Makseviiside määratlus
    public static class PaymentType
    {
        public static readonly string Cash = "Cash"; // Sularaha
        public static readonly string Card = "Card"; // Kaart
        public static readonly string Transfer = "Transfer"; // Ülekanne
    }

    // Tellimuse staatuste määratlus
    public static class OrderStatuses
    {
        public const string Pending = "Pending"; // Ootel
        public const string Processing = "Processing"; // Töötlemisel
        public const string InPayment = "In the payment process"; // Maksmise protsessis
        public const string Completed = "Completed"; // Täidetud
        public const string Cancelled = "Cancelled"; // Tühistatud
        public const string Failed = "Failed"; // Ebaõnnestunud
        public const string OnHold = "OnHold"; // Ootel (peatuses)
        public const string Refunded = "Refunded"; // Tagasimakstud
        public const string PaymentError = "Payment error"; // Maksetehingu viga
    }

    // Makse olekute määratlus
    public static class PaymentStatuses
    {
        public const string Pending = "Pending"; // Ootel
        public const string Completed = "Completed"; // Täidetud
        public const string Failed = "Failed"; // Ebaõnnestunud
        public const string Cancelled = "Cancelled"; // Tühistatud
        public const string Refunded = "Refunded"; // Tagasimakstud
        public const string Authorized = "Authorized"; // Autentitud
        public const string PartiallyRefunded = "PartiallyRefunded"; // Osaliselt tagasimakstud
    }
}