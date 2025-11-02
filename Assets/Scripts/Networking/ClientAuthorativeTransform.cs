using Unity.Netcode.Components;

public class ClientAuthoritativeTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false; // false = client is authority
    }
}
