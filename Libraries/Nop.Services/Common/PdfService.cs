// RTL Support provided by Credo inc (www.credo.co.il  ||   info@credo.co.il)

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Html;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using OfficeOpenXml.Style;
using StackExchange.Profiling.Internal;
using Syncfusion.Pdf.Tables;

namespace Nop.Services.Common
{
    /// <summary>
    /// PDF service
    /// </summary>
    public partial class PdfService : IPdfService
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftCardService _giftCardService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly INopFileProvider _fileProvider;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ISettingService _settingService;
        private readonly IShipmentService _shipmentService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public PdfService(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardService giftCardService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            INopFileProvider fileProvider,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IRewardPointService rewardPointService,
            ISettingService settingService,
            IShipmentService shipmentService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            IVendorService vendorService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            PdfSettings pdfSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            MediaSettings mediaSettings)
        {
            _addressSettings = addressSettings;
            _addressService = addressService;
            _catalogSettings = catalogSettings;
            _countryService = countryService;
            _currencySettings = currencySettings;
            _addressAttributeFormatter = addressAttributeFormatter;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardService = giftCardService;
            _languageService = languageService;
            _localizationService = localizationService;
            _measureService = measureService;
            _fileProvider = fileProvider;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _settingService = settingService;
            _shipmentService = shipmentService;
            _storeContext = storeContext;
            _stateProvinceService = stateProvinceService;
            _storeService = storeService;
            _vendorService = vendorService;
            _workContext = workContext;
            _measureSettings = measureSettings;
            _pdfSettings = pdfSettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get font
        /// </summary>
        /// <returns>Font</returns>
        protected virtual Font GetFont()
        {
            //nopCommerce supports Unicode characters
            //nopCommerce uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
            //It was downloaded from http://savannah.gnu.org/projects/freefont
            return GetFont(_pdfSettings.FontFileName);
        }

        /// <summary>
        /// Get font
        /// </summary>
        /// <param name="fontFileName">Font file name</param>
        /// <returns>Font</returns>
        protected virtual Font GetFont(string fontFileName)
        {
            if (fontFileName == null)
                throw new ArgumentNullException(nameof(fontFileName));

            var fontPath = _fileProvider.Combine(_fileProvider.MapPath("~/App_Data/Pdf/"), fontFileName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new Font(baseFont, 10, Font.NORMAL);
            return font;
        }

        /// <summary>
        /// Get font direction
        /// </summary>
        /// <param name="lang">Language</param>
        /// <returns>Font direction</returns>
        protected virtual int GetDirection(Language lang)
        {
            return lang.Rtl ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;
        }

        /// <summary>
        /// Get element alignment
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="isOpposite">Is opposite?</param>
        /// <returns>Element alignment</returns>
        protected virtual int GetAlignment(Language lang, bool isOpposite = false)
        {
            //if we need the element to be opposite, like logo etc`.
            if (!isOpposite)
                return lang.Rtl ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;

            return lang.Rtl ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
        }

        /// <summary>
        /// Get PDF cell
        /// </summary>
        /// <param name="resourceKey">Locale</param>
        /// <param name="lang">Language</param>
        /// <param name="font">Font</param>
        /// <returns>PDF cell</returns>
        protected virtual PdfPCell GetPdfCell(string resourceKey, Language lang, Font font)
        {
            return new PdfPCell(new Phrase(_localizationService.GetResource(resourceKey, lang.Id), font));
        }

        /// <summary>
        /// Get PDF cell
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="font">Font</param>
        /// <returns>PDF cell</returns>
        protected virtual PdfPCell GetPdfCell(object text, Font font)
        {
            return new PdfPCell(new Phrase(text.ToString(), font));
        }

        protected virtual PdfPCell GetPdfCell(string text, Font font, int verticalAlignment, int border)
        {
            var phrase = new Phrase(text, font);
            var cell = new PdfPCell(phrase);
            cell.VerticalAlignment = verticalAlignment;
            cell.Border = border;

            return cell;
        }

        /// <summary>
        /// Get paragraph
        /// </summary>
        /// <param name="resourceKey">Locale</param>
        /// <param name="lang">Language</param>
        /// <param name="font">Font</param>
        /// <param name="args">Locale arguments</param>
        /// <returns>Paragraph</returns>
        protected virtual Paragraph GetParagraph(string resourceKey, Language lang, Font font, params object[] args)
        {
            return GetParagraph(resourceKey, string.Empty, lang, font, args);
        }

        /// <summary>
        /// Get paragraph
        /// </summary>
        /// <param name="resourceKey">Locale</param>
        /// <param name="indent">Indent</param>
        /// <param name="lang">Language</param>
        /// <param name="font">Font</param>
        /// <param name="args">Locale arguments</param>
        /// <returns>Paragraph</returns>
        protected virtual Paragraph GetParagraph(string resourceKey, string indent, Language lang, Font font, params object[] args)
        {
            var formatText = _localizationService.GetResource(resourceKey, lang.Id);
            return new Paragraph(indent + (args.Any() ? string.Format(formatText, args) : formatText), font);
        }

        /// <summary>
        /// Print footer
        /// </summary>
        /// <param name="pdfSettingsByStore">PDF settings</param>
        /// <param name="pdfWriter">PDF writer</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="lang">Language</param>
        /// <param name="font">Font</param>
        protected virtual void PrintFooter(PdfSettings pdfSettingsByStore, PdfWriter pdfWriter, Rectangle pageSize, Language lang, Font font)
        {
            if (string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1) && string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2))
                return;

            var column1Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1)
                ? new List<string>()
                : pdfSettingsByStore.InvoiceFooterTextColumn1
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            var column2Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2)
                ? new List<string>()
                : pdfSettingsByStore.InvoiceFooterTextColumn2
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            if (!column1Lines.Any() && !column2Lines.Any())
                return;

            var totalLines = Math.Max(column1Lines.Count, column2Lines.Count);
            const float margin = 43;

            //if you have really a lot of lines in the footer, then replace 9 with 10 or 11
            var footerHeight = totalLines * 9;
            var directContent = pdfWriter.DirectContent;
            directContent.MoveTo(pageSize.GetLeft(margin), pageSize.GetBottom(margin) + footerHeight);
            directContent.LineTo(pageSize.GetRight(margin), pageSize.GetBottom(margin) + footerHeight);
            directContent.Stroke();

            var footerTable = new PdfPTable(2)
            {
                WidthPercentage = 100f,
                RunDirection = GetDirection(lang)
            };
            footerTable.SetTotalWidth(new float[] { 250, 250 });

            //column 1
            if (column1Lines.Any())
            {
                var column1 = new PdfPCell(new Phrase())
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };

                foreach (var footerLine in column1Lines)
                {
                    column1.Phrase.Add(new Phrase(footerLine, font));
                    column1.Phrase.Add(new Phrase(Environment.NewLine));
                }

                footerTable.AddCell(column1);
            }
            else
            {
                var column = new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER };
                footerTable.AddCell(column);
            }

            //column 2
            if (column2Lines.Any())
            {
                var column2 = new PdfPCell(new Phrase())
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };

                foreach (var footerLine in column2Lines)
                {
                    column2.Phrase.Add(new Phrase(footerLine, font));
                    column2.Phrase.Add(new Phrase(Environment.NewLine));
                }

                footerTable.AddCell(column2);
            }
            else
            {
                var column = new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER };
                footerTable.AddCell(column);
            }

            footerTable.WriteSelectedRows(0, totalLines, pageSize.GetLeft(margin), pageSize.GetBottom(margin) + footerHeight, directContent);
        }

        /// <summary>
        /// Print order notes
        /// </summary>
        /// <param name="pdfSettingsByStore">PDF settings</param>
        /// <param name="order">Order</param>
        /// <param name="lang">Language</param>
        /// <param name="titleFont">Title font</param>
        /// <param name="doc">Document</param>
        /// <param name="font">Font</param>
        protected virtual void PrintOrderNotes(PdfSettings pdfSettingsByStore, Order order, Language lang, Font titleFont, Document doc, Font font)
        {
            if (!pdfSettingsByStore.RenderOrderNotes)
                return;

            var orderNotes = _orderService.GetOrderNotesByOrderId(order.Id, true)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList();

            if (!orderNotes.Any())
                return;

            var notesHeader = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };

            var cellOrderNote = GetPdfCell("PDFInvoice.OrderNotes", lang, titleFont);
            cellOrderNote.Border = Rectangle.NO_BORDER;
            notesHeader.AddCell(cellOrderNote);
            doc.Add(notesHeader);
            doc.Add(new Paragraph(" "));

            var notesTable = new PdfPTable(2)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };
            notesTable.SetWidths(lang.Rtl ? new[] { 70, 30 } : new[] { 30, 70 });

            //created on
            cellOrderNote = GetPdfCell("PDFInvoice.OrderNotes.CreatedOn", lang, font);
            cellOrderNote.BackgroundColor = BaseColor.LightGray;
            cellOrderNote.HorizontalAlignment = Element.ALIGN_CENTER;
            notesTable.AddCell(cellOrderNote);

            //note
            cellOrderNote = GetPdfCell("PDFInvoice.OrderNotes.Note", lang, font);
            cellOrderNote.BackgroundColor = BaseColor.LightGray;
            cellOrderNote.HorizontalAlignment = Element.ALIGN_CENTER;
            notesTable.AddCell(cellOrderNote);

            foreach (var orderNote in orderNotes)
            {
                cellOrderNote = GetPdfCell(_dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc), font);
                cellOrderNote.HorizontalAlignment = Element.ALIGN_LEFT;
                notesTable.AddCell(cellOrderNote);

                cellOrderNote = GetPdfCell(HtmlHelper.ConvertHtmlToPlainText(_orderService.FormatOrderNoteText(orderNote), true, true), font);
                cellOrderNote.HorizontalAlignment = Element.ALIGN_LEFT;
                notesTable.AddCell(cellOrderNote);

                //should we display a link to downloadable files here?
                //I think, no. Anyway, PDFs are printable documents and links (files) are useful here
            }

            doc.Add(notesTable);
        }

        /// <summary>
        /// Print totals
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="order">Order</param>
        /// <param name="font">Text font</param>
        /// <param name="titleFont">Title font</param>
        /// <param name="doc">PDF document</param>
        protected virtual void PrintTotals(int vendorId, Language lang, Order order, Font font, Font titleFont, Font whitefont, Document doc)
        {
            //vendors cannot see totals
            if (vendorId != 0)
                return;
            Paragraph HorizontalLine = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.Black, Element.ALIGN_LEFT, 1)));
            doc.Add(HorizontalLine);
            doc.Add(AddEmptyLine());
            //subtotal
            var totalsTable = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };
            totalsTable.DefaultCell.Border = Rectangle.NO_BORDER;

            var languageId = lang.Id;

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax &&
                !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                var orderSubtotalInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                var orderSubtotalInclTaxStr = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true,
                    order.CustomerCurrencyCode, languageId, true);

                var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.Sub-Total", languageId)} {orderSubtotalInclTaxStr}", font);
                p.HorizontalAlignment = Element.ALIGN_RIGHT;
                p.Border = Rectangle.NO_BORDER;
                totalsTable.AddCell(p);
            }
            else
            {
                //excluding tax

                var orderSubtotalExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                var orderSubtotalExclTaxStr = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true,
                    order.CustomerCurrencyCode, languageId, false);

                var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.Sub-Total", languageId)} {orderSubtotalExclTaxStr}", font);
                p.HorizontalAlignment = Element.ALIGN_RIGHT;
                p.Border = Rectangle.NO_BORDER;
                totalsTable.AddCell(p);
            }

            //discount (applied to order subtotal)
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
            {
                //order subtotal
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax &&
                    !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
                {
                    //including tax

                    var orderSubTotalDiscountInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                    var orderSubTotalDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(
                        -orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                    var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.Discount", languageId)} {orderSubTotalDiscountInCustomerCurrencyStr}", font);
                    p.HorizontalAlignment = Element.ALIGN_RIGHT;
                    p.Border = Rectangle.NO_BORDER;
                    totalsTable.AddCell(p);
                }
                else
                {
                    //excluding tax

                    var orderSubTotalDiscountExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                    var orderSubTotalDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(
                        -orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                    var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.Discount", languageId)} {orderSubTotalDiscountInCustomerCurrencyStr}", font);
                    p.HorizontalAlignment = Element.ALIGN_RIGHT;
                    p.Border = Rectangle.NO_BORDER;
                    totalsTable.AddCell(p);
                }
            }

            //shipping
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var orderShippingInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                    var orderShippingInclTaxStr = _priceFormatter.FormatShippingPrice(
                        orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                    var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.Shipping", languageId)} {orderShippingInclTaxStr}", font);
                    p.HorizontalAlignment = Element.ALIGN_RIGHT;
                    p.Border = Rectangle.NO_BORDER;
                    totalsTable.AddCell(p);
                }
                else
                {
                    //excluding tax
                    var orderShippingExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                    var orderShippingExclTaxStr = _priceFormatter.FormatShippingPrice(
                        orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                    var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.Shipping", languageId)} {orderShippingExclTaxStr}", font);
                    p.HorizontalAlignment = Element.ALIGN_RIGHT;
                    p.Border = Rectangle.NO_BORDER;
                    totalsTable.AddCell(p);
                }
            }

            //payment fee
            if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
            {
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var paymentMethodAdditionalFeeInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                    var paymentMethodAdditionalFeeInclTaxStr = _priceFormatter.FormatPaymentMethodAdditionalFee(
                        paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                    var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", languageId)} {paymentMethodAdditionalFeeInclTaxStr}", font);
                    p.HorizontalAlignment = Element.ALIGN_RIGHT;
                    p.Border = Rectangle.NO_BORDER;
                    totalsTable.AddCell(p);
                }
                else
                {
                    //excluding tax
                    var paymentMethodAdditionalFeeExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                    var paymentMethodAdditionalFeeExclTaxStr = _priceFormatter.FormatPaymentMethodAdditionalFee(
                        paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                    var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", languageId)} {paymentMethodAdditionalFeeExclTaxStr}", font);
                    p.HorizontalAlignment = Element.ALIGN_RIGHT;
                    p.Border = Rectangle.NO_BORDER;
                    totalsTable.AddCell(p);
                }
            }

            //tax
            var taxStr = string.Empty;
            var taxRates = new SortedDictionary<decimal, decimal>();
            bool displayTax;
            var displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    taxRates = _orderService.ParseTaxRates(order, order.TaxRates);

                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    taxStr = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                        false, languageId);
                }
            }

            if (displayTax)
            {
                var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.Tax", languageId)} {taxStr}", font);
                p.HorizontalAlignment = Element.ALIGN_RIGHT;
                p.Border = Rectangle.NO_BORDER;
                totalsTable.AddCell(p);
            }

            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    var taxRate = string.Format(_localizationService.GetResource("PDFInvoice.TaxRate", languageId),
                        _priceFormatter.FormatTaxRate(item.Key));
                    var taxValue = _priceFormatter.FormatPrice(
                        _currencyService.ConvertCurrency(item.Value, order.CurrencyRate), true, order.CustomerCurrencyCode,
                        false, languageId);

                    var p = GetPdfCell($"{taxRate} {taxValue}", font);
                    p.HorizontalAlignment = Element.ALIGN_RIGHT;
                    p.Border = Rectangle.NO_BORDER;
                    totalsTable.AddCell(p);
                }
            }

            //discount (applied to order total)
            if (order.OrderDiscount > decimal.Zero)
            {
                var orderDiscountInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                var orderDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency,
                    true, order.CustomerCurrencyCode, false, languageId);

                var p = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.Discount", languageId)} {orderDiscountInCustomerCurrencyStr}", font);
                p.HorizontalAlignment = Element.ALIGN_RIGHT;
                p.Border = Rectangle.NO_BORDER;
                totalsTable.AddCell(p);
            }

            //gift cards
            foreach (var gcuh in _giftCardService.GetGiftCardUsageHistory(order))
            {
                var gcTitle = string.Format(_localizationService.GetResource("PDFInvoice.GiftCardInfo", languageId),
                    _giftCardService.GetGiftCardById(gcuh.GiftCardId)?.GiftCardCouponCode);
                var gcAmountStr = _priceFormatter.FormatPrice(
                    -_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate), true,
                    order.CustomerCurrencyCode, false, languageId);

                var p = GetPdfCell($"{gcTitle} {gcAmountStr}", font);
                p.HorizontalAlignment = Element.ALIGN_RIGHT;
                p.Border = Rectangle.NO_BORDER;
                totalsTable.AddCell(p);
            }

            //reward points
            if (order.RedeemedRewardPointsEntryId.HasValue && _rewardPointService.GetRewardPointsHistoryEntryById(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                var rpTitle = string.Format(_localizationService.GetResource("PDFInvoice.RewardPoints", languageId),
                    -redeemedRewardPointsEntry.Points);
                var rpAmount = _priceFormatter.FormatPrice(
                    -_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate),
                    true, order.CustomerCurrencyCode, false, languageId);

                var p = GetPdfCell($"{rpTitle} {rpAmount}", font);
                p.HorizontalAlignment = Element.ALIGN_RIGHT;
                p.Border = Rectangle.NO_BORDER;
                totalsTable.AddCell(p);
            }

            
            //order total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            //Added to convert the amount into negative amount in the case of Credit Order
            if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Credit)
            {
                orderTotalInCustomerCurrency = orderTotalInCustomerCurrency * -1;
            }
            var orderTotalStr = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

            var pTotal = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.OrderTotal", languageId)} {orderTotalStr}", whitefont);

            pTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
            pTotal.Border = Rectangle.NO_BORDER;
            pTotal.BackgroundColor = BaseColor.Black;
            pTotal.PaddingBottom = 8f;
            pTotal.PaddingTop = 5f;
            totalsTable.AddCell(pTotal);
            totalsTable.AddCell(AddEmptyLine());         

            doc.Add(totalsTable);
        }

        /// <summary>
        /// Print products
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="titleFont">Title font</param>
        /// <param name="doc">Document</param>
        /// <param name="order">Order</param>
        /// <param name="font">Text font</param>
        /// <param name="attributesFont">Product attributes font</param>
        protected virtual void PrintPickingProducts(int vendorId, Language lang, Font titleFont, Document doc, Order order, Font font, Font attributesFont)
        {
            var productsHeader = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f,
            };
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER, BackgroundColor = BaseColor.LightGray };

            productsHeader.AddCell(GetPdfCell(" ", font, PdfCell.ALIGN_LEFT, 0));
            doc.Add(productsHeader);

            var productsTable = new PdfPTable(5)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };

            productsTable.SetWidths(new float[] { 10, 10, 60, 10, 10 });
           
            //Image
            var cellProductItem = GetPdfCell("PDFInvoice.Image", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.PaddingBottom = 8f;
            cellProductItem.PaddingTop = 5f;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);
           
            //SKU
            cellProductItem = GetPdfCell("PDFInvoice.SKU", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //product name
            cellProductItem = GetPdfCell("PDFInvoice.ProductName", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //qty
            cellProductItem = GetPdfCell("PDFInvoice.ProductQuantity", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //Total weight
            cellProductItem = GetPdfCell("PDFInvoice.TotalWeight", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //a vendor should have access only to products
            var orderItems = _orderService.GetOrderItems(order.Id, vendorId: vendorId);
            var vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? _vendorService.GetVendorsByProductIds(orderItems.Select(item => item.ProductId).ToArray()) : new List<Vendor>();

            foreach (var orderItem in orderItems)
            {
                var product = _productService.GetProductById(orderItem.ProductId);
                //image
                var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                var pictureSize = _mediaSettings.ProductThumbPictureSize;
                string ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize);
                string FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture);

                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(ImageUrl);

                //Resize image depend upon your need

                jpg.ScaleToFit(140f, 120f);

                //Give space before image

                jpg.SpacingBefore = 10f;

                //Give some space after the image

                jpg.SpacingAfter = 1f;

                jpg.Alignment = Element.ALIGN_LEFT;

                productsTable.AddCell(jpg);
               
                //SKU
                var sku = _productService.FormatSku(product, orderItem.AttributesXml);
                cellProductItem = GetPdfCell(sku ?? string.Empty, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);

                //product name
                var name = _localizationService.GetLocalized(product, x => x.Name, lang.Id);
                //pAttribTable.AddCell(new Paragraph(name, font));
                //cellProductItem.AddElement(new Paragraph(name, font));
                cellProductItem = GetPdfCell(name ?? string.Empty, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //qty
                cellProductItem = GetPdfCell(orderItem.Quantity, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //Weight
                cellProductItem = GetPdfCell(orderItem.ItemWeight, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }

            doc.Add(productsTable);
        }

        protected virtual void PrintPackingProducts(Language lang, Font titleFont, Document doc, Shipment shipment, Font font, Font attributesFont)
        {
            doc.Add(AddEmptyLine());

            var productsTable = new PdfPTable(6)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };

            productsTable.SetWidths(new float[] { 10, 10, 50, 10, 10, 10 });
            //Image
            var cellProductItem = GetPdfCell("PDFInvoice.Image", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.PaddingBottom = 8f;
            cellProductItem.PaddingTop = 5f;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //SKU
            cellProductItem = GetPdfCell("PDFInvoice.SKU", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //product name
            cellProductItem = GetPdfCell("PDFInvoice.ProductName", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //qty
            cellProductItem = GetPdfCell("PDFInvoice.ProductQuantity", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //Total weight
            cellProductItem = GetPdfCell("PDFInvoice.TotalWeight", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //Total weight
            cellProductItem = GetPdfCell("PDFInvoice.TotalPrice", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //a vendor should have access only to products
            var Shipment = _shipmentService.GetShipmentItemsByShipmentId(shipment.Id);


            foreach (var orderItem in Shipment)
            {

                var ShipmentItem = _orderService.GetOrderItemById(orderItem.OrderItemId);
                var product = _productService.GetProductById(ShipmentItem.ProductId);
                //image
                var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                var pictureSize = _mediaSettings.ProductThumbPictureSize;
                string ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize);
                string FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture);

                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(ImageUrl);

                //Resize image depend upon your need

                jpg.ScaleToFit(140f, 120f);

                //Give space before image

                jpg.SpacingBefore = 10f;

                //Give some space after the image

                jpg.SpacingAfter = 1f;

                jpg.Alignment = Element.ALIGN_LEFT;

                productsTable.AddCell(jpg);
                //SKU
                var sku = _productService.FormatSku(product, ShipmentItem.AttributesXml);
                cellProductItem = GetPdfCell(sku ?? string.Empty, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);

                //product name
                var name = _localizationService.GetLocalized(product, x => x.Name, lang.Id);
                //pAttribTable.AddCell(new Paragraph(name, font));
                //cellProductItem.AddElement(new Paragraph(name, font));
                cellProductItem = GetPdfCell(name ?? string.Empty, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //qty
                cellProductItem = GetPdfCell(orderItem.Quantity, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);

                //Weight
                cellProductItem = GetPdfCell(ShipmentItem.ItemWeight.GetValueOrDefault(0).ToString("0.#"), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);

                //Price
                var orderSubtotalInclTaxStr = _priceFormatter.FormatPrice(ShipmentItem.PriceInclTax);
                cellProductItem = GetPdfCell(orderSubtotalInclTaxStr, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);
            }

            doc.Add(productsTable);
        }

        protected virtual void PrintInvoiceProducts(Language lang, Font titleFont, Document doc, Order order, Font font, Font attributesFont)
        {
            doc.Add(AddEmptyLine());

            var productsTable = new PdfPTable(6)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f,
            };



            productsTable.SetWidths(new float[] { 10, 10, 50, 10, 10, 10 });
            productsTable.DefaultCell.Border = Rectangle.NO_BORDER;
            //Image
            var cellProductItem = GetPdfCell("PDFInvoice.Image", lang, font);
            cellProductItem.BackgroundColor = BaseColor.Black;
            cellProductItem.PaddingBottom = 8f;
            cellProductItem.PaddingTop = 5f;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            cellProductItem.Border = Rectangle.NO_BORDER;
            productsTable.AddCell(cellProductItem);


            //SKU
            cellProductItem = GetPdfCell("PDFInvoice.SKU", lang, font);
            cellProductItem.BackgroundColor = BaseColor.Black;
            cellProductItem.PaddingBottom = 8f;
            cellProductItem.PaddingTop = 5f;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            cellProductItem.Border = Rectangle.NO_BORDER;
            productsTable.AddCell(cellProductItem);

            //product name
            cellProductItem = GetPdfCell("PDFInvoice.ProductName", lang, font);
            cellProductItem.BackgroundColor = BaseColor.Black;
            cellProductItem.PaddingBottom = 8f;
            cellProductItem.PaddingTop = 5f;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            cellProductItem.Border = Rectangle.NO_BORDER;
            productsTable.AddCell(cellProductItem);

            //qty
            cellProductItem = GetPdfCell("PDFInvoice.ProductQuantity", lang, font);
            cellProductItem.BackgroundColor = BaseColor.Black;
            cellProductItem.PaddingBottom = 8f;
            cellProductItem.PaddingTop = 5f;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            cellProductItem.Border = Rectangle.NO_BORDER;
            productsTable.AddCell(cellProductItem);

            //Total weight
            cellProductItem = GetPdfCell("PDFInvoice.Price", lang, font);
            cellProductItem.BackgroundColor = BaseColor.Black;
            cellProductItem.PaddingBottom = 8f;
            cellProductItem.PaddingTop = 5f;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            cellProductItem.Border = Rectangle.NO_BORDER;
            productsTable.AddCell(cellProductItem);

            //Total price
            cellProductItem = GetPdfCell("PDFInvoice.TotalPrice", lang, font);
            cellProductItem.BackgroundColor = BaseColor.Black;
            cellProductItem.PaddingBottom = 8f;
            cellProductItem.PaddingTop = 5f;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            cellProductItem.Border = Rectangle.NO_BORDER;
            productsTable.AddCell(cellProductItem);

            //a vendor should have access only to products
            var orderItems = _orderService.GetOrderItems(order.Id, vendorId: 0);
            var vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? _vendorService.GetVendorsByProductIds(orderItems.Select(item => item.ProductId).ToArray()) : new List<Vendor>();

            foreach (var orderItem in orderItems)
            {
                var product = _productService.GetProductById(orderItem.ProductId);
                var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                var pictureSize = _mediaSettings.ProductThumbPictureSize;
                string ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize);
                string FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture);

                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(ImageUrl);

                //Resize image depend upon your need

                jpg.ScaleToFit(140f, 120f);

                //Give space before image

                jpg.SpacingBefore = 10f;

                //Give some space after the image

                jpg.SpacingAfter = 1f;

                jpg.Alignment = Element.ALIGN_LEFT;

                productsTable.AddCell(jpg);



                //SKU
                var sku = _productService.FormatSku(product, orderItem.AttributesXml);
                cellProductItem = GetPdfCell(sku ?? string.Empty, attributesFont);
                cellProductItem.Border = Rectangle.NO_BORDER;
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);

                //product name
                var name = _localizationService.GetLocalized(product, x => x.Name, lang.Id);
                //pAttribTable.AddCell(new Paragraph(name, font));
                //cellProductItem.AddElement(new Paragraph(name, font));
                cellProductItem = GetPdfCell(name ?? string.Empty, attributesFont);
                cellProductItem.Border = Rectangle.NO_BORDER;
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);

                //qty
                cellProductItem = GetPdfCell(orderItem.Quantity, attributesFont);
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                cellProductItem.Border = Rectangle.NO_BORDER;
                productsTable.AddCell(cellProductItem);

                //Price
                var productPrice = _priceFormatter.FormatPrice(product.Price);
                cellProductItem = GetPdfCell(productPrice, attributesFont);
                cellProductItem.Border = Rectangle.NO_BORDER;
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);

                //Total Price
                var orderSubtotalInclTaxStr = _priceFormatter.FormatPrice(orderItem.PriceExclTax);
                cellProductItem = GetPdfCell(orderSubtotalInclTaxStr, attributesFont);
                cellProductItem.Border = Rectangle.NO_BORDER;
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);
            }

            doc.Add(productsTable);
        }


        /// <summary>
        /// Print shipping info
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="order">Order</param>
        /// <param name="titleFont">Title font</param>
        /// <param name="font">Text font</param>
        /// <param name="addressTable">PDF table for address</param>
        protected virtual void PrintShippingInfo(Language lang, Order order, Font titleFont, Font font, PdfPTable addressTable)
        {

            var billingAddressPdf = new PdfPTable(1) { RunDirection = GetDirection(lang) };
            billingAddressPdf.DefaultCell.Border = Rectangle.NO_BORDER;

            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER };

            if (order.ShippingAddressId != null)
            {
                var shippingAddress = _addressService.GetAddressById(order.ShippingAddressId.Value);

                cell.AddElement(new Paragraph("Shipping Address: ", titleFont));
                cell.AddElement(new Paragraph(string.Concat(shippingAddress.FirstName, " ", shippingAddress.LastName), font));
                cell.AddElement(new Paragraph(shippingAddress.Address1, font));
                cell.AddElement(new Paragraph(shippingAddress.Address2, font));
                cell.AddElement(new Paragraph(string.Concat(shippingAddress.City, " ", shippingAddress.ZipPostalCode), font));
                cell.AddElement(new Paragraph("New Zealand", font));
                cell.AddElement(new Paragraph(string.Concat("Shipping Method: ", order.ShippingMethod), font));
                //cell.AddElement(new Paragraph(string.Concat("Customer Notes: ", shippingAddress.CustomerNote), font));
            }

            addressTable.AddCell(cell);
        }

        /// <summary>
        /// Print billing info
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="titleFont">Title font</param>
        /// <param name="order">Order</param>
        /// <param name="font">Text font</param>
        /// <param name="addressTable">Address PDF table</param>
        protected virtual void PrintBillingInfo(Language lang, Font titleFont, Order order, Font font, PdfPTable addressTable)
        {
            var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

            var billingAddressPdf = new PdfPTable(1) { RunDirection = GetDirection(lang) };
            billingAddressPdf.DefaultCell.Border = Rectangle.NO_BORDER;

            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER };


            cell.AddElement(new Paragraph("Billing Address: ", titleFont));
            cell.AddElement(new Paragraph(string.Concat(billingAddress.FirstName, " ", billingAddress.LastName), font));
            cell.AddElement(new Paragraph(billingAddress.Address1, font));
            cell.AddElement(new Paragraph(billingAddress.Address2, font));
            cell.AddElement(new Paragraph(string.Concat(billingAddress.City, " ", billingAddress.ZipPostalCode), font));
            cell.AddElement(new Paragraph("New Zealand", font));
            cell.AddElement(new Paragraph(string.Concat("Email: ", billingAddress.Email), font));
            cell.AddElement(new Paragraph(string.Concat("Tel: ", billingAddress.PhoneNumber), font));
            cell.AddElement(new Paragraph(string.Concat("Customer Notes: ", billingAddress.CustomerNote), font));
            addressTable.AddCell(cell);
        }

        /// <summary>
        /// Print header
        /// </summary>
        /// <param name="pdfSettingsByStore">PDF settings</param>
        /// <param name="lang">Language</param>
        /// <param name="order">Order</param>
        /// <param name="font">Text font</param>
        /// <param name="titleFont">Title font</param>
        /// <param name="doc">Document</param>
        protected virtual void PrintHeader(PdfSettings pdfSettingsByStore, Language lang, Order order, Font font, Font titleFont, Document doc)
        {
            //logo
            var logoPicture = _pictureService.GetPictureById(pdfSettingsByStore.LogoPictureId);
            var logoExists = logoPicture != null;

            //header
            var headerTable = new PdfPTable(logoExists ? 2 : 1)
            {
                RunDirection = GetDirection(lang)
            };
            headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

            //store info
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            var anchor = new Anchor(store.Url.Trim('/'), font)
            {
                Reference = store.Url
            };

            var cellHeader = GetPdfCell(string.Format(_localizationService.GetResource("PDFInvoice.Order#", lang.Id), order.CustomOrderNumber), titleFont);
            cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
            cellHeader.Phrase.Add(new Phrase(anchor));
            cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
            cellHeader.Phrase.Add(GetParagraph("PDFInvoice.OrderDate", lang, font, _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString("D", new CultureInfo(lang.LanguageCulture))));
            cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
            cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
            cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHeader.Border = Rectangle.NO_BORDER;

            headerTable.AddCell(cellHeader);

            if (logoExists)
                headerTable.SetWidths(lang.Rtl ? new[] { 0.2f, 0.8f } : new[] { 0.8f, 0.2f });
            headerTable.WidthPercentage = 100f;

            //logo               
            if (logoExists)
            {
                var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                var logo = Image.GetInstance(logoFilePath);
                logo.Alignment = GetAlignment(lang, true);
                logo.ScaleToFit(65f, 65f);

                var cellLogo = new PdfPCell { Border = Rectangle.NO_BORDER };
                cellLogo.AddElement(logo);
                headerTable.AddCell(cellLogo);
            }

            doc.Add(headerTable);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Print an order to PDF
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        /// <param name="vendorId">Vendor identifier to limit products; 0 to print all products. If specified, then totals won't be printed</param>
        /// <returns>A path of generated file</returns>
        public virtual string PrintOrderToPdf(Order order, int languageId = 0, int vendorId = 0)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var fileName = $"order_{order.OrderGuid}_{CommonHelper.GenerateRandomDigitCode(4)}.pdf";
            var filePath = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot/files/exportimport"), fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var orders = new List<Order> { order };
                PrintOrdersToPdf(fileStream, orders, languageId, vendorId);
            }

            return filePath;
        }

        public virtual string PrintPicklistToPdf(Order order, int languageId = 0, int vendorId = 0)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var fileName = $"order_{order.OrderGuid}_{CommonHelper.GenerateRandomDigitCode(4)}.pdf";
            var filePath = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot/files/exportimport"), fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var orders = new List<Order> { order };
                PrintPicklistToPdf(fileStream, orders, languageId, vendorId);
            }

            return filePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="languageId"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public virtual void PrintPicklistToPdf(Stream stream, IList<Order> orders, int languageId = 0, int vendorId = 0)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (orders == null)
                throw new ArgumentNullException(nameof(orders));

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.Letter;
            }

            var doc = new Document(pageSize);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.Black;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);

            var ordCount = orders.Count;
            var ordNum = 0;

            foreach (var order in orders)
            {
                //by default _pdfSettings contains settings for the current active store
                //and we need PdfSettings for the store which was used to place an order
                //so let's load it based on a store of the current order
                //var pdfSettingsByStore = _settingService.LoadSetting<PdfSettings>(order.StoreId);

                
                var lang = _languageService.GetLanguageById(languageId == 0 ? order.CustomerLanguageId : languageId);
                //if (lang == null || !lang.Published)
                //    lang = _workContext.WorkingLanguage;

                var addressTable = new PdfPTable(1);
                //if (lang.Rtl)
                addressTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                addressTable.DefaultCell.Border = Rectangle.NO_BORDER;
                addressTable.WidthPercentage = 100f;

                var store = _storeService.GetStoreById(order.StoreId);

                //Print header information
                var header = new PdfPTable(2) { WidthPercentage = 100f };
                header.SetWidths(new int[] { 65, 35 });
                header.AddCell(AddLogo());
                header.AddCell(AddOrderInformation(order, font, titleFont, lang));
                doc.Add(header);
                doc.Add(new Paragraph(string.Empty));

                var address = new PdfPTable(2) { WidthPercentage = 100f };
                address.SetWidths(new int[] { 35, 65 });
                address.AddCell(AddFromAddress(store, font, titleFont));
                address.AddCell(AddToddress1(order, font, titleFont, doc));
                //address.AddCell(GetPdfCell(" ", font, PdfCell.ALIGN_LEFT, 0));
                doc.Add(address);

                PrintPickingProducts(vendorId, lang, titleFont, doc, order, font, attributesFont);

                ordNum++;
                if (ordNum < ordCount)
                {
                    doc.NewPage();
                }
            }

            doc.Close();
        }

        /// <summary>
        /// Print orders to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="orders">Orders</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        /// <param name="vendorId">Vendor identifier to limit products; 0 to print all products. If specified, then totals won't be printed</param>
        public virtual void PrintOrdersToPdf(Stream stream, IList<Order> orders, int languageId = 0, int vendorId = 0)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (orders == null)
                throw new ArgumentNullException(nameof(orders));

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.Letter;
            }

            var doc = new Document(pageSize);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont("Gotham-Medium.ttf");
            titleFont.SetStyle(Font.BOLD);
            titleFont.Size = 8.5f;
            titleFont.Color = BaseColor.Black;
            var font = GetFont("Gotham-Medium.ttf");
            font.Size = 8.5f;
            var Whitefont = GetFont("Gotham-Medium.ttf");
            Whitefont.Color = BaseColor.White;
            Whitefont.Size = 8.5f;
            var attributesFont = GetFont("Gotham-Medium.ttf");
            attributesFont.SetStyle(Font.NORMAL);
            attributesFont.Size = 8.5f;

            var ordCount = orders.Count;
            var ordNum = 0;

            foreach (var order in orders)
            {
                ////by default _pdfSettings contains settings for the current active store
                ////and we need PdfSettings for the store which was used to place an order
                ////so let's load it based on a store of the current order
                //var pdfSettingsByStore = _settingService.LoadSetting<PdfSettings>(order.StoreId);

                //if (lang == null || !lang.Published)
                //    lang = _workContext.WorkingLanguage;

                ////header
                //PrintHeader(pdfSettingsByStore, lang, order, font, titleFont, doc);

                ////addresses
                //PrintAddresses(vendorId, lang, titleFont, order, font, doc);

                //products
                //PrintProducts(vendorId, lang, titleFont, doc, order, font, attributesFont);

                var lang = _languageService.GetLanguageById(languageId == 0 ? order.CustomerLanguageId : languageId);
                var store = _storeService.GetStoreById(order.StoreId);

                //Print header information
                var header = new PdfPTable(2) { WidthPercentage = 100f };
                var emptycell = GetPdfCell(" ", Whitefont);
                emptycell.Border = Rectangle.NO_BORDER;
                header.SetWidths(new int[] { 50, 50 });
                header.AddCell(AddLogo());
                header.AddCell(emptycell);
                doc.Add(header);
                doc.Add(AddEmptyLine());

                //From address
                var address = new PdfPTable(3) { WidthPercentage = 100f };
                address.SetWidths(new int[] { 35, 30, 35 });
                address.AddCell(AddFromAddress(store, font, titleFont));
                address.AddCell(emptycell);
                address.AddCell(emptycell);
                doc.Add(address);
                doc.Add(AddEmptyLine());
                Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.Black, Element.ALIGN_LEFT, 1)));
                doc.Add(p);
                doc.Add(AddEmptyLine());

                //Billing and Shipping address
                address = new PdfPTable(3) { WidthPercentage = 100f };
                address.SetWidths(new int[] { 35, 30, 35 });
                PrintBillingInfo(lang, titleFont, order, font, address);
                PrintShippingInfo(lang, order, titleFont, font, address);
                address.AddCell(AddInvoiceInformation(order, font, titleFont, lang));
                doc.Add(address);
                doc.Add(AddEmptyLine());

                PrintInvoiceProducts(lang, titleFont, doc, order, Whitefont, attributesFont);

                //totals
                PrintTotals(vendorId, lang, order, font, titleFont, Whitefont, doc);

                //Add payment method
                AddPaymentMethod(order, font, doc);

                ordNum++;
                if (ordNum < ordCount)
                {
                    doc.NewPage();
                }
            }

            doc.Add(AddGreatingMessageInvoice(attributesFont));
            var footer = new PdfPTable(2) { WidthPercentage = 100f };
            var emptycell1 = GetPdfCell(" ", Whitefont);
            emptycell1.Border = Rectangle.NO_BORDER;
            footer.SetWidths(new int[] { 5, 5});
            footer.AddCell(AddLogo11());
            footer.AddCell(emptycell1);
            doc.Add(footer);
           

            doc.Close();
        }

        /// <summary>
        /// Print shipping slips to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipments">Shipments</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        public virtual void PrintShippingSlipsToPdf(Stream stream, IList<Shipment> shipments, int languageId = 0)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (shipments == null)
                throw new ArgumentNullException(nameof(shipments));

            var pageSize = PageSize.A6.Rotate();

            
            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.Letter;
                }

            var doc = new Document(pageSize);
             PdfWriter writer = PdfWriter.GetInstance(doc, stream);
            
            doc.Open();
            iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.Black;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);

          

                var shipmentCount = shipments.Count;
            var shipmentNum = 0;

            foreach (var shipment in shipments)
            {
                var order = _orderService.GetOrderById(shipment.OrderId);
                var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
                var address = _addressService.GetAddressById(order.ShippingAddressId.Value);

                var lang = _languageService.GetLanguageById(languageId == 0 ? order.CustomerLanguageId : languageId);
                if (lang == null || !lang.Published)
                    lang = _workContext.WorkingLanguage;

                var addressTable = new PdfPTable(1);
                if (lang.Rtl)
                    addressTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                addressTable.DefaultCell.Border = Rectangle.NO_BORDER;
                addressTable.WidthPercentage = 100f;

                //Print header information
                var header = new PdfPTable(1) { WidthPercentage = 100f };
                header.SetWidths(new int[] { 100});
                 header.AddCell(AddLogo3());
                doc.Add(header);
                doc.Add(new Paragraph(string.Empty));

                //var header1 = new PdfPTable(1) { WidthPercentage = 100f };
                //header1.SetWidths(new int[] { 100 });
                //header1.AddCell(AddOrderInformation(shipment, order, store, font, titleFont));
                //doc.Add(header1);
                //doc.Add(new Paragraph(string.Empty));

                //Print address information
                var table = new PdfPTable(3) { WidthPercentage = 100f};
                table.SetWidths(new int[] {40,30,30});

                table.AddCell(AddOrderInformation1(shipment, order, store, font, titleFont, cb, shipment.TrackingNumber));
               
                table.AddCell(AddFromAddress(store, font, titleFont));
                table.AddCell(AddToddress(address, font, titleFont));
                table.AddCell(new Paragraph());
                doc.Add(table);
                doc.Add(new Paragraph(string.Empty));

                ////Print bar code information
                //table = new PdfPTable(2) { WidthPercentage = 100f };
                //table.SetWidths(new int[] { 40, 60 });
               
                //table.AddCell(AddBarCode12812(cb, shipment.TrackingNumber));
                //doc.Add(table);
                //doc.Add(new Paragraph());


                shipmentNum++;
                if (shipmentNum < shipmentCount)
                {
                    doc.NewPage();
                }
            }

            doc.Close();
        }

        private PdfPCell AddLogo()
        {
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER };

            var logoPicture = _pictureService.GetPictureById(_pdfSettings.LogoPictureId);
            if (logoPicture != null)
            {
                var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                var logo = Image.GetInstance(logoFilePath);
                logo.ScaleToFit(250f, 60f);
                cell.AddElement(logo);
             }

            return cell;
        }
        private PdfPCell AddLogo1()
        {
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER };

            var logoPicture = _pictureService.GetPictureById(_pdfSettings.LogoPictureId);
            if (logoPicture != null)
            {
                var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                var logo = Image.GetInstance(logoFilePath);
                logo.ScaleToFit(150f, 20f);
                cell.AddElement(logo);
            }

            return cell;
        }
        private PdfPCell AddLogo11()
        {
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_RIGHT, Border = Rectangle.NO_BORDER };

            var logoPicture = _pictureService.GetPictureById(_pdfSettings.LogoPictureId);
            if (logoPicture != null)
            {
                var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                var logo = Image.GetInstance(logoFilePath);
                logo.ScaleToFit(150f, 20f);
                cell.AddElement(logo);
            }

            return cell;
        }
        private PdfPCell AddLogo2()
        {
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER };

            var logoPicture = _pictureService.GetPictureById(_pdfSettings.LogoPictureId);
            if (logoPicture != null)
            {
                var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                var logo = Image.GetInstance(logoFilePath);
                logo.ScaleToFit(250f, 40f);
                cell.AddElement(logo);
            }

            return cell;
        }
        private PdfPCell AddLogo3()
        {
            var cell = new PdfPCell { VerticalAlignment = Element.ALIGN_RIGHT, Border = Rectangle.NO_BORDER };
            cell.PaddingLeft = 70;
            var logoPicture = _pictureService.GetPictureById(_pdfSettings.LogoPictureId);
            if (logoPicture != null)
            {
                var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                var logo = Image.GetInstance(logoFilePath);
                logo.ScaleToFit(250f, 40f);
                cell.AddElement(logo);
            }

            return cell;
        }
        private PdfPCell AddBarCode128(PdfContentByte cb, string text)
        {
            var cell = new PdfPCell { HorizontalAlignment = Element.ALIGN_RIGHT, Border = Rectangle.NO_BORDER };
            float width = 525;

            Barcode128 code128 = new Barcode128();
            code128.CodeType = Barcode.CODE128;
            code128.ChecksumText = true;
            code128.GenerateChecksum = true;
            code128.StartStopText = true;
            code128.Code = text ?? String.Empty;
            iTextSharp.text.Image image128 = code128.CreateImageWithBarcode(cb, null, null);

            image128.ScaleToFit(width, 140f);

            //Calculate the position of the bar code
            float x = ((width / 2) - (image128.ScaledWidth / 2));
            cell.AddElement(new Paragraph(new Chunk(image128, x, 0)));

            return cell;
        }
        private PdfPCell AddBarCode12812(PdfContentByte cb, string text)
        {
            var cell = new PdfPCell { HorizontalAlignment = Element.ALIGN_RIGHT, Border = Rectangle.NO_BORDER };
            float width = 300;

            Barcode128 code128 = new Barcode128();
            code128.CodeType = Barcode.CODE128;
            code128.ChecksumText = true;
            code128.GenerateChecksum = true;
            code128.StartStopText = true;
            code128.Code = text ?? String.Empty;
            iTextSharp.text.Image image128 = code128.CreateImageWithBarcode(cb, null, null);

            image128.ScaleToFit(width, 50f);

            ////Calculate the position of the bar code
            //float x = ((width / 2) - (image128.ScaledWidth / 2));
            cell.AddElement(new Paragraph(new Chunk(image128, 0, 0)));
            return cell;
        }

        private PdfPCell AddBarCode(PdfContentByte cb, string text)
        {
            var cell = new PdfPCell { HorizontalAlignment = Element.ALIGN_RIGHT, Border = Rectangle.NO_BORDER };
            float width = 50;

            Barcode128 code128 = new Barcode128();
            code128.CodeType = Barcode.CODE128;
            code128.ChecksumText = true;
            code128.GenerateChecksum = true;
            code128.StartStopText = true;
            code128.Code = text ?? String.Empty;
            iTextSharp.text.Image image128 = code128.CreateImageWithBarcode(cb, null, null);

            image128.ScaleToFit(width, 50f);

            //Calculate the position of the bar code
            float x = ((width / 2) - (image128.ScaledWidth / 2));
            cell.AddElement(new Paragraph(new Chunk(image128, x, 0)));

            return cell;
        }
        private PdfPCell AddBarCode1281(PdfContentByte cb, string text)
        {
            var cell = new PdfPCell { HorizontalAlignment = Element.ALIGN_RIGHT, Border = Rectangle.NO_BORDER };
            float width = 300;

            Barcode128 code128 = new Barcode128();
            code128.CodeType = Barcode.CODE128;
            code128.ChecksumText = true;
            code128.GenerateChecksum = true;
            code128.StartStopText = true;
            code128.Code = text ?? String.Empty;
            iTextSharp.text.Image image128 = code128.CreateImageWithBarcode(cb, null, null);

            image128.ScaleToFit(width, 140f);

            //Calculate the position of the bar code
            float x = ((width / 2) - (image128.ScaledWidth / 2));
            cell.AddElement(new Paragraph(new Chunk(image128, x, 0)));

            return cell;
        }


        private PdfPCell AddOrderInformation(Shipment shipment, Order order, Store store, Font font, Font titleFont)
        {
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER, BackgroundColor = BaseColor.LightGray };
            cell.PaddingRight = 15;
            cell.PaddingLeft = 15;

            cell.AddElement(new Paragraph($"Order No: {order.Id}", titleFont));
            cell.AddElement(new Paragraph($"Weight: {shipment.TotalWeight} Kg", font));
            cell.AddElement(new Paragraph($"Email: {store.CompanyEmail}", font));
         

            return cell;
        }
        private PdfPCell AddOrderInformation1(Shipment shipment, Order order, Store store, Font font, Font titleFont, PdfContentByte cb, string text)
        {
            
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER, BackgroundColor = BaseColor.White };
           
            //cell.AddElement(AddBarCode(cb, shipment.TrackingNumber));
            cell.AddElement(new Paragraph($"Order No: {order.Id}", titleFont));
            cell.AddElement(new Paragraph($"Weight: {shipment.TotalWeight} Kg", font));
            cell.AddElement(new Paragraph($"Email: {store.CompanyEmail}", font));
            cell.AddElement(new Paragraph($"Box No: {shipment.AdminComment}", font));
            cell.AddElement(new Paragraph(" "));

            float width = 350;

            Barcode128 code128 = new Barcode128();
            code128.CodeType = Barcode.CODE128;
            code128.ChecksumText = true;
            code128.GenerateChecksum = true;
            code128.StartStopText = true;
            code128.Code = text ?? String.Empty;
            iTextSharp.text.Image image128 = code128.CreateImageWithBarcode(cb, null, null);

            image128.ScaleToFit(width, 60f);

            ////Calculate the position of the bar code
            //float x = ((width / 2) - (image128.ScaledWidth / 2));
            cell.AddElement(new Paragraph(new Chunk(image128, 0, 0)));





            return cell;
        }

        private PdfPCell AddOrderInformation(Order order, Font font, Font titleFont, Language lang)
        {
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER, BackgroundColor = BaseColor.LightGray };
            cell.PaddingRight = 15;
            cell.PaddingLeft = 15;

            cell.AddElement(new Paragraph($"Order No: {order.Id}", titleFont));
            cell.AddElement(new Paragraph($"Order Date: {order.CreatedOnUtc.ToString("dd/MM/yyyy")}", font));

            return cell;
        }

        private PdfPCell AddInvoiceInformation(Order order, Font font, Font titleFont, Language lang)
        {
            var FontColour = new BaseColor(211, 211, 211);
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER, BackgroundColor = FontColour };
            cell.PaddingRight = 15;
            cell.PaddingLeft = 15;

            cell.AddElement(new Paragraph($"Invoice No: INV{order.Id}", titleFont));
            cell.AddElement(new Paragraph($"Order No: {order.Id}", titleFont));
            if (order.PaidDateUtc.HasValue)
            {
                cell.AddElement(new Paragraph($"Invoice Date: {order.PaidDateUtc.Value.ToString("dd/MM/yyyy")}", font));
            }
            else
            {
                cell.AddElement(new Paragraph($"Invoice Date: -", font));
            }
            cell.AddElement(new Paragraph($"Order Date: {order.CreatedOnUtc.ToString("dd/MM/yyyy")}", font));

            return cell;
        }

        private PdfPCell AddFromAddress(Store store, Font font, Font titleFont)
        {
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER };

            cell.AddElement(new Paragraph("From: ", titleFont));
            cell.AddElement(new Paragraph(store.CompanyName, font));
            cell.AddElement(new Paragraph(store.CompanyAddress, font));
            cell.AddElement(new Paragraph($"{store.CompanyEmail}", font));
            cell.AddElement(new Paragraph($"{store.CompanyPhoneNumber}", font));
            cell.AddElement(new Paragraph($"GST: {store.CompanyVat}", font));

            return cell;
        }

        private PdfPCell AddToddress(Address address, Font font, Font titleFont)
        {
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER };

            cell.AddElement(new Paragraph("To: ", titleFont));
            cell.AddElement(new Paragraph(string.Concat(address.FirstName, " ", address.LastName), font));
            cell.AddElement(new Paragraph(string.Concat (address.Address1, " ", address.Address2), font));
             cell.AddElement(new Paragraph(string.Concat (address.City, address.County), font));
            cell.AddElement(new Paragraph($"{address.Email}", font));
            cell.AddElement(new Paragraph($"{address.PhoneNumber}", font));

            return cell;
        }

        private PdfPCell AddToddress1(Order order, Font font, Font titleFont, Document doc)
        {
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER };

            string paymentMethodAndStatus = string.Empty;
            if (order.PaymentMethodSystemName.Equals("Payments.PayInStore") && order.PaymentStatus == PaymentStatus.Pending)
                paymentMethodAndStatus = "Pay In Store/Pending";
            else if (order.PaymentMethodSystemName.Equals("Payments.PayInStore") && order.PaymentStatus == PaymentStatus.Paid)
                paymentMethodAndStatus = "Pay In Store/Paid";
            else if (order.PaymentMethodSystemName.Equals("Payments.Windcave"))
                paymentMethodAndStatus = "Credit Card / Account to Account / Paid";
            if (order.PaymentMethodSystemName.Equals("Payments.Credit") && order.PaymentStatus == PaymentStatus.Credit)
                paymentMethodAndStatus = "Credit/Pending";
            else if (order.PaymentMethodSystemName.Equals("Payments.Credit") && order.PaymentStatus == PaymentStatus.Paid)
                paymentMethodAndStatus = "Credit/Paid";

            cell.AddElement(new Paragraph($"Shipping Method: {order.ShippingMethod}", font));
            cell.AddElement(new Paragraph($"Payment Method: {paymentMethodAndStatus}", font));
            if (order.ShippingAddressId != null)
            {
                var shippingAddress = _addressService.GetAddressById(order.ShippingAddressId.Value);
                cell.AddElement(new Paragraph($"Customer Notes: {shippingAddress.CustomerNote}", font));
            }
            //cell.AddElement(new Paragraph(string.Concat(address.Address1, " ", address.Address2), font));
            //cell.AddElement(new Paragraph(string.Concat(address.City, address.County), font));
            //cell.AddElement(new Paragraph($"{address.Email}", font));
            //cell.AddElement(new Paragraph($"{address.PhoneNumber}", font));

            return cell;
        }
        private void AddPaymentMethod(Order order, Font titleFont, Document doc)
        {
            var table = new PdfPTable(1)
            {
                WidthPercentage = 100f,
           };
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_RIGHT, Border = Rectangle.NO_BORDER };
            string paymentMethodAndStatus = string.Empty;
            if (order.PaymentMethodSystemName.Equals("Payments.PayInStore") && order.PaymentStatus == PaymentStatus.Pending)
                paymentMethodAndStatus = "Pay In Store/Pending";
            else if (order.PaymentMethodSystemName.Equals("Payments.PayInStore") && order.PaymentStatus == PaymentStatus.Paid)
                paymentMethodAndStatus = "Pay In Store/Paid";
            else if (order.PaymentMethodSystemName.Equals("Payments.Windcave"))
                paymentMethodAndStatus = "Credit Card / Account to Account / Paid";
            if (order.PaymentMethodSystemName.Equals("Payments.Credit") && order.PaymentStatus == PaymentStatus.Credit)
                paymentMethodAndStatus = "Credit/Pending";
            else if (order.PaymentMethodSystemName.Equals("Payments.Credit") && order.PaymentStatus == PaymentStatus.Paid)
                paymentMethodAndStatus = "Credit/Paid";

            cell.AddElement(new Paragraph($"Method of Payment / Status: {paymentMethodAndStatus}", titleFont));
            cell.AddElement(new Paragraph(" "));
            table.AddCell(cell);

            doc.Add(table);
        }

        private PdfPTable AddGreatingMessageInvoice(Font titleFont)
        {
            var table = new PdfPTable(1)
            {
                WidthPercentage = 100f
            };
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_RIGHT, Border = Rectangle.NO_BORDER };

            cell.AddElement(new Paragraph("Thank you for ordering at Fenchurch Liquor Store!", titleFont));
            cell.AddElement(new Paragraph(" "));
            cell.AddElement(new Paragraph("Cheers!", titleFont));
            cell.AddElement(new Paragraph(" "));
            cell.AddElement(new Paragraph("Signature", titleFont));
            cell.AddElement(new Paragraph(" "));
            //cell.AddElement(AddLogo());
            table.AddCell(cell);

            return table;
        }

        private PdfPTable AddGreatingMessage(Font titleFont)
        {
            var table = new PdfPTable(1)
            {
                WidthPercentage = 100f
            };
            var cell = new PdfPCell { VerticalAlignment = PdfPCell.ALIGN_LEFT, Border = Rectangle.NO_BORDER };

            cell.AddElement(new Paragraph("Thank you for ordering at Fenchurch Liquor Store!", titleFont));
            cell.AddElement(new Paragraph(" "));
            cell.AddElement(new Paragraph("Cheers!", titleFont));
            table.AddCell(cell);

            return table;
        }

        private PdfPTable AddEmptyLine()
        {
            var productsHeader = new PdfPTable(1)
            {
                WidthPercentage = 100f
            };
            var phrase = new Phrase(" ");
            var cell = new PdfPCell(phrase);
            cell.Border = Rectangle.NO_BORDER;

            productsHeader.AddCell(cell);

            return productsHeader;
        }

        /// <summary>
        /// Print packaging slips to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipments">Shipments</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        public virtual void PrintPackagingSlipsToPdf(Stream stream, IList<Shipment> shipments, int languageId = 0)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (shipments == null)
                throw new ArgumentNullException(nameof(shipments));

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.Letter;
            }
            var shipmentFirst = shipments.FirstOrDefault();
            var shipmentsList = _shipmentService.GetShipmentsByOrderId(shipmentFirst.OrderId).ToList();
            var shipmentCount = _shipmentService.GetShipmentsByOrderId(shipmentFirst.OrderId).Count();
            int index = shipmentsList.Select((Value, Index) => new { Value, Index }).FirstOrDefault(p => p.Value.Id == shipmentFirst.Id).Index;
            index = index + 1;

            var doc = new Document(pageSize);
            //PdfWriter.GetInstance(doc, stream);
            PdfWriter writer = PdfWriter.GetInstance(doc, stream);
            doc.Header = new HeaderFooter(new Phrase("Page " + index + " of " + shipmentCount), false);
            doc.Open();

    
            iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;
            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.Black;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);


            var shipmentNum = 0;
            var lang = _languageService.GetLanguageById(languageId);

            foreach (var shipment in shipments)
            {
                var order = _orderService.GetOrderById(shipment.OrderId);
                var store = _storeService.GetStoreById(order.StoreId);
                lang = _languageService.GetLanguageById(languageId == 0 ? order.CustomerLanguageId : languageId);
                //Print header information
                var header = new PdfPTable(2) { WidthPercentage = 100f };
                header.SetWidths(new int[] { 65, 35 });
                header.AddCell(AddLogo());
                header.AddCell(AddOrderInformation(order, font, titleFont, lang));
                doc.Add(header);
                doc.Add(AddEmptyLine());

                //From address
                var address = new PdfPTable(2) { WidthPercentage = 100f };
                address.SetWidths(new int[] { 35, 65 });
                address.AddCell(AddFromAddress(store, font, titleFont));
                address.AddCell(GetPdfCell(" ", font, PdfCell.ALIGN_LEFT, 0));
                doc.Add(address);
                doc.Add(AddEmptyLine());

                //Billing and Shipping address
                address = new PdfPTable(2) { WidthPercentage = 100f };
                address.SetWidths(new int[] { 35, 65 });
                PrintBillingInfo(lang, titleFont, order, font, address);
                PrintShippingInfo(lang, order, titleFont, font, address);
                doc.Add(address);
                doc.Add(AddEmptyLine());

                PrintPackingProducts(lang, titleFont, doc, shipment, font, attributesFont);
                doc.Add(AddGreatingMessage(attributesFont));
                var table = new PdfPTable(1) { WidthPercentage = 100f };
                table.AddCell(AddBarCode1281(cb, shipment.TrackingNumber));
                doc.Add(table);
                shipmentNum++;
                if (shipmentNum < shipmentCount)
                {
                    doc.NewPage();
                }
            }



            doc.Close();
        }

        /// <summary>
        /// Print products to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="products">Products</param>
        public virtual void PrintProductsToPdf(Stream stream, IList<Product> products)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var lang = _workContext.WorkingLanguage;

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.Letter;
            }

            var doc = new Document(pageSize);
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.Black;
            var font = GetFont();

            var productNumber = 1;
            var prodCount = products.Count;

            foreach (var product in products)
            {
                var productName = _localizationService.GetLocalized(product, x => x.Name, lang.Id);
                var productDescription = _localizationService.GetLocalized(product, x => x.FullDescription, lang.Id);

                var productTable = new PdfPTable(1) { WidthPercentage = 100f };
                productTable.DefaultCell.Border = Rectangle.NO_BORDER;
                if (lang.Rtl)
                {
                    productTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                }

                productTable.AddCell(new Paragraph($"{productNumber}. {productName}", titleFont));
                productTable.AddCell(new Paragraph(" "));
                productTable.AddCell(new Paragraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(productDescription, decode: true)), font));
                productTable.AddCell(new Paragraph(" "));

                if (product.ProductType == ProductType.SimpleProduct)
                {
                    //simple product
                    //render its properties such as price, weight, etc
                    var priceStr = $"{product.Price:0.00} {_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode}";
                    if (product.IsRental)
                        priceStr = _priceFormatter.FormatRentalProductPeriod(product, priceStr);
                    productTable.AddCell(new Paragraph($"{_localizationService.GetResource("PDFProductCatalog.Price", lang.Id)}: {priceStr}", font));
                    productTable.AddCell(new Paragraph($"{_localizationService.GetResource("PDFProductCatalog.SKU", lang.Id)}: {product.Sku}", font));

                    if (product.IsShipEnabled && product.Weight > decimal.Zero)
                        productTable.AddCell(new Paragraph($"{_localizationService.GetResource("PDFProductCatalog.Weight", lang.Id)}: {product.Weight:0.00} {_measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name}", font));

                    if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                        productTable.AddCell(new Paragraph($"{_localizationService.GetResource("PDFProductCatalog.StockQuantity", lang.Id)}: {_productService.GetTotalStockQuantity(product)}", font));

                    productTable.AddCell(new Paragraph(" "));
                }

                var pictures = _pictureService.GetPicturesByProductId(product.Id);
                if (pictures.Any())
                {
                    var table = new PdfPTable(2) { WidthPercentage = 100f };
                    if (lang.Rtl)
                    {
                        table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    }

                    foreach (var pic in pictures)
                    {
                        var picBinary = _pictureService.LoadPictureBinary(pic);
                        if (picBinary == null || picBinary.Length <= 0)
                            continue;

                        var pictureLocalPath = _pictureService.GetThumbLocalPath(pic, 200, false);
                        var cell = new PdfPCell(Image.GetInstance(pictureLocalPath))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            Border = Rectangle.NO_BORDER
                        };
                        table.AddCell(cell);
                    }

                    if (pictures.Count % 2 > 0)
                    {
                        var cell = new PdfPCell(new Phrase(" "))
                        {
                            Border = Rectangle.NO_BORDER
                        };
                        table.AddCell(cell);
                    }

                    productTable.AddCell(table);
                    productTable.AddCell(new Paragraph(" "));
                }

                if (product.ProductType == ProductType.GroupedProduct)
                {
                    //grouped product. render its associated products
                    var pvNum = 1;
                    foreach (var associatedProduct in _productService.GetAssociatedProducts(product.Id, showHidden: true))
                    {
                        productTable.AddCell(new Paragraph($"{productNumber}-{pvNum}. {_localizationService.GetLocalized(associatedProduct, x => x.Name, lang.Id)}", font));
                        productTable.AddCell(new Paragraph(" "));

                        //uncomment to render associated product description
                        //string apDescription = associated_localizationService.GetLocalized(product, x => x.ShortDescription, lang.Id);
                        //if (!string.IsNullOrEmpty(apDescription))
                        //{
                        //    productTable.AddCell(new Paragraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(apDescription)), font));
                        //    productTable.AddCell(new Paragraph(" "));
                        //}

                        //uncomment to render associated product picture
                        //var apPicture = _pictureService.GetPicturesByProductId(associatedProduct.Id).FirstOrDefault();
                        //if (apPicture != null)
                        //{
                        //    var picBinary = _pictureService.LoadPictureBinary(apPicture);
                        //    if (picBinary != null && picBinary.Length > 0)
                        //    {
                        //        var pictureLocalPath = _pictureService.GetThumbLocalPath(apPicture, 200, false);
                        //        productTable.AddCell(Image.GetInstance(pictureLocalPath));
                        //    }
                        //}

                        productTable.AddCell(new Paragraph($"{_localizationService.GetResource("PDFProductCatalog.Price", lang.Id)}: {associatedProduct.Price:0.00} {_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode}", font));
                        productTable.AddCell(new Paragraph($"{_localizationService.GetResource("PDFProductCatalog.SKU", lang.Id)}: {associatedProduct.Sku}", font));

                        if (associatedProduct.IsShipEnabled && associatedProduct.Weight > decimal.Zero)
                            productTable.AddCell(new Paragraph($"{_localizationService.GetResource("PDFProductCatalog.Weight", lang.Id)}: {associatedProduct.Weight:0.00} {_measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name}", font));

                        if (associatedProduct.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                            productTable.AddCell(new Paragraph($"{_localizationService.GetResource("PDFProductCatalog.StockQuantity", lang.Id)}: {_productService.GetTotalStockQuantity(associatedProduct)}", font));

                        productTable.AddCell(new Paragraph(" "));

                        pvNum++;
                    }
                }

                doc.Add(productTable);

                productNumber++;

                if (productNumber <= prodCount)
                {
                    doc.NewPage();
                }
            }

            doc.Close();
        }

        #endregion
    }
}