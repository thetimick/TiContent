// ⠀
// ApiService.cs
// TiContent
// 
// Created by the_timick on 27.04.2025.
// ⠀

namespace TiContent.Services.API;

public interface IApiService
{
    Task Search(string query);
}

public class ApiService: IApiService
{
    public Task Search(string query)
    {
        throw new NotImplementedException();
    }
}