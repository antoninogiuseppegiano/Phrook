using System;
using System.Text.Json.Serialization;
 
namespace Phrook.Models.ResponseModels
{
    public partial class GoogleBooksApiByParametersResponseModel
	{
		/* [JsonPropertyName("kind")]
		public string Kind { get; set; }

		[JsonPropertyName("totalItems")]
		public int TotalItems { get; set; } */

		[JsonPropertyName("items")]
		public Item[] Items { get; set; }
	}

	public partial class Item
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

		/* [JsonPropertyName("saleInfo")]
		public SaleInfo SaleInfo { get; set; }

		[JsonPropertyName("accessInfo")]
		public AccessInfo AccessInfo { get; set; } */
	}

	/* public partial class AccessInfo
	{
		[JsonPropertyName("country")]
		public string Country { get; set; }

		[JsonPropertyName("viewability")]
		public string Viewability { get; set; }

		[JsonPropertyName("embeddable")]
		public bool Embeddable { get; set; }

		[JsonPropertyName("publicDomain")]
		public bool PublicDomain { get; set; }

		[JsonPropertyName("textToSpeechPermission")]
		public string TextToSpeechPermission { get; set; }

		[JsonPropertyName("epub")]
		public Epub Epub { get; set; }

		[JsonPropertyName("pdf")]
		public Epub Pdf { get; set; }

		[JsonPropertyName("webReaderLink")]
		public Uri WebReaderLink { get; set; }

		[JsonPropertyName("accessViewStatus")]
		public string AccessViewStatus { get; set; }

		[JsonPropertyName("quoteSharingAllowed")]
		public bool QuoteSharingAllowed { get; set; }
	} */

	/* public partial class Epub
	{
		[JsonPropertyName("isAvailable")]
		public bool IsAvailable { get; set; }
	}

	public partial class SaleInfo
	{
		[JsonPropertyName("country")]
		public string Country { get; set; }

		[JsonPropertyName("saleability")]
		public string Saleability { get; set; }

		[JsonPropertyName("isEbook")]
		public bool IsEbook { get; set; }
	} */

	public partial class VolumeInfo
	{
		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("authors")]
		public string[] Authors { get; set; }

		/* [JsonPropertyName("publisher")]
		public string Publisher { get; set; }

		[JsonPropertyName("publishedDate")]
		public string PublishedDate { get; set; } */

		[JsonPropertyName("description")]
		public string Description { get; set; }

		[JsonPropertyName("industryIdentifiers")]
		public IndustryIdentifier[] IndustryIdentifiers { get; set; }

		/* [JsonPropertyName("readingModes")]
		public ReadingModes ReadingModes { get; set; } */

		[JsonPropertyName("pageCount")]
		public int PageCount { get; set; }

		/* [JsonPropertyName("printType")]
		public string PrintType { get; set; } */

		[JsonPropertyName("categories")]
		public string[] Categories { get; set; }

		/* [JsonPropertyName("maturityRating")]
		public string MaturityRating { get; set; }

		[JsonPropertyName("allowAnonLogging")]
		public bool AllowAnonLogging { get; set; }

		[JsonPropertyName("contentVersion")]
		public string ContentVersion { get; set; } */

		[JsonPropertyName("imageLinks")]
		public ImageLinks ImageLinks { get; set; }

		[JsonPropertyName("language")]
		public string Language { get; set; }

		[JsonPropertyName("previewLink")]
		public Uri PreviewLink { get; set; }

		[JsonPropertyName("infoLink")]
		public Uri InfoLink { get; set; }

		[JsonPropertyName("canonicalVolumeLink")]
		public Uri CanonicalVolumeLink { get; set; }
	}

	public partial class ImageLinks
	{
		[JsonPropertyName("smallThumbnail")]
		public Uri SmallThumbnail { get; set; }

		[JsonPropertyName("thumbnail")]
		public Uri Thumbnail { get; set; }
	}

	public partial class IndustryIdentifier
	{
		[JsonPropertyName("type")]
		public string Type { get; set; }

		[JsonPropertyName("identifier")]
		public string Identifier { get; set; }
	}

	/* public partial class ReadingModes
	{
		[JsonPropertyName("text")]
		public bool Text { get; set; }

		[JsonPropertyName("image")]
		public bool Image { get; set; }
	} */
}