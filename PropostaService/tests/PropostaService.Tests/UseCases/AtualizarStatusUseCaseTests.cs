using Moq;
using PropostaService.Application.DTOs;
using PropostaService.Application.Events;
using PropostaService.Application.Ports;
using PropostaService.Application.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Ports;

namespace PropostaService.Tests.UseCases;

public class AtualizarStatusUseCaseTests
{
    private readonly Mock<IPropostaRepository> _repositoryMock = new();
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly AtualizarStatusUseCase _useCase;
    private readonly Proposta _proposta = new("Teste", "000.000.000-00", 1000m);

    public AtualizarStatusUseCaseTests()
    {
        _repositoryMock
            .Setup(r => r.ObterPorIdAsync(_proposta.Id))
            .ReturnsAsync(_proposta);

        _repositoryMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Proposta>()))
            .Returns(Task.CompletedTask);

        _useCase = new AtualizarStatusUseCase(_repositoryMock.Object, _eventPublisherMock.Object);
    }

    [Theory]
    [InlineData("Aprovada")]
    [InlineData("Rejeitada")]
    [InlineData("EmAnalise")]
    public async Task Deve_atualizar_status_valido(string status)
    {
        var result = await _useCase.ExecutarAsync(_proposta.Id, new AtualizarStatusRequest(status));

        Assert.NotNull(result);
        Assert.Equal(status, result.Status);
    }

    [Fact]
    public async Task Deve_publicar_evento_kafka_ao_atualizar_status()
    {
        await _useCase.ExecutarAsync(_proposta.Id, new AtualizarStatusRequest("Aprovada"));

        _eventPublisherMock.Verify(
            p => p.PublishAsync(It.IsAny<string>(), It.IsAny<PropostaStatusAtualizadaEvent>()),
            Times.Once);
    }

    [Fact]
    public async Task Deve_retornar_null_quando_proposta_nao_encontrada()
    {
        var result = await _useCase.ExecutarAsync(Guid.NewGuid(), new AtualizarStatusRequest("Aprovada"));

        Assert.Null(result);
    }

    [Fact]
    public async Task Deve_lancar_excecao_para_status_invalido()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecutarAsync(_proposta.Id, new AtualizarStatusRequest("StatusInvalido")));
    }
}
