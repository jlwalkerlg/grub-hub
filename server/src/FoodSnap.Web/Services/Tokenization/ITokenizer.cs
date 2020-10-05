using System;
using FoodSnap.Shared;

namespace FoodSnap.Web.Services.Tokenization
{
    public interface ITokenizer
    {
        string Encode(string data);
        string Encode(string data, DateTimeOffset expiry);
        Result<string> Decode(string token);
    }
}
