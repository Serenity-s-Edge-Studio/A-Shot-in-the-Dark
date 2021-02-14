public class Magazine<T> : Stackable<T> where T : Ammo
{
    public Magazine(IStackable<T> item, int amount) : base(item, amount)
    {
    }

    public Magazine(T item, int amount, int maxSize) : base(item, amount, maxSize)
    {
    }
}
