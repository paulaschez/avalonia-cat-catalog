using System;
using System.Text.Json.Serialization;

namespace CatalogoAvalonia.Model;

[Serializable]
public class Gato
{
    
    
    /// <summary>
    /// Tamaño máximo del nombre
    /// </summary>
    public const int MaxNombre = 20;

    private string _nombre;

    /// <summary>
    /// Nombre de la mascota formateado para que sea la primera letra en mayúscula
    /// </summary>
    public string Nombre
    {
        get => _nombre;
        set
        {
            if (!string.IsNullOrEmpty(value) && value.Length <= MaxNombre)
            {
                _nombre = FormatearNombre(value.Trim()) ;
            }
            else
            {
                _nombre = FormatearNombre(value.Trim().Substring(0, MaxNombre));
            }
        }
    }
    
    
    /// <summary>
    /// Género de la mascota
    /// </summary>
    public Sexo Genero { get; set; }


    /// <summary>
    /// Condición de si está castrado
    /// </summary>
    public bool EstaCastrado { get; set; }
    
    /// <summary>
    /// Condición de si tiene chip
    /// </summary>
    public bool TieneChip { get; set; }
    
    
    /// <summary>
    ///  Fecha de nacimiento de la mascota
    /// </summary>
    private DateTime _fechaNacimiento;

    /// <summary>
    /// Fecha de nacimiento de la mascota
    /// </summary>
    public DateTime FechaNacimiento
    {
        get => _fechaNacimiento;
        set
        {
            if (value <= DateTime.Today)
            {
                _fechaNacimiento = value;
            }
        }
    }
    
    /// <summary>
    ///  Fotografía de la mascota
    /// </summary>
    public byte[]? Foto {get; set; }

   
    /// <summary>
    /// Raza del Gato
    /// </summary>
    public RazaGato Raza { get; set; }
    
    /// <summary>
    /// Color del Gato
    /// </summary>
    public ColorGato Color { get; set; }
    
    /// <summary>
    /// Constructor de objeto Gato
    /// </summary>
    /// <param name="nombre"></param>
    /// <param name="genero"></param>
    /// <param name="estaCastrado"></param>
    /// <param name="tieneChip"></param>
    /// <param name="fechaNacimiento"></param>
    /// <param name="raza"></param>
    /// <param name="color"></param>
    /// <param name="foto"></param>
    [JsonConstructor]
    public Gato(string nombre, Sexo genero, bool estaCastrado, bool tieneChip, DateTime fechaNacimiento,RazaGato raza, ColorGato color, byte[]? foto ) 
    {
        Raza = raza;
        Color = color;
        Nombre = nombre;
        Genero = genero;
        EstaCastrado = estaCastrado;
        TieneChip = tieneChip;
        FechaNacimiento = fechaNacimiento;

        if (foto != null)
        {
            Foto = foto;
        }
    }
    
    
    /// <summary>
    /// Método toString de objeto Gato
    /// </summary>
    /// <returns>Descripción del objeto Gato</returns>
    public override string ToString()
    {
        return $"{Nombre} | Género: {Genero} | " +
               $"Castrado: {(EstaCastrado ? "Sí" : "No")} | " +
               $"Chip: {(TieneChip ? "Sí" : "No")} | " +
               $"Nacimiento: {FechaNacimiento:dd/MM/yyyy} | Raza: {Raza} | Color: {Color}";
    }
    

    
    // Convierte el primer carácter a mayúscula y el resto a minúscula
    private string FormatearNombre(string nombre)
    {
        return char.ToUpper(nombre[0]) + nombre.Substring(1).ToLower();
    }

   
    
}

public enum ColorGato
{
    Blanco = 1,
    Negro = 2,
    Gris = 3,
    Naranja = 4,
    Atigrado = 5,
    Tricolor = 6,
    Bicolor = 7,
    Marron = 8,
    Crema = 9,
    AzulGrisaceo = 10
}
public enum RazaGato
{
    Persa = 1,
    Siames = 2,
    MaineCoon = 3,
    Bengal = 4,
    Esfinge = 5,
    Birmano = 6,
    ScottishFold = 7,
    AzulRuso = 8,
    Ragdoll = 9,
    Abisinio = 10,
    BritishShorthair= 11,
    Otra = 12, 
    NoEspecificada= 13
}

public enum Sexo
{
    Macho = 1,
    Hembra = 2
}