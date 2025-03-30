using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CatalogoAvalonia.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CatalogoGatos.ViewModel;

public partial class MainViewModel : ObservableObject

{
    // Rutas y configuración inicial
    public static string RutaImg = "../../../Assets/Images/";
    public static string CarpetaDatos = Path.Combine(Directory.GetCurrentDirectory(), "../../../Datos");
    public static string RutaArchivo = Path.Combine(CarpetaDatos, "gatos.json");
    public static string RutaImgDefecto = Path.Combine(RutaImg, "gato_no_encontrado.jpg");

    // Propiedades observables
    public ObservableCollection<ColorItem> Colores { get; set; } // Lista de objetos ColorItem
    public ObservableCollection<RazaGato> Razas { get; set; } // Lista de razas
    public ColorItem SelectedColor { get; set; } // ColorItem seleccionado en el combobox
    public RazaGato SelectedRaza { get; set; } // Raza seleccionada en el combobox

    [ObservableProperty] private Bitmap _imgColor; // Imagen para mostrar el color
    [ObservableProperty] private string _txtBtnAdd; // Texto para el hover del botón añadir/aceptar
    [ObservableProperty] private string _txtBtnDelete; // Texto para el hover del botón borrar/cancelar
    [ObservableProperty] private string _txtDialog; // Texto para el contenido del dialog
    [ObservableProperty] private bool _mostrarDialog; // Booleano para controlar si mostrar o no el dialogo
    [ObservableProperty] private bool _mostrarDosOp; // Booleano para mostrar uno o dos botones en el dialog 
    [ObservableProperty] private Bitmap _imgAceptarAdd; // Icono para el botón de añadir o aceptar
    [ObservableProperty] private ObservableCollection<Gato> _gatos; // Lista de los gatos
    [ObservableProperty] private int _indice; // Indice para controlar qué gato se muestra

    // Variables del gato
    [ObservableProperty] private string _nombre;
    [ObservableProperty] private Sexo _sexo;
    [ObservableProperty] private ColorGato _color;
    [ObservableProperty] private string _fechaNacimiento;
    [ObservableProperty] private bool _estaCastrado;
    [ObservableProperty] private bool _tieneChip;
    [ObservableProperty] private RazaGato _raza;
    [ObservableProperty] private bool _esMacho;
    [ObservableProperty] private bool _esHembra;
    [ObservableProperty] private Bitmap _selectedImg; // Imagen seleccionada


    [ObservableProperty] private SolidColorBrush _colorBrush; // Color del círculo 
    [ObservableProperty] private DateTime _selectedDate; // Fecha seleccionada en el comboBox
    [ObservableProperty] private DateTime _fechaActual;

    [ObservableProperty] private Boolean _esVer; // Booleano para controlar si está en modo navegacion (true) o modo añadir (false)
    [ObservableProperty] private bool _isLast; // Booleano para comprobar si se ha alcanzado el final
    [ObservableProperty] private bool _isFirst; // Booleano para comprobar si se ha alcanzado el principio
    [ObservableProperty] private bool _estaVacia; // Booleano para comprobar si la lista está vacía
    [ObservableProperty] private Boolean _esBorrar; // Booleano para controlar si se está borrando (para el dialog)

    public MainViewModel()
    {
        FechaActual = DateTime.Now;
        Colores = new ObservableCollection<ColorItem>();
        Razas = new ObservableCollection<RazaGato>();
        Gatos = new ObservableCollection<Gato>();

        InicializarListas();
        TxtBtnAdd = "Añadir Nuevo";
        ImgAceptarAdd = new Bitmap(RutaImg + "add_delete.png");

        Cargar(); // Carga la lista de gatos
    }

    
    // Método para cambiar la vista de la navegación
    private void CambiarInfGato(Gato gato)
    {
        CambiarEstadoBotones();

        EsBorrar = false;
        MostrarDialog = false;
        MostrarDosOp = false;
        ImgColor = null;
        SelectedDate = gato.FechaNacimiento;
        TxtBtnAdd = "Añadir Nuevo";
        TxtBtnDelete = "Borrar Actual";
        EsVer = true;
        Nombre = gato.Nombre;
        FechaNacimiento = gato.FechaNacimiento.ToShortDateString();
        Raza = gato.Raza;
        Color = gato.Color;
        EsMacho = gato.Genero == Sexo.Macho;
        EsHembra = gato.Genero == Sexo.Hembra;
        EstaCastrado = gato.EstaCastrado;
        SelectedImg = gato.Foto == null ? new Bitmap(RutaImgDefecto) : ByteArrayToBitmap(gato.Foto);
        TieneChip = gato.TieneChip;
        CambiarImgColor(gato.Color);
        ImgAceptarAdd = new Bitmap(RutaImg + "add_delete.png");
    }

