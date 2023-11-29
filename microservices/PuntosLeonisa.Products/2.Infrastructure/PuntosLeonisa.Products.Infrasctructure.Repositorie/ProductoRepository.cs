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
using System.Collections;

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
    private  int CheckDynamicType(dynamic dynamicObject)
    {
        // Chequea si el objeto es nulo
        if (dynamicObject == null)
        {
            Console.WriteLine("El objeto es nulo.");
            throw new ArgumentException("objeto nulo");
        }

        Type type = dynamicObject.GetType();

        // Chequea si es un string
        if (type == typeof(string))
        {
            return 1;
        }
        // Chequea si es una colección (por ejemplo, listas, arrays, etc.)
        else if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            return 2;
        }
        else
        {
            Console.WriteLine($"El objeto es de tipo: {type}");
            return 3;
        }
    }
    #endregion

    #region Publicos

    public async Task<PagedResult<Producto>> GetProductsByFiltersAndRange(ProductosFilters queryObject)
    {
        var query = _context.Set<Producto>().AsQueryable();
        Expression? combinedExpression = null;
        var parameter = Expression.Parameter(typeof(Producto), "p");
        var maxPropertyEnd = queryObject.MaxRangePropertyNameEnd;
        var minPropertyEnd = queryObject.MinRangePropertyNameEnd;

        // Construyendo filtros
        foreach (var filter in queryObject.Filters)
        {
            var key = filter.Key;
            var values = filter.Value; // Lista de valores

            var propertyInfo = typeof(Producto).GetProperty(filter.Key.Replace(maxPropertyEnd, "").Replace(minPropertyEnd, ""));
            if (propertyInfo == null)
            {
                continue; // La propiedad no existe en el modelo, se ignora el filtro
            }

            Expression expression;

            Expression? orExpression = null;
            var type = CheckDynamicType(values);
            switch (type)
            {
                case 1:
                   var  convertedValue = ConvertToType(values, propertyInfo.PropertyType);

                    if (key.EndsWith(maxPropertyEnd) || key.EndsWith(minPropertyEnd))
                    {
                        var actualKey = key.Replace(maxPropertyEnd, "").Replace(minPropertyEnd, "");
                        var member = Expression.Property(parameter, actualKey);
                        var constant = Expression.Constant(convertedValue, propertyInfo.PropertyType);

                        expression = key.EndsWith(maxPropertyEnd) ? Expression.LessThanOrEqual(member, constant) : Expression.GreaterThanOrEqual(member, constant);
                    }
                    else
                    {
                        var member = Expression.Property(parameter, propertyInfo);
                        expression = Expression.Equal(member, Expression.Constant(convertedValue, propertyInfo.PropertyType));
                    }

                    //combinedExpression = combinedExpression == null ? expression : Expression.AndAlso(combinedExpression, expression);
                    orExpression = orExpression == null ? expression : Expression.AndAlso(orExpression, expression);
                    break;
                case 2:
                    var valuesList = values as IEnumerable;
                    foreach (var value in valuesList)
                    {
                        // Convertir el valor char a string
                        string stringValue2 = value.ToString();

                        // Ahora llama a ConvertToType con una cadena
                        var convertedValue2 = ConvertToType(stringValue2, propertyInfo.PropertyType);

                        var member2 = Expression.Property(parameter, propertyInfo);
                        var constant2 = Expression.Constant(convertedValue2, propertyInfo.PropertyType);
                        var equalExpression2 = Expression.Equal(member2, constant2);

                        orExpression = orExpression == null ? equalExpression2 : Expression.OrElse(orExpression, equalExpression2);
                    }
                    break;
                case 3:
                    break;
                default:
                    break;
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
            .GroupBy(p => p.Marca)
            .Select(p => p.Key)
            .Distinct()
            .ToList();

        // Calcular los puntos máximos y mínimos, ignorando los nulos
        var puntosValidos = await _context.Set<Producto>().Select(p => p.Puntos).ToListAsync();

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

    public async Task<IEnumerable<Producto>> GetByRef(string referencia)
    {
        {
            var response = await _context.Set<Producto>().Where(p=>p.Referencia==referencia).ToListAsync();
            return response;
        }
    }

    #endregion
}