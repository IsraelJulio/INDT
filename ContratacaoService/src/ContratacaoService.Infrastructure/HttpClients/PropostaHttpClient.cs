using System.Text.Json;
using ContratacaoService.Domain.Ports;

namespace ContratacaoService.Infrastructure.HttpClients;

public class PropostaHttpClient(HttpClient httpClient) : IPropostaClient
{
    public async Task<string?> ObterStatusPropostaAsync(Guid propostaId)
    {
        var response = await httpClient.GetAsync($"/propostas/{propostaId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("status").GetString();
    }
}