    // Método para cambiar la imagen o el color al círculo de "muestra"
    private void CambiarImgColor(ColorGato color)
    {
        var colorItem = Colores.FirstOrDefault(c => c.Color == color);

        if (colorItem.ImgColor != null)
        {
            ImgColor = colorItem.ImgColor;
        }
        else
        {
            ColorBrush = colorItem.ColorBrush;
        }
    }

    private void InicializarListas()
    {
        foreach (ColorGato color in Enum.GetValues(typeof(ColorGato)))
            Colores.Add(new ColorItem(color));

        foreach (RazaGato raza in Enum.GetValues(typeof(RazaGato)))
            Razas.Add(raza);

        SelectedColor = Colores.FirstOrDefault();
        SelectedRaza = Razas.FirstOrDefault();
    }

    // Método al pulsar el botón de Añadir / Aceptar
    [RelayCommand]
    private void Add()
    {
        if (TxtBtnAdd == "Añadir Nuevo") // Si está en modo navegación 
        {
            CambiarLayoutAdd();
        }
        else // Si está en la vista AÑADIR
        {
            AddNuevoGato();
        }
    }

    // Método para cambiar la vista a AÑADIR
    private void CambiarLayoutAdd()
    {
        TxtBtnAdd = "Aceptar";
        TxtBtnDelete = "Cancelar";
        EsVer = false;
        EstaCastrado = false;
        TieneChip = false;
        EsHembra = false;
        EsMacho = false;
        SelectedImg = new Bitmap(RutaImgDefecto);
        ImgAceptarAdd = new Bitmap(RutaImg + "aceptar.png");
        Nombre = "";
        SelectedColor = Colores[0];
        SelectedRaza = Razas[0];
        SelectedDate = FechaActual;
    }

    // Método para comprobar que los campos son correctos
    private bool ValidarCampos(out string mensajeError)
    {
        var errores = new System.Text.StringBuilder();

        if (string.IsNullOrEmpty(Nombre)) // Si el nombre está vacío
            errores.AppendLine("Nombre es obligatorio.");

        if (!(EsMacho || EsHembra)) // Si no se ha seleccionado ningún botón
            errores.AppendLine("Debe seleccionar un sexo.");

        mensajeError = errores.ToString();
        return mensajeError.Length == 0; // Mensaje personalizado de error
    }

    // Método para añadir un nuevo gato
    private void AddNuevoGato()
    {
        // Comprueba si todos los campos son correctos
        if (ValidarCampos(out var mensajeError))
        {
            
            var sexo = EsMacho ? Sexo.Macho : Sexo.Hembra; // Obtiene el sexo
            byte[]? foto = BitmapToByteArray(SelectedImg); // Obtiene la imagen
            Gatos.Add(new Gato(Nombre, sexo, EstaCastrado, TieneChip, SelectedDate, SelectedRaza, SelectedColor.Color,
                foto)); // Crea el gato
            Indice = Gatos.Count - 1; // Obtiene el indice de nuevo
            EstaVacia = false;
            CambiarInfGato(Gatos[Indice]); // Muestra la información del nuevo gato
            
            MostrarDialogo(2, "Éxito\nSe ha añadido correctamente.");
            
            TxtBtnAdd = "Añadir Nuevo";
           
            Guardar(); // Guarda la nueva lista
        }
        else
        {
            MostrarDialogo(2, $"Error\n{mensajeError}"); // Muestra el mensaje de error
        }
    }

