namespace WebApiRepoteria.AL.Interfaz
{
    public interface IEncriptaEN
    {
        string EncriptarHash(string CadenaOriginal);
        string Desencriptar(string CadaneString);
        string Encriptar(string CadaneString);
    }
}
