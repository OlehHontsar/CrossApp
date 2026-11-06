namespace Core.Abstractions;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain;

public interface ICatalogImporter
{
    Task<IReadOnlyList<Product>> LoadAsync(string path, CancellationToken ct = default);
}