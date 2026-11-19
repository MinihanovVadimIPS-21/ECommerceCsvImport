using ECommerceCsvImport.Models;
using ECommerceCsvImport.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceCsvImport.Controllers;

public class ProductsController : Controller
{
    private readonly IProductRepository _repository;
    private readonly IProductImportService _importService;

    public ProductsController(
        IProductRepository repository,
        IProductImportService importService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _importService = importService ?? throw new ArgumentNullException(nameof(importService));
    }

    [HttpGet]
    public IActionResult Index()
    {
        var vm = new ProductsIndexViewModel { Products = _repository.GetAll() };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View(new ImportViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Import(IFormFile file)
    {
        var vm = new ImportViewModel();

        if (file == null || file.Length == 0)
        {
            var r = new ImportResult();
            r.Errors.Add(new ImportError { RowNumber = 0, Message = "Файл не выбран или пуст" });
            vm.Result = r;
            return View(vm);
        }
        var lines = new List<string>();
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            while (reader.Peek() >= 0)
            {
                lines.Add(reader.ReadLine()!);
            }
        }
        vm.Result = _importService.ImportFromLines(lines);
        return View(vm);
    }
}