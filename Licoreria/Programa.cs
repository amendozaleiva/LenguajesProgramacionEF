namespace Licoreria;

public class Programa
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string carpeta = args.Length > 0 ? args[0] : BuscarCarpetaDatos();
        ArchivoDatos archivos = new ArchivoDatos(carpeta);
        CajaLicoreria caja = new CajaLicoreria();

        List<Producto> productos = archivos.CargarProductos();
        if (productos.Count == 0)
        {
            productos = ArchivoDatos.ProductosIniciales();
            archivos.GuardarProductos(productos);
        }

        for (int i = 0; i < productos.Count; i++)
        {
            caja.Productos.Add(productos[i]);
        }

        List<Venta> ventas = archivos.CargarVentas();
        for (int i = 0; i < ventas.Count; i++)
        {
            caja.Ventas.Add(ventas[i]);
        }

        bool seguir = true;
        while (seguir)
        {
            MostrarMenu();
            string? opcion = LeerTexto("Elige una opcion: ");
            if (opcion == null)
            {
                break;
            }

            switch (opcion.Trim())
            {
                case "1":
                    MostrarProductos(caja);
                    break;
                case "2":
                    RegistrarPedido(caja, archivos);
                    break;
                case "3":
                    MostrarVentasDelDia(caja);
                    break;
                case "4":
                    archivos.GuardarProductos(caja.Productos);
                    archivos.GuardarVentas(caja.Ventas);
                    Console.WriteLine("Datos guardados. Hasta luego.");
                    seguir = false;
                    break;
                default:
                    Console.WriteLine("Opcion no valida.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private static void MostrarMenu()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("     LICORERIA - REGISTRO DE PEDIDOS");
        Console.WriteLine("========================================");
        Console.WriteLine("1. Ver productos");
        Console.WriteLine("2. Registrar pedido");
        Console.WriteLine("3. Ver ventas del dia");
        Console.WriteLine("4. Salir");
    }

    private static void MostrarProductos(CajaLicoreria caja)
    {
        Console.WriteLine();
        Console.WriteLine("{0,-8} {1,-28} {2,10} {3,8}", "Codigo", "Producto", "Precio", "Stock");
        Console.WriteLine(new string('-', 58));
        for (int i = 0; i < caja.Productos.Count; i++)
        {
            Producto producto = caja.Productos[i];
            Console.WriteLine(
                "{0,-8} {1,-28} {2,10} {3,8}",
                producto.Codigo,
                producto.Nombre,
                "S/ " + producto.Precio.ToString("0.00"),
                producto.Stock);
        }
    }

    private static void RegistrarPedido(CajaLicoreria caja, ArchivoDatos archivos)
    {
        string? cliente = LeerTexto("Nombre del cliente: ");
        if (cliente == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(cliente))
        {
            Console.WriteLine("El nombre del cliente no puede estar vacio.");
            return;
        }

        int? codigo = LeerEntero("Codigo del producto: ");
        if (codigo == null)
        {
            return;
        }

        int? cantidad = LeerEntero("Cantidad: ");
        if (cantidad == null)
        {
            return;
        }

        Producto? producto = caja.BuscarPorCodigo(codigo.Value);
        if (producto != null && cantidad.Value > 0 && producto.Stock >= cantidad.Value)
        {
            double total = CajaLicoreria.CalcularTotal(producto.Precio, cantidad.Value);
            Console.WriteLine("Total a cobrar: S/ " + total.ToString("0.00"));
            string? confirma = LeerTexto("Confirmar pedido (s/n): ");
            if (confirma == null || !confirma.Trim().Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Pedido cancelado.");
                return;
            }
        }

        string mensaje;
        bool ok = caja.RegistrarPedido(cliente, codigo.Value, cantidad.Value, out mensaje);
        Console.WriteLine(mensaje);
        if (ok)
        {
            archivos.GuardarProductos(caja.Productos);
            archivos.GuardarVentas(caja.Ventas);
        }
    }

    private static void MostrarVentasDelDia(CajaLicoreria caja)
    {
        string hoy = DateTime.Now.ToString("dd/MM/yyyy");
        List<Venta> delDia = caja.VentasDeLaFecha(hoy);
        Console.WriteLine();
        Console.WriteLine("Ventas del " + hoy);
        if (delDia.Count == 0)
        {
            Console.WriteLine("Todavia no hay ventas hoy.");
            return;
        }

        for (int i = 0; i < delDia.Count; i++)
        {
            Venta venta = delDia[i];
            Console.WriteLine(
                venta.Numero + ". " + venta.Cliente
                + " | " + venta.NombreProducto
                + " | cant. " + venta.Cantidad
                + " | S/ " + venta.Total.ToString("0.00")
                + " | " + venta.Fecha);
        }

        Console.WriteLine("Total recaudado: S/ " + CajaLicoreria.SumarTotales(delDia).ToString("0.00"));
    }

    private static string? LeerTexto(string mensaje)
    {
        Console.Write(mensaje);
        return Console.ReadLine();
    }

    private static int? LeerEntero(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string? texto = Console.ReadLine();
            if (texto == null)
            {
                return null;
            }

            int numero;
            if (int.TryParse(texto.Trim(), out numero))
            {
                return numero;
            }

            Console.WriteLine("Ingresa un numero entero.");
        }
    }

    private static string BuscarCarpetaDatos()
    {
        DirectoryInfo? dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (int i = 0; i < 6 && dir != null; i++)
        {
            string proyecto = Path.Combine(dir.FullName, "Licoreria.csproj");
            if (File.Exists(proyecto))
            {
                return Path.Combine(dir.FullName, "datos");
            }

            dir = dir.Parent;
        }

        return Path.Combine(Directory.GetCurrentDirectory(), "datos");
    }
}
