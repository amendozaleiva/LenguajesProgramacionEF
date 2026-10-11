namespace Licoreria.Pruebas;

public class PruebasCaja
{
    [Fact]
    public void CalcularTotal_DosPilsen_DevuelveTrece()
    {
        double total = CajaLicoreria.CalcularTotal(6.50, 2);

        Assert.Equal(13.00, total, 2);
    }

    [Fact]
    public void RegistrarPedido_ConStock_DescuentaYAgregaLaVenta()
    {
        CajaLicoreria caja = CajaConPilsen();

        string mensaje;
        bool ok = caja.RegistrarPedido("Ana", 1, 2, out mensaje);

        Assert.True(ok);
        Assert.Equal(22, caja.Productos[0].Stock);
        Assert.Single(caja.Ventas);
        Assert.Equal(13.00, caja.Ventas[0].Total, 2);
        Assert.Equal("Ana", caja.Ventas[0].Cliente);
    }

    [Fact]
    public void RegistrarPedido_SinStock_NoDescuentaNiVende()
    {
        CajaLicoreria caja = CajaConPilsen();

        string mensaje;
        bool ok = caja.RegistrarPedido("Luis", 1, 100, out mensaje);

        Assert.False(ok);
        Assert.Equal(24, caja.Productos[0].Stock);
        Assert.Empty(caja.Ventas);
        Assert.Contains("stock", mensaje);
    }

    [Fact]
    public void RegistrarPedido_CantidadCero_NoRegistra()
    {
        CajaLicoreria caja = CajaConPilsen();

        string mensaje;
        bool ok = caja.RegistrarPedido("Luis", 1, 0, out mensaje);

        Assert.False(ok);
        Assert.Empty(caja.Ventas);
    }

    [Fact]
    public void RegistrarPedido_CodigoQueNoExiste_NoRegistra()
    {
        CajaLicoreria caja = CajaConPilsen();

        string mensaje;
        bool ok = caja.RegistrarPedido("Luis", 99, 1, out mensaje);

        Assert.False(ok);
        Assert.Empty(caja.Ventas);
    }

    [Fact]
    public void RegistrarPedido_ClienteVacio_NoRegistra()
    {
        CajaLicoreria caja = CajaConPilsen();

        string mensaje;
        bool ok = caja.RegistrarPedido("   ", 1, 1, out mensaje);

        Assert.False(ok);
        Assert.Equal(24, caja.Productos[0].Stock);
    }

    [Fact]
    public void SumarTotales_DosVentas_SumaLosImportes()
    {
        List<Venta> ventas = new List<Venta>();
        ventas.Add(new Venta { Total = 13.00 });
        ventas.Add(new Venta { Total = 6.50 });

        double suma = CajaLicoreria.SumarTotales(ventas);

        Assert.Equal(19.50, suma, 2);
    }

    [Fact]
    public void VentasDeLaFecha_SoloDevuelveLasDeEseDia()
    {
        CajaLicoreria caja = new CajaLicoreria();
        caja.Ventas.Add(new Venta { Numero = 1, Total = 10, Fecha = "11/10/2026 10:00" });
        caja.Ventas.Add(new Venta { Numero = 2, Total = 5, Fecha = "10/10/2026 10:00" });

        List<Venta> delDia = caja.VentasDeLaFecha("11/10/2026");

        Assert.Single(delDia);
        Assert.Equal(1, delDia[0].Numero);
    }

    private static CajaLicoreria CajaConPilsen()
    {
        CajaLicoreria caja = new CajaLicoreria();
        caja.Productos.Add(new Producto
        {
            Codigo = 1,
            Nombre = "Cerveza Pilsen 620 ml",
            Precio = 6.50,
            Stock = 24
        });
        return caja;
    }
}
