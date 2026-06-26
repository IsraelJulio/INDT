using Moq;
using PropostaService.Application.DTOs;
using PropostaService.Application.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Ports;

namespace PropostaService.Tests.UseCases;

public class CriarPropostaUseCaseTests
{
    private readonly Mock<IPropostaRepository> _repositoryMock = new();
    private readonly CriarPropostaUseCase _useCase;

    public CriarPropostaUseCaseTests()
    {
        _repositoryMock
            .Setup(r => r.CriarAsync(It.IsAny<Proposta>()))
            .ReturnsAsync((Proposta p) => p);

        _useCase = new CriarPropostaUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task Deve_criar_proposta_com_status_EmAnalise()
    {
        var request = new CriarPropostaRequest("João Silva", "123.456.789-00", 10000m);

        var result = await _useCase.ExecutarAsync(request);

        Assert.Equal("EmAnalise", result.Status);
        Assert.Equal("João Silva", result.NomeProponente);
        Assert.Equal(10000m, result.ValorCoberto);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Deve_chamar_repositorio_uma_vez()
    {
        var request = new CriarPropostaRequest("Maria", "111.222.333-44", 5000m);

        await _useCase.ExecutarAsync(request);

        _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Proposta>()), Times.Once);
    }
}
