namespace Licoreria;

public class Venta
{
    public int Numero { get; set; }
    public string Cliente { get; set; } = "";
    public int CodigoProducto { get; set; }
    public string NombreProducto { get; set; } = "";
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
    public double Total { get; set; }
    public string Fecha { get; set; } = "";
}
