namespace Licoreria;

public class CajaLicoreria
{
    public List<Producto> Productos { get; } = new List<Producto>();
    public List<Venta> Ventas { get; } = new List<Venta>();

    public static double CalcularTotal(double precio, int cantidad)
    {
        return Math.Round(precio * cantidad, 2);
    }

    public static double SumarTotales(List<Venta> ventas)
    {
        double suma = 0;
        for (int i = 0; i < ventas.Count; i++)
        {
            suma = suma + ventas[i].Total;
        }

        return Math.Round(suma, 2);
    }

    public Producto? BuscarPorCodigo(int codigo)
    {
        for (int i = 0; i < Productos.Count; i++)
        {
            if (Productos[i].Codigo == codigo)
            {
                return Productos[i];
            }
        }

        return null;
    }

    public List<Venta> VentasDeLaFecha(string fecha)
    {
        List<Venta> delDia = new List<Venta>();
        for (int i = 0; i < Ventas.Count; i++)
        {
            if (Ventas[i].Fecha.StartsWith(fecha))
            {
                delDia.Add(Ventas[i]);
            }
        }

        return delDia;
    }

    public bool RegistrarPedido(string cliente, int codigo, int cantidad, out string mensaje)
    {
        if (string.IsNullOrWhiteSpace(cliente))
        {
            mensaje = "El nombre del cliente no puede estar vacio.";
            return false;
        }

        if (cantidad <= 0)
        {
            mensaje = "La cantidad debe ser mayor que cero.";
            return false;
        }

        Producto? producto = BuscarPorCodigo(codigo);
        if (producto == null)
        {
            mensaje = "No existe un producto con ese codigo.";
            return false;
        }

        if (producto.Stock < cantidad)
        {
            mensaje = "No hay stock suficiente. Stock actual: " + producto.Stock;
            return false;
        }

        double total = CalcularTotal(producto.Precio, cantidad);
        producto.Stock = producto.Stock - cantidad;

        Venta venta = new Venta();
        venta.Numero = SiguienteNumero();
        venta.Cliente = cliente.Trim();
        venta.CodigoProducto = producto.Codigo;
        venta.NombreProducto = producto.Nombre;
        venta.Cantidad = cantidad;
        venta.PrecioUnitario = producto.Precio;
        venta.Total = total;
        venta.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        Ventas.Add(venta);

        mensaje = "Pedido registrado. Total a cobrar: S/ " + total.ToString("0.00");
        return true;
    }

    public int SiguienteNumero()
    {
        int siguiente = 1;
        for (int i = 0; i < Ventas.Count; i++)
        {
            if (Ventas[i].Numero >= siguiente)
            {
                siguiente = Ventas[i].Numero + 1;
            }
        }

        return siguiente;
    }
}
