namespace Licoreria.Pruebas;

public class PruebasArchivo
{
    [Fact]
    public void GuardarYCargarProductos_ConservaCodigoPrecioYStock()
    {
        string carpeta = CarpetaTemporal();
        ArchivoDatos archivos = new ArchivoDatos(carpeta);
        List<Producto> original = new List<Producto>();
        original.Add(new Producto
        {
            Codigo = 1,
            Nombre = "Cerveza Pilsen 620 ml",
            Precio = 6.50,
            Stock = 22
        });

        archivos.GuardarProductos(original);
        List<Producto> cargados = archivos.CargarProductos();

        Assert.Single(cargados);
        Assert.Equal(1, cargados[0].Codigo);
        Assert.Equal("Cerveza Pilsen 620 ml", cargados[0].Nombre);
        Assert.Equal(6.50, cargados[0].Precio, 2);
        Assert.Equal(22, cargados[0].Stock);
    }

    [Fact]
    public void GuardarYCargarVentas_ConservaElTotal()
    {
        string carpeta = CarpetaTemporal();
        ArchivoDatos archivos = new ArchivoDatos(carpeta);
        List<Venta> original = new List<Venta>();
        original.Add(new Venta
        {
            Numero = 1,
            Cliente = "Ana",
            CodigoProducto = 1,
            NombreProducto = "Cerveza Pilsen 620 ml",
            Cantidad = 2,
            PrecioUnitario = 6.50,
            Total = 13.00,
            Fecha = "11/10/2026 21:00"
        });

        archivos.GuardarVentas(original);
        List<Venta> cargadas = archivos.CargarVentas();

        Assert.Single(cargadas);
        Assert.Equal("Ana", cargadas[0].Cliente);
        Assert.Equal(13.00, cargadas[0].Total, 2);
        Assert.Equal("11/10/2026 21:00", cargadas[0].Fecha);
    }

    [Fact]
    public void Cargar_SiElArchivoNoExiste_DevuelveListaVacia()
    {
        string carpeta = CarpetaTemporal();
        ArchivoDatos archivos = new ArchivoDatos(carpeta);

        Assert.Empty(archivos.CargarProductos());
        Assert.Empty(archivos.CargarVentas());
    }

    [Fact]
    public void CargarProductos_IgnoraUnaLineaMalEscrita()
    {
        string carpeta = CarpetaTemporal();
        ArchivoDatos archivos = new ArchivoDatos(carpeta);
        File.WriteAllLines(archivos.RutaProductos, new[]
        {
            "esto no es un producto",
            "2|Agua San Luis 625 ml|2.00|30"
        });

        List<Producto> cargados = archivos.CargarProductos();

        Assert.Single(cargados);
        Assert.Equal(2, cargados[0].Codigo);
    }

    private static string CarpetaTemporal()
    {
        string carpeta = Path.Combine(Path.GetTempPath(), "licoreria-prueba-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(carpeta);
        return carpeta;
    }
}
