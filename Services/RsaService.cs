using System.Security.Cryptography;

public static class RSAKeyProvider
{
    public static RSA GetPrivateKey()
    {
        string privateKeyText = File.ReadAllText("private.pem");
        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyText.ToCharArray());
        return rsa;
    }

    public static RSA GetPublicKey()
    {
        string publicKeyText = File.ReadAllText("public.pem");
        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyText.ToCharArray());
        return rsa;
    }
}