using System.Collections.Generic;

namespace Core.Dto;

// Універсальний контейнер для відмовостійкої передачі результатів розбору
public sealed record ImportResult<T>(
    IReadOnlyList<T> Items, 
    IReadOnlyList<string> Errors
);
