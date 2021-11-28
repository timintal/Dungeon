namespace Tests.ProceduralGeneration
{
    public interface IMapGenerator
    {
        public int[,] Generate(int seed);
    }
}