namespace Cgr.Api.Dtos;

/// <summary>
/// Payload para criar uma pessoa.
/// </summary>
public class PessoaCreateRequest
{
    /// <summary>
    /// Nome com limite de 200 caracteres.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Idade nao negativa.
    /// </summary>
    public int Idade { get; set; }
}

/// <summary>
/// Payload para atualizar uma pessoa existente.
/// </summary>
public class PessoaUpdateRequest
{
    /// <summary>
    /// Nome com limite de 200 caracteres.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Idade nao negativa.
    /// </summary>
    public int Idade { get; set; }
}

/// <summary>
/// Resposta de leitura de pessoa.
/// </summary>
public class PessoaResponse
{
    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Idade da pessoa.
    /// </summary>
    public int Idade { get; set; }
}

