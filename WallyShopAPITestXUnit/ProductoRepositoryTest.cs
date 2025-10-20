
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallyShopAPI.Entidades;
using WallyShopAPI.Repositorios;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WallyShopAPITestXUnit
{
    public class ProductoRepositoryTest : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ProductoRepository _repository;

        public ProductoRepositoryTest()
        {
            // Configurar DbContext en memoria para pruebas
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nombre único por test
                .Options;

            _context = new AppDbContext(options);
            _repository = new ProductoRepository(_context);

            // Seed data inicial
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Agregar datos de prueba
            var rol = new Rol
            {
                Id = 1,
                Nombre = "Usuario"
            };

            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Usuario Test",
                Email = "test@test.com",
                RolId = 1,
                Rol = rol
            };

            var productos = new List<Producto>
            {
                new Producto
                {
                    Id = 1,
                    Nombre = "Producto 1",
                    Descripcion = "Descripción 1",
                    Precio = 100.50m,
                    Estado = true,
                    UsuarioId = 1,
                    Usuario = usuario,
                    Imagen = new byte[0]
                },
                new Producto
                {
                    Id = 2,
                    Nombre = "Producto 2",
                    Descripcion = "Descripción 2",
                    Precio = 200.75m,
                    Estado = true,
                    UsuarioId = 1,
                    Usuario = usuario,
                    Imagen = new byte[0]
                },
                new Producto
                {
                    Id = 3,
                    Nombre = "Producto Inactivo",
                    Descripcion = "Descripción 3",
                    Precio = 150.00m,
                    Estado = false,
                    UsuarioId = 1,
                    Usuario = usuario,
                    Imagen = new byte[0]
                }
            };

            // Agregar en el orden correcto para respetar las relaciones
            _context.Roles.Add(rol);
            _context.Usuarios.Add(usuario);
            _context.productos.AddRange(productos);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Fact]
        public async Task GetAllAsync_RetornaTodosLosProductos()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, p => p.Nombre == "Producto 1");
            Assert.Contains(result, p => p.Nombre == "Producto 2");
            Assert.Contains(result, p => p.Nombre == "Producto Inactivo");
        }

        [Fact]
        public async Task GetAllAsync_IncluyeUsuarioRelacionado()
        {
            // Act
            var result = await _repository.GetAllAsync();
            var producto = result.First();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(producto.Usuario);
            Assert.Equal("Usuario Test", producto.Usuario.Nombre);
            Assert.NotNull(producto.Usuario.Rol);
            Assert.Equal("Usuario", producto.Usuario.Rol.Nombre);
        }

        [Fact]
        public async Task GetByIdAsync_RetornaProductoCuandoExiste()
        {
            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Producto 1", result.Nombre);
            Assert.Equal(100.50m, result.Precio);
            Assert.True(result.Estado);
            Assert.Equal("Descripción 1", result.Descripcion);
        }

        [Fact]
        public async Task GetByIdAsync_IncluyeUsuarioYRol()
        {
            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Usuario);
            Assert.Equal("Usuario Test", result.Usuario.Nombre);
            Assert.Equal("test@test.com", result.Usuario.Email);
            Assert.NotNull(result.Usuario.Rol);
            Assert.Equal("Usuario", result.Usuario.Rol.Nombre);
        }

        [Fact]
        public async Task GetByIdAsync_RetornaNullCuandoNoExiste()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_AgregaProductoCorrectamente()
        {
            // Arrange
            var nuevoProducto = new Producto
            {
                Nombre = "Nuevo Producto",
                Descripcion = "Nueva Descripción",
                Precio = 300.00m,
                Estado = true,
                UsuarioId = 1,
                Imagen = new byte[] { 1, 2, 3 }
            };

            // Act
            var result = await _repository.AddAsync(nuevoProducto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Nuevo Producto", result.Nombre);
            Assert.True(result.Id > 0); // Verificar que se asignó un ID
            Assert.Equal(new byte[] { 1, 2, 3 }, result.Imagen);

            // Verificar que realmente se guardó en la base de datos
            var productoEnDb = await _context.productos.FindAsync(result.Id);
            Assert.NotNull(productoEnDb);
            Assert.Equal("Nuevo Producto", productoEnDb.Nombre);
        }

        [Fact]
        public async Task UpdateAsync_ActualizaProductoCorrectamente()
        {
            // Arrange
            var productoExistente = await _repository.GetByIdAsync(1);
            productoExistente.Nombre = "Producto Actualizado";
            productoExistente.Descripcion = "Descripción Actualizada";
            productoExistente.Precio = 150.00m;
            productoExistente.Estado = false;
            productoExistente.Imagen = new byte[] { 4, 5, 6 };

            // Act
            var result = await _repository.UpdateAsync(productoExistente);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Producto Actualizado", result.Nombre);
            Assert.Equal("Descripción Actualizada", result.Descripcion);
            Assert.Equal(150.00m, result.Precio);
            Assert.False(result.Estado);
            Assert.Equal(new byte[] { 4, 5, 6 }, result.Imagen);

            // Verificar que los cambios persisten en la base de datos
            var productoActualizado = await _repository.GetByIdAsync(1);
            Assert.Equal("Producto Actualizado", productoActualizado.Nombre);
        }

        [Fact]
        public async Task UpdateAsync_MantieneRelaciones()
        {
            // Arrange
            var productoExistente = await _repository.GetByIdAsync(1);
            var usuarioOriginal = productoExistente.Usuario;

            productoExistente.Nombre = "Producto Modificado";

            // Act
            var result = await _repository.UpdateAsync(productoExistente);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Usuario);
            Assert.Equal(usuarioOriginal.Id, result.Usuario.Id);
            Assert.Equal(usuarioOriginal.Nombre, result.Usuario.Nombre);
        }

        [Fact]
        public async Task DeleteAsync_EliminaProductoCuandoExiste()
        {
            // Act
            var result = await _repository.DeleteAsync(1);

            // Assert
            Assert.True(result);

            // Verificar que el producto ya no existe
            var productoEliminado = await _repository.GetByIdAsync(1);
            Assert.Null(productoEliminado);
        }

        [Fact]
        public async Task DeleteAsync_NoEliminaCotizacionesRelacionadas()
        {
            // Arrange - Crear una cotización relacionada
            var cotizacion = new Cotizacion
            {
                Id = 1,
                ProductoId = 1,
                UsuarioId = 1,
                Fecha = DateTime.Now,
                Contacto = 1234
            };
            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(1);

            // Assert
            Assert.True(result);

            // Verificar que la cotización sigue existiendo (por el DeleteBehavior.Restrict)
            var cotizacionEnDb = await _context.Cotizaciones.FindAsync(1);
            Assert.NotNull(cotizacionEnDb);
        }

        [Fact]
        public async Task DeleteAsync_RetornaFalseCuandoProductoNoExiste()
        {
            // Act
            var result = await _repository.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddAsync_ProductoConImagenVacia()
        {
            // Arrange
            var nuevoProducto = new Producto
            {
                Nombre = "Producto Sin Imagen",
                Descripcion = "Descripción sin imagen",
                Precio = 250.00m,
                Estado = true,
                UsuarioId = 1,
                Imagen = Array.Empty<byte>()
            };

            // Act
            var result = await _repository.AddAsync(nuevoProducto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Producto Sin Imagen", result.Nombre);
            Assert.Empty(result.Imagen);
        }

        [Fact]
        public async Task GetAllAsync_ProductosConRelacionesCompletas()
        {
            // Act
            var result = await _repository.GetAllAsync();
            var producto = result.First();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, p =>
            {
                Assert.NotNull(p.Usuario);
                Assert.NotNull(p.Usuario.Rol);
                Assert.NotNull(p.Cotizaciones);
            });
        }
    }
}