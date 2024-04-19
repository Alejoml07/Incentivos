using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class OrdenOP
    {

        public OrdenOP()
        {
            billingInformation = new BillingInformation();
            orderRecipient = new OrderRecipient();
            orderRecipient.address = new Address();
            orderRecipient.items = new List<Item>();

        }

        public string? additionalField5 { get; set; }
        public string? allowBackOrder { get; set; }
        public string? avscode { get; set; }
        public double? baseSubTotal { get; set; }
        public BillingInformation? billingInformation { get; set; }
        public string? cid_CVV2Response { get; set; }
        public string? comments { get; set; }
        public string? countryCode { get; set; }
        public double? discount { get; set; }
        public string? dniID { get; set; }
        public string? expirationDate { get; set; }
        public bool? freeShipping { get; set; }
        public double? giftCertificateAmount { get; set; }
        public string? giftWrapping { get; set; }
        public string? invoice { get; set; }
        public string? ipAddress { get; set; }
        public int? languageId { get; set; }
        public string? loguinUser { get; set; }
        public string? macAddress { get; set; }
        public int? memberId { get; set; }
        public string? message { get; set; }
        public string? orderDate { get; set; }
        public string? orderNumber { get; set; }
        public OrderRecipient? orderRecipient { get; set; }
        public string? preauthDate { get; set; }
        public string? preauthorization { get; set; }
        public string? promotionCode { get; set; }
        public string? shipComplete { get; set; }
        public string? shipping { get; set; }
        public string? status { get; set; }
        public double? subTotal { get; set; }
        public double? tax { get; set; }
        public string? tenderBank { get; set; }
        public string? tenderCode { get; set; }
        public string? tenderReference { get; set; }
        public double? total { get; set; }
        public string? transactionId { get; set; }
    }

    public class BillingInformation
    {
        public string? addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string? addressLine3 { get; set; }
        public string? city { get; set; }
        public string? colonia { get; set; }
        public string? companyName { get; set; }
        public string? country { get; set; }
        public string? emailAddress { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? middleInitial { get; set; }
        public string? municipioDelegacion { get; set; }
        public string? phoneNumber { get; set; }
        public string? stateProvince { get; set; }
        public string? zipCode { get; set; }
    }

    public class Address
    {
        public string? addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string? addressLine3 { get; set; }
        public string? city { get; set; }
        public string? colonia { get; set; }
        public string? companyName { get; set; }
        public string? country { get; set; }
        public string? emailAddress { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? middleInitial { get; set; }
        public string? municipioDelegacion { get; set; }
        public string? phoneNumber { get; set; }
        public string? stateProvince { get; set; }
        public string? zipCode { get; set; }
    }

    public class Item
    {
        public string? barCode { get; set; }
        public double? discount { get; set; }
        public string? giftCardExpirationDate { get; set; }
        public string? giftCardFromName { get; set; }
        public string? giftCardMessage { get; set; }
        public string? giftCardNumber { get; set; }
        public string? giftCardToEmailAddress { get; set; }
        public string? giftCardToName { get; set; }
        public int? giftCardVerification { get; set; }
        public int? giftQuantity { get; set; }
        public string? isGiftCard { get; set; }
        public string? isGiftWrap { get; set; }
        public string? isHardCopy { get; set; }
        public string? isOnSale { get; set; }
        public string? isTaxFree { get; set; }
        public string? itemName { get; set; }
        public double? itemPrice { get; set; }
        public double? price { get; set; }
        public int? quantity { get; set; }
        public double? salePrice { get; set; }
        public string? sku { get; set; }
    }

    public class OrderRecipient
    {
        public Address? address { get; set; }
        public double? baseSubTotal { get; set; }
        public double? discount { get; set; }
        public string? giftMessageText { get; set; }
        public int? giftWrapping { get; set; }
        public List<Item>? items { get; set; }
        public int? recipientId { get; set; }
        public string? shipping { get; set; }
        public string? shippingMethod { get; set; }
        public double? subTotal { get; set; }
        public double? tax { get; set; }
        public double? total { get; set; }
    }
}
