namespace Grumpy.DacpacMerge.Interfaces
{
    public interface IPackageFactory
    {
        IPackage Create(string fileName);
    }
}