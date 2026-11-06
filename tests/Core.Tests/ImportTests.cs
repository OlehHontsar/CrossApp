namespace Core.Tests;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Core.Services;
using Core.Storage;
using Xunit;

public class ImportTests
{
    [Fact]
    // 5) Скасування імпорту[cite: 7]
    public async Task ImportAsync_WhenAlreadyCancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        
        var service = new CatalogService(new InMemoryCatalogStore());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.ImportAsync(new JsonCatalogImporter(), "any.json", cts.Token));
    }

    [Fact]
    // 6) Некоректний запис у файлі викликає DomainException, файл видаляється у finally[cite: 7]
    public async Task ImportAsync_WithInvalidRecord_ThrowsDomainException()
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "[{\"id\":\"1\",\"sku\":\"SKU-ERR\",\"name\":\"Test\",\"unit\":\"шт\",\"quantity\":-1}]");

            var service = new CatalogService(new InMemoryCatalogStore());
            var importer = new JsonCatalogImporter();

            await Assert.ThrowsAsync<DomainException>(
                () => service.ImportAsync(importer, tempFile));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}