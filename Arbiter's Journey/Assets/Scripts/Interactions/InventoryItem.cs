public class InventoryItem
{
    public int value { get; set; } = 0;
    public string name { get; set; } = "Thing";
    public string description { get; set; } = "A thing.";

    /// <summary>
    ///  Used to instantiate a prefab representing this item in the overworld or in some other visual context.
    /// </summary>
    public string prefabName { get; set; }

    public override string ToString()
    {
        return name.ToString();
    }

}