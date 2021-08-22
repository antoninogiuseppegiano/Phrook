using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phrook.Models.Enums;
using Phrook.Models.Exceptions;
using Phrook.Models.Options;
using Phrook.Models.ResponseModels;
using Phrook.Models.Services.Application;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.HttpClients
{
	public class GoogleBooksClient : IGoogleBooksClient
	{
		private readonly HttpClient _client;
		private readonly ILogger<GoogleBooksClient> _logger;
		private readonly IOptionsMonitor<GoogleBooksApiOptions> googleBooksApiOptions;
		public GoogleBooksClient(HttpClient client, ILogger<GoogleBooksClient> logger, IOptionsMonitor<GoogleBooksApiOptions> googleBooksApiOptions)
		{
			this.googleBooksApiOptions = googleBooksApiOptions;
			this._logger = logger;
			this._client = client;
		}

		private string GetApiUrl(string value, GoogleBooksApiType requestType = GoogleBooksApiType.ISBN)
		{
			string url = googleBooksApiOptions.CurrentValue.Url;
			switch (requestType)
			{
				case GoogleBooksApiType.Id:
					{
						return $"{url}/{value}";
					}
				case GoogleBooksApiType.ISBN:
					{
						return $"{url}?q=+isbn:{value}";

					}
				default:
					{
						throw new ApiException(value);
					}
			}
		}
		private string GetApiUrl(string title, string author)
		{
			string url = googleBooksApiOptions.CurrentValue.Url;
			bool isTitle = !string.IsNullOrWhiteSpace(title);
			bool isAuthor = !string.IsNullOrWhiteSpace(author);
			if(isTitle && isAuthor) 
			{
				return $"{url}?q=+intitle:{title}+inauthor:{author}&orderBy=relevance";
			}
			else if(isTitle)
			{
				return $"{url}?q=+intitle:{title}&orderBy=relevance";
			}
			else 
			{
				return $"{url}?q=+inauthor:{author}&orderBy=relevance";
			}
		}

		public async Task<BookOverviewViewModel> GetBookByIdAsync(string id)
		{
			try
			{
				using var responseStream = await _client.GetStreamAsync(GetApiUrl(id, GoogleBooksApiType.Id));
				var deserialized = await JsonSerializer.DeserializeAsync<GoogleBooksApiByIdResponseModel>(responseStream);

				BookOverviewViewModel viewModel = new()
				{
					Id = deserialized.Id,
					ISBN = deserialized.VolumeInfo.IndustryIdentifiers.Where(ii => ii.Type.Equals("ISBN_13", StringComparison.InvariantCultureIgnoreCase)).Select(ii => ii.Identifier).SingleOrDefault(),
					Title = deserialized.VolumeInfo.Title,
					// Author = deserialized.VolumeInfo.Authors.Aggregate("", (authors, next) => authors += " - " + next),
					Author = string.Join(" - ", deserialized.VolumeInfo.Authors),
					ImagePath = deserialized.VolumeInfo.ImageLinks.Thumbnail.ToString(),
					Description = deserialized.VolumeInfo.Description
				};
				return viewModel;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Something went wrong when calling Google Books Api with id {id}");
				throw new ApiException(id);
			}
		}

		public async Task<string> GetIdFromISBNAsync(string isbn)
		{
			try
			{
				using var responseStream = await _client.GetStreamAsync(GetApiUrl(isbn, GoogleBooksApiType.ISBN));
				var bookId = (await JsonSerializer.DeserializeAsync<GoogleBooksApiByParametersResponseModel>(responseStream)).Items.First().Id;
				return bookId;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Something went wrong when calling Google Books Api with ISBN {isbn}");
				throw new ApiException(isbn);
			}
		}

		public async Task<ListViewModel<SearchedBookViewModel>> GetBooksByTitleAuthorAsync(string title, string author)
		{
			try
			{
				title = title?.Trim();
				author = author?.Trim();
				using var responseStream = await _client.GetStreamAsync(GetApiUrl(title, author));
				var deserialized = await JsonSerializer.DeserializeAsync<GoogleBooksApiByParametersResponseModel>(responseStream);
				ListViewModel<SearchedBookViewModel> viewModel = new() { Results = new() };
				string isbn, image, authors;
				if (deserialized.Items == null)
				{
					return viewModel;
				}
				foreach (var Item in deserialized.Items)
				{
					if (Item.VolumeInfo.IndustryIdentifiers != null && Item.VolumeInfo.IndustryIdentifiers.Where(ii => ii.Type.Equals("ISBN_13", StringComparison.InvariantCultureIgnoreCase)).Any())
					{
						isbn = Item.VolumeInfo.IndustryIdentifiers.Where(ii => ii.Type.Equals("ISBN_13", StringComparison.InvariantCultureIgnoreCase)).Select(ii => ii.Identifier).SingleOrDefault();
					}
					else continue;

					image = Item.VolumeInfo.ImageLinks == null ? "#" : Item.VolumeInfo.ImageLinks.Thumbnail.ToString();
					if (Item.VolumeInfo.Authors != null && Item.VolumeInfo.Authors.Any())
					{
						authors = Item.VolumeInfo.Authors.Aggregate("", (authors, next) => authors += " - " + next).Substring(3);
					}
					else
					{
						authors = "Sconosciuto";
					}

					
					viewModel.Results.Add(
						new SearchedBookViewModel
						{
							Id = Item.Id,
							ISBN = isbn,
							Title = Item.VolumeInfo.Title,
							Author = authors,
							ImagePath = image
						}
					);
					viewModel.TotalCount++;
				}
				return viewModel;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Something went wrong when calling Google Books Api with title {title} and author {author}");
				throw new ApiException(title, author);
			}
		}
	}
}