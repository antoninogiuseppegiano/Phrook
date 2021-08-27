using System.Text.Json.Serialization;

namespace Phrook.Models.ResponseModels
{

	public partial class GoogleBooksApiByIdResponseModel
	{
		/* [JsonPropertyName("kind")]
		public string Kind { get; set; } */

		[JsonPropertyName("id")]
		public string Id { get; set; }

		/* [JsonPropertyName("etag")]
		public string Etag { get; set; }

		[JsonPropertyName("selfLink")]
		public Uri SelfLink { get; set; } */

		[JsonPropertyName("volumeInfo")]
		public VolumeInfo VolumeInfo { get; set; }

		/* [JsonPropertyName("layerInfo")]
		public LayerInfo LayerInfo { get; set; }

		[JsonPropertyName("saleInfo")]
		public SaleInfo SaleInfo { get; set; }

		[JsonPropertyName("accessInfo")]
		public AccessInfo AccessInfo { get; set; } */
	}

	/*
	public partial class AccessInfo
	{
		[JsonPropertyName("country")]
		public string Country { get; set; }

		[JsonPropertyName("epub")]
		public Epub Epub { get; set; }

		[JsonPropertyName("pdf")]
		public Pdf Pdf { get; set; }

		[JsonPropertyName("accessViewStatus")]
		public string AccessViewStatus { get; set; }
	}

	public partial class Epub
	{
		[JsonPropertyName("isAvailable")]
		public bool IsAvailable { get; set; }

		[JsonPropertyName("acsTokenLink")]
		public Uri AcsTokenLink { get; set; }
	}

	public partial class Pdf
	{
		[JsonPropertyName("isAvailable")]
		public bool IsAvailable { get; set; }
	}

	public partial class LayerInfo
	{
		[JsonPropertyName("layers")]
		public Layer[] Layers { get; set; }
	}

	public partial class Layer
	{
		[JsonPropertyName("layerId")]
		public string LayerId { get; set; }

		[JsonPropertyName("volumeAnnotationsVersion")]
		[JsonConverter(typeof(ParseStringConverter))]
		public long VolumeAnnotationsVersion { get; set; }
	}

	public partial class SaleInfo
	{
		[JsonPropertyName("country")]
		public string Country { get; set; }

		[JsonPropertyName("listPrice")]
		public SaleInfoListPrice ListPrice { get; set; }

		[JsonPropertyName("retailPrice")]
		public SaleInfoListPrice RetailPrice { get; set; }

		[JsonPropertyName("buyLink")]
		public Uri BuyLink { get; set; }

		[JsonPropertyName("offers")]
		public Offer[] Offers { get; set; }
	}

	public partial class SaleInfoListPrice
	{
		[JsonPropertyName("amount")]
		public double Amount { get; set; }

		[JsonPropertyName("currencyCode")]
		public string CurrencyCode { get; set; }
	}

	public partial class Offer
	{
		[JsonPropertyName("finskyOfferType")]
		public long FinskyOfferType { get; set; }

		[JsonPropertyName("listPrice")]
		public OfferListPrice ListPrice { get; set; }

		[JsonPropertyName("retailPrice")]
		public OfferListPrice RetailPrice { get; set; }

		[JsonPropertyName("giftable")]
		public bool Giftable { get; set; }
	}

	public partial class OfferListPrice
	{
		[JsonPropertyName("amountInMicros")]
		public long AmountInMicros { get; set; }

		[JsonPropertyName("currencyCode")]
		public string CurrencyCode { get; set; }
	}
	*/

	/*
	public partial class VolumeInfo
	{
		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("subtitle")]
		public string Subtitle { get; set; }

		[JsonPropertyName("authors")]
		public string[] Authors { get; set; }

		[JsonPropertyName("publisher")]
		public string Publisher { get; set; }

		[JsonPropertyName("publishedDate")]
		public DateTimeOffset PublishedDate { get; set; }

		[JsonPropertyName("description")]
		public string Description { get; set; }

		[JsonPropertyName("readingModes")]
		public ReadingModes ReadingModes { get; set; }

		[JsonPropertyName("maturityRating")]
		public string MaturityRating { get; set; }

		[JsonPropertyName("allowAnonLogging")]
		public bool AllowAnonLogging { get; set; }

		[JsonPropertyName("contentVersion")]
		public string ContentVersion { get; set; }

		[JsonPropertyName("panelizationSummary")]
		public PanelizationSummary PanelizationSummary { get; set; }

		[JsonPropertyName("imageLinks")]
		public ImageLinks ImageLinks { get; set; }

		[JsonPropertyName("previewLink")]
		public Uri PreviewLink { get; set; }

		[JsonPropertyName("infoLink")]
		public Uri InfoLink { get; set; }

		[JsonPropertyName("canonicalVolumeLink")]
		public Uri CanonicalVolumeLink { get; set; }
	}
	*/

	/*
	public partial class ImageLinks
	{
		[JsonPropertyName("smallThumbnail")]
		public Uri SmallThumbnail { get; set; }

		[JsonPropertyName("thumbnail")]
		public Uri Thumbnail { get; set; }

		[JsonPropertyName("small")]
		public Uri Small { get; set; }

		[JsonPropertyName("medium")]
		public Uri Medium { get; set; }

		[JsonPropertyName("large")]
		public Uri Large { get; set; }
	}

	public partial class PanelizationSummary
	{
		[JsonPropertyName("containsEpubBubbles")]
		public bool ContainsEpubBubbles { get; set; }

		[JsonPropertyName("containsImageBubbles")]
		public bool ContainsImageBubbles { get; set; }
	}

	public partial class ReadingModes
	{
		[JsonPropertyName("text")]
		public bool Text { get; set; }

		[JsonPropertyName("image")]
		public bool Image { get; set; }
	}
	*/
}