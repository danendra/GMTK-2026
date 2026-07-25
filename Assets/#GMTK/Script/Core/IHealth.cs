namespace GMTK
{
    public interface IHealth
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        void TakeDamage(float damage);
    }
}
