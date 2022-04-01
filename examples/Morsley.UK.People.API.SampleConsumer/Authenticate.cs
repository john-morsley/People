﻿using Morsley.UK.People.API.Contracts.Requests;
using Morsley.UK.People.API.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Morsley.UK.People.API.SampleConsumer;
public class Security
{
    private const string LoginURL = "https://localhost:5002/api/login";

    public async static Task<bool> AuthenticateAsync(HttpClient client, string username, string password)
    {
        Console.Write("Authenticating... ");
        var token = await GetJwtTokenAsync(client, username, password);
        if (token == null)
        {
            Console.WriteLine("Failed to Authenticate!");
            return false;
        }
        client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        Console.WriteLine("Authenticated.");

        return true;
    }

    protected async static Task<string?> GetJwtTokenAsync(HttpClient client, string username, string password)
    {
        var request = new LoginRequest(username, password);
        var result = await client.PostAsJsonAsync(LoginURL, request);
        if (!result.IsSuccessStatusCode) return null;
        if (result.StatusCode != HttpStatusCode.OK) return null;
        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
        if (response == null) return null;
        return response.Token;
    }

}