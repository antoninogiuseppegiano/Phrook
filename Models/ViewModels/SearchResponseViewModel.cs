using Phrook.Models.Enums;

namespace Phrook.Models.ViewModels
{
    public class SearchResponseViewModel<ResponseModel, InputModel>
    {
        public ResponseModel Response { get; set; }
		public InputModel Input {get; set; }
		public GoogleBooksApiType SearchType;
    }
}