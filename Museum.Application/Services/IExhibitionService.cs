using Museum.Contracts.Exhibitions;

namespace Museum.Application.Services;

/// <summary>
/// Интерфейс службы управления выставками.
/// </summary>
public interface IExhibitionService
{
    /// <summary>Возвращает список всех выставок.</summary>
    public IReadOnlyList<ExhibitionDto> GetAll();

    /// <summary>Возвращает выставку по ее идентификатору.</summary>
    public ExhibitionDto? GetById(int id);

    /// <summary>Создает новую выставку.</summary>
    public ExhibitionDto Create(ExhibitionCreateUpdateDto dto);

    /// <summary>Обновляет сведения о выставке.</summary>
    public bool Update(int id, ExhibitionCreateUpdateDto dto);

    /// <summary>Удаляет выставку по ее идентификатору.</summary>
    public void Delete(int id);
}