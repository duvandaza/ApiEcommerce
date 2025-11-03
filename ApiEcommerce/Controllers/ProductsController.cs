using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Dtos.Responses;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Authorize (Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryrepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, 
            IMapper mapper,
            ICategoryRepository categoryRepository,
            IFileService fileService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryrepository = categoryRepository;
            _fileService = fileService;
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [AllowAnonymous]
        [HttpGet("{productId:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProduct(int productId)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                return NotFound($"El producto con el id ${productId} no existe");
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [AllowAnonymous]
        [HttpGet("Page", Name = "GetProductInPage")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductInPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize=5)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest("Los parametros de paginacion no son validos");
            var totalProducts = _productRepository.GetTotalProducts();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            if (pageNumber > totalPages) return NotFound("No hay mas paginas disponibles");
            var products = _productRepository.GetProductsInPage(pageNumber, pageSize);
            var productDto = _mapper.Map<List<ProductDto>>(products);
            var paginationResponse = new PaginationResponse<ProductDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = productDto,
            };
            return Ok(paginationResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePoductAsync([FromForm] CreateProductDto createProductDto)
        {
            if (createProductDto == null) return BadRequest(ModelState);
            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomErro", "El producto ya existe");
                return BadRequest(ModelState);
            }
            if (!_categoryrepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomErro", "La categoria no existe");
                return BadRequest(ModelState);
            }
            var product = _mapper.Map<Product>(createProductDto);
            var (imageUrl, imgUrlLocal) = await _fileService.SaveProductImageAsync(
                createProductDto.Image, product.ProductId.ToString(), HttpContext.Request, "ProductsImages"
            );
            product.ImgUrl = imageUrl;
            product.ImgUrlLocal = imgUrlLocal;
            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomErro", $"Algo salio mal al guardar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }
            var createProduct = _productRepository.GetProduct(product.ProductId);
            var productDto = _mapper.Map<ProductDto>(createProduct);
            return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productDto);
        }

        [HttpGet("searchProductByCategory/{CategoryId:int}", Name = "GetProductForCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductForCategory(int CategoryId)
        {
            var products = _productRepository.GetProductsForCategory(CategoryId);
            if (products.Count == 0)
            {
                return NotFound($"No hay Productos con esta categoria");
            }
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("searchProductByNameDescription/{searchterm}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SearchProducts(string searchterm)
        {
            var products = _productRepository.SearchProducts(searchterm);
            if (products.Count == 0)
            {
                return NotFound($"No hay Productos con el nombre o descripcion {searchterm}.");
            }
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BuyProduct(string name, int quantity)
        {
            if (string.IsNullOrEmpty(name) || quantity <= 0)
                return BadRequest("El nombre del producto o la cantidad no son validos");

            var foundProduct = _productRepository.ProductExists(name);
            if (!foundProduct) return NotFound($"El producto con el nombre  {name} no existe");

            if (!_productRepository.BuyProduct(name, quantity))
            {
                ModelState.AddModelError("CustomError", $"No se pudo comprar el producto {name} o la cantidad solicitada es mayor al stock disponible");
                return BadRequest(ModelState);
            }
            var units = quantity == 1 ? "unidad" : "unidades";
            return Ok($"Se compro {quantity} {units} del producto '{name}'");
        }

        [HttpPut("{productId:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePoductAsync(int productId, [FromBody] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null) return BadRequest(ModelState);
            if (!_productRepository.ProductExists(productId))
            {
                ModelState.AddModelError("CustomErro", "El producto no existe");
                return BadRequest(ModelState);
            }
            if (!_categoryrepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomErro", $"La categoria no exite");
                return BadRequest(ModelState);
            }
            var product = _mapper.Map<Product>(updateProductDto);
            product.ProductId = productId;
            var (imageUrl, imgUrlLocal) = await _fileService.SaveProductImageAsync(
                updateProductDto.Image, product.ProductId.ToString(), HttpContext.Request, "ProductsImages"
            );
            product.ImgUrl = imageUrl;
            product.ImgUrlLocal = imgUrlLocal;
            if (!_productRepository.UpdateProductDto(product))
            {
                ModelState.AddModelError("CustomErro", $"Algo salio mal al actualizar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteProduct(int productId)
        {
            if (productId == 0)
            {
                return BadRequest(ModelState);
            }
            var product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                return NotFound($"El producto con el id ${productId} no existe");
            }
            if (!_productRepository.DeleteProductDto(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal al eliminar el producto {product.Name}");

            }
            return NoContent();
        }

    }
}
