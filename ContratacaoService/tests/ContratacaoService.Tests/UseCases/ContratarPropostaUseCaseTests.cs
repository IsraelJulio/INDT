using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.UseCases;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports;
using Moq;

namespace ContratacaoService.Tests.UseCases;

public class ContratarPropostaUseCaseTests
{
    private readonly Mock<IContratacaoRepository> _repositoryMock = new();
    private readonly Mock<IPropostaClient> _propostaClientMock = new();
    private readonly ContratarPropostaUseCase _useCase;
    private readonly Guid _propostaId = Guid.NewGuid();

    public ContratarPropostaUseCaseTests()
    {
        _repositoryMock
            .Setup(r => r.ObterPorPropostaIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Contratacao?)null);

        _repositoryMock
            .Setup(r => r.CriarAsync(It.IsAny<Contratacao>()))
            .ReturnsAsync((Contratacao c) => c);

        _useCase = new ContratarPropostaUseCase(_repositoryMock.Object, _propostaClientMock.Object);
    }

    [Fact]
    public async Task Deve_contratar_proposta_aprovada()
    {
        _propostaClientMock
            .Setup(c => c.ObterStatusPropostaAsync(_propostaId))
            .ReturnsAsync("Aprovada");

        var result = await _useCase.ExecutarAsync(new ContratarRequest(_propostaId));

        Assert.Equal(_propostaId, result.PropostaId);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_proposta_nao_aprovada()
    {
        _propostaClientMock
            .Setup(c => c.ObterStatusPropostaAsync(_propostaId))
            .ReturnsAsync("EmAnalise");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(new ContratarRequest(_propostaId)));
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_proposta_nao_encontrada()
    {
        _propostaClientMock
            .Setup(c => c.ObterStatusPropostaAsync(_propostaId))
            .ReturnsAsync((string?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _useCase.ExecutarAsync(new ContratarRequest(_propostaId)));
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_ja_contratada()
    {
        _repositoryMock
            .Setup(r => r.ObterPorPropostaIdAsync(_propostaId))
            .ReturnsAsync(Contratacao.Criar(_propostaId));

        _propostaClientMock
            .Setup(c => c.ObterStatusPropostaAsync(_propostaId))
            .ReturnsAsync("Aprovada");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(new ContratarRequest(_propostaId)));
    }
}
