using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GromaxMobileApis.Models.Services
{
    public class SparePartsModel
    {
        public class SparePartsOrderRequest : IValidatableObject
        {

            [Range(0, 1, ErrorMessage = "PaymentStatus must be either 0 or 1.")]
            public int PaymentStatus { get; set; }

            [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
            public string Remarks { get; set; }

            // Payment Details

            public decimal? PaymentAmount { get; set; }

            [StringLength(20, ErrorMessage = "PaymentMode cannot exceed 20 characters.")]
            public string PaymentMode { get; set; }

            [StringLength(100, ErrorMessage = "TransactionId cannot exceed 100 characters.")]
            public string TransactionId { get; set; }

            [StringLength(100, ErrorMessage = "ReferenceNo cannot exceed 100 characters.")]
            public string ReferenceNo { get; set; }

            [StringLength(500, ErrorMessage = "PaymentRemarks cannot exceed 500 characters.")]
            public string PaymentRemarks { get; set; }

            // Order Details

            [Required(ErrorMessage = "SparePartsOrderDetails is required.")]
            [MinLength(1, ErrorMessage = "At least one spare part is required.")]
            public List<SparePartsOrderDetailRequest> SparePartsOrderDetails { get; set; }
                = new List<SparePartsOrderDetailRequest>();


            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (PaymentStatus == 1)
                {
                    if (!PaymentAmount.HasValue || PaymentAmount <= 0)
                    {
                        yield return new ValidationResult(
                            "PaymentAmount is required when PaymentStatus is 1.",
                            new[] { nameof(PaymentAmount) });
                    }

                    if (string.IsNullOrWhiteSpace(PaymentMode))
                    {
                        yield return new ValidationResult(
                            "PaymentMode is required when PaymentStatus is 1.",
                            new[] { nameof(PaymentMode) });
                    }
                }
            }
        }


        public class SparePartsOrderDetailRequest
        {
            [Range(1, int.MaxValue, ErrorMessage = "Invalid SparePartId.")]
            public int SparePartId { get; set; }

            [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
            public decimal Quantity { get; set; }
        }


        public class SparePartsOrderResponse
        {
            public int StatusCode { get; set; }

            public string Message { get; set; } = string.Empty;

            public Guid OrderId { get; set; }

            public string OrderNo { get; set; } = string.Empty;

            public decimal TotalQty { get; set; }

            public decimal TotalAmount { get; set; }

            public decimal PaidAmount { get; set; }

            public decimal BalanceAmount { get; set; }
        }


        public class UpdateSparePartsPaymentRequest : IValidatableObject
        {
            [Required(ErrorMessage = "OrderId is required.")]
            public string OrderId { get; set; }

            [Range(0, 1, ErrorMessage = "PaymentStatus must be either 0 or 1.")]
            public int PaymentStatus { get; set; }

            [Range(0.01, double.MaxValue, ErrorMessage = "PaymentAmount must be greater than zero.")]
            public decimal? PaymentAmount { get; set; }

            [StringLength(20, ErrorMessage = "PaymentMode cannot exceed 20 characters.")]
            public string PaymentMode { get; set; }

            [StringLength(100, ErrorMessage = "TransactionId cannot exceed 100 characters.")]
            public string TransactionId { get; set; }

            [StringLength(100, ErrorMessage = "ReferenceNo cannot exceed 100 characters.")]
            public string ReferenceNo { get; set; }

            [StringLength(500, ErrorMessage = "PaymentRemarks cannot exceed 500 characters.")]
            public string PaymentRemarks { get; set; }


            public IEnumerable<ValidationResult> Validate(
                ValidationContext validationContext)
            {
                // OrderId validation
                if (string.IsNullOrWhiteSpace(OrderId))
                {
                    yield return new ValidationResult(
                        "OrderId is required.",
                        new[] { nameof(OrderId) });
                }
                else if (!Guid.TryParse(OrderId, out _))
                {
                    yield return new ValidationResult(
                        "OrderId must be a valid GUID.",
                        new[] { nameof(OrderId) });
                }


                // PaymentStatus = 1
                if (PaymentStatus == 1)
                {
                    // Payment Amount
                    if (!PaymentAmount.HasValue || PaymentAmount <= 0)
                    {
                        yield return new ValidationResult(
                            "PaymentAmount is required and must be greater than zero when PaymentStatus is 1.",
                            new[] { nameof(PaymentAmount) });
                    }

                    // Payment Mode
                    if (string.IsNullOrWhiteSpace(PaymentMode))
                    {
                        yield return new ValidationResult(
                            "PaymentMode is required when PaymentStatus is 1.",
                            new[] { nameof(PaymentMode) });
                    }
                }
            }
        }

        public class UpdateSparePartsPaymentResponse
        {
            public int StatusCode { get; set; }

            public string Message { get; set; }

            public Guid OrderId { get; set; }

            public decimal TotalAmount { get; set; }

            public decimal PaidAmount { get; set; }

            public decimal BalanceAmount { get; set; }

            public string PaymentStatus { get; set; }
        }

        public class SparePartsOrderDropdownResponse
        {
            public string DropDownName { get; set; }

            public string ColumnName { get; set; }
        }

        public class SparePartsPaginationResponse
        {
            public List<SparesPartMaster> Data { get; set; } = new();

            public int TotalRecords { get; set; }

            public int PageNumber { get; set; }

            public int PageSize { get; set; }

            public int TotalPages { get; set; }
        }

        public class SparePartsOrderListResponse
        {
            public List<GetSparePartsOrderResponse> Data { get; set; }
                = new List<GetSparePartsOrderResponse>();

            public int? TotalRecords { get; set; }
            public decimal? TotalRecordsValue { get; set; }
            public int? TotalPendingInvoice { get; set; }
            public decimal? TotalPendingInvoiceValue { get; set; }
            public int? TotalPendingPayment { get; set; }
            public decimal? TotalPendingPaymentValue { get; set; }
            public int? TotalPendingDelivered { get; set; }
            public decimal? TotalPendingDeliveredValue { get; set; }

        }

        public class GetSparePartsOrderResponse
        {
            public Guid OrderId { get; set; }
            public string OrderNo { get; set; }
            public DateTime? OrderDate { get; set; }
            public string DealerCode { get; set; }



            public string DealerName { get; set; }
            public string DealerLocation { get; set; }
            public string DealerState { get; set; }

            public string OrderStatus { get; set; }
            public decimal? TotalQty { get; set; }
            public decimal? TotalAmount { get; set; }
            public string PaymentStatus { get; set; }
            public decimal? PaidAmount { get; set; }
            public decimal? BalanceAmount { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CreatedDate { get; set; }
            public decimal? TotalGSTAmount { get; set; }
            public bool IsInvoicing { get; set; }
            public bool IsDelivered { get; set; }
            public bool IsPaymentSuccess { get; set; }
            public decimal? PaymentAmount { get; set; }
            public decimal? InvTaxableAmount { get; set; }
            public decimal? InvTaxAmount { get; set; }
            public decimal? InvTotalAmount { get; set; }
            public DateTime? PaymentDate { get; set; }
            public string PaymentMode { get; set; }
            public string ReferenceNo { get; set; }
            public string Remarks { get; set; }
            public string TransactionId { get; set; }
            public string DocketNumber { get; set; }
            public string InvoiceNumber { get; set; }
            public string PDFUrl { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public DateTime? InvoiceDate { get; set; }
        }

        public class GetSparePartsOrderDetailResponse
        {
            public Guid OrderDetailId { get; set; }
            public Guid OrderId { get; set; }
            public int SparePartId { get; set; }
            public decimal Quantity { get; set; }
            public decimal Rate { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal LineTotal { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool IsDeleted { get; set; }
            public decimal? GstRate { get; set; }
            public decimal? TotalTaxableAmount { get; set; }
            public decimal? Ndp { get; set; }
            public string Desc { get; set; }
            public string Material { get; set; }
            public decimal CurrentQty { get; set; }
        }

        public class UpdateSparePartsOrderItemRequest : IValidatableObject
        {
            public string OrderDetailId { get; set; }

            [Required(ErrorMessage = "Action is required.")]
            public string Action { get; set; }

            public decimal? Quantity { get; set; }
            public string OrderId { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (string.Equals(Action?.Trim(), "CANCEL_ORDER", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(OrderId))
                    {
                        yield return new ValidationResult(
                            "OrderId is required when Action is CANCEL_ORDER.",
                            new[] { nameof(OrderId) });
                    }
                    else if (!Guid.TryParse(OrderId, out _))
                    {
                        yield return new ValidationResult(
                            "OrderId must be a valid GUID.",
                            new[] { nameof(OrderId) });
                    }

                    yield break;
                }

                else
                {
                    if (string.IsNullOrWhiteSpace(OrderDetailId))
                    {
                        yield return new ValidationResult(
                            "OrderDetailId is required.",
                            new[] { nameof(OrderDetailId) });
                    }

                    else if (!Guid.TryParse(OrderDetailId, out _))
                    {
                        yield return new ValidationResult(
                            "OrderDetailId must be a valid GUID.",
                            new[] { nameof(OrderDetailId) });
                    }
                    yield break;

                }
            }

        }

        public class UpdateSparePartsOrderItemResponse
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public Guid OrderId { get; set; }
            public Guid OrderDetailId { get; set; }
            public string Action { get; set; }
            public decimal TotalQty { get; set; }
            public decimal TotalTaxableAmount { get; set; }
            public decimal TotalGSTAmount { get; set; }
            public decimal GrandTotal { get; set; }
        }

        public class CreateSpareOrderInvoiceRequest : IValidatableObject
        {
            public string OrderId { get; set; }

            public string InvoiceNumber { get; set; }

            public DateTime? InvoiceDate { get; set; }

            public string DocketNumber { get; set; }

            public decimal TaxableAmount { get; set; }

            public decimal TaxAmount { get; set; }

            public decimal TotalAmount { get; set; }

            public string PDFUrl { get; set; }
            public IFormFile PDF { get; set; }
            public bool IsInvoicing { get; set; }
            public bool IsDelivered { get; set; }


            public IEnumerable<ValidationResult> Validate(
                ValidationContext validationContext)
            {
                // OrderId
                if (string.IsNullOrWhiteSpace(OrderId))
                {
                    yield return new ValidationResult(
                        "OrderId is required.",
                        new[] { nameof(OrderId) });
                }
                else if (!Guid.TryParse(OrderId, out _))
                {
                    yield return new ValidationResult(
                        "OrderId must be a valid GUID.",
                        new[] { nameof(OrderId) });
                }


                // Invoice Number
                if (string.IsNullOrWhiteSpace(InvoiceNumber))
                {
                    yield return new ValidationResult(
                        "InvoiceNumber is required.",
                        new[] { nameof(InvoiceNumber) });
                }
                else if (InvoiceNumber.Length > 50)
                {
                    yield return new ValidationResult(
                        "InvoiceNumber cannot exceed 50 characters.",
                        new[] { nameof(InvoiceNumber) });
                }


                // Invoice Date
                if (InvoiceDate.HasValue &&
                    InvoiceDate.Value > DateTime.Now)
                {
                    yield return new ValidationResult(
                        "InvoiceDate cannot be greater than current date.",
                        new[] { nameof(InvoiceDate) });
                }


                // Docket Number
                if (!string.IsNullOrWhiteSpace(DocketNumber) &&
                    DocketNumber.Length > 100)
                {
                    yield return new ValidationResult(
                        "DocketNumber cannot exceed 100 characters.",
                        new[] { nameof(DocketNumber) });
                }


                // Taxable Amount
                if (TaxableAmount <= 0)
                {
                    yield return new ValidationResult(
                        "TaxableAmount must be greater than zero.",
                        new[] { nameof(TaxableAmount) });
                }


                // Tax Amount
                if (TaxAmount < 0)
                {
                    yield return new ValidationResult(
                        "TaxAmount cannot be negative.",
                        new[] { nameof(TaxAmount) });
                }


                // Total Amount
                if (TotalAmount <= 0)
                {
                    yield return new ValidationResult(
                        "TotalAmount must be greater than zero.",
                        new[] { nameof(TotalAmount) });
                }


                // Total = Taxable + Tax
                if (TotalAmount != TaxableAmount + TaxAmount)
                {
                    yield return new ValidationResult(
                        "TotalAmount must be equal to TaxableAmount plus TaxAmount.",
                        new[] { nameof(TotalAmount) });
                }


                // PDF URL
                if (!string.IsNullOrWhiteSpace(PDFUrl) &&
                    PDFUrl.Length > 500)
                {
                    yield return new ValidationResult(
                        "PDFUrl cannot exceed 500 characters.",
                        new[] { nameof(PDFUrl) });
                }
            }
        }



        public class CreateSpareOrderInvoiceResponse
        {
            public int StatusCode { get; set; }

            public string Message { get; set; }

            public Guid InvoiceId { get; set; }

            public Guid OrderId { get; set; }

            public string InvoiceNumber { get; set; }

            public DateTime InvoiceDate { get; set; }

            public string DealerCode { get; set; }

            public string DocketNumber { get; set; }

            public decimal TaxableAmount { get; set; }

            public decimal TaxAmount { get; set; }

            public decimal TotalAmount { get; set; }

            public string PDFUrl { get; set; }

            public string InvoiceStatus { get; set; }
        }


        public class SpareOrderInvoiceResponse
        {
            public Guid InvoiceId { get; set; }
            public Guid OrderId { get; set; }

            public string InvoiceNumber { get; set; }
            public DateTime? InvoiceDate { get; set; }

            public string DealerCode { get; set; }
            public string DocketNumber { get; set; }

            public decimal TaxableAmount { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal TotalAmount { get; set; }

            public string PDFUrl { get; set; }

            public string CreatedBy { get; set; }
            public DateTime? CreatedDate { get; set; }

            public bool IsDelivered { get; set; }
            public DateTime? DeliveryDate { get; set; }
        }

    }
}
