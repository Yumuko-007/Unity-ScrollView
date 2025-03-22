namespace Game
{
    public interface IUnitPool
    {
        int Count { get; }

        void Despwan(UnitBase unit);

        void DespwanAll();

        int IndexOf(UnitBase unit);
    }
}