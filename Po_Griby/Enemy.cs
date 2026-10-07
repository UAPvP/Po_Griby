class Enemy : Entity
{
    public string Name { get; private set; }
    public int Health { get; set; }
    public int MaxHealth { get; private set; }
    public int ATK_Damage { get; private set; }
    public int ATK_Speed { get; private set; }
    public int DEF_Shield { get; private set; }

    public bool IsAlive => Health > 0;

    public Enemy(string name, char symbol, int health, int damage, int speed = 2, int shield = 0)
    {
        Name = name;
        Symbol = symbol;
        Health = MaxHealth = health;
        ATK_Damage = damage;
        ATK_Speed = speed;
        DEF_Shield = shield;
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Math.Max(1, damage - DEF_Shield);
        Health -= actualDamage;
        if (Health < 0) Health = 0;
    }
}
