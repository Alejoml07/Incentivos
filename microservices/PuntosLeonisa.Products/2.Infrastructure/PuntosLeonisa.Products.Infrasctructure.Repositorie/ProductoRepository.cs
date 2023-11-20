using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using System.Linq.Expressions;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Products.Domain.Service.DTO.Genericos;
using System.Linq;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using System.Reflection.Metadata;

namespace PuntosLeonisa.Products.Infrasctructure.Repositorie;
public class ProductoRepository : Repository<Producto>, IProductoRepository
{
    private readonly ProductContext _context;
    public ProductoRepository(ProductContext context) : base(context)
    {
        _context = context;
    }
    #region Privados
    private static object ConvertToType(string value, Type targetType)
    {
        if (targetType == typeof(string))
        {
            return value;
        }
        else if (targetType == typeof(float?) && float.TryParse(value, out var floatValue))
        {
            return floatValue;
        }
        else if (targetType == typeof(double?) && double.TryParse(value, out var doubleValue))
        {
            return doubleValue;
        }
        else if (targetType == typeof(DateTime?) && DateTime.TryParse(value, out var dateTimeValue))
        {
            return dateTimeValue;
        }
        else if (targetType == typeof(char?) && value.Length == 1)
        {
            return value[0];
        }
        // Aquí puedes añadir más tipos según sea necesario
        else
        {
            throw new InvalidCastException($"No se puede convertir el valor '{value}' al tipo '{targetType.Name}'.");
        }
    }

    #endregion

    #region Publicos

    public async Task<PagedResult<Producto>> GetProductsByFiltersAndRange(ProductosFilters queryObject)
    {
        var query = _context.Set<Producto>().AsQueryable();
        Expression? combinedExpression = null;
        var parameter = Expression.Parameter(typeof(Producto), "p");

        // Construyendo filtros
        foreach (var filter in queryObject.Filters)
        {
            var key = filter.Key;
            var values = filter.Value; // Lista de valores

            var propertyInfo = typeof(Producto).GetProperty(key);
            if (propertyInfo == null) continue;

            Expression? orExpression = null;
            foreach (var value in values)
            {
                // Convertir el valor char a string
                string stringValue = value.ToString();

                // Ahora llama a ConvertToType con una cadena
                var convertedValue = ConvertToType(stringValue, propertyInfo.PropertyType);

                var member = Expression.Property(parameter, propertyInfo);
                var constant = Expression.Constant(convertedValue, propertyInfo.PropertyType);
                var equalExpression = Expression.Equal(member, constant);

                orExpression = orExpression == null ? equalExpression : Expression.OrElse(orExpression, equalExpression);
            }

            if (orExpression != null)
            {
                combinedExpression = combinedExpression == null ? orExpression : Expression.AndAlso(combinedExpression, orExpression);
            }
        }

        if (combinedExpression != null)
        {
            var lambda = Expression.Lambda<Func<Producto, bool>>(combinedExpression, parameter);
            query = query.Where(lambda);
        }

        // Ordenamiento
        if (queryObject.OrderBy != null)
        {
            var orderMode = queryObject.OrderMode.ToUpper() == "ASC" ? "OrderBy" : "OrderByDescending";
            var propertyInfo = typeof(Producto).GetProperty(queryObject.OrderBy);
            if (propertyInfo != null)
            {
                var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                MethodCallExpression resultExp = Expression.Call(typeof(Queryable), orderMode, new Type[] { typeof(Producto), propertyInfo.PropertyType }, query.Expression, Expression.Quote(orderByExp));
                query = query.Provider.CreateQuery<Producto>(resultExp);
            }
        }

        // Paginación
        var skip = (queryObject.Page - 1) * queryObject.PageSize;
        var take = queryObject.PageSize;

        var pagedData = await query.Skip(skip).Take(take).ToListAsync();

        // Resultado paginado
        var totalCount = await query.CountAsync();
        var pagedResult = new PagedResult<Producto>
        {
            Data = pagedData,
            TotalCount = totalCount,
            PageNumber = queryObject.Page,
            PageSize = queryObject.PageSize
        };

        return pagedResult;
    }

    public async Task<FiltroDto> ObtenerFiltros(GeneralFiltersWithResponseDto generalFiltersWithResponseDto)
    {
        List<Producto>? productos = null;
        if (generalFiltersWithResponseDto?.ApplyFiltro != null)
        {
           var pagedResult = await this.GetProductsByFiltersAndRange(generalFiltersWithResponseDto.ApplyFiltro);
            productos = pagedResult.Data.ToList();
        }
        else
        {
            // Obtén todos los productos
            productos = await _context.Set<Producto>().ToListAsync();
        }
        // Agrupar categorías y subcategorías en memoria
        var categoriasConSubcategorias = productos
            .GroupBy(p => p.CategoriaNombre)
            .Select(group => new Categoria
            {
                CategoriaNombre = group.Key,
                Subcategorias = group.Select(g => g.SubCategoriaNombre).Distinct().ToList()
            })
            .ToList();

        // Obtener marcas únicas
        var marcas = productos
            .Select(p => p.Marca)
            .Distinct()
            .ToList();

        // Calcular los puntos máximos y mínimos, ignorando los nulos
        var puntosValidos = productos
            .Select(p => p.Puntos)
            .Where(p => p.HasValue)
            .Select(p => p.Value);

        int puntosMin = puntosValidos.Any() ? (int)puntosValidos.Min() : 0;
        int puntosMax = puntosValidos.Any() ? (int)puntosValidos.Max() : 0;

        FiltroDto filtroDto = new FiltroDto
        {
            Categorias = categoriasConSubcategorias,
            Marca = marcas,
            PuntosMin = puntosMin,
            PuntosMax = puntosMax
        };

        return filtroDto;
    }

    #endregion
}