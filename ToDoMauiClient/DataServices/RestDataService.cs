﻿using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ToDoMauiClient.Models;

namespace ToDoMauiClient.DataServices;

public class RestDataService : IRestDataService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseAddress;
    private readonly string _url;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public RestDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5154" : "https://localhost:7092";
        _url = $"{_baseAddress}/api";

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task AddToDoAsync(ToDo toDo)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("-----> No Internet Access...");
            return;
        }
        try
        {
            string jsonToDo = JsonSerializer.Serialize<ToDo>(toDo, _jsonSerializerOptions);
            StringContent content = new StringContent(jsonToDo, Encoding.UTF8,"application/json");

            var response = await _httpClient.PostAsync($"{_url}/todo",content);

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Succesfully created ToDo");
            }
            else
            {
                Debug.WriteLine("---> Non Http 2xx response");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Whoops exception: {ex.Message}");
        }
        return;
    }

    public async Task DeleteToDoAsync(int id)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("-----> No Internet Access...");
            return;
        }
        try
        {
            var response = await _httpClient.DeleteAsync($"{_url}/todo/{id}");
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Succesfully deleted ToDo");
            }
            else
            {
                Debug.WriteLine("---> Non Http 2xx response");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Whoops exception: {ex.Message}");
        }
        return;
    }

    public async Task<List<ToDo>> GetAllToDosAsync()
    {
        List<ToDo> todos = new();

        if(Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("-----> No Internet Access...");
            return todos;
        }
        try
        {
            var response = await _httpClient.GetAsync($"{_url}/todo");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                todos = JsonSerializer.Deserialize<List<ToDo>>(content, _jsonSerializerOptions);
            }
            else
            {
                Debug.WriteLine("-----> Non Http 2xx response");
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Whoops exception: {ex.Message}");
        }
        return todos;
    }

    public async Task UpdateToDoAsync(ToDo toDo)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine("-----> No Internet Access...");
            return;
        }
        try
        {
            string jsonToDo = JsonSerializer.Serialize<ToDo>(toDo, _jsonSerializerOptions);
            StringContent content = new StringContent(jsonToDo, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_url}/todo/{toDo.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Succesfully updated ToDo");
            }
            else
            {
                Debug.WriteLine("---> Non Http 2xx response");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Whoops exception: {ex.Message}");
        }
        return;
    }
}
