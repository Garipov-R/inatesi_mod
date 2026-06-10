namespace InatesiCharacter.Testing.Character.Bot3
{
    public enum EntityFlag
    {
        nobody = 0,
        Human = 1 << 0,
        Monster = 1 << 1,
        Monster2 = 1 << 2,

        All = ~0
    }
}