    // Cambiar el estilo del botón al seleccionar cada botón
    private void SeleccionarSexo(Sexo sexo)
    {
        EsMacho = sexo == Sexo.Macho;
        EsHembra = sexo == Sexo.Hembra;
    }

    // Métodos al pulsar los botones de sexo
    [RelayCommand]
    private void SeleccionarMacho() => SeleccionarSexo(Sexo.Macho);
    
    [RelayCommand]
    private void SeleccionarHembra() => SeleccionarSexo(Sexo.Hembra);

    
    // Método al pulsar el botón de Borrar / Cancelar
    [RelayCommand]
    private void BtnBorrarCancelar()
    {
        if (EsVer) // Si se está en la vista de navegación (es para borrar el gato)
        {
            MostrarDialogo(1, "¿Desea borrar a este gato?"); // Se controla con el dialog
            EsBorrar = true;
        }
        else // Si está en la vista de añadir (para cancelar el proceso)
        {
            Cancelar(); 
        }
    }


    // Método para cuando se pulsa OK/ Aceptar en el Dialog (Confirmar)
    [RelayCommand]
    private void PulsarOk()
    {
        MostrarDialog = false;
        
        if (EsBorrar) // Si el dialog era al borrar un gato
        {
            if (Gatos.Count < 1) return;
            Gatos.RemoveAt(Indice);
            MostrarDialogo(2, $"Éxito\nSe ha borrado correctamente.");

            Guardar();

            if (Gatos.Count > 0)
            {
                // Ajustar el índice para que no quede fuera de rango
                if (Indice >= Gatos.Count)
                {
                    Indice = Gatos.Count - 1;
                }
                CambiarInfGato(Gatos[Indice]); // Actualizar la información del gato
            }
            else
            {
                VistaListaVacia();
                // Si la lista queda vacía, llamar a un método que lo maneje (por ejemplo, limpiar la vista)
            }
        }
    }
    
    // Método para cuando se pulsa cualquier botón de cancelar 
    [RelayCommand]
    private void Cancelar()
    {
        CambiarInfGato(Gatos[Indice]);
    }
   
    // Método para cargar los datos del json en la lista
    [RelayCommand]
    private void Cargar()
    {
        // Verificar si el archivo existe para evitar excepciones
        if (!File.Exists(RutaArchivo))
        {
            VistaListaVacia();
            return;
        }

        // Leer JSON desde el archivo
        var json = File.ReadAllText(RutaArchivo);

        // Convertir JSON en objeto
        var gatosCargados = JsonSerializer.Deserialize<List<Gato>>(json);

        // Asignar a la propiedad si no es null
        if (gatosCargados == null) return;
        EsVer = true;
        Gatos = new ObservableCollection<Gato>(gatosCargados);
        if (Gatos.Count == 0)
        {
            VistaListaVacia(); // Si esta vacía se muestra la vista vacía
        }
        else
        {
            CambiarInfGato(Gatos[Indice]); // Si no muestra la información del gato primero 
        }
    }
    
    // Método para guardar la lista en el json
    [RelayCommand]
    private void Guardar()
    {
        // Crear la carpeta si no existe
        if (!Directory.Exists(CarpetaDatos))
        {
            Directory.CreateDirectory(CarpetaDatos);
        }

        // Serializar objeto a JSON
        string json = JsonSerializer.Serialize(new List<Gato>(Gatos));

        // Guardar en un archivo
        File.WriteAllText(RutaArchivo, json);
    }

