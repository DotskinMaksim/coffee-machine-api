using CoffeeMachineAPI.Models;

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

    public static class ClientDiscount
    {
        public static readonly decimal Value = 0.8m;
    }
    public static class VATRate
    {
        public static readonly decimal Value = 0.2m;
    }
    public static class BonusRate
    {
        public static readonly decimal Value = 0.05m;
    }
    public static class SugarScale
    {
        public static readonly int Min = 0;
        public static readonly int Max = 4;

    }
    public static class Cups
    {
        public static readonly List<CupSize> Sizes =new List<CupSize>
        {
            new CupSize { Id = 1, Name = "Small", Code = 'S', VolumeInMl = 250, Multiplier = 1.0m },
            new CupSize { Id = 2, Name = "Medium", Code = 'M', VolumeInMl = 350, Multiplier = 1.2m },
            new CupSize { Id = 3, Name = "Large", Code = 'L', VolumeInMl = 500, Multiplier = 1.5m }
        };

    }

    public static class SystemUserIds
    {
        public static readonly int AdminUser = 1;
        public static readonly int UnknownUser = 2;
        public static readonly int GuestUser = 3;
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