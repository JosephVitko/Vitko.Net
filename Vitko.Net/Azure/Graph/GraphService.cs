using System.Net;
using Azure.Identity;
using Microsoft.Graph;

namespace Azure.Graph;

/// <summary>
/// Methods for interacting with Microsoft Graph.
/// </summary>
public class GraphService
{
    private GraphServiceClient GraphClient { get; set; }
    
    public GraphService(IEnumerable<string> scopes, string clientId, string tenantId, string clientSecret)
    {
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        GraphClient = new GraphServiceClient(credential, scopes);
    }

    public async Task<User?> GetUser(string userId, List<string>? fields = null)
    {
        fields ??= new List<string>();

        try
        {
            return await GraphClient.Users[userId]
                .Request()
                .Select(string.Join(",", fields))
                .GetAsync();
        }
        catch (ServiceException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
                return null;
            
            throw;
        }
    }
    
    public async Task<List<User>> GetUsersByQuery(string query, List<string>? fields = null)
    {
        fields ??= new List<string>();
        
        var request = GraphClient.Users
            .Request()
            .Filter(query)
            .Select(string.Join(",", fields));

        var result = await request.GetAsync();

        // handle all pages
        var users = new List<User>();
        users.AddRange(result.CurrentPage);
        while (result.NextPageRequest != null)
        {
            result = await result.NextPageRequest.GetAsync();
            users.AddRange(result.CurrentPage);
        }
        
        return users;
    }

    public async Task<List<User>> GetUsersByAttribute(string attribute, string value, List<string>? fields = null)
    {
        return await GetUsersByQuery($"{attribute} eq '{value}'", fields);
    }
    

    public async Task CreateUser(User user)
    {
        await GraphClient.Users
            .Request()
            .AddAsync(user);
    }
    
    public async Task UpdateUser(User user)
    {
        await GraphClient.Users[user.Id]
            .Request()
            .UpdateAsync(user);
    }
    
    public async Task DeleteUser(string userId)
    {
        await GraphClient.Users[userId]
            .Request()
            .DeleteAsync();
    }
    
    
}