using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpResponseExtensions
    {
        public static void AddPaginationHeaders(this HttpResponse response
        ,PaginationHeaders headers) {
            var options = new JsonSerializerOptions{
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(headers, options));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}