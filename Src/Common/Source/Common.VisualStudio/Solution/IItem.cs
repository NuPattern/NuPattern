namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Represents an item in a hierarchy. Every item 
    /// can contain child items in turn (known as dependent files
    /// in Visual Studio), hence, this interface implements
    /// <see cref="IItemContainer"/>.
    /// </summary>
    public interface IItem : IItemContainer, IDataContainer
    {

    }
}
