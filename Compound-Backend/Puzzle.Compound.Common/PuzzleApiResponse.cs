using System;

namespace Puzzle.Compound.Common
{
    public class PuzzleApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public bool Ok { get; set; }
        public object Result { get; set; }

        public PuzzleApiResponse(string message = "", int statusCode = 200, object result = null)
        {
            this.StatusCode = statusCode;
            this.Message = message == string.Empty ? "Success" : message;
            this.Ok = (result == null) ? false : true;
        }

        public PuzzleApiResponse(DateTime sentDate, object result = null)
        {
            this.StatusCode = 200;
            this.Ok = (result == null) ? false : true;
            this.Result = result;
            this.Message = "Success";
        }

        public PuzzleApiResponse(object result)
        {
            this.Ok = (result == null) ? false : true;
            this.StatusCode = 200;
            this.Result = result;
        }

    }

    public class Pagination
    {
        public int TotalItemsCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
