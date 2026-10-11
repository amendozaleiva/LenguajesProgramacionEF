using System.Globalization;

namespace Licoreria;

public class ArchivoDatos
{
    private readonly string _rutaProductos;
    private readonly string _rutaVentas;

    public ArchivoDatos(string carpeta)
    {
        Directory.CreateDirectory(carpeta);
        _rutaProductos = Path.Combine(carpeta, "productos.txt");
        _rutaVentas = Path.Combine(carpeta, "ventas.txt");
    }

    public string RutaProductos => _rutaProductos;
    public string RutaVentas => _rutaVentas;

    public List<Producto> CargarProductos()
    {
        List<Producto> lista = new List<Producto>();
        if (!File.Exists(_rutaProductos))
        {
            return lista;
        }

        string[] lineas = File.ReadAllLines(_rutaProductos);
        for (int i = 0; i < lineas.Length; i++)
        {
            Producto? producto = LeerProducto(lineas[i]);
            if (producto != null)
            {
                lista.Add(producto);
            }
        }

        return lista;
    }

    public void GuardarProductos(List<Producto> productos)
    {
        List<string> lineas = new List<string>();
        for (int i = 0; i < productos.Count; i++)
        {
            Producto producto = productos[i];
            lineas.Add(
                producto.Codigo
                + "|" + producto.Nombre
                + "|" + producto.Precio.ToString("0.00", CultureInfo.InvariantCulture)
                + "|" + producto.Stock);
        }

        File.WriteAllLines(_rutaProductos, lineas);
    }

    public List<Venta> CargarVentas()
    {
        List<Venta> lista = new List<Venta>();
        if (!File.Exists(_rutaVentas))
        {
            return lista;
        }

        string[] lineas = File.ReadAllLines(_rutaVentas);
        for (int i = 0; i < lineas.Length; i++)
        {
            Venta? venta = LeerVenta(lineas[i]);
            if (venta != null)
            {
                lista.Add(venta);
            }
        }

        return lista;
    }

    public void GuardarVentas(List<Venta> ventas)
    {
        List<string> lineas = new List<string>();
        for (int i = 0; i < ventas.Count; i++)
        {
            Venta venta = ventas[i];
            lineas.Add(
                venta.Numero
                + "|" + venta.Cliente
                + "|" + venta.CodigoProducto
                + "|" + venta.NombreProducto
                + "|" + venta.Cantidad
                + "|" + venta.PrecioUnitario.ToString("0.00", CultureInfo.InvariantCulture)
                + "|" + venta.Total.ToString("0.00", CultureInfo.InvariantCulture)
                + "|" + venta.Fecha);
        }

        File.WriteAllLines(_rutaVentas, lineas);
    }

    public static List<Producto> ProductosIniciales()
    {
        List<Producto> lista = new List<Producto>();
        lista.Add(new Producto { Codigo = 1, Nombre = "Cerveza Pilsen 620 ml", Precio = 6.50, Stock = 24 });
        lista.Add(new Producto { Codigo = 2, Nombre = "Cerveza Cristal 620 ml", Precio = 6.00, Stock = 18 });
        lista.Add(new Producto { Codigo = 3, Nombre = "Pisco Quebranta 750 ml", Precio = 42.00, Stock = 10 });
        lista.Add(new Producto { Codigo = 4, Nombre = "Ron Cartavio 750 ml", Precio = 36.00, Stock = 8 });
        lista.Add(new Producto { Codigo = 5, Nombre = "Vino tinto 750 ml", Precio = 25.00, Stock = 6 });
        lista.Add(new Producto { Codigo = 6, Nombre = "Agua San Luis 625 ml", Precio = 2.00, Stock = 30 });
        lista.Add(new Producto { Codigo = 7, Nombre = "Gaseosa Inca Kola 500 ml", Precio = 3.50, Stock = 20 });
        return lista;
    }

    private static Producto? LeerProducto(string linea)
    {
        if (string.IsNullOrWhiteSpace(linea))
        {
            return null;
        }

        string[] partes = linea.Split('|');
        if (partes.Length != 4)
        {
            return null;
        }

        int codigo;
        double precio;
        int stock;
        if (!int.TryParse(partes[0], out codigo))
        {
            return null;
        }

        if (!double.TryParse(partes[2], NumberStyles.Float, CultureInfo.InvariantCulture, out precio))
        {
            return null;
        }

        if (!int.TryParse(partes[3], out stock))
        {
            return null;
        }

        Producto producto = new Producto();
        producto.Codigo = codigo;
        producto.Nombre = partes[1];
        producto.Precio = precio;
        producto.Stock = stock;
        return producto;
    }

    private static Venta? LeerVenta(string linea)
    {
        if (string.IsNullOrWhiteSpace(linea))
        {
            return null;
        }

        string[] partes = linea.Split('|');
        if (partes.Length != 8)
        {
            return null;
        }

        int numero;
        int codigo;
        int cantidad;
        double precio;
        double total;
        if (!int.TryParse(partes[0], out numero))
        {
            return null;
        }

        if (!int.TryParse(partes[2], out codigo))
        {
            return null;
        }

        if (!int.TryParse(partes[4], out cantidad))
        {
            return null;
        }

        if (!double.TryParse(partes[5], NumberStyles.Float, CultureInfo.InvariantCulture, out precio))
        {
            return null;
        }

        if (!double.TryParse(partes[6], NumberStyles.Float, CultureInfo.InvariantCulture, out total))
        {
            return null;
        }

        Venta venta = new Venta();
        venta.Numero = numero;
        venta.Cliente = partes[1];
        venta.CodigoProducto = codigo;
        venta.NombreProducto = partes[3];
        venta.Cantidad = cantidad;
        venta.PrecioUnitario = precio;
        venta.Total = total;
        venta.Fecha = partes[7];
        return venta;
    }
}
