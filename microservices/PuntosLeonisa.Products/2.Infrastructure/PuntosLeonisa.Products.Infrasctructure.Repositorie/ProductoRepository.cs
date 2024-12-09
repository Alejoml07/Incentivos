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
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using PuntosLeonisa.Products.Domain.Model;

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
    private int CheckDynamicType(dynamic dynamicObject)
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



    public async Task<PagedResult<IGrouping<string, Producto>>> GetProductsByFiltersAndRange(ProductosFilters queryObject)
    {

        var query = _context.Set<Producto>().Where(x => x.Roles.Contains(queryObject.TipoUsuario) && x.Puntos != null && x.Cantidad != 0 && x.Estado != "2" && x.Cantidad != null).AsQueryable(); // x.ProveedorLite.CiudadRestringida.Contains(queryObject.Ciudad) &&

        Expression? combinedExpression = null;
        var parameter = Expression.Parameter(typeof(Producto), "p");
        var maxPropertyEnd = queryObject.MaxRangePropertyNameEnd;
        var minPropertyEnd = queryObject.MinRangePropertyNameEnd;



        //[El resto del código de construcción de filtros permanece igual]
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
                    var convertedValue = ConvertToType(values, propertyInfo.PropertyType);

                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        convertedValue = convertedValue.ToString().ToUpper();
                    }

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
                        // Convertir el valor de la propiedad a mayúsculas para la comparación
                        var upperMember = Expression.Call(member, typeof(string).GetMethod("ToUpper", new Type[] { }));
                        var constant = Expression.Constant(convertedValue, typeof(string));
                        expression = Expression.Equal(upperMember, constant);
                    }

                    //combinedExpression = combinedExpression == null ? expression : Expression.AndAlso(combinedExpression, expression);
                    orExpression = orExpression == null ? expression : Expression.AndAlso(orExpression, expression);
                    break;
                case 2:
                    var valuesList = values as IEnumerable;
                    foreach (var value in valuesList)
                    {
                        // Convertir el valor char a string
                        string stringValue2 = value.ToString().ToUpper();

                        // Ahora llama a ConvertToType con una cadena
                        var convertedValue2 = ConvertToType(stringValue2, propertyInfo.PropertyType);

                        if (propertyInfo.PropertyType == typeof(string))
                        {
                            convertedValue2 = convertedValue2.ToString().ToUpper();
                        }

                        var member2 = Expression.Property(parameter, propertyInfo);
                        // Convertir el valor de la propiedad a mayúsculas para la comparación
                        var upperMember2 = Expression.Call(member2, typeof(string).GetMethod("ToUpper", new Type[] { }));
                        var constant2 = Expression.Constant(stringValue2, typeof(string));
                        var equalExpression2 = Expression.Equal(upperMember2, constant2);

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

        //// Ordenamiento
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


        // Agrupar por referencia y luego aplicar paginación en los grupos
        // Realizar la agrupación en la base de datos y recuperar solo los identificadores de grupo
        // Recuperar los datos filtrados en memoria
        var allFilteredData = await query.ToListAsync();

        // Agrupar los datos en memoria
        var groupedData = allFilteredData.GroupBy(p => p.Referencia)
                                         .Skip((queryObject.Page - 1) * queryObject.PageSize)
                                         .Take(queryObject.PageSize)
                                         .ToList();

        // Calcular el total de grupos
        int totalGroups = allFilteredData.GroupBy(p => p.Referencia).Count();

        // Resultado paginado con agrupación
        var pagedResult = new PagedResult<IGrouping<string, Producto>>
        {
            Data = groupedData,
            TotalCount = totalGroups,
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
            // Obtén los productos filtrados sin paginación
            var filtros = generalFiltersWithResponseDto.ApplyFiltro;
            if (filtros != null)
            {
                filtros.Page = 1;
                filtros.PageSize = int.MaxValue;
            }
            var pagedResult = await GetProductsByFiltersAndRange(filtros ?? generalFiltersWithResponseDto.ApplyFiltro);
            productos = pagedResult.Data.SelectMany(group => group).ToList();

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
            .GroupBy(p => p.Marca.ToUpper())
            .Select(p => p.Key.ToUpper())
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
            var response = await _context.Set<Producto>().Where(p => p.Referencia == referencia).ToListAsync();
            return response;
        }
    }

    public async Task<IEnumerable<Producto>> GetProductByProveedor(string nit)
    {
        {
            var response = await _context.Set<Producto>().Where(p => p.ProveedorLite.Nit == nit && p.Estado != "2").ToListAsync();
            return response;
        }      
    }

    public async Task<IEnumerable<Producto>> GetProductByProveedorOrAll(TipoUsuarioDto[] data)
    {
        var productos = new HashSet<Producto>();

        foreach (var item in data)
        {
            if (item.TipoUsuario == "0" && item.Proveedor == "0")
            {
                var todosProductos = await _context.Set<Producto>()
                                                   
                                                   .ToListAsync();
                productos.UnionWith(todosProductos);
            }
            if (item.TipoUsuario == "0" && item.Proveedor != "0")
            {
                var productosPorProveedor = await _context.Set<Producto>()
                                                          .Where(x => x.ProveedorLite.Nombres == item.Proveedor)
                                                          .ToListAsync();
                productos.UnionWith(productosPorProveedor);
            }
            if (item.TipoUsuario != "0" && item.Proveedor == "0")
            {
                var productosPorTipoUsuario = await _context.Set<Producto>()
                                                            .Where(x => x.Roles.Contains(item.TipoUsuario))
                                                            .ToListAsync();
                productos.UnionWith(productosPorTipoUsuario);
            }
        }

        return productos.ToList();
    }

    public async Task<PagedResult<IGrouping<string, Producto>>> GetProductsForSearch(SearchDto data)
    {
        var Page = 1;
        var PageSize = 12;
        List<Producto>? productos = null;
        productos = await _context.Set<Producto>().Where(x => x.Nombre.ToLower().Contains(data.Busqueda)).Where(x => x.Roles.Contains(data.TipoUsuario) && x.Puntos != null && x.Cantidad != 0 && x.Estado != "2" && x.Cantidad != null).ToListAsync();

        // Agrupar los datos en memoria
        var groupedData = productos.GroupBy(p => p.Referencia)
                                         .Skip((Page - 1) * PageSize)
                                         .Take(PageSize)
                                         .ToList();

        // Calcular el total de grupos
        int totalGroups = productos.GroupBy(p => p.Referencia).Count();

        // Resultado paginado con agrupación
        var pagedResult = new PagedResult<IGrouping<string, Producto>>
        {
            Data = groupedData,
            TotalCount = totalGroups,
            PageNumber = Page,
            PageSize = PageSize
        };

        return pagedResult;
    }

    public async Task<PagedResult<IGrouping<string, Producto>>> GetProductsForSearchAll(SearchDto data)
    {
        var Page = 1;
        var PageSize = 12;
        List<Producto>? productos = null;
        productos = await _context.Set<Producto>().Where(x => x.Roles.Contains(data.TipoUsuario) && x.Puntos != null && x.Cantidad != 0 && x.Estado != "2" && x.Cantidad != null).ToListAsync();

        // Agrupar los datos en memoria
        var groupedData = productos.GroupBy(p => p.Referencia)
                                         .Skip((Page - 1) * PageSize)
                                         .Take(PageSize)
                                         .ToList();

        // Calcular el total de grupos
        int totalGroups = productos.GroupBy(p => p.Referencia).Count();

        // Resultado paginado con agrupación
        var pagedResult = new PagedResult<IGrouping<string, Producto>>
        {
            Data = groupedData,
            TotalCount = totalGroups,
            PageNumber = Page,
            PageSize = PageSize
        };

        return pagedResult;
    }

    public async Task<IEnumerable<Producto>> GetProductByName(string nombre)
    {
        var response = await _context.Set<Producto>().Where(p => p.Nombre == nombre).ToListAsync();
        return response;
    }

    #endregion


}