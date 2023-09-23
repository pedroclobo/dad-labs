using System.Text;

public interface IListaNomes
{
    public void AddName(string name);
    public string GetNames();
    public void EraseNames();
}

public class NameManager : IListaNomes
{
    private List<string> names = new List<string>();

    public void AddName(string name)
    {
        names.Add(name);
    }

    public string GetNames()
    {
        if (names.Count == 0)
        {
            return "Empty name list.";
        }

        StringBuilder sb = new StringBuilder();
        foreach (string name in names)
        {
            sb.AppendLine(name);
        }

        return sb.ToString().Trim();
    }

    public void EraseNames()
    {
        names.Clear();
    }
}