    // Método para cambiar la vista cuando la lista de gatos está vacía
    private void VistaListaVacia()
    {
        EsVer = true;
        Nombre = "???";
        EsHembra = false;
        EsMacho = false;
        Color = Colores[0].Color;
        FechaNacimiento = FechaActual.ToShortDateString();
        SelectedDate = FechaActual;
        Raza = Razas[12];
        EstaCastrado = false;
        TieneChip = false;
        EstaVacia = true;
        SelectedImg = new Bitmap(RutaImgDefecto);
        MostrarDialogo(2, "No quedan más gatos! Añade uno :)");
        CambiarEstadoBotones();
    }

    
    // Método para seleccionar una foto y mostrarla 
    [RelayCommand]
    private async void SubirFoto(Window ventanaPadre)
    {
        var dlg = new OpenFileDialog(); 
        dlg.Filters.Add(new FileDialogFilter() { Name = "Imágenes JPEG", Extensions = { "jpg" } });
        dlg.Filters.Add(new FileDialogFilter() { Name = "Imágenes PNG", Extensions = { "png" } });
        dlg.Filters.Add(new FileDialogFilter() { Name = "Todos los archivos", Extensions = { "*" } });
        dlg.AllowMultiple = false;

        var result = await dlg.ShowAsync(ventanaPadre);
        if (result != null)
        {
            string rutaFoto = result[0];
            SelectedImg = new Bitmap(rutaFoto);
        }
    }
    
    
    // Método para mostrar la información del gato anterior de la lista
    [RelayCommand]
    private void Anterior()
    {
        if (Indice > 0)
        {
            Indice -= 1;
            CambiarInfGato(Gatos[Indice]);
        }
    }

    // Método para mostrar la información del gato siguiente de la lista
    [RelayCommand]
    private void Siguiente()
    {
        if (Indice < Gatos.Count - 1)
        {
            Indice += 1;
            CambiarInfGato(Gatos[Indice]);
        }

    }

   // Método para mostrar el diálogo
    private void MostrarDialogo(int op, string texto)
    {
        MostrarDialog = true;
        MostrarDosOp = op == 1;
        TxtDialog = texto;
    }

    
    // Método para convertir la imagen en bytes en BitMap
    private Bitmap ByteArrayToBitmap(byte[] byteArray)
    {
        Stream stream = new MemoryStream(byteArray);
        return new Bitmap(stream);
    }

    // Método para convertir la imagen en Bitmap en bytes

    private byte[] BitmapToByteArray(Bitmap bitmap)
    {
        MemoryStream memoryStream = new MemoryStream();
        bitmap.Save(memoryStream);
        return memoryStream.ToArray();
    }

    
    
    // Método para cambiar el estado de los botones
    private void CambiarEstadoBotones()
    {
        
        if (EstaVacia)
        {
            IsLast = true;
            IsFirst = true;
        }
        else
        {
            IsLast = Indice == Gatos.Count - 1;
            IsFirst = Indice == 0;
        }

    }
}

// Clase para cada Color
public class ColorItem
{
    public ColorGato Color { get; set; } // Color en sí
    public SolidColorBrush? ColorBrush { get; set; } // Valor del color
    public Bitmap? ImgColor { get; set; } // Imagen del color

    public ColorItem(ColorGato color)
    {
        string nombreArchivo = "";
        string colorHex = "";

        switch (color)
        {
            case ColorGato.Atigrado:
                nombreArchivo = "atigrado.png";
                break;
            case ColorGato.Bicolor:
                nombreArchivo = "bicolor.png";
                break;
            case ColorGato.Tricolor:
                nombreArchivo = "tricolor.png";
                break;
            case ColorGato.Blanco:
                colorHex = "#ffffff";
                break;
            case ColorGato.Crema:
                colorHex = "#fff5ee"; 
                break;
            case ColorGato.Gris:
                colorHex = "#808080"; 
                break;
            case ColorGato.Marron:
                colorHex = "#8B4513"; 
                break;
            case ColorGato.Naranja:
                colorHex = "#FFA500";
                break;
            case ColorGato.AzulGrisaceo:
                colorHex = "#607D8B"; 
                break;
            case ColorGato.Negro:
                colorHex = "#111111"; 
                break;
        }

        // Si hay un nombre de archivo (es decir, se quiere usar una imagen)
        if (!string.IsNullOrEmpty(nombreArchivo))
        {
            ImgColor = new Bitmap(MainViewModel.RutaImg + nombreArchivo);
        }
        // Si no hay nombre de archivo, se usa un color sólido
        else if (!string.IsNullOrEmpty(colorHex))
        {
            ColorBrush = new SolidColorBrush(Avalonia.Media.Color.Parse(colorHex));
        }
        
        Color = color;
    }
}