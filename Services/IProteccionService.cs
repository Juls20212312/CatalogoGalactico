namespace CatalogoGalactico.Services;

public interface IProteccionService
{
    string Cifrar(string texto);

    string Descifrar(string textoCifrado);
}