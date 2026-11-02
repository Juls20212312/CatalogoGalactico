using System.Security.Cryptography;
using System.Text;

namespace CatalogoGalactico.Services;

public class ProteccionService : IProteccionService
{
    private const int TamanoNonce = 12;
    private const int TamanoTag = 16;

    private readonly byte[] _clave;

    public ProteccionService(IConfiguration configuration)
    {
        string? claveBase64 = configuration["Encryption:Key"];

        if (string.IsNullOrWhiteSpace(claveBase64))
        {
            throw new InvalidOperationException(
                "Falta configurar la clave de cifrado en Secret Manager."
            );
        }

        try
        {
            _clave = Convert.FromBase64String(claveBase64);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException(
                "La clave de cifrado no tiene un formato válido."
            );
        }

        if (_clave.Length != 32)
        {
            throw new InvalidOperationException(
                "La clave de cifrado debe tener exactamente 32 bytes."
            );
        }
    }

    public string Cifrar(string texto)
    {
        if (string.IsNullOrEmpty(texto))
        {
            return string.Empty;
        }

        byte[] nonce = RandomNumberGenerator.GetBytes(TamanoNonce);
        byte[] datos = Encoding.UTF8.GetBytes(texto);
        byte[] cifrado = new byte[datos.Length];
        byte[] tag = new byte[TamanoTag];

        using var aes = new AesGcm(_clave, TamanoTag);

        aes.Encrypt(nonce, datos, cifrado, tag);

        byte[] resultado = new byte[
            TamanoNonce + TamanoTag + cifrado.Length
        ];

        Buffer.BlockCopy(nonce, 0, resultado, 0, TamanoNonce);

        Buffer.BlockCopy(
            tag, 0, resultado, TamanoNonce, TamanoTag
        );

        Buffer.BlockCopy(
            cifrado, 0, resultado,
            TamanoNonce + TamanoTag, cifrado.Length
        );

        return Convert.ToBase64String(resultado);
    }

    public string Descifrar(string textoCifrado)
    {
        if (string.IsNullOrEmpty(textoCifrado))
        {
            return string.Empty;
        }

        byte[] datos;

        try
        {
            datos = Convert.FromBase64String(textoCifrado);
        }
        catch (FormatException)
        {
            throw new CryptographicException(
                "El texto cifrado no tiene un formato válido."
            );
        }

        if (datos.Length < TamanoNonce + TamanoTag)
        {
            throw new CryptographicException(
                "El contenido cifrado está incompleto."
            );
        }

        byte[] nonce = new byte[TamanoNonce];
        byte[] tag = new byte[TamanoTag];

        int tamanoCifrado = datos.Length - TamanoNonce - TamanoTag;
        byte[] cifrado = new byte[tamanoCifrado];
        byte[] texto = new byte[tamanoCifrado];

        Buffer.BlockCopy(datos, 0, nonce, 0, TamanoNonce);

        Buffer.BlockCopy(
            datos, TamanoNonce, tag, 0, TamanoTag
        );

        Buffer.BlockCopy(
            datos, TamanoNonce + TamanoTag,
            cifrado, 0, tamanoCifrado
        );

        using var aes = new AesGcm(_clave, TamanoTag);

        aes.Decrypt(nonce, cifrado, tag, texto);

        return Encoding.UTF8.GetString(texto);
    }
